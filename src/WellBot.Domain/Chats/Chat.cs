using System.ComponentModel.DataAnnotations;

namespace WellBot.Domain.Chats;

/// <summary>
/// Contains information about a chat.
/// </summary>
public class Chat
{
    /// <summary>
    /// Chat id.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Chat id in Telegram.
    /// </summary>
    [Required]
    public long TelegramId { get; set; }

    /// <summary>
    /// Associated chat data.
    /// </summary>
    public IList<ChatData> Data { get; set; } = new List<ChatData>();

    /// <summary>
    /// History of messages that were sent in the chat.
    /// </summary>
    public IList<MessageLog> MessageLogs { get; set; } = new List<MessageLog>();

    /// <summary>
    /// Users who opted out from their messages being logged in this chat.
    /// </summary>
    public IList<MessageLogOptOut> MessageLogOptOuts { get; set; } = new List<MessageLogOptOut>();
}
