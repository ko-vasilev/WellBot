using System;
using System.Collections.Generic;
using System.Linq;
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
            if (message == null || !ShouldReply(message))
            {
                return;
            }

            var maxRetriesCount = 5;
            while (maxRetriesCount > 0)
            {
                PassiveReplyOption replyOption = null;
                try
                {
                    replyOption = await randomService.QueryRandomAsync(dbContext.PassiveReplyOptions.AsNoTracking(), cancellationToken);
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

        private bool ShouldReply(Message message)
        {
            // By default reply 1 in 500 messages
            var maxProbability = 500;

            var repliedToBot = message.ReplyToMessage != null && message.ReplyToMessage.From?.Username == telegramBotSettings.TelegramBotUsername;
            if (repliedToBot)
            {
                maxProbability = 7;
            }
            else
            {
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
                    maxProbability = 4;
                }
            }

            var randomVal = randomService.GetRandom(maxProbability);
            var shouldReply = randomVal == 0;
            return shouldReply;
        }

        private bool ContainsBotMention(string text, IEnumerable<MessageEntity> messageEntities)
        {
            var botNickname = "@" + telegramBotSettings.TelegramBotUsername;
            return text != null
                && messageEntities.Any(e => e.Type == Telegram.Bot.Types.Enums.MessageEntityType.Mention && text.Substring(e.Offset, e.Length) == botNickname);
        }
    }
}
