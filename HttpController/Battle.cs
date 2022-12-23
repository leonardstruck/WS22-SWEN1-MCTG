using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController;

[HttpEndpoint("/battles", HttpMethod.POST, "Auth")]
public class Battle : IEndpointController
{
    public Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        Console.WriteLine(ctx.Request.Headers["Authorization"]);
        Thread.Sleep(5000);
        throw new NotImplementedException();
    }
}