using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using WellBot.Domain.Chats.Entities;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.UseCases.Chats.Dtos;
using WellBot.UseCases.Chats.RegularMessageHandles.Reply;

namespace WellBot.UseCases.Chats.AdminControl
{
    /// <summary>
    /// Handler for <see cref="AdminControlCommand"/>.
    /// </summary>
    internal class AdminControlCommandHandler : AsyncRequestHandler<AdminControlCommand>
    {
        private readonly IAppDbContext appDbContext;
        private readonly TelegramMessageService telegramMessageService;
        private readonly ILogger<AdminControlCommandHandler> logger;
        private readonly MemeChannelService memeChannelService;
        private readonly Lazy<PassiveTopicService> passiveTopicService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AdminControlCommandHandler(IAppDbContext appDbContext, TelegramMessageService telegramMessageService, ILogger<AdminControlCommandHandler> logger, MemeChannelService memeChannelService, Lazy<PassiveTopicService> passiveTopicService)
        {
            this.appDbContext = appDbContext;
            this.telegramMessageService = telegramMessageService;
            this.logger = logger;
            this.memeChannelService = memeChannelService;
            this.passiveTopicService = passiveTopicService;
        }

        /// <inheritdoc/>
        protected override async Task Handle(AdminControlCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Arguments == "add slap")
                {
                    await AddSlapOptionAsync(request.Message);
                    await telegramMessageService.SendSuccessAsync(request.Message.Chat.Id);
                    return;
                }

                const string PassiveAdd = "passive add";
                if (request.Arguments.StartsWith(PassiveAdd))
                {
                    var arguments = request.Arguments.Substring(PassiveAdd.Length).Trim();
                    if (await AddPassiveReplyOptionAsync(request.Message, arguments))
                    {
                        await telegramMessageService.SendSuccessAsync(request.Message.Chat.Id);
                    }
                    return;
                }

                const string PassiveProbability = "passive probability";
                if (request.Arguments.StartsWith(PassiveProbability))
                {
                    var arguments = request.Arguments.Substring(PassiveProbability.Length).Trim();
                    if (await SetTopicProbabilityAsync(arguments))
                    {
                        await telegramMessageService.SendSuccessAsync(request.Message.Chat.Id);
                    }
                    return;
                }

                if (request.Arguments == "meme")
                {
                    await SetMemeChannelAsync(request.Message);
                    await telegramMessageService.SendSuccessAsync(request.Message.Chat.Id);
                    return;
                }

                if (request.Arguments == "broadcast")
                {
                    await BroadcastMessageAsync(request.Message);
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling admin command {command}", request.Arguments);
            }
        }

        private async Task AddSlapOptionAsync(Message message)
        {
            var replyMessage = message.ReplyToMessage;
            if (replyMessage?.Animation == null)
            {
                return;
            }

            var slapOption = new SlapOption
            {
                FileId = replyMessage.Animation.FileId
            };
            appDbContext.SlapOptions.Add(slapOption);
            await appDbContext.SaveChangesAsync();
        }

        private async Task<bool> AddPassiveReplyOptionAsync(Message message, string arguments)
        {
            var replyMessage = message.ReplyToMessage;
            if (replyMessage == null)
            {
                return false;
            }

            var options = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var topicIds = passiveTopicService.Value.SearchTopics(options).ToList();
            if (!topicIds.Any())
            {
                const string defaultTopic = "regular";
                topicIds = passiveTopicService.Value.SearchTopics(new[] { defaultTopic }).ToList();
            }
            var topics = await appDbContext.PassiveTopics.Where(t => topicIds.Contains(t.Id))
                .ToListAsync();
            bool isBatchMode = options.Contains("batch");

            var text = telegramMessageService.GetMessageTextHtml(replyMessage);
            if (isBatchMode && replyMessage.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                foreach (var line in text.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                {
                    var lineOption = new PassiveReplyOption
                    {
                        Text = line,
                        DataType = DataType.Text,
                        PassiveTopics = topics
                    };
                    appDbContext.PassiveReplyOptions.Add(lineOption);
                }
                await appDbContext.SaveChangesAsync();
                return true;
            }

            var replyOption = new PassiveReplyOption
            {
                Text = text,
                DataType = DataType.Text,
                PassiveTopics = topics
            };
            if (replyMessage.Type != Telegram.Bot.Types.Enums.MessageType.Text)
            {
                var dataType = telegramMessageService.GetFile(replyMessage, out var attachedDocument);
                if (dataType == null)
                {
                    return false;
                }
                replyOption.DataType = dataType.Value;
                replyOption.FileId = attachedDocument.FileId;
            }

            appDbContext.PassiveReplyOptions.Add(replyOption);
            await appDbContext.SaveChangesAsync();
            return true;
        }

        private async Task SetMemeChannelAsync(Message message)
        {
            var replyMessage = message.ReplyToMessage;
            if (replyMessage == null)
            {
                return;
            }

            if (replyMessage.ForwardFromChat == null)
            {
                return;
            }

            var currentChannelInfo = await appDbContext.MemeChannels.FirstOrDefaultAsync();
            if (currentChannelInfo == null)
            {
                currentChannelInfo = new MemeChannelInfo();
                appDbContext.MemeChannels.Add(currentChannelInfo);
            }

            currentChannelInfo.ChannelId = replyMessage.ForwardFromChat.Id;
            currentChannelInfo.LatestMessageId = replyMessage.ForwardFromMessageId.Value;
            await appDbContext.SaveChangesAsync();
            memeChannelService.CurrentMemeChatId = currentChannelInfo.ChannelId;
        }

        private async Task BroadcastMessageAsync(Message originalMessage)
        {
            var messageToSend = originalMessage.ReplyToMessage;
            if (messageToSend == null)
            {
                return;
            }

            var chats = await appDbContext.Chats
                .Select(c => c.TelegramId)
                .ToListAsync();
            var text = telegramMessageService.GetMessageTextHtml(messageToSend);
            var messageData = new GenericMessage
            {
                Text = text,
                DataType = DataType.Text
            };
            if (messageToSend.Type != Telegram.Bot.Types.Enums.MessageType.Text)
            {
                var dataType = telegramMessageService.GetFile(messageToSend, out var attachedDocument);
                if (dataType == null)
                {
                    return;
                }
                messageData.DataType = dataType.Value;
                messageData.FileId = attachedDocument.FileId;
            }

            foreach (var chat in chats)
            {
                try
                {
                    await telegramMessageService.SendMessageAsync(messageData, new ChatId(chat));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error broadcasting to {chatId} chat", chat);
                }
            }
        }

        private async Task<bool> SetTopicProbabilityAsync(string arguments)
        {
            var parsedArguments = arguments.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (parsedArguments.Length != 2)
            {
                return false;
            }

            if (!int.TryParse(parsedArguments[1], out var probability))
            {
                return false;
            }

            var topicName = parsedArguments[0];
            var topic = await appDbContext.PassiveTopics.FirstOrDefaultAsync(t => t.Name == topicName);
            if (topic == null)
            {
                return false;
            }

            topic.Probability = probability;
            await appDbContext.SaveChangesAsync();

            return true;
        }
    }
}
