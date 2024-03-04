using WellBot.Domain.Chats;

namespace WellBot.UseCases.Chats.Dtos
{
    /// <summary>
    /// Contains a basic information about a message.
    /// </summary>
    public class GenericMessage
    {
        /// <summary>
        /// If this message contains a file - id of that file.
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// Type of the message data.
        /// </summary>
        public DataType DataType { get; set; }

        /// <summary>
        /// Text associated with the message.
        /// </summary>
        public string Text { get; set; }
    }
}
