using DataLayer;
using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController;

[HttpEndpoint("/seedDatabase", HttpMethod.POST)]
public class SeedDatabase : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        await SeedRepository.SeedDatabase();
        return ctx;
    }
}