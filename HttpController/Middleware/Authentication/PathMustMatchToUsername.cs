using HttpServer;
using Models;

namespace HttpController.Middleware.Authentication;

[HttpMiddleware("Auth_PathMustMatchToUsername")]
public class PathMustMatchToUsername : IMiddleware
{
    public Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        // Get the username from the path
        var username = ctx.Request.GetPathSegments().Last();
        
        // Get the username from the token
        var tokenUser = ctx.Data["user"] as User ?? throw new Exception("No user in context");
        
        // Early return if the username matches token or is admin
        if (username == tokenUser.Username || tokenUser.Username == "admin") return Task.FromResult(ctx);
        
        // return a 401
        
        ctx.Response.Status = 401;
        ctx.Response.StatusMessage = "Unauthorized";
        ctx.Response.Json(new
        {
            status = "error",
            message = "Unauthorized"
        });
        ctx.Abort = true;

        return Task.FromResult(ctx);
    }
}