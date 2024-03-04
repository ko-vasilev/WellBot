using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WellBot.Domain.Chats;

/// <summary>
/// Contains a registration information for Daily Pidor game.
/// </summary>
public class PidorRegistration
{
    /// <summary>
    /// Id of the registration.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Id of the associated chat.
    /// </summary>
    public int ChatId { get; set; }

    /// <summary>
    /// Associated chat.
    /// </summary>
    [ForeignKey(nameof(ChatId))]
    public Chat? Chat { get; set; }

    /// <summary>
    /// Id of the Telegram user.
    /// </summary>
    public long TelegramUserId { get; set; }

    /// <summary>
    /// When the registration happened.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// When the user has won the game with this registration.
    /// </summary>
    public IList<ChatPidor> Wins { get; set; } = new List<ChatPidor>();

    /// <summary>
    /// Name user had in telegram when he signed up for the game.
    /// </summary>
    [MaxLength(63)]
    public required string OriginalTelegramUserName { get; set; }
}
