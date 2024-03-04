using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WellBot.Domain.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.RegularMessageHandles.Reply;

/// <summary>
/// Allows checking if a messages matches any of the topics.
/// </summary>
public class PassiveTopicService
{
    private readonly ITelegramBotSettings telegramBotSettings;

    /// <summary>
    /// Constructor.
    /// </summary>
    public PassiveTopicService(ITelegramBotSettings telegramBotSettings)
    {
        this.telegramBotSettings = telegramBotSettings;
    }

    private List<Topic> topics = new();

    /// <summary>
    /// Get the list of topics that match the specified message.
    /// </summary>
    /// <param name="message">Message data.</param>
    /// <returns>List of matching topics with their probabilities.</returns>
    public IEnumerable<MatchingTopic> GetMatchingTopics(Message message)
    {
        var repliedToBot = message.ReplyToMessage != null && message.ReplyToMessage.From?.Username == telegramBotSettings.TelegramBotUsername;
        var isMeme = message.Type == MessageType.Photo || message.Type == MessageType.Video;
        if (!repliedToBot)
        {
            if (message.Entities != null)
            {
                repliedToBot = ContainsBotMention(message.Text, message.Entities);
            }
            if (!repliedToBot && message.CaptionEntities != null)
            {
                repliedToBot = ContainsBotMention(message.Caption, message.CaptionEntities);
            }
        }

        var matches = new List<MatchingTopic>();
        foreach (var topic in topics)
        {
            var isMatch = true;
            if (topic.IsMeme.HasValue && topic.IsMeme != isMeme)
            {
                isMatch = false;
            }
            if (isMatch && topic.IsDirect.HasValue && topic.IsDirect != repliedToBot)
            {
                isMatch = false;
            }
            if (topic.IsAudio.HasValue)
            {
                var isAudio = message.Type == MessageType.Audio;
                if (topic.IsAudio != isAudio)
                {
                    isMatch = false;
                }
            }
            if (isMatch && topic.Regex != null)
            {
                if (message.Text == null || !topic.Regex.IsMatch(message.Text))
                {
                    isMatch = false;
                }
            }

            if (isMatch)
            {
                matches.Add(new MatchingTopic
                {
                    Id = topic.Id,
                    Probability = topic.Probability
                });
                if (topic.IsExclusive)
                {
                    break;
                }
            }
        }

        return matches;
    }

    /// <summary>
    /// Get ids of topics by their names.
    /// </summary>
    /// <param name="names">Names of the topics to search for.</param>
    /// <returns>List with ids of mathing topics.</returns>
    public IEnumerable<int> SearchTopics(IEnumerable<string> names)
    {
        return topics
            .Where(t => names.Contains(t.Name))
            .Select(t => t.Id);
    }

    /// <summary>
    /// Update the list of existing topics.
    /// </summary>
    /// <param name="currentTopics">New list of topics. The old list will be cleared.</param>
    public void Update(IEnumerable<PassiveTopic> currentTopics)
    {
        var replacingTopics = currentTopics
            // Make sure exclusive topics are tested first
            .OrderByDescending(topic => topic.IsExclusive)
            .Select(topic => new Topic
            {
                Id = topic.Id,
                IsDirect = topic.IsDirectMessage,
                IsMeme = topic.IsMeme,
                Name = topic.Name,
                Probability = topic.Probability,
                Regex = string.IsNullOrEmpty(topic.Regex) ? null : new Regex(topic.Regex, RegexOptions.Compiled | RegexOptions.IgnoreCase),
                IsExclusive = topic.IsExclusive,
                IsAudio = topic.IsAudio
            });

        topics.Clear();
        topics.AddRange(replacingTopics);
    }

    private bool ContainsBotMention(string? text, IEnumerable<MessageEntity> messageEntities)
    {
        var botNickname = "@" + telegramBotSettings.TelegramBotUsername;
        return text != null
            && messageEntities.Any(e => e.Type == MessageEntityType.Mention && text.Substring(e.Offset, e.Length) == botNickname);
    }

    /// <summary>
    /// Information about a topic.
    /// </summary>
    public class MatchingTopic
    {
        /// <summary>
        /// Topic id.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Probability of this topic being triggered.
        /// </summary>
        public int Probability { get; init; }
    }

    private class Topic
    {
        public int Id { get; init; }

        public Regex? Regex { get; init; }

        public bool? IsDirect { get; init; }

        public bool? IsMeme { get; init; }

        public bool? IsAudio { get; init; }

        public required string Name { get; init; }

        public int Probability { get; init; }

        public bool IsExclusive { get; init; }
    }
}
