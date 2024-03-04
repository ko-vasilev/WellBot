using Microsoft.AspNetCore.Cors.Infrastructure;

namespace WellBot.Web.Infrastructure.Startup;

/// <summary>
/// CORS options setup.
/// </summary>
internal class CorsOptionsSetup
{
    public const string CorsPolicyName = "AllowFrontend";

    private readonly bool isDevelopment;
    private readonly string[] frontendOrigins;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="isDevelopment">Is development mode enabled.</param>
    /// <param name="frontendOrigins">Frontend origins.</param>
    public CorsOptionsSetup(bool isDevelopment, params string[]? frontendOrigins)
    {
        this.isDevelopment = isDevelopment;
        this.frontendOrigins = frontendOrigins ?? new string[] { };
    }

    /// <summary>
    /// Setup CORS method.
    /// </summary>
    /// <param name="options">CORS options.</param>
    public void Setup(CorsOptions options)
    {
        options.AddPolicy(CorsPolicyName,
            builder =>
            {
                if (isDevelopment)
                {
                    builder.AllowAnyOrigin();
                }
                else
                {
                    builder.WithOrigins(frontendOrigins.ToArray())
                        .AllowCredentials();
                }
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetPreflightMaxAge(TimeSpan.FromDays(1));
            });
    }
}
