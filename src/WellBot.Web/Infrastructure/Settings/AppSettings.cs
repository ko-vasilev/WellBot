namespace WellBot.Web.Infrastructure.Settings
{
    /// <summary>
    /// Global application settings.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Telegram bot API token.
        /// </summary>
        public string BotToken { get; init; }

        /// <summary>
        /// Address of the web application (in https://... format, without the trailing slash).
        /// </summary>
        public string HostAddress { get; init; }

        /// <inheritdoc />
        public int RegularPassiveRepliesProbability { get; init; }

        /// <summary>
        /// API key for the SerpApi service.
        /// </summary>
        public string SerpApiKey { get; set; }
    }
}
