namespace WellBot.Infrastructure.Abstractions.Interfaces
{
    /// <summary>
    /// Contains settings for telegram bot.
    /// </summary>
    public interface ITelegramBotSettings
    {
        /// <summary>
        /// Username of the telegram bot without the @ symbol.
        /// </summary>
        string TelegramBotUsername { get; }

        /// <summary>
        /// Defines a probability of bot replying to a random message.
        /// This will be converted into "1 in X".
        /// </summary>
        int RegularPassiveRepliesProbability { get; }
    }
}
