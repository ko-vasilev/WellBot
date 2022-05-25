using System;

namespace WellBot.Domain.Chats.Entities
{
    /// <summary>
    /// Template for automatic messages.
    /// </summary>
    public class AutomaticMessageTemplate
    {
        /// <summary>
        /// Id of the template.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id of the associated chat where message should be sent.
        /// </summary>
        public int ChatId { get; set; }

        /// <summary>
        /// Associated chat where message should be sent.
        /// </summary>
        public Chat Chat { get; set; }

        /// <summary>
        /// Message to send.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Optional text query to search for an image to attach to the message.
        /// </summary>
        public string ImageSearchQuery { get; set; }

        /// <summary>
        /// Interval for sending this message.
        /// </summary>
        public string CronInterval { get; set; }

        /// <summary>
        /// When the last time this message was sent.
        /// </summary>
        public DateTime LastTriggeredDate { get; set; }
    }
}
