using DataLayer;
using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController;

[HttpEndpoint("/seedDatabase", HttpMethod.POST)]
public class SeedDatabase : IEndpointController
{
    public void HandleRequest(HttpRequest req, HttpResponse res)
    {
        var seed = new SeedRepository();
        var task = seed.Write();
        task.Wait();
    }
}