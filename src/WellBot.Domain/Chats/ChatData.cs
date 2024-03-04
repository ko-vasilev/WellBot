using System.ComponentModel.DataAnnotations;

namespace WellBot.Domain.Chats
{
    /// <summary>
    /// Contains information about a data stored for a chat.
    /// Allows storing a key-value combination where value is either a text or a reference to Telegram document.
    /// </summary>
    public class ChatData
    {
        /// <summary>
        /// Id of the entity.
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
        public Chat Chat { get; set; }

        /// <summary>
        /// Key of this data.
        /// In lower case.
        /// </summary>
        public string Key { get; set; }

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
        /// Indicates if the message in this data contains mention of a user.
        /// </summary>
        public bool HasUserMention { get; set; }
    }
}
