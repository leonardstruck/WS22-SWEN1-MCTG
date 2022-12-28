using DataLayer;
using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Trading;

[HttpEndpoint("/tradings/*", HttpMethod.DELETE, "Auth", "Auth_MustBeTradeOwner")]
public class DeleteTrade : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var tradeId = Guid.Parse(ctx.Request.GetPathSegments().Last());
        await TradeRepository.DeleteTrade(tradeId);

        return ctx;
    }
}