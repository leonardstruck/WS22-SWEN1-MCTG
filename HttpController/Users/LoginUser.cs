using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Users;

[HttpEndpoint("/sessions", HttpMethod.POST, "BodyNotNull", "SchemaValidation_Credentials")]
public class LoginUser : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var credentials = ctx.Request.DeserializeBody<Credentials>();

        var token = await UserRepository.LoginUser(credentials);
        
        if(token == null)
        {
            ctx.Response.Status = 401;
            ctx.Response.StatusMessage = "Unauthorized";
            
            ctx.Response.Json(new {status = "error", message = "Invalid credentials"});
            return ctx;
        }
        
        ctx.Response.Json(new {status = "ok", data = new {token}});
        return ctx;
    }
}