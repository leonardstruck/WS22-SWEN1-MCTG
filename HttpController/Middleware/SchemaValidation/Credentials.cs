using HttpServer;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace HttpController.Middleware.SchemaValidation;

[HttpMiddleware("SchemaValidation_Credentials")]
public class SchemaValidationCredentials : IMiddleware
{
    public Task<HttpContext> HandleRequest(HttpContext ctx)
    {
        string schemaJson = @"{
            'type': 'object',
            'properties': {
                'Username': {
                    'type': 'string',
                },
                'Password': {
                    'type': 'string',
                }
            },
            'required': ['Username', 'Password']
        }";

        JSchema schema = JSchema.Parse(schemaJson);
        JObject json = JObject.Parse(ctx.Request.Body ?? string.Empty);

        if (!json.IsValid(schema))
        {
            ctx.Abort = true;
            ctx.Response.Status = 400;
            ctx.Response.Json(new {status="error", message="Bad request. Please provide Username and Password."});
        }

        return Task.FromResult(ctx);
    }
}