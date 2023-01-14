using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Users;

[HttpEndpoint("/dailyReward", HttpMethod.POST, "Auth")]
public class GetDailyReward : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        // get user id from context
        var user = ctx.Data["user"] as User ?? throw new Exception("User not in context");
        var userId = (Guid)user.Id!;
        
        // try to add daily reward to user
        var reward = await UserRepository.CollectDailyReward(userId);
        
        // if reward is false then user already collected reward
        if (reward == false)
        {
            ctx.Response.Status = 400;
            ctx.Response.StatusMessage = "Bad Request";
            ctx.Response.Json(new
            {
                status = "error",
                message = "You already collected your daily reward"
            });
            
            return ctx;
        }
        
        ctx.Response.Json(new
        {
            status = "ok"
        });
        return ctx;
    }
}