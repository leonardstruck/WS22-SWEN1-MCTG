using HttpServer;

namespace HttpController.Middleware;

[HttpMiddleware("BodyNotNull")]
public class BodyNotNull : IMiddleware
{
    public Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        // This middleware checks if the request body is null or empty
        if (!string.IsNullOrEmpty(ctx.Request.Body)) return Task.FromResult(ctx);
        
        ctx.Response.Status = 400;
        ctx.Response.StatusMessage = "Bad Request";
            
        ctx.Response.Json(new { status = "error", error = "Request body is empty" });
        ctx.Abort = true;

        return Task.FromResult(ctx);
    }
}