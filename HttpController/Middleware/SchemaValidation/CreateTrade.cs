using HttpServer;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace HttpController.Middleware.SchemaValidation;

[HttpMiddleware("SchemaValidation_CreateTrade")]
public class CreateTrade : IMiddleware
{
    public Task<HttpContext> HandleRequest(HttpContext ctx) {
        string schemaJson = @"{
                'type': 'object',
                'properties': {
                    'CardToTrade': { type: 'string' },
                    'Type': { type: 'string' },
                    'MinimumDamage': { type: 'number' }
                }, 
                'required': ['CardToTrade', 'Type', 'MinimumDamage']
            }";
        
        var schema = JSchema.Parse(schemaJson);
        var json = JObject.Parse(ctx.Request.Body ?? "");
        
        if (!json.IsValid(schema))
        {
            ctx.Response.Status = 400;
            ctx.Response.StatusMessage = "Bad Request";
            ctx.Response.Json(new
            {
                status = "error",
                message = "Bad Request. Please provide CardToTrade, Type and MinimumDamage"
            });
        }
        return Task.FromResult(ctx);
    }
}