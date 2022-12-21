using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Users;

[HttpEndpoint("/users", HttpMethod.POST, "BodyNotNull", "ValidateSchema_Credentials")]
public class RegisterUser : IEndpointController
{
    public async Task HandleRequest(HttpRequest req, HttpResponse res)
    {
        var credentials = req.DeserializeBody<Credentials>();

        var user = await UserRepository.RegisterUser(credentials);
        
        if(user == null)
        {
            res.Status = 409;
            res.Json(new {status = "error", message = "User already exists"});
            return;
        }
        
        res.Status = 201;
        res.Json(new {status = "ok", message = "User created", data = user});
    }
}