using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Packages;

[HttpEndpoint("/transactions/packages", HttpMethod.POST, "Auth")]
public class AcquirePackage : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        // get user from context
        var user = ctx.Data["user"] as User ?? throw new Exception("User not found in context");
        var userId =  (Guid) user.Id!;
        
        // try to deduct the package price from the user's balance
        if (!await UserRepository.SpendCoins(userId, 5))
        {
            ctx.Response.Status = 403;
            ctx.Response.StatusMessage = "Forbidden";
            ctx.Response.Json(new
            {
                status = "error",
                message = "Insufficient funds"
            });
            return ctx;
        }
        
        // acquire the package
        var package = await PackageRepository.GetRandomPackage(userId);
        
        // refund the user if no package was available
        if (package == null)
        {
            await UserRepository.RefundCoins(userId, 5);
            ctx.Response.Status = 404;
            ctx.Response.StatusMessage = "Not Found";
            ctx.Response.Json(new
            {
                status = "error",
                message = "No packages available"
            });
            return ctx;
        }
        
        // return the acquired package
        ctx.Response.Json(new
        {
            status = "success",
            data = package
        });

        return ctx;
    }
}