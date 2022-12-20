namespace HttpServer;

public interface IMiddleware
{
    public MiddlewareResult HandleRequest(HttpRequest req, HttpResponse res);
}

public class MiddlewareResult
{
    public HttpRequest Request;
    public HttpResponse Response;
    public bool Abort = false;
    public object? additionalData = null;
    
    public MiddlewareResult(HttpRequest req, HttpResponse res)
    {
        Request = req;
        Response = res;
    }
    
    public MiddlewareResult(HttpRequest req, HttpResponse res, bool abort)
    {
        Request = req;
        Response = res;
        Abort = abort;
    }
    
    public MiddlewareResult(HttpRequest req, HttpResponse res, bool abort, object? data)
    {
        Request = req;
        Response = res;
        Abort = abort;
        additionalData = data;
    }
    
    public MiddlewareResult(HttpRequest req, HttpResponse res, object? data)
    {
        Request = req;
        Response = res;
        additionalData = data;
    }
}