using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.Web.Infrastructure.Telegram;

/// <summary>
/// Contains bot-specific settings.
/// </summary>
public class TelegramBotSettings : ITelegramBotSettings
{
    /// <inheritdoc />
    public required string TelegramBotUsername { get; set; }
}
