using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using WellBot.Domain.Chats;
using WellBot.DomainServices.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Data.SetChatData;

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
            await botClient.SendMessageAsync(request.ChatId, "Укажите ключ для сохранения");
            return;
        }
        if (string.IsNullOrEmpty(key))
        {
            await botClient.SendMessageAsync(request.ChatId, "Не могу сохранить с таким ключом");
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
            data.HasUserMention = message.Entities.Any(e => e.Type == MessageEntityTypes.Mention || e.Type == MessageEntityTypes.TextMention);
        }

        var dataType = telegramMessageService.GetFileId(message, out var attachedFileId);
        if (dataType != null && attachedFileId != null)
        {
            data.DataType = dataType.Value;
            data.FileId = attachedFileId;
        }

        dbContext.ChatDatas.Add(data);
        await dbContext.SaveChangesAsync();
        await botClient.SendMessageAsync(request.ChatId, $"Сохранил как \"{key}\"");
    }

    private bool TryParseKey(string arguments, out string key, out string remainder)
    {
        if (string.IsNullOrWhiteSpace(arguments))
        {
            key = string.Empty;
            remainder = string.Empty;
            return false;
        }

        var spaceSymbol = arguments.IndexOf(' ');
        var newLineSymbol = arguments.IndexOf('\n');

        var breakSymbolPosition = spaceSymbol;
        // Use the first existing break symbol.
        if (breakSymbolPosition == -1 || (newLineSymbol > 0 && newLineSymbol < spaceSymbol))
        {
            breakSymbolPosition = newLineSymbol;
        }
        if (breakSymbolPosition == -1)
        {
            key = arguments;
            remainder = string.Empty;
            return true;
        }

        key = arguments.Substring(0, breakSymbolPosition);
        remainder = arguments.Substring(breakSymbolPosition).Trim();
        return true;
    }
}
