using DataLayer;
using HttpServer;
using Models;

namespace HttpController.Middleware.Authentication;

[HttpMiddleware("Auth_MustNotBeTradeOwner")]
public class MustNotBeTradeOwner : IMiddleware
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var user = ctx.Data["user"] as User ?? throw new Exception("User not found in context");
        var userId = (Guid)user.Id!;

        var tradeId = ctx.Request.GetPathSegments().Last();

        var tradeOwner = await TradeRepository.GetTradeOwner(Guid.Parse(tradeId));

        if (tradeOwner == null)
        {
            ctx.Response.Status = 404;
            ctx.Response.StatusMessage = "Not Found";
            ctx.Response.Json(new
            {
                status = "error",
                message = "Trade not found"
            });
            ctx.Abort = true;
            return ctx;
        }
        if (tradeOwner != userId) return ctx;
        
        ctx.Response.Status = 403;
        ctx.Response.StatusMessage = "Forbidden";
        ctx.Response.Json(new
        {
            status = "error",
            message = "You cannot perform this action on your own trade"
        });
        ctx.Abort = true;

        return ctx;    }
}