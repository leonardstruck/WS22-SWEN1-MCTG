using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Packages;

[HttpEndpoint("/transactions/packages", HttpMethod.POST)]
public class AcquirePackage : IEndpointController
{
    public Task<HttpContext> HandleRequest(HttpContext context)
    {
        throw new NotImplementedException();
    }
}