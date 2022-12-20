using System.Net.Sockets;
using System.Text;

namespace HttpServer;

public class HttpRequest
{
    public string Method { get; private set; }
    public string Path { get; private set; }
    public string ProtocolVersion { get; private set; }

    public Dictionary<string, string> Headers;
    public Dictionary<string, string?> Params;

    public string? Body { get; private set; }

    private HttpRequest(string method, string path, string protocolVersion, Dictionary<string, string> headers, Dictionary<string, string?> queryParams, string? body)
    {
        Method = method;
        Path = path;
        ProtocolVersion = protocolVersion;
        Headers = headers;
        Body = body;
        Params = queryParams;
    }

    public static HttpRequest CreateInstance(NetworkStream stream)
    {
        StreamReader reader = new(stream);
        var requestLine = ParseRequestLine(reader);
        var headers = ParseHeaders(reader);
        var queryParams = ParseParams(requestLine.Item2);
        var body = ParseBody(int.Parse(headers.GetValueOrDefault("Content-Length", "0")), reader);

        return new HttpRequest(requestLine.Item1, requestLine.Item2, requestLine.Item3, headers, queryParams, body);
    }
    

    private static Tuple<string, string, string> ParseRequestLine(StreamReader reader)
    {
        var requestLine = reader.ReadLine();
        if (string.IsNullOrEmpty(requestLine))
        {
            throw new InvalidDataException("HttpRequest may not begin with an empty line");
        }

        var split = requestLine.Split(" ");
        return new Tuple<string, string, string>(split[0], split[1], split[2]);
    }

    private static Dictionary<string, string> ParseHeaders(StreamReader reader)
    {
        Dictionary<string, string> parsedHeaders = new();
        while (true)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            var split = line.Split(": ");
            parsedHeaders[split[0]] = split[1];
        }

        return parsedHeaders;
    }

    private static Dictionary<string, string?> ParseParams(string path)
    {
        // split path into path and query params
        var split = path.Split("?");
        if (split.Length == 1)
            return new Dictionary<string, string?>();
        
        var queryParams = split[1];
        var splitQueryParams = queryParams.Split("&");
        var parsedParams = new Dictionary<string, string?>();
        
        foreach (var param in splitQueryParams)
        {
            var splitParam = param.Split("=");
            parsedParams[splitParam[0]] = splitParam.Length == 2 ? splitParam[1] : null;
        }
        
        return parsedParams;
    }

    private static string? ParseBody(int contentLength, StreamReader reader)
    {
        if (contentLength == 0)
            return null;

        StringBuilder bld = new();
        while (bld.Length < contentLength)
        {
            bld.Append((char)reader.Read());
        }

        return bld.ToString();
    }
    
    public string[] GetPathSegments()
    {
        return Path.Split("/");
    }
}