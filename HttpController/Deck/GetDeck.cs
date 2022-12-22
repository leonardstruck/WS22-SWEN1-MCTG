using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Deck;

[HttpEndpoint("/deck", HttpMethod.GET, "Auth")]
public class GetDeck : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var user = ctx.Data["user"] as User ?? throw new Exception("User not found in context");
        var userId = (Guid)user.Id!;

        var cardsInDeck = await CardRepository.GetCardsInDeckByOwner(userId);
        
        // change status code to 204 if no cards in deck
        if (!cardsInDeck.Any())
        {
            ctx.Response.Status = 204;
            ctx.Response.StatusMessage = "No content";
        }
        
        ctx.Response.Json(new
        {
            status = "ok",
            data = cardsInDeck
        });

        return ctx;
    }
}