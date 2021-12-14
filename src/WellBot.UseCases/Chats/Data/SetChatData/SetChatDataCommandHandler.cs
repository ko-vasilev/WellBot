using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WellBot.Domain.Chats.Entities;
using WellBot.DomainServices.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Data.SetChatData
{
    /// <summary>
    /// Handler for <see cref="SetChatDataCommand"/>.
    /// </summary>
    internal class SetChatDataCommandHandler : AsyncRequestHandler<SetChatDataCommand>
    {
        private readonly IAppDbContext dbContext;
        private readonly ITelegramBotClient botClient;
        private readonly CurrentChatService currentChatService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SetChatDataCommandHandler(IAppDbContext dbContext, ITelegramBotClient botClient, CurrentChatService currentChatService)
        {
            this.dbContext = dbContext;
            this.botClient = botClient;
            this.currentChatService = currentChatService;
        }

        /// <inheritdoc/>
        protected override async Task Handle(SetChatDataCommand request, CancellationToken cancellationToken)
        {
            if (!TryParseKey(request.Arguments, out var key, out var remainder))
            {
                await botClient.SendTextMessageAsync(request.ChatId, "Укажите ключ для сохранения");
                return;
            }

            var existingItem = await dbContext.ChatDatas.FirstOrDefaultAsync(d => d.ChatId == currentChatService.ChatId && d.Key == key);
            if (existingItem != null)
            {
                dbContext.ChatDatas.Remove(existingItem);
            }

            var message = request.Message;
            if (message.ReplyToMessage != null)
            {
                message = message.ReplyToMessage;
                remainder = message.Text ?? message.Caption;
            }

            var data = new ChatData
            {
                ChatId = currentChatService.ChatId,
                Text = remainder,
                Key = key,
                DataType = DataType.Text,
            };
            if (message.Type != MessageType.Text)
            {
                var dataType = GetFile(message, out var attachedDocument);
                if (dataType == null)
                {
                    await botClient.SendTextMessageAsync(request.ChatId, $"Не поддерживаемый формат сообщения");
                    return;
                }
                data.DataType = dataType.Value;
                data.FileId = attachedDocument.FileId;
            }
            dbContext.ChatDatas.Add(data);
            await dbContext.SaveChangesAsync();
            await botClient.SendTextMessageAsync(request.ChatId, $"Сохранил как <b>{HttpUtility.HtmlEncode(key)}</b>", ParseMode.Html);
        }

        private bool TryParseKey(string arguments, out string key, out string remainder)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                key = null;
                remainder = null;
                return false;
            }

            var spaceSymbol = arguments.IndexOf(' ');
            if (spaceSymbol == -1)
            {
                key = arguments;
                remainder = string.Empty;
                return true;
            }

            key = arguments.Substring(0, spaceSymbol);
            remainder = arguments.Substring(spaceSymbol).Trim();
            return true;
        }

        private DataType? GetFile(Message message, out FileBase file)
        {
            if (message.Photo != null)
            {
                file = message.Photo.OrderByDescending(p => p.Height).First();
                return DataType.Photo;
            }

            if (message.Audio != null)
            {
                file = message.Audio;
                return DataType.Audio;
            }

            if (message.Animation != null)
            {
                file = message.Animation;
                return DataType.Animation;
            }

            if (message.Sticker != null)
            {
                file = message.Sticker;
                return DataType.Sticker;
            }

            if (message.Video != null)
            {
                file = message.Video;
                return DataType.Video;
            }

            if (message.VideoNote != null)
            {
                file = message.VideoNote;
                return DataType.VideoNote;
            }

            if (message.Voice != null)
            {
                file = message.Voice;
                return DataType.Voice;
            }

            if (message.Document != null)
            {
                file = message.Document;
                return DataType.Document;
            }

            file = null;
            return null;
        }
    }
}
