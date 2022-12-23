using DataLayer;
using HttpServer;
using Models;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Users;

[HttpEndpoint("/users/*", HttpMethod.PUT,
    "Auth", "Auth_PathMustMatchToUsername", "BodyNotNull", "SchemaValidation_UpdateUser")]
public class UpdateUser : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var user = ctx.Data["user"] as User ?? throw new Exception("User not in context");

        var updatedUser = ctx.Request.DeserializeBody<User>();
        
        // Add user Id to updated user
        updatedUser.Id = user.Id;
        updatedUser.Username = user.Username;
        
        // Update user
        await UserRepository.UpdateUser(updatedUser);

        ctx.Response.Json(new
        {
            status = "ok",
            message = "User updated",
            data = new
            {
                updatedUser.Id,
                updatedUser.Username,
                updatedUser.Name,
                updatedUser.Bio,
                updatedUser.Image
            }
        });
        return ctx;
    }
}