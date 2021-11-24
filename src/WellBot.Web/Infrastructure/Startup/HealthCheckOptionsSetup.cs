using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WellBot.Web.Infrastructure.Startup
{
    /// <summary>
    /// The class returns configured health check.
    /// More health checks can be found here https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks .
    /// </summary>
    internal class HealthCheckOptionsSetup
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        /// <summary>
        /// Returns <see cref="HealthCheckOptions" />.
        /// </summary>
        public HealthCheckOptions Setup(HealthCheckOptions options)
        {
            options.ResponseWriter = WriteResponse;
            return options;
        }

        private static async Task WriteResponse(HttpContext context, HealthReport report)
        {
            await context.Response.WriteAsJsonAsync(new
            {
                Status = report.Status,
                Results = report.Entries.Select(e => new
                {
                    Name = e.Key,
                    Status = e.Value.Status,
                    Description = e.Value.Description
                })
            },
                JsonSerializerOptions);
        }
    }
}
