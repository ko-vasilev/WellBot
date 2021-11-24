using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace WellBot.Web.Infrastructure.Startup
{
    /// <summary>
    /// Swagger UI options.
    /// </summary>
    internal class SwaggerUIOptionsSetup
    {
        /// <summary>
        /// Setup.
        /// </summary>
        /// <param name="options">Swagger generation options.</param>
        public void Setup(SwaggerUIOptions options)
        {
            options.ShowExtensions();
            options.SwaggerEndpoint("/swagger/v1/swagger.json?v=1", "API Documentation");
            options.EnableValidator();
            options.EnableDeepLinking();
            options.DisplayOperationId();
        }
    }
}
