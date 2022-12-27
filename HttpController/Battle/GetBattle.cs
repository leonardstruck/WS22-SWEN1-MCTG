using DataLayer;
using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Battle;

[HttpEndpoint("/battles/*", HttpMethod.GET, "Auth", "Auth_MustBePlayerInBattle")]
public class GetBattle : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var battleId = Guid.Parse(ctx.Request.GetPathSegments().Last());

        var battle = await BattleRepository.GetBattle(battleId);

        if (battle == null)
        {
            ctx.Response.Status = 404;
            ctx.Response.StatusMessage = "Not Found";
            ctx.Response.Json(new
            {
                status = "error",
                message = "Battle not found"
            });
            return ctx;
        }
        
        ctx.Response.Json(battle);

        return ctx;
    }
}