using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Deck;

[HttpEndpoint("/deck", HttpMethod.PUT, "Auth")]
public class ConfigureDeck : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        // Get User ID
        var user = ctx.Data["user"] as User ?? throw new Exception("User not found in context");
        var userId = (Guid)user.Id!;
        
        var cardIds = ctx.Request.DeserializeBody<List<Guid>>();
        
        // check if the required amount of card ids is present
        if (cardIds.Count != 4)
        {
            ctx.Response.Status = 400;
            ctx.Response.StatusMessage = "Bad request";
            ctx.Response.Json(new
            {
                status = "error",
                message = "You need to provide 4 card ids"
            });

            return ctx;
        }

        if(!await CardRepository.PutCardsInDeck(userId, cardIds.ToArray()))
        {
            ctx.Response.Status = 403;
            ctx.Response.StatusMessage = "Forbidden";
            ctx.Response.Json(new
            {
                status = "error",
                message = "At least one of the provided cards does not belong to the user or is not available."
            });

            return ctx;
        }
        
        ctx.Response.Json(new
        {
            status = "ok",
            message = "The deck has been successfully configured."
        });

        return ctx; 
    }
}