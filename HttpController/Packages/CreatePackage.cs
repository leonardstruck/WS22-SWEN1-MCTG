using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Packages;

[HttpEndpoint("/packages", HttpMethod.POST, "Auth_Verify", "Auth_MustBeAdmin")]
public class CreatePackage : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        return ctx;
    }
}