using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Cards;

[HttpEndpoint("/cards", HttpMethod.GET, "Auth")]
public class GetCards : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var user = ctx.Data["user"] as User ?? throw new Exception("User not found in context");
        
        var userId = (Guid)user.Id!;

        var cards = await CardRepository.GetCardsByOwner(userId);
        
        // return cards if there are any
        if (!cards.Any())
        {
            ctx.Response.Status = 204;
            ctx.Response.StatusMessage = "No content";
        }
        
        ctx.Response.Json(new
        {
            status = "ok",
            data = cards
        });

        return ctx;
    }
}