using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Trading;

[HttpEndpoint("/tradings/*", HttpMethod.POST, "BodyNotNull", "Auth", "Auth_MustNotBeTradeOwner")]
public class HandleTrade : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var user = ctx.Data["user"] as User ?? throw new Exception("User not found in context");
        var userId = (Guid)user.Id!;
        
        var tradeId = Guid.Parse(ctx.Request.GetPathSegments().Last());
        var tradeCardId = Guid.Parse(ctx.Request.Body!);

        // check if offered card is owned by user
        if (!await CardRepository.VerifyCardOwnership(tradeCardId, userId))
        {
            ctx.Response.Status = 403;
            ctx.Response.StatusMessage = "Forbidden";
            ctx.Response.Json(new
            {
                status = "error",
                message = "You do not own the card you are trying to trade"
            });
            
            return ctx;
        }
        if (!await TradeRepository.CheckIfOfferedCardMatchesCriteria(tradeId, tradeCardId))
        {
            ctx.Response.Status = 403;
            ctx.Response.StatusMessage = "Forbidden";
            ctx.Response.Json(new
            {
                status = "error",
                message = "The card you are trying to trade does not match the criteria of the trade"
            });
            
            return ctx;
        }

        await TradeRepository.CarryOutTrade(tradeId, tradeCardId);
        return ctx;
    }
}