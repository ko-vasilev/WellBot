using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WellBot.Domain.Chats
{
    /// <summary>
    /// Contains info about a daily pidor game winner.
    /// </summary>
    public class ChatPidor
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Id of the associated registration.
        /// </summary>
        public int RegistrationId { get; set; }

        /// <summary>
        /// Associated registration.
        /// </summary>
        public PidorRegistration Registration { get; set; }

        /// <summary>
        /// Id of the associated chat.
        /// </summary>
        public int ChatId { get; set; }

        /// <summary>
        /// Associated chat.
        /// </summary>
        public Chat Chat { get; set; }

        /// <summary>
        /// Day for which the game was played.
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime GameDay { get; set; }

        /// <summary>
        /// Id of the message used to notify about the game results.
        /// </summary>
        public int UsedMessageId { get; set; }

        /// <summary>
        /// Message used to notify about the game results.
        /// </summary>
        public PidorMessage UsedMessage { get; set; }
    }
}
