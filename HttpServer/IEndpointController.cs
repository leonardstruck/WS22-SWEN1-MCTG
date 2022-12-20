namespace HttpServer;

public interface IEndpointController
{ 
    public void HandleRequest(HttpRequest req, HttpResponse res);
}