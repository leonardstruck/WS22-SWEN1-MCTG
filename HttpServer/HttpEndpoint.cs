namespace HttpServer;

public class HttpEndpointAttribute : System.Attribute
{
    public string Path { get; }
    public HttpMethod Method { get; }

    public HttpEndpointAttribute(string path, HttpMethod method)
    {
        Path = path;
        Method = method;
    }
}