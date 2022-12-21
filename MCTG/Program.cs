using System.Net;
using HttpServer;
using HttpServer.Resolver;

Listener listener = new(IPAddress.Loopback, 10001);

var endpoints = EndpointResolver.FindEndpointControllers();
var middlewares = MiddlewareResolver.FindMiddleware();

listener.RegisterEndpoint(endpoints);
listener.RegisterMiddleware(middlewares);

listener.Start();