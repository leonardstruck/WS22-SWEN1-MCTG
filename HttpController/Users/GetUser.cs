using DataLayer;
using HttpServer;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Users;

[HttpEndpoint("/users/*", HttpMethod.GET, "Auth")]
public class GetUser : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        // get username from url
        var username = ctx.Request.GetPathSegments().Last();

        // get user from database
        var user = await UserRepository.GetUserByUsername(username);

        if (user == null)
        {
            ctx.Response.Status = 404;
            ctx.Response.StatusMessage = "Not Found";
            
            return ctx;
        }
        
        // return user as json
        ctx.Response.Json(new
        {
            status = "ok",
            data = new
            {
                user.Id,
                user.Username,
                user.Name,
                user.Bio,
                user.Image
            }
        });
        
        return ctx;
    }
}