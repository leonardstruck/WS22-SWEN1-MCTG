namespace HttpServer;

public class HttpEndpointAttribute : Attribute
{
    public string Path { get; }
    public HttpMethod Method { get; }
    public string[] MiddlewareKeys { get; }

    public HttpEndpointAttribute(string path, HttpMethod method)
    {
        Path = path;
        Method = method;
        MiddlewareKeys = Array.Empty<string>();
    }
    
    public HttpEndpointAttribute(string path, HttpMethod method, params string[] middleware)
    {
        Path = path;
        Method = method;
        MiddlewareKeys = middleware;
    }
}

public interface IEndpointController
{
    public Task HandleRequest(HttpRequest req, HttpResponse res);
}

public class EndpointEntry
{
    public EndpointEntry(IEndpointController controller, string[] middlewareKeys)
    {
        Controller = controller;
        MiddlewareKeys = middlewareKeys;
    }

    public IEndpointController Controller { get; }
    public string[] MiddlewareKeys { get; }
}