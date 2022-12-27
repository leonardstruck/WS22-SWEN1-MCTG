using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Stats;

[HttpEndpoint("/stats", HttpMethod.GET, "Auth")]
public class GetStats : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        // get user from context
        var user = ctx.Data["user"] as User ?? throw new Exception("User not found in context");
        var userId = (Guid)user.Id!;
        
        // get stats from database
        var stats = await BattleRepository.GetStatsByUserId(userId);
        
        ctx.Response.Json(new
        {
            status = "ok",
            data = stats
        });
        
        return ctx;
    }
}