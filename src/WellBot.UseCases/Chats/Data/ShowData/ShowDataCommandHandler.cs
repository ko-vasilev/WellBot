using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Types;
using WellBot.Domain.Chats.Entities;
using WellBot.DomainServices.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Data.ShowData
{
    /// <summary>
    /// Handler for <see cref="ShowDataCommand"/>.
    /// </summary>
    internal class ShowDataCommandHandler : AsyncRequestHandler<ShowDataCommand>
    {
        private readonly IAppDbContext dbContext;
        private readonly ITelegramBotClient botClient;
        private readonly CurrentChatService currentChatService;
        private readonly MessageRateLimitingService messageRateLimitingService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ShowDataCommandHandler(IAppDbContext dbContext, ITelegramBotClient botClient, CurrentChatService currentChatService, MessageRateLimitingService messageRateLimitingService)
        {
            this.dbContext = dbContext;
            this.botClient = botClient;
            this.currentChatService = currentChatService;
            this.messageRateLimitingService = messageRateLimitingService;
        }

        /// <inheritdoc/>
        protected override async Task Handle(ShowDataCommand request, CancellationToken cancellationToken)
        {
            var key = (request.Key ?? string.Empty).ToLowerInvariant();
            var data = await dbContext.ChatDatas.FirstOrDefaultAsync(d => d.ChatId == currentChatService.ChatId && d.Key == key, cancellationToken);
            if (data == null)
            {
                await botClient.SendTextMessageAsync(request.ChatId, "Не могу найти данных по этому ключу", replyToMessageId: request.MessageId);
                return;
            }

            var rateLimit = ShouldRateLimitItem(data);
            if (rateLimit)
            {
                // Do not allow outputting the item too frequently
                if (messageRateLimitingService.IsRateLimited(request.ChatId, request.SenderUserId, data.Key, out var allowedIn))
                {
                    string duration = $"{allowedIn.Seconds} секунд";
                    if (allowedIn.Minutes > 0)
                    {
                        duration = $"{allowedIn.Minutes} минут";
                    }
                    await botClient.SendTextMessageAsync(request.ChatId, $"Подождите " + duration, replyToMessageId: request.MessageId);
                    return;
                }
            }

            Telegram.Bot.Types.InputFiles.InputOnlineFile file = null;
            if (data.FileId != null)
            {
                file = new Telegram.Bot.Types.InputFiles.InputOnlineFile(data.FileId);
            }
            switch (data.DataType)
            {
                case Domain.Chats.Entities.DataType.Animation:
                    await botClient.SendAnimationAsync(request.ChatId, file);
                    break;
                case Domain.Chats.Entities.DataType.Audio:
                    await botClient.SendAudioAsync(request.ChatId, file, caption: data.Text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    break;
                case Domain.Chats.Entities.DataType.Document:
                    await botClient.SendDocumentAsync(request.ChatId, file, caption: data.Text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    break;
                case Domain.Chats.Entities.DataType.Photo:
                    await botClient.SendPhotoAsync(request.ChatId, file, caption: data.Text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    break;
                case Domain.Chats.Entities.DataType.Sticker:
                    await botClient.SendStickerAsync(request.ChatId, file);
                    break;
                case Domain.Chats.Entities.DataType.Text:
                    await botClient.SendTextMessageAsync(request.ChatId, data.Text, Telegram.Bot.Types.Enums.ParseMode.Html, disableNotification: true);
                    break;
                case Domain.Chats.Entities.DataType.Video:
                    await botClient.SendVideoAsync(request.ChatId, file, caption: data.Text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    break;
                case Domain.Chats.Entities.DataType.VideoNote:
                    await botClient.SendVideoNoteAsync(request.ChatId, file);
                    break;
                case Domain.Chats.Entities.DataType.Voice:
                    await botClient.SendVoiceAsync(request.ChatId, file, caption: data.Text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    break;
                default:
                    await botClient.SendTextMessageAsync(request.ChatId, "Неожиданный формат файла " + data.DataType);
                    break;
            }

            if (rateLimit)
            {
                messageRateLimitingService.LimitRate(request.ChatId, request.SenderUserId, data.Key);
            }
        }

        /// <summary>
        /// Check if there should be a rate limit for accessing the specified chat data.
        /// </summary>
        /// <param name="chatData">Chat data.</param>
        /// <returns>True if request rate limit should be applied when displaying the data.</returns>
        private bool ShouldRateLimitItem(ChatData chatData)
        {
            return chatData.HasUserMention;
        }
    }
}
