using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Trading;


[HttpEndpoint("/tradings", HttpMethod.POST, "SchemaValidation_CreateTrade", "Auth")]
public class CreateTrade : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var user = ctx.Data["user"] as User ?? throw new Exception("User not found in context");
        var userId = (Guid)user.Id!;

        var trade = ctx.Request.DeserializeBody<Trade>();

        // verify that the user has the card
        if(!await CardRepository.VerifyCardOwnership(trade.CardToTrade, userId))
        {
            ctx.Response.Status = 403;
            ctx.Response.StatusMessage = "Forbidden";
            ctx.Response.Json(new
            {
                status = "error",
                message = "You do not own this card"
            });
            
            return ctx;
        }
        
        // verify that the card is not already being traded
        if (await TradeRepository.CheckIfTradeExists(trade.CardToTrade))
        {
            ctx.Response.Status = 409;
            ctx.Response.StatusMessage = "Conflict";
            ctx.Response.Json(new
            {
                status = "error",
                message = "This card is already being traded"
            });
            
            return ctx;
        }
        
        // create trading offer
        await TradeRepository.AddTrade(trade);

        ctx.Response.Json( new
        {
            status = "ok",
            message = "Trade created"
        });
        return ctx;
    }
}