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

                if (request.Arguments == "meme")
                {
                    await SetMemeChannelAsync(request.Message);
                    await telegramMessageService.SendSuccessAsync(request.Message.Chat.Id);
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
                var dataType = telegramMessageService.GetFile(message, out var attachedDocument);
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
    }
}
