using DataLayer;
using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Stats;

[HttpEndpoint("/scoreboard", HttpMethod.GET, "Auth")]
public class GetScoreboard : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var scores = await BattleRepository.GetStatsOrderedByElo();

        ctx.Response.Json(new
        {
            status = "ok",
            data = scores
        });

        return ctx;
    }
}