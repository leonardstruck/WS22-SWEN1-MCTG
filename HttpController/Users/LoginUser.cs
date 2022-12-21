using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Users;

[HttpEndpoint("/sessions", HttpMethod.POST, "BodyNotNull", "ValidateSchema_Credentials")]
public class LoginUser : IEndpointController
{
    public async Task HandleRequest(HttpRequest req, HttpResponse res)
    {
        var credentials = req.DeserializeBody<Credentials>();

        var token = await UserRepository.LoginUser(credentials);
        
        if(token == null)
        {
            res.Status = 401;
            res.StatusMessage = "Unauthorized";
            
            res.Json(new {status = "error", message = "Invalid credentials"});
            return;
        }
        
        res.Json(new {status = "ok", data = new {token}});
    }
}