namespace HttpServer;

public interface IMiddleware
{
    public Task<HttpContext> HandleRequest(HttpContext ctx);
}

public class HttpMiddlewareAttribute : Attribute
{
    public string Key;
    public HttpMiddlewareAttribute(string key)
    {
        Key = key;
    }
}