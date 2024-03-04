using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WellBot.Domain.Chats
{
    /// <summary>
    /// Specific conversation topic.
    /// </summary>
    public class PassiveTopic
    {
        /// <summary>
        /// Id of the entity.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of the topic.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Regex that would trigger this topic.
        /// </summary>
        public string Regex { get; set; }

        /// <summary>
        /// Whether or not the topic should be triggered when the message was sent by replying to bot's message
        /// rr by mentioning the bot.
        /// </summary>
        public bool? IsDirectMessage { get; set; }

        /// <summary>
        /// Whether or not the topic should be triggered when a message is a "meme".
        /// </summary>
        public bool? IsMeme { get; set; }

        /// <summary>
        /// Indicates if the topic should be triggered when a message is an "audio".
        /// </summary>
        public bool? IsAudio { get; set; }

        /// <summary>
        /// Indicates if this topic is exclusive and no other topics should be used with it.
        /// </summary>
        public bool IsExclusive { get; set; }

        /// <summary>
        /// Defines a probability of trigger to the topic.
        /// The lower this value, the higher the chance of triggering the topic.
        /// For example, setting this to 1 would trigger this topic on every matching message.
        /// </summary>
        public int Probability { get; set; }

        /// <summary>
        /// List of replies that can be triggered by this topic.
        /// </summary>
        public ICollection<PassiveReplyOption> ReplyOptions { get; set; }
    }
}
