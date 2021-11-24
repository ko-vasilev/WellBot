using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
        [MaxLength(100)]
        public string TelegramId { get; set; }
    }
}
