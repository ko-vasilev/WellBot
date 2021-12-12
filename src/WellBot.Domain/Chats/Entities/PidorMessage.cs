using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WellBot.Domain.Chats.Entities
{
    /// <summary>
    /// A message that can be used to notify about the game results.
    /// </summary>
    public class PidorMessage
    {
        /// <summary>
        /// Symbol that is used to separate messages that should be sent as separate messages.
        /// </summary>
        public const string MessagesSeparator = "|";

        /// <summary>
        /// Placeholder that will be replaced with username (i.e. @testuser).
        /// </summary>
        public const string UsernamePlaceholder = "{0}";

        /// <summary>
        /// Id.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Raw message text.
        /// </summary>
        public string MessageRaw { get; set; }

        /// <summary>
        /// Weight of the message. Messages with higher weight have higher chance of being chosen.
        /// </summary>
        public MessageWeight Weight { get; set; }

        /// <summary>
        /// Id of telegram user this message should be applied to.
        /// </summary>
        public long? TelegramUserId { get; set; }

        /// <summary>
        /// Allows to specify a specific day this message is active on.
        /// When used, a year is ignored.
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? GameDay { get; set; }

        /// <summary>
        /// Day of week this message should be triggered on.
        /// </summary>
        public DayOfWeek? DayOfWeek { get; set; }

        /// <summary>
        /// List of games played where this message was used for notification.
        /// </summary>
        public IList<ChatPidor> UsedWins { get; set; } = new List<ChatPidor>();

        /// <summary>
        /// Indicates if a message is active and can be used.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
