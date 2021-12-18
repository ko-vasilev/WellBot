﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly TelegramMessageService telegramMessageService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SetChatDataCommandHandler(IAppDbContext dbContext, ITelegramBotClient botClient, CurrentChatService currentChatService, TelegramMessageService telegramMessageService)
        {
            this.dbContext = dbContext;
            this.botClient = botClient;
            this.currentChatService = currentChatService;
            this.telegramMessageService = telegramMessageService;
        }

        /// <inheritdoc/>
        protected override async Task Handle(SetChatDataCommand request, CancellationToken cancellationToken)
        {
            if (!TryParseKey(request.Arguments, out var key, out var saveText))
            {
                await botClient.SendTextMessageAsync(request.ChatId, "Укажите ключ для сохранения");
                return;
            }

            var storeKey = key.ToLowerInvariant();
            var existingItem = await dbContext.ChatDatas.FirstOrDefaultAsync(d => d.ChatId == currentChatService.ChatId && d.Key == storeKey);
            if (existingItem != null)
            {
                dbContext.ChatDatas.Remove(existingItem);
            }

            var message = request.Message;
            if (message.ReplyToMessage != null)
            {
                message = message.ReplyToMessage;
                saveText = telegramMessageService.GetMessageTextHtml(message);
            }

            var data = new ChatData
            {
                ChatId = currentChatService.ChatId,
                Text = saveText,
                Key = storeKey,
                DataType = DataType.Text,
                HasUserMention = false
            };
            if (message.Entities != null)
            {
                data.HasUserMention = message.Entities.Any(e => e.Type == MessageEntityType.Mention || e.Type == MessageEntityType.TextMention);
            }
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
            await botClient.SendTextMessageAsync(request.ChatId, $"Сохранил как \"{key}\"");
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