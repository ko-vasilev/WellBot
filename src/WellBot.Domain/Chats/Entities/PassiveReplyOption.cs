using System.ComponentModel.DataAnnotations;

namespace WellBot.Domain.Chats.Entities
{
    /// <summary>
    /// Option that can be used by bot as a passive reply to some messages.
    /// </summary>
    public class PassiveReplyOption
    {
        /// <summary>
        /// Id of the option.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Type of the stored value.
        /// </summary>
        public DataType DataType { get; set; }

        /// <summary>
        /// Id of the file.
        /// </summary>
        [MaxLength(150)]
        public string FileId { get; set; }

        /// <summary>
        /// Any additional text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Indicates if this message should be sent for direct messages to bot (via mention or replies).
        /// </summary>
        public bool IsDirectMessage { get; set; }

        /// <summary>
        /// Indicates if this message should be sent for messages related to Dota.
        /// </summary>
        public bool IsDota { get; set; }

        /// <summary>
        /// Indicates if this message should be sent for memes.
        /// </summary>
        public bool IsMeme { get; set; }
    }
}
