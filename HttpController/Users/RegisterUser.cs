using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Users;

[HttpEndpoint("/users", HttpMethod.POST)]
public class RegisterUser : IEndpointController
{
    public void HandleRequest(HttpRequest req, HttpResponse res)
    {
        throw new NotImplementedException();
    }
}