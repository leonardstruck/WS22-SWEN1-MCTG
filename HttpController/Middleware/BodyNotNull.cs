using HttpServer;

namespace HttpController.Middleware;

[MiddlewareKey("BodyNotNull")]
public class BodyNotNull : IMiddleware
{
    public MiddlewareResult HandleRequest(HttpRequest req, HttpResponse res)
    {
        Console.WriteLine("BodyNotNull middleware ran");
        throw new NotImplementedException();
    }
}