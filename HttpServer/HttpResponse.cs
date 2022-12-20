using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace HttpServer;

public class HttpResponse
{
    private readonly NetworkStream _stream;
    private readonly Encoding _encoding = Encoding.UTF8;
    public string Protocol { get; set; } = "HTTP/1.1";
    public int Status { get; set; } = 200;
    private readonly Dictionary<string, string> _headers = new();

    public void SetHeader(string key, string value)
    {
        _headers[key] = value;
    }


    public string StatusMessage
    {
        get;
        set;
    } = "OK";

    public string? Body { get; set; }
    
    public HttpResponse(NetworkStream stream)
    {
        _stream = stream;
    }
    
    public void Json(object obj)
    {
        Body = JsonSerializer.Serialize(obj);
        SetHeader("Content-Type", "application/json");
    }

    public void Send()
    {
        // Add Content-Length Header
        var length = Body?.Length ?? 0;

        if (length > 0)
        {
            _headers.Add("Content-Length", length.ToString());
            if (!_headers.ContainsKey("Content-Type"))
            {
                // Add default Content-Type Header
                SetHeader("Content-Type", "text/plain; charset=UTF-8");
            }
        }
        
        StringBuilder bld = new();
        
        bld.Append($"{Protocol} {Status} {StatusMessage}");
        bld.Append($"\r\n");

        foreach (var header in _headers)
        {
            bld.Append($"{header.Key}: {header.Value}");
            bld.Append($"\r\n");
        }
        
        bld.Append($"\r\n");

        if (Body != null)
        {
            bld.Append(Body);
        }
        
        byte[] sendBytes = _encoding.GetBytes(bld.ToString());
        
        _stream.Write(sendBytes, 0, sendBytes.Length);
        _stream.Close();
    }
}