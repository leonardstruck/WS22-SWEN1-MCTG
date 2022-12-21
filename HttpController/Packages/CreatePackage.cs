using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Packages;

[HttpEndpoint("/packages", HttpMethod.POST, "Auth", "Auth_MustBeAdmin")]
public class CreatePackage : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        return ctx;
    }
}