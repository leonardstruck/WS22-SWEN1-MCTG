using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Users;

[HttpEndpoint("/users", HttpMethod.POST, "BodyNotNull", "SchemaValidation_Credentials")]
public class RegisterUser : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var credentials = ctx.Request.DeserializeBody<Credentials>();

        var user = await UserRepository.RegisterUser(credentials);
        
        if(user == null)
        {
            ctx.Response.Status = 409;
            ctx.Response.StatusMessage = "Conflict";
            ctx.Response.Json(new {status = "error", message = "User already exists"});
            return ctx;
        }
        
        ctx.Response.Status = 201;
        ctx.Response.Json(new {status = "ok", message = "User created", data = user});
        return ctx;
    }
}