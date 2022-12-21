using HttpServer;
using Models;
namespace HttpController.Middleware.SchemaValidation;

[HttpMiddleware("ValidateSchema_Credentials")]
public class SchemaValidationCredentials : IMiddleware
{
    public MiddlewareResult HandleRequest(HttpRequest req, HttpResponse res)
    {
        var cred = req.DeserializeBody<Credentials>();
        var isValid = !(string.IsNullOrEmpty(cred.Username) || string.IsNullOrEmpty(cred.Password));


        if (isValid) return new MiddlewareResult(req, res);
        
        res.Status = 400;
        res.Json(new {status="error", message="Bad request. Please provide Username and Password."});

        return new MiddlewareResult(req, res, !isValid);
    }
}