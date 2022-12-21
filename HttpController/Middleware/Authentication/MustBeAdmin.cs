using HttpServer;
using Models;

namespace HttpController.Middleware.Authentication;

[HttpMiddleware("Auth_MustBeAdmin")]
public class MustBeAdmin : IMiddleware
{
    public Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        // check if user is in context
        if (ctx.Data.ContainsKey("user"))
        {
            // get User from context
            var user = ctx.Data["user"] as User;
            
            if(user?.Username == "admin")
                return Task.FromResult(ctx);
            
            // return 403
            ctx.Response.Status = 403;
            ctx.Response.StatusMessage = "Forbidden";
            ctx.Response.Json(new {status = "error", message = "You are not allowed to access this resource"});
        }
        else
        {
            throw new Exception("User not found in context. Run Auth middleware first");
        }
        return Task.FromResult(ctx);
    }
}