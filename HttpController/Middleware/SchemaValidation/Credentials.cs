using HttpServer;
using Models;
namespace HttpController.Middleware.SchemaValidation;

[HttpMiddleware("SchemaValidation_Credentials")]
public class SchemaValidationCredentials : IMiddleware
{
    public Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var cred = ctx.Request.DeserializeBody<Credentials>();
        
        var isValid = !(string.IsNullOrEmpty(cred.Username) || string.IsNullOrEmpty(cred.Password));


        if (!isValid)
        {
            ctx.Abort = true;
            ctx.Response.Status = 400;
            ctx.Response.Json(new {status="error", message="Bad request. Please provide Username and Password."});
        }

        return Task.FromResult(ctx);
    }
}