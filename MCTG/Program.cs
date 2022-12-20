using System.Net;
using HttpServer;

Listener listener = new(IPAddress.Loopback, 10001);

var endpoints = EndpointResolver.FindEndpointControllers();
listener.RegisterEndpoint(endpoints);

listener.Start();