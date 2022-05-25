using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WellBot.UseCases.Chats.AutomaticMessages.SendAutomaticMessages;

namespace WellBot.Web.Infrastructure.RecurringJobs
{
    /// <summary>
    /// Send automatic messages in Telegram chats.
    /// </summary>
    public class SendAutomaticMessages
    {
        private readonly IMediator mediator;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SendAutomaticMessages(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Send messages.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task SendAsync(CancellationToken cancellationToken)
        {
            // Send messages only from 9:00 through 21:00 in GMT+4
            const int startTimeHour = 9 - 4;
            const int endTimeHour = 22 - 4;

            var shouldTrySend = DateTime.UtcNow.Hour >= startTimeHour
                && DateTime.UtcNow.Hour < endTimeHour;
            if (!shouldTrySend)
            {
                return;
            }

            await mediator.Send(new SendAutomaticMessagesCommand(), cancellationToken);
        }
    }
}
