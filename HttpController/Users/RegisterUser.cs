using HttpController.Middleware;
using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Users;

[HttpEndpoint("/users", HttpMethod.POST, "BodyNotNull")]
public class RegisterUser : IEndpointController
{
    public void HandleRequest(HttpRequest req, HttpResponse res)
    {
    }
}