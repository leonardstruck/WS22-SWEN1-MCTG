using DataLayer;
using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController;

[HttpEndpoint("/seedDatabase", HttpMethod.POST)]
public class SeedDatabase : IEndpointController
{
    public async Task HandleRequest(HttpRequest req, HttpResponse res)
    {
        var seed = new SeedRepository();
        await seed.Write();
    }
}