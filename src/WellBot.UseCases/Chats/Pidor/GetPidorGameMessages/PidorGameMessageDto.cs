using System;
using WellBot.Domain.Chats;

namespace WellBot.UseCases.Chats.Pidor.GetPidorGameMessages
{
    /// <summary>
    /// Contains basic information about pidor game messages.
    /// </summary>
    public class PidorGameMessageDto
    {
        /// <inheritdoc cref="PidorMessage.Id"/>
        public int Id { get; set; }

        /// <inheritdoc cref="PidorMessage.MessageRaw"/>
        public string MessageRaw { get; set; }

        /// <inheritdoc cref="PidorMessage.Weight"/>
        public MessageWeight Weight { get; set; }

        /// <inheritdoc cref="PidorMessage.TelegramUserId"/>
        public long? TelegramUserId { get; set; }

        /// <inheritdoc cref="PidorMessage.GameDay"/>
        public DateTime? GameDay { get; set; }

        /// <inheritdoc cref="PidorMessage.DayOfWeek"/>
        public DayOfWeek? DayOfWeek { get; set; }
    }
}
