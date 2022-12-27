using DataLayer;
using HttpServer;
using Models;

namespace HttpController.Middleware.Authentication;

[HttpMiddleware("Auth_MustBePlayerInBattle")]
public class MustBePlayerInBattle : IMiddleware
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        // Get the player from the context
        var user = ctx.Data["user"] as User ?? throw new Exception("No user found in context");;
        
        // If user is admin skip the check
        if (user.Username == "admin") return ctx;
        
        var userId = (Guid)user.Id!;
        
        // Get the battle from the context
        var battleId = Guid.Parse(ctx.Request.GetPathSegments().Last());
        
        // Check if the player is in the battle
        if (!await BattleRepository.IsUserParticipantInBattle(battleId, userId))
        {
            ctx.Abort = true;
            ctx.Response.Json(new
            {
                status = "error",
                message = "You are not a participant in this battle"
            });
            return ctx;
        }

        return ctx;
    }
}