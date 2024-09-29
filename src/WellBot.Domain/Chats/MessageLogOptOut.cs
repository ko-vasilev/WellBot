namespace WellBot.Domain.Chats;

/// <summary>
/// Contains information about users who opted out from message log tracing.
/// </summary>
public class MessageLogOptOut
{
    /// <summary>
    /// Id of the entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Id of the chat in which user has opted out of logging.
    /// </summary>
    public int ChatId { get; set; }

    /// <summary>
    /// Chat reference.
    /// </summary>
    public Chat? Chat { get; set; }

    /// <summary>
    /// Id of the user in Telegram who opted out of receiving messages.
    /// </summary>
    public required long TelegramUserId { get; set; }
}
