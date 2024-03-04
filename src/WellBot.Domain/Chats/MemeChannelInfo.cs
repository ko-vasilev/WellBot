using System.ComponentModel.DataAnnotations;

namespace WellBot.Domain.Chats
{
    /// <summary>
    /// Information about a channel that will be used to get memes.
    /// </summary>
    public class MemeChannelInfo
    {
        /// <summary>
        /// Data identifier.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Id of the channel.
        /// </summary>
        public long ChannelId { get; set; }

        /// <summary>
        /// Id of the latest message in the channel.
        /// </summary>
        public int LatestMessageId { get; set; }
    }
}
