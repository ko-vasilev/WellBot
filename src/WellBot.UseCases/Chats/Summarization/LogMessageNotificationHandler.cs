using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.BotAPI.AvailableTypes;
using WellBot.Domain.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.UseCases.Chats.RegularMessageHandles;

namespace WellBot.UseCases.Chats.Summarization;

/// <summary>
/// Log a certain event in a chat.
/// </summary>
internal class LogMessageNotificationHandler : INotificationHandler<MessageNotification>
{
    private readonly IAppDbContext dbContext;
    private readonly TelegramMessageService telegramMessageService;
    private static readonly Dictionary<long, int> telegramChatMapping = new();

    /// <summary>
    /// Constructor.
    /// </summary>
    public LogMessageNotificationHandler(IAppDbContext dbContext, TelegramMessageService telegramMessageService)
    {
        this.dbContext = dbContext;
        this.telegramMessageService = telegramMessageService;
    }

    /// <inheritdoc />
    public async Task Handle(MessageNotification notification, CancellationToken cancellationToken)
    {
        var messageDescription = GetEventInformation(notification.Message);
        if (messageDescription == null)
        {
            return;
        }

        var actorName = telegramMessageService.GetUserFullName(notification.Message.From);
        if (actorName == string.Empty)
        {
            actorName = "somebody";
        }

        var messageDate = DateTimeOffset.FromUnixTimeSeconds(notification.Message.Date).UtcDateTime;
        var fullDescription = $"{messageDate:yyyy-MM-dd HH:mm:ss} Событие #{notification.Message.MessageId} ";
        if (notification.Message.ForwardOrigin != null)
        {
            // TODO: different format for different forward origins?
            fullDescription += $"переслал ";
        }
        fullDescription += messageDescription;

        var chatId = await GetChatIdAsync(notification.Message.Chat.Id, cancellationToken);

        var messageLog = new MessageLog()
        {
            Message = fullDescription,
            MessageDate = messageDate,
            ChatId = chatId,
            Sender = actorName
        };
        dbContext.MessageLogs.Add(messageLog);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private string? GetBasicMessageInformation(Message message)
    {
        if (message.Text != null)
        {
            return $"отправил сообщение: \"{message.Text}\"";
        }
        if (message.Animation != null)
        {
            return "отправил картинку";
        }
        if (message.Audio != null)
        {
            return "отправил аудиозапись";
        }
        if (message.Dice != null)
        {
            return "бросил " + message.Dice.Emoji;
        }
        if (message.Document != null)
        {
            return "отправил документ";
        }
        if (message.LeftChatMember != null)
        {
            return $"{message.LeftChatMember.FirstName} {message.LeftChatMember.LastName} вышел из канала";
        }
        if (message.NewChatMembers != null)
        {
            var names = message.NewChatMembers.Select(m => $"{m.FirstName} {m.LastName}").ToList();
            var namesString = string.Join(", ", names);
            if (names.Count > 1)
            {
                return namesString + " присоединились к группе";
            }
            return namesString + " присоединился к группе";
        }
        if (message.Photo != null)
        {
            return "отправил фотографию";
        }
        if (message.Poll != null)
        {
            return $"отправил голосование с вопросом {message.Poll.Question}";
        }
        if (message.Sticker != null)
        {
            return $"отправил стикер с эмоцией {message.Sticker.Emoji}";
        }
        if (message.Video != null)
        {
            return "отправил видео";
        }
        if (message.VideoChatEnded != null)
        {
            return "завершил видеозвонок";
        }
        if (message.VideoChatStarted != null)
        {
            return "начал видеозвонок";
        }
        if (message.Voice != null)
        {
            return "отправил голосовое сообщение";
        }

        return null;
    }

    private string? GetEventInformation(Message message)
    {
        var messageInfo = GetBasicMessageInformation(message);
        if (messageInfo == null)
        {
            return null;
        }

        if (message.Caption != null)
        {
            messageInfo += $" с подписью: \"{message.Caption}\"";
        }
        if (message.ReplyToMessage != null)
        {
            messageInfo += $" ответом на Событие #{message.ReplyToMessage.MessageId}";
        }

        return messageInfo;
    }

    private async Task<int> GetChatIdAsync(long telegramChatId, CancellationToken cancellationToken)
    {
        var chatId = await dbContext.Chats.Where(c => c.TelegramId == telegramChatId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (chatId != default)
        {
            return chatId;
        }

        var chat = new Domain.Chats.Chat()
        {
            TelegramId = telegramChatId
        };
        dbContext.Chats.Add(chat);
        await dbContext.SaveChangesAsync(cancellationToken);
        return chat.Id;
    }
}
