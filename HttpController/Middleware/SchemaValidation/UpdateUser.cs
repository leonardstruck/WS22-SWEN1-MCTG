using HttpServer;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace HttpController.Middleware.SchemaValidation;

[HttpMiddleware("SchemaValidation_UpdateUser")]
public class UpdateUser : IMiddleware
{
    public Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        string schemaJson = @"{
            'type': 'object',
            'properties': {
                'Name': {
                    'type': 'string',
                },
                'Bio': {
                    'type': 'string',
                },
                'Image': {
                    'type': 'string',
                },
            },
            'required': ['Name', 'Bio', 'Image']
        }";

        JSchema schema = JSchema.Parse(schemaJson);
        JObject json = JObject.Parse(ctx.Request.Body ?? string.Empty);

        if (!json.IsValid(schema))
        {
            ctx.Abort = true;
            ctx.Response.Status = 400;
            ctx.Response.Json(new {status="error", message="Bad request. Please provide Name, Bio and Image."});
        }

        return Task.FromResult(ctx);
    }
}