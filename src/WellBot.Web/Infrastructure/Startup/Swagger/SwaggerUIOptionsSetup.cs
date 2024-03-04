using Swashbuckle.AspNetCore.SwaggerUI;

namespace WellBot.Web.Infrastructure.Startup.Swagger;

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
        // Preserve authorization token after browser page refresh.
        options.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    }
}
