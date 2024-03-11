using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using WellBot.DomainServices.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Data.DeleteChatData;

/// <summary>
/// Handler for <see cref="DeleteChatDataCommand"/>.
/// </summary>
internal class DeleteChatDataCommandHandler : AsyncRequestHandler<DeleteChatDataCommand>
{
    private readonly IAppDbContext dbContext;
    private readonly ITelegramBotClient botClient;
    private readonly CurrentChatService currentChatService;
    private readonly TelegramMessageService telegramMessageService;

    public DeleteChatDataCommandHandler(IAppDbContext dbContext, ITelegramBotClient botClient, CurrentChatService currentChatService, TelegramMessageService telegramMessageService)
    {
        this.dbContext = dbContext;
        this.botClient = botClient;
        this.currentChatService = currentChatService;
        this.telegramMessageService = telegramMessageService;
    }

    /// <inheritdoc/>
    protected override async Task Handle(DeleteChatDataCommand request, CancellationToken cancellationToken)
    {
        var key = (request.Key ?? string.Empty).ToLowerInvariant();
        var data = await dbContext.ChatDatas.FirstOrDefaultAsync(d => d.ChatId == currentChatService.ChatId && d.Key == key, cancellationToken);
        if (data == null)
        {
            await botClient.SendMessageAsync(request.ChatId,
                "Не могу найти данных по этому ключу",
                replyParameters: new() { MessageId = request.MessageId });
            return;
        }

        dbContext.ChatDatas.Remove(data);
        await dbContext.SaveChangesAsync();
        await telegramMessageService.SendSuccessAsync(request.ChatId, request.MessageId);
    }
}
