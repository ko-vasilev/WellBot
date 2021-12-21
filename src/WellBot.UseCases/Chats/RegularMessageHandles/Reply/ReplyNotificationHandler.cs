using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using WellBot.Domain.Chats.Entities;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.RegularMessageHandles.Reply
{
    /// <summary>
    /// Randomly replies to incoming messages.
    /// </summary>
    internal class ReplyNotificationHandler : INotificationHandler<MessageNotification>
    {
        private readonly RandomService randomService;
        private readonly IAppDbContext dbContext;
        private readonly ITelegramBotSettings telegramBotSettings;
        private readonly ILogger<ReplyNotificationHandler> logger;
        private readonly TelegramMessageService telegramMessageService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReplyNotificationHandler(RandomService randomService, IAppDbContext dbContext, ITelegramBotSettings telegramBotSettings, ILogger<ReplyNotificationHandler> logger, TelegramMessageService telegramMessageService)
        {
            this.randomService = randomService;
            this.dbContext = dbContext;
            this.telegramBotSettings = telegramBotSettings;
            this.logger = logger;
            this.telegramMessageService = telegramMessageService;
        }

        /// <inheritdoc />
        public async Task Handle(MessageNotification notification, CancellationToken cancellationToken)
        {
            var message = notification.Message;
            if (message == null || !ShouldReply(message, out var isDirect, out var isDota))
            {
                return;
            }

            var maxRetriesCount = 5;
            IQueryable<PassiveReplyOption> optionsQuery = dbContext.PassiveReplyOptions;
            if (isDota)
            {
                optionsQuery = optionsQuery.Where(opt => opt.IsDota);
            }
            else
            {
                optionsQuery = optionsQuery.Where(opt => !opt.IsDota)
                    .Where(opt => isDirect || !opt.IsDirectMessage);
            }

            var allOptions = await optionsQuery
                .Select(opt => opt.Id)
                .ToListAsync(cancellationToken);
            while (maxRetriesCount > 0)
            {
                PassiveReplyOption replyOption = null;
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

        private static readonly Regex IsDotaMessageRegex = new Regex(@"(\W|^)((дот)|(дотк)|(дотан))(а|е|у)?(\W|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private bool ShouldReply(Message message, out bool isDirect, out bool isDota)
        {
            var probability = ParseProbability(message, out isDirect, out isDota);

            var randomVal = randomService.GetRandom(probability);
            var shouldReply = randomVal == 0;
            return shouldReply;
        }

        private int ParseProbability(Message message, out bool isDirect, out bool isDota)
        {
            isDirect = false;
            isDota = !string.IsNullOrEmpty(message.Text) && IsDotaMessageRegex.IsMatch(message.Text);
            if (isDota)
            {
                return 4;
            }

            var repliedToBot = message.ReplyToMessage != null && message.ReplyToMessage.From?.Username == telegramBotSettings.TelegramBotUsername;
            if (repliedToBot)
            {
                isDirect = true;
                return 4;
            }

            var mentionedBot = false;
            if (message.Entities != null)
            {
                mentionedBot = ContainsBotMention(message.Text, message.Entities);
            }
            if (!mentionedBot && message.CaptionEntities != null)
            {
                mentionedBot = ContainsBotMention(message.Caption, message.CaptionEntities);
            }

            if (mentionedBot)
            {
                isDirect = true;
                return 2;
            }

            return telegramBotSettings.RegularPassiveRepliesProbability;
        }

        private bool ContainsBotMention(string text, IEnumerable<MessageEntity> messageEntities)
        {
            var botNickname = "@" + telegramBotSettings.TelegramBotUsername;
            return text != null
                && messageEntities.Any(e => e.Type == Telegram.Bot.Types.Enums.MessageEntityType.Mention && text.Substring(e.Offset, e.Length) == botNickname);
        }
    }
}
