using MediatR;
using Microsoft.EntityFrameworkCore;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Summarization.LogOptIn;

/// <summary>
/// Handler for <see cref="LogOptInCommand"/>.
/// </summary>
internal class LogOptInCommandHandler(IAppDbContext dbContext,
    TelegramMessageService telegramMessageService)
    : IRequestHandler<LogOptInCommand, Unit>
{
    /// <inheritdoc/>
    public async Task<Unit> Handle(LogOptInCommand request, CancellationToken cancellationToken)
    {
        var existingOptOuts = await dbContext.MessageLogOptOuts
            .Where(o => o.TelegramUserId == request.TelegramUserId && o.Chat!.TelegramId == request.ChatId)
            .ToListAsync(cancellationToken);
        dbContext.MessageLogOptOuts.RemoveRange(existingOptOuts);
        await dbContext.SaveChangesAsync(cancellationToken);

        await telegramMessageService.SendSuccessAsync(request.ChatId, request.MessageId);
        return default;
    }
}
