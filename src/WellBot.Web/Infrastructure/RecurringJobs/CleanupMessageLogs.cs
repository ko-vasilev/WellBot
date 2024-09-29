using MediatR;
using Microsoft.EntityFrameworkCore;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.UseCases.Chats.AutomaticMessages.SendAutomaticMessages;

namespace WellBot.Web.Infrastructure.RecurringJobs;

/// <summary>
/// Send automatic messages in Telegram chats.
/// </summary>
public class CleanupMessageLogs(IAppDbContext dbContext)
{
    /// <summary>
    /// Send messages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task CleanupAsync(CancellationToken cancellationToken)
    {
        // Remove all messages older than 1 day.
        var removeThreshold = DateTime.UtcNow.AddDays(-1);
        var messageLogsToRemove = await dbContext.MessageLogs
            .Where(m => m.MessageDate <= removeThreshold)
            .ToListAsync(cancellationToken);
        dbContext.MessageLogs.RemoveRange(messageLogsToRemove);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
