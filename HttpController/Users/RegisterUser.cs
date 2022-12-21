using HttpController.Middleware;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Users;

[HttpEndpoint("/users", HttpMethod.POST, "BodyNotNull")]
public class RegisterUser : IEndpointController
{
    public void HandleRequest(HttpRequest req, HttpResponse res)
    {
        var user = req.DeserializeBody<RegisterUserParams>();

        throw new NotImplementedException();
    }
}


// Use this class to deserialize the body of the request and to keep the password private
class RegisterUserParams
{
    public string Username { get; set; }
    public string Password { get; set; }
}