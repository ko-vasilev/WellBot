using System;
using WellBot.Domain.Chats.Entities;

namespace WellBot.UseCases.Chats.AutomaticMessages.GetAutomaticMessages
{
    /// <summary>
    /// Contains information about an automatic message.
    /// </summary>
    public class AutomaticMessageDto
    {
        /// <inheritdoc cref="AutomaticMessageTemplate.Id"/>
        public int Id { get; set; }

        /// <inheritdoc cref="AutomaticMessageTemplate.ChatId"/>
        public int ChatId { get; set; }

        /// <inheritdoc cref="AutomaticMessageTemplate.Message"/>
        public string Message { get; set; }

        /// <inheritdoc cref="AutomaticMessageTemplate.ImageSearchQuery"/>
        public string ImageSearchQuery { get; set; }

        /// <inheritdoc cref="AutomaticMessageTemplate.CronInterval"/>
        public string CronInterval { get; set; }

        /// <inheritdoc cref="AutomaticMessageTemplate.LastTriggeredDate"/>
        public DateTime LastTriggeredDate { get; set; }
    }
}
