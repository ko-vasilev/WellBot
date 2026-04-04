using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WellBot.Web.Infrastructure.Startup.Swagger;

/// <summary>
/// Generates standard example for Swagger document properties. For example it puts
/// correct values for "address1", "state", "email" fields.
/// </summary>
internal sealed class SwaggerExampleSetterSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Maps property name to example value.
    /// </summary>
    private static readonly IDictionary<string, JsonNode> propertyNameExampleMap =
        new Dictionary<string, JsonNode>
        {
            // General.
            ["email"] = JsonValue.Create("test@example.com")!,
            ["firstname"] = JsonValue.Create("John")!,
            ["lastname"] = JsonValue.Create("Doe")!,
            ["password"] = JsonValue.Create("123")!,
            ["token"] = JsonValue.Create("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9eyJzdWIiOiJmd2QyaXZhbkBnbWFpbC5jb20")!,
            ["accessToken"] = JsonValue.Create("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9eyJzdWIiOiJmd2QyaXZhbkBnbWFpbC5jb20")!,
            ["refreshToken"] = JsonValue.Create("gjofdjaojoas23fweok")!,
            ["expires"] = JsonValue.Create(3600)!,
            ["color"] = JsonValue.Create("#00ff00")!,
            ["username"] = JsonValue.Create("johndoe")!,
            ["url"] = JsonValue.Create("https://example.org")!,
            ["state"] = JsonValue.Create("CA")!,
            ["country"] = JsonValue.Create("USA")!,
            ["phone"] = JsonValue.Create("523-523-1129")!,
            ["phone1"] = JsonValue.Create("643-234-6734")!,
            ["phone2"] = JsonValue.Create("123-634-2167")!,
            ["skype"] = JsonValue.Create("skypeacc")!,
            ["address1"] = JsonValue.Create("555 East Main St., Suite 5")!,
            ["address2"] = JsonValue.Create("Chester, NJ 07930")!,
            ["ip"] = JsonValue.Create("192.168.11.103")!,
            ["year"] = JsonValue.Create(DateTime.Now.Year.ToString())!,
            ["startdate"] = JsonValue.Create(DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd"))!,
            ["enddate"] = JsonValue.Create(DateTime.Now.ToString("yyyy-MM-dd"))!

            // Custom.
        };

    /// <inheritdoc />
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is not OpenApiSchema mutableSchema || mutableSchema.Properties == null)
        {
            return;
        }

        foreach (var property in mutableSchema.Properties)
        {
            if (property.Value is not OpenApiSchema propertySchema)
            {
                continue;
            }

            var key = property.Key.ToLower();
            if (propertyNameExampleMap.TryGetValue(key, out var example))
            {
                propertySchema.Example = example;
            }
            else if (key.EndsWith("date", StringComparison.OrdinalIgnoreCase))
            {
                propertySchema.Example = JsonValue.Create(DateTime.Now.ToString("yyyy-MM-dd"))!;
            }
            else if (key.EndsWith("time", StringComparison.OrdinalIgnoreCase))
            {
                propertySchema.Example = JsonValue.Create("10:25:00")!;
            }
        }
    }
}
