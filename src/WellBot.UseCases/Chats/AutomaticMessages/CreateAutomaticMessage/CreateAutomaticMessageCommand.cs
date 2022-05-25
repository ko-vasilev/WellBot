using System;
using MediatR;
using WellBot.Domain.Chats.Entities;

namespace WellBot.UseCases.Chats.AutomaticMessages.CreateAutomaticMessage
{
    /// <summary>
    /// Create a new automatic message.
    /// </summary>
    public class CreateAutomaticMessageCommand : IRequest<int>
    {
        /// <inheritdoc cref="AutomaticMessageTemplate.ChatId"/>
        public int ChatId { get; set; }

        /// <inheritdoc cref="AutomaticMessageTemplate.Message"/>
        public string Message { get; set; }

        /// <inheritdoc cref="AutomaticMessageTemplate.ImageSearchQuery"/>
        public string ImageSearchQuery { get; set; }

        /// <inheritdoc cref="AutomaticMessageTemplate.CronInterval"/>
        public string CronInterval { get; set; }

        /// <summary>
        /// Time after which the job should start running.
        /// </summary>
        public DateTime RunFrom { get; set; }
    }
}
