namespace WellBot.Domain.Chats;

/// <summary>
/// A summary information about a message or a certain event in the chat.
/// </summary>
public class MessageLog
{
    /// <summary>
    /// Id of the message.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Id of the chat.
    /// </summary>
    public int ChatId { get; set; }

    /// <summary>
    /// Chat in which the message occurred.
    /// </summary>
    public Chat? Chat { get; set; }

    /// <summary>
    /// Date when the message was sent.
    /// </summary>
    public DateTime MessageDate { get; set; }

    /// <summary>
    /// Message or information about an event.
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Name of the person who sent the message.
    /// </summary>
    public required string Sender { get; set; }
}
