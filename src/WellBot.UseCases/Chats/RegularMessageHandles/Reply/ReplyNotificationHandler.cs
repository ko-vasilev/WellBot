using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using WellBot.Domain.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.RegularMessageHandles.Reply;

/// <summary>
/// Randomly replies to incoming messages.
/// </summary>
internal class ReplyNotificationHandler : INotificationHandler<MessageNotification>
{
    private readonly RandomService randomService;
    private readonly IAppDbContext dbContext;
    private readonly ILogger<ReplyNotificationHandler> logger;
    private readonly TelegramMessageService telegramMessageService;
    private readonly PassiveTopicService passiveTopicService;

    /// <summary>
    /// Constructor.
    /// </summary>
    public ReplyNotificationHandler(RandomService randomService, IAppDbContext dbContext, ILogger<ReplyNotificationHandler> logger, TelegramMessageService telegramMessageService, PassiveTopicService passiveTopicService)
    {
        this.randomService = randomService;
        this.dbContext = dbContext;
        this.logger = logger;
        this.telegramMessageService = telegramMessageService;
        this.passiveTopicService = passiveTopicService;
    }

    /// <inheritdoc />
    public async Task Handle(MessageNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        if (message == null || !ShouldReply(message, out var topics))
        {
            return;
        }

        var topicIds = topics.ToList();
        var allOptions = await dbContext.PassiveTopics
            .Where(t => topicIds.Contains(t.Id))
            .SelectMany(t => t.ReplyOptions)
            .Distinct()
            .Select(option => option.Id)
            .ToListAsync(cancellationToken);
        var maxRetriesCount = 5;
        while (maxRetriesCount > 0)
        {
            PassiveReplyOption? replyOption = null;
            try
            {
                var optionId = randomService.PickRandom(allOptions);
                replyOption = await dbContext.PassiveReplyOptions
                    .FirstAsync(opt => opt.Id == optionId, cancellationToken);
                await telegramMessageService.SendMessageAsync(new Dtos.GenericMessage
                {
                    Text = replyOption.Text,
                    FileId = replyOption.FileId,
                    DataType = replyOption.DataType
                },
                message.Chat.Id,
                message.MessageId);

                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error replying with message {reply}", replyOption);
            }

            --maxRetriesCount;
        }
    }

    private bool ShouldReply(Message message, out IEnumerable<int> topicIds)
    {
        var topics = passiveTopicService.GetMatchingTopics(message);
        topicIds = topics.Select(t => t.Id);

        var highestProbabilityTopic = topics
            .OrderBy(t => t.Probability)
            .FirstOrDefault();
        if (highestProbabilityTopic == null)
        {
            return false;
        }

        var randomVal = randomService.GetRandom(highestProbabilityTopic.Probability);
        var shouldReply = randomVal == 0;
        return shouldReply;
    }
}
