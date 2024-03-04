using WellBot.Domain.Chats;

namespace WellBot.UseCases.Chats.Topics.GetTopicList
{
    /// <summary>
    /// Contains information about a topic.
    /// </summary>
    public class TopicDto
    {
        /// <inheritdoc cref="PassiveTopic.Id"/>
        public int Id { get; set; }

        /// <inheritdoc cref="PassiveTopic.Name"/>
        public string Name { get; set; }

        /// <inheritdoc cref="PassiveTopic.Regex"/>
        public string Regex { get; set; }

        /// <inheritdoc cref="PassiveTopic.IsDirectMessage"/>
        public bool? IsDirectMessage { get; set; }

        /// <inheritdoc cref="PassiveTopic.IsMeme"/>
        public bool? IsMeme { get; set; }

        /// <inheritdoc cref="PassiveTopic.IsAudio"/>
        public bool? IsAudio { get; set; }

        /// <inheritdoc cref="PassiveTopic.IsExclusive"/>
        public bool IsExclusive { get; set; }

        /// <inheritdoc cref="PassiveTopic.Probability"/>
        public int Probability { get; set; }
    }
}
