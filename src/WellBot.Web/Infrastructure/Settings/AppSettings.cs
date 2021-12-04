using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.Web.Infrastructure.Settings
{
    /// <summary>
    /// Global application settings.
    /// </summary>
    public class AppSettings : ITelegramBotSettings
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
        public string TelegramBotUsername { get; init; }
    }
}
