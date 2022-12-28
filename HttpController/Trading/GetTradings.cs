using DataLayer;
using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Trading;

[HttpEndpoint("/tradings", HttpMethod.GET, "Auth")]
public class GetTradings : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var tradings = await TradeRepository.GetTrades();

        if (tradings.Length == 0)
        {
            ctx.Response.Status = 204;
            ctx.Response.StatusMessage = "No Content";
            ctx.Response.Json(new
            {
                status = "ok",
                message = "No tradings found",
                data = tradings,
            });
            return ctx;
        }
        
        ctx.Response.Json(new
        {
            status = "ok",
            data = tradings
        });
        
        return ctx;
    }
}