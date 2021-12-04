namespace WellBot.Infrastructure.Abstractions.Interfaces
{
    /// <summary>
    /// Contains settings for telegram bot.
    /// </summary>
    public interface ITelegramBotSettings
    {
        /// <summary>
        /// Username of the telegram bot.
        /// </summary>
        string TelegramBotUsername { get; }
    }
}
