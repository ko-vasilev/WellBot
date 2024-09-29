using MediatR;
using Microsoft.EntityFrameworkCore;
using WellBot.Domain.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Summarization.LogOptOut;

/// <summary>
/// Handler for <see cref="LogOptOutCommand"/>.
/// </summary>
internal class LogOptOutCommandHandler(IAppDbContext dbContext,
    TelegramMessageService telegramMessageService)
    : IRequestHandler<LogOptOutCommand, Unit>
{
    /// <inheritdoc/>
    public async Task<Unit> Handle(LogOptOutCommand request, CancellationToken cancellationToken)
    {
        var optOutExists = await dbContext.Chats
            .Where(c => c.TelegramId == request.ChatId)
            .SelectMany(c => c.MessageLogOptOuts)
            .AnyAsync(o => o.TelegramUserId == request.TelegramUserId, cancellationToken);
        if (!optOutExists)
        {
            var chat = await GetChatAsync(request.ChatId, cancellationToken);
            var optOut = new MessageLogOptOut()
            {
                Chat = chat,
                TelegramUserId = request.TelegramUserId,
            };
            dbContext.MessageLogOptOuts.Add(optOut);

            // Clear history of the user's message logs.
            var existingMessageLogs = await dbContext.MessageLogs
                .Where(ml => ml.SenderTelegramId == request.TelegramUserId && ml.Chat!.TelegramId == request.ChatId)
                .ToListAsync(cancellationToken);
            foreach (var messageLog in existingMessageLogs)
            {
                messageLog.Sender = "somebody";
                messageLog.Message = null;
                messageLog.SenderTelegramId = null;
            }
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        await telegramMessageService.SendSuccessAsync(request.ChatId, request.MessageId);
        return default;
    }

    private async Task<Chat> GetChatAsync(long telegramChatId, CancellationToken cancellationToken)
    {
        var chat = await dbContext.Chats.Where(c => c.TelegramId == telegramChatId)
            .FirstOrDefaultAsync(cancellationToken);

        if (chat == null)
        {
            chat = new Chat()
            {
                TelegramId = telegramChatId
            };
            dbContext.Chats.Add(chat);
        }

        return chat;
    }
}
