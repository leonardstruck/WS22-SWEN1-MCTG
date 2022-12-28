using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Cards;

[HttpEndpoint("/tradeableCards", HttpMethod.GET, "Auth")]
public class GetTradeableCards : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var user = ctx.Data["user"] as User ?? throw new Exception("User not found in context");
        var userId = (Guid)user.Id!;

        var tradeableCards = await CardRepository.GetTradeableCardsByOwner(userId);
        
        ctx.Response.Json(new
        {
            status = "ok",
            data = tradeableCards
        });

        return ctx;
    }
}