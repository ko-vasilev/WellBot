namespace WellBot.Web.Infrastructure.Settings;

/// <summary>
/// Global application settings.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Telegram bot API token.
    /// </summary>
    public required string BotToken { get; init; }

    /// <summary>
    /// Address of the web application (in https://... format, without the trailing slash).
    /// </summary>
    public required string HostAddress { get; init; }

    /// <summary>
    /// API key for the SerpApi service.
    /// </summary>
    public required string SerpApiKey { get; set; }
}
