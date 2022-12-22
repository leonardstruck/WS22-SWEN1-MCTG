using System.Text;
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
        
        // check if format parameter is set
        if (ctx.Request.Params.ContainsKey("format") && ctx.Request.Params["format"] == "plain")
        {
            StringBuilder sb = new StringBuilder();
            foreach (var card in cardsInDeck)
            {
                sb.AppendLine($"{card.Name}: {card.Damage}");
            }
            // return plain format
            ctx.Response.SetHeader("Content-Type", "text/plain");
            ctx.Response.Body = sb.ToString();
            
            return ctx;
        }

        // serialize cards to json
        ctx.Response.Json(new
        {
            status = "ok",
            data = cardsInDeck
        });

        return ctx;
    }
}