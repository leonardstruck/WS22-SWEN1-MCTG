using DataLayer;
using HttpServer;

namespace HttpController.Middleware.Authentication;

[HttpMiddleware("Auth_ValidToken")]
public class ValidToken : IMiddleware
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        // check if the token is in the header
        if(ctx.Request.Headers.ContainsKey("Authorization"))
        {
            // get the token
            var token = ctx.Request.Headers["Authorization"].Split(" ")[1];
            
            // check if the token is valid
            var user = await UserRepository.GetUserByToken(token);
            if (user == null)
            {
                ctx.Abort = true;
                ctx.Response.Status = 401;
                ctx.Response.StatusMessage = "Unauthorized";
                ctx.Response.Json(new { status = "error", message = "Invalid token" });
            }
        } else {
            ctx.Abort = true;
            ctx.Response.Status = 401;
            ctx.Response.StatusMessage = "Unauthorized";
            ctx.Response.Json(new { status = "error", message = "No token provided" });
        }

        return ctx;
    }
}