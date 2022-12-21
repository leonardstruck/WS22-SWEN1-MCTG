using HttpServer;

namespace HttpController.Middleware;

[HttpMiddleware("BodyNotNull")]
public class BodyNotNull : IMiddleware
{
    public MiddlewareResult HandleRequest(HttpRequest req, HttpResponse res)
    {
        // This middleware checks if the request body is null or empty
        if (string.IsNullOrEmpty(req.Body))
        {
            res.Status = 400;
            res.StatusMessage = "Bad Request";
            
            res.Json(new { status = "error", error = "Request body is empty" });

            return new MiddlewareResult(req, res, true);
        }
        else
        {
            return new MiddlewareResult(req, res);
        }
    }
}