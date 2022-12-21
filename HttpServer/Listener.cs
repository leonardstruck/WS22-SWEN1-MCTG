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

    private void HandleClientCommunication(object? threadObject)
    {
        if (threadObject == null)
        {
            throw new ArgumentException("You have to provide a client socket");
        }

        TcpClient clientSocket = (TcpClient)threadObject;

        Console.WriteLine($"Accepted Client from {clientSocket.Client.RemoteEndPoint}");
        try
        {
            HttpRequest request = HttpRequest.CreateInstance(clientSocket.GetStream());
            HttpResponse response = new(clientSocket.GetStream());

            // Split Path
            var pathSplit = request.Path.Split("/");

            // Build Key (check if path has multiple forward slashes and replace with wildcard *)
            var key = request.Method + "%%/" + pathSplit[1] +
                      (pathSplit.Length > 2 && pathSplit[2].Length > 0 ? "/*" : "");
            
            // strip query params from key
            if (key.Contains("?"))
            {
                key = key.Substring(0, key.IndexOf("?", StringComparison.Ordinal));
            }

            if (_endpoints.ContainsKey(key))
            {
                Console.WriteLine($"{clientSocket.Client.RemoteEndPoint} - REQ - {request.Method} {request.Path}");
                try
                {
                    // Run Middlewares
                    foreach (var middlewareKey in _endpoints[key].MiddlewareKeys)
                    {
                        if (_middlewares.ContainsKey(middlewareKey))
                        {
                            var middlewareResult = _middlewares[middlewareKey].HandleRequest(request, response);
                            // If middleware cancels request, stop processing
                            if (middlewareResult.Abort)
                            {
                                Console.WriteLine($"{clientSocket.Client.RemoteEndPoint} - RES - {response.Status}");
                                response.Send();
                                return;
                            }

                            
                            response = middlewareResult.Response;
                            request = middlewareResult.Request;
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException($"Middleware with key {middlewareKey} not found");
                        }
                    }
                    
                    // Run Request with EndpointController
                    _endpoints[key].Controller.HandleRequest(request, response);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while handling request: " + e.Message);
                    response.Status = 500;
                    response.StatusMessage = "Internal Server Error";
                }
            }
            else
            {
                Console.WriteLine(
                    $"{clientSocket.Client.RemoteEndPoint} - ERR - Request to unknown resource: {request.Method} {request.Path}");
                response.Status = 404;
                response.StatusMessage = "Not Found";
                response.Body = "The requested resource could not be found";
            }

            Console.WriteLine($"{clientSocket.Client.RemoteEndPoint} - RES - {response.Status}");
            response.Send();
        }
        catch (SocketException socketException)
        {
            Console.WriteLine($"SocketException: {socketException.Message}");
        }

    }

}
