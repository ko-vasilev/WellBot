using MediatR;
using WellBot.UseCases.Chats.AutomaticMessages.SendAutomaticMessages;

namespace WellBot.Web.Infrastructure.RecurringJobs;

/// <summary>
/// Send automatic messages in Telegram chats.
/// </summary>
public class SendAutomaticMessages
{
    private readonly IMediator mediator;
    private readonly ILogger<SendAutomaticMessages> logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    public SendAutomaticMessages(IMediator mediator, ILogger<SendAutomaticMessages> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    /// <summary>
    /// Send messages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task SendAsync(CancellationToken cancellationToken)
    {
        // Send messages only from 10:00 through 21:00 in GMT+4
        const int startTimeHour = 10 - 4;
        const int endTimeHour = 22 - 4;

        var currentHour = DateTime.UtcNow.Hour;
        var shouldTrySend = currentHour >= startTimeHour
            && currentHour < endTimeHour;
        if (!shouldTrySend)
        {
            logger.LogInformation("Skipping automatic messages send, current hour is {hour}", currentHour);
            return;
        }

        await mediator.Send(new SendAutomaticMessagesCommand(), cancellationToken);
    }
}
