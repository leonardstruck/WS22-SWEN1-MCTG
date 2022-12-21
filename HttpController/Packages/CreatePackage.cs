using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Packages;

[HttpEndpoint("/packages", HttpMethod.POST, "Auth_ValidToken")]
public class CreatePackage : IEndpointController
{
    public Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        throw new NotImplementedException();
    }
}