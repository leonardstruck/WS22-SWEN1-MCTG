namespace HttpServer;

public class HttpContext
{
    public HttpRequest Request { get; set; }
    public HttpResponse Response { get; set; }
    public Dictionary<string, object> Data { get; set; }
    
    public HttpContext(HttpRequest request, HttpResponse response)
    {
        Request = request;
        Response = response;
        Data = new Dictionary<string, object>();
    }

    public bool Abort { get; set; } = false;
}