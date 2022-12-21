using System.Net;
using System.Net.Sockets;

namespace HttpServer;

public class Listener
{
    private readonly TcpListener _httpServer;
    private bool _interrupt;

    public Listener(IPAddress localAddress, int port)
    {
        _httpServer = new(localAddress, port);
    }

    private readonly Dictionary<string, EndpointEntry> _endpoints = new();
    private readonly Dictionary<string, IMiddleware> _middlewares = new();

    // ReSharper disable once MemberCanBePrivate.Global
    public void RegisterEndpoint(IEndpointController controller)
    {
        Attribute[] attributes = Attribute.GetCustomAttributes(controller.GetType());
        foreach (var attribute in attributes)
        {
            if (attribute is not HttpEndpointAttribute endpoint) continue;
            var path = endpoint.Path;
            var method = endpoint.Method;

            var key = endpoint.Method + "%%" + endpoint.Path;
            Console.WriteLine($"Registered Endpoint {method.ToString()} {path}");
                
            if (_endpoints.ContainsKey(key))
            {
                throw new ArgumentOutOfRangeException($"Endpoint {method.ToString()} {path} already registered");
            }
                
            _endpoints.Add(key, new EndpointEntry(controller, endpoint.MiddlewareKeys));
        }
    }

    public void RegisterEndpoint(IEndpointController[] controllers)
    {
        foreach (var controller in controllers)
        {
            RegisterEndpoint(controller);
        }
    }

    public void RegisterMiddleware(IMiddleware middleware)
    {
        Attribute[] attributes = Attribute.GetCustomAttributes(middleware.GetType());
        foreach (var attribute in attributes)
        {
            if (attribute is not HttpMiddlewareAttribute key) continue;
            if (_middlewares.ContainsKey(key.Key))
            {
                throw new ArgumentOutOfRangeException($"Middleware with key {key.Key} already registered");
            }
                
            _middlewares.Add(key.Key, middleware);
            Console.WriteLine($"Registered Middleware {key.Key}");
        }
    }
    
    public void RegisterMiddleware(IMiddleware[] middlewares)
    {
        foreach (var middleware in middlewares)
        {
            RegisterMiddleware(middleware);
        }
    }

    public void Start()
    {
        Console.CancelKeyPress += delegate
        {
            Console.WriteLine();
            Console.WriteLine("Shutting down...");
            _interrupt = true;
        };
        
        _httpServer.Start();
        Console.WriteLine($"Ready to accept connections on {_httpServer.LocalEndpoint}");
        try
        {
            while (!_interrupt)
            {
                // wait for client connection
                TcpClient newClient = _httpServer.AcceptTcpClient();
                newClient.LingerState = new LingerOption(false, 0);
                
                // client connected
                // create thread to handle communication
                Thread communicationThread = new Thread(HandleClientCommunication);
                communicationThread.Start(newClient);
            }
        } finally { _httpServer.Stop(); }
    }

    private async void HandleClientCommunication(object? threadObject)
    {
        if (threadObject == null)
        {
            throw new ArgumentException("You have to provide a client socket");
        }

        TcpClient clientSocket = (TcpClient)threadObject;

        Console.WriteLine($"Accepted Client from {clientSocket.Client.RemoteEndPoint}");
        try
        {
            HttpContext ctx = new HttpContext(HttpRequest.CreateInstance(clientSocket.GetStream()), new HttpResponse(clientSocket.GetStream()));

            // Split Path
            var pathSplit = ctx.Request.Path.Split("/");

            // Build Key (check if path has multiple forward slashes and replace with wildcard *)
            var key = ctx.Request.Method + "%%/" + pathSplit[1] +
                      (pathSplit.Length > 2 && pathSplit[2].Length > 0 ? "/*" : "");
            
            // strip query params from key
            if (key.Contains("?"))
            {
                key = key.Substring(0, key.IndexOf("?", StringComparison.Ordinal));
            }

            if (_endpoints.ContainsKey(key))
            {
                Console.WriteLine($"{clientSocket.Client.RemoteEndPoint} - REQ - {ctx.Request.Method} {ctx.Request.Path}");
                try
                {
                    // Run Middlewares
                    foreach (var middlewareKey in _endpoints[key].MiddlewareKeys)
                    {
                        if (_middlewares.ContainsKey(middlewareKey))
                        {
                            ctx = await _middlewares[middlewareKey].HandleRequest(ctx);
                           
                            if (ctx.Abort)
                            {
                                Console.WriteLine($"{clientSocket.Client.RemoteEndPoint} - RES - {ctx.Response.Status}");
                                ctx.Response.Send();
                                return;
                            }
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException($"Middleware with key {middlewareKey} not found");
                        }
                    }
                    
                    // Run Request with EndpointController
                    ctx = await _endpoints[key].Controller.HandleRequest(ctx);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while handling request: " + e.Message);
                    ctx.Response.Status = 500;
                    ctx.Response.StatusMessage = "Internal Server Error";
                }
            }
            else
            {
                Console.WriteLine(
                    $"{clientSocket.Client.RemoteEndPoint} - ERR - Request to unknown resource: {ctx.Request.Method} {ctx.Request.Path}");
                ctx.Response.Status = 404;
                ctx.Response.StatusMessage = "Not Found";
                ctx.Response.Body = "The requested resource could not be found";
            }

            Console.WriteLine($"{clientSocket.Client.RemoteEndPoint} - RES - {ctx.Response.Status}");
            ctx.Response.Send();
        }
        catch (SocketException socketException)
        {
            Console.WriteLine($"SocketException: {socketException.Message}");
        }

    }

}
