using DataLayer;
using HttpServer;
using Models.Card;
using HttpMethod = HttpServer.HttpMethod;

namespace HttpController.Packages;

[HttpEndpoint("/packages", HttpMethod.POST, 
    "Auth", "Auth_MustBeAdmin", "SchemaValidation_CreatePackage")]
public class CreatePackage : IEndpointController
{
    public async Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        var genericCards = ctx.Request.DeserializeBody<List<GenericCard>>();

        await PackageRepository.CreatePackage(genericCards.ToArray());

        ctx.Response.Status = 201;
        ctx.Response.StatusMessage = "Created";
        ctx.Response.Json(new {status = "ok"});
        return ctx;
    }
}