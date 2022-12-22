using HttpServer;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace HttpController.Middleware.SchemaValidation;

[HttpMiddleware("SchemaValidation_CreatePackage")]
public class CreatePackage : IMiddleware
{
    public Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        string schemaJson = @"{
            'type': 'array',
            'items': {
                'type': 'object',
                'properties': {
                    'Name': {
                        'type': 'string'
                    },
                    'Damage': {
                        'type': 'integer'
                    },
                },
            },
            maxItems: 5,
            minItems: 5
        }";
        
        var schema = JSchema.Parse(schemaJson);
        var json = JArray.Parse(ctx.Request.Body ?? "");
        
        if (!json.IsValid(schema))
        {
            ctx.Response.Status = 400;
            ctx.Response.StatusMessage = "Bad Request";
            ctx.Response.Json(new
            {
                status = "error",
                message = "Bad Request. Please provide array of 5 objects with Name and Damage properties"
            });
        }
        return Task.FromResult(ctx);
    }
}