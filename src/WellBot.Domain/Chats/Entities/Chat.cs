using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WellBot.Domain.Chats.Entities
{
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
        /// List of registrations for daily Pidor game in this chat.
        /// </summary>
        public IList<PidorRegistration> PidorRegistrations { get; set; } = new List<PidorRegistration>();

        /// <summary>
        /// List of chat pidor winners.
        /// </summary>
        public IList<ChatPidor> Pidors { get; set; } = new List<ChatPidor>();
    }
}
