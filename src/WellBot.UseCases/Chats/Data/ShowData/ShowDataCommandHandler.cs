using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
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

        /// <summary>
        /// Constructor.
        /// </summary>
        public ShowDataCommandHandler(IAppDbContext dbContext, ITelegramBotClient botClient, CurrentChatService currentChatService)
        {
            this.dbContext = dbContext;
            this.botClient = botClient;
            this.currentChatService = currentChatService;
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
                    await botClient.SendAudioAsync(request.ChatId, file, caption: data.Text);
                    break;
                case Domain.Chats.Entities.DataType.Document:
                    await botClient.SendDocumentAsync(request.ChatId, file, caption: data.Text);
                    break;
                case Domain.Chats.Entities.DataType.Photo:
                    await botClient.SendPhotoAsync(request.ChatId, file, caption: data.Text);
                    break;
                case Domain.Chats.Entities.DataType.Sticker:
                    await botClient.SendStickerAsync(request.ChatId, file);
                    break;
                case Domain.Chats.Entities.DataType.Text:
                    await botClient.SendTextMessageAsync(request.ChatId, data.Text, Telegram.Bot.Types.Enums.ParseMode.Html, disableNotification: true);
                    break;
                case Domain.Chats.Entities.DataType.Video:
                    await botClient.SendVideoAsync(request.ChatId, file, caption: data.Text);
                    break;
                case Domain.Chats.Entities.DataType.VideoNote:
                    await botClient.SendVideoNoteAsync(request.ChatId, file);
                    break;
                case Domain.Chats.Entities.DataType.Voice:
                    await botClient.SendVoiceAsync(request.ChatId, file, caption: data.Text);
                    break;
                default:
                    await botClient.SendTextMessageAsync(request.ChatId, "Неожиданный формат файла " + data.DataType);
                    break;
            }
        }
    }
}
