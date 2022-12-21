using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Users;

[HttpEndpoint("/users", HttpMethod.POST, "BodyNotNull", "ValidateSchema_Credentials")]
public class RegisterUser : IEndpointController
{
    public void HandleRequest(HttpRequest req, HttpResponse res)
    {
        var credentials = req.DeserializeBody<Credentials>();

        UserRepository.RegisterUser(credentials);
        throw new NotImplementedException();
    }
}