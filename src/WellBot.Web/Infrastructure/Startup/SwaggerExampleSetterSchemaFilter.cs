using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WellBot.Web.Infrastructure.Startup
{
    /// <summary>
    /// Generates standard example for Swagger document properties. For example it puts
    /// correct values for "address1", "state", "email" fields.
    /// </summary>
    internal sealed class SwaggerExampleSetterSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Maps property name to example value.
        /// </summary>
        private static readonly IDictionary<string, IOpenApiPrimitive> propertyNameExampleMap =
            new Dictionary<string, IOpenApiPrimitive>
            {
                // General.
                ["email"] = new OpenApiString("test@example.com"),
                ["firstname"] = new OpenApiString("John"),
                ["lastname"] = new OpenApiString("Doe"),
                ["password"] = new OpenApiPassword("123"),
                ["token"] = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9eyJzdWIiOiJmd2QyaXZhbkBnbWFpbC5jb20"),
                ["accessToken"] = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9eyJzdWIiOiJmd2QyaXZhbkBnbWFpbC5jb20"),
                ["refreshToken"] = new OpenApiString("gjofdjaojoas23fweok"),
                ["expires"] = new OpenApiInteger(3600),
                ["color"] = new OpenApiString("#00ff00"),
                ["username"] = new OpenApiString("johndoe"),
                ["url"] = new OpenApiString("https://example.org"),
                ["state"] = new OpenApiString("CA"),
                ["country"] = new OpenApiString("USA"),
                ["phone"] = new OpenApiString("523-523-1129"),
                ["phone1"] = new OpenApiString("643-234-6734"),
                ["phone2"] = new OpenApiString("123-634-2167"),
                ["skype"] = new OpenApiString("skypeacc"),
                ["address1"] = new OpenApiString("555 East Main St., Suite 5"),
                ["address2"] = new OpenApiString("Chester, NJ 07930"),
                ["ip"] = new OpenApiString("192.168.11.103"),
                ["year"] = new OpenApiString(DateTime.Now.Year.ToString()),
                ["startdate"] = new OpenApiString(DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd")),
                ["enddate"] = new OpenApiString(DateTime.Now.ToString("yyyy-MM-dd"))

                // Custom.
            };

        /// <inheritdoc />
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null)
            {
                return;
            }

            foreach (var property in schema.Properties)
            {
                var key = property.Key.ToLower();
                if (propertyNameExampleMap.TryGetValue(key, out IOpenApiPrimitive example))
                {
                    property.Value.Example = example;
                }
                else if (key.EndsWith("date", StringComparison.OrdinalIgnoreCase))
                {
                    property.Value.Example = new OpenApiString(DateTime.Now.ToString("yyyy-MM-dd"));
                }
                else if (key.EndsWith("time", StringComparison.OrdinalIgnoreCase))
                {
                    property.Value.Example = new OpenApiString("10:25:00");
                }
            }
        }
    }
}
