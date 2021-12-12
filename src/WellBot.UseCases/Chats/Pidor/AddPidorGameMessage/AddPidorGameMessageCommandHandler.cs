using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Saritasa.Tools.Domain.Exceptions;
using WellBot.Domain.Chats.Entities;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Pidor.AddPidorGameMessage
{
    /// <summary>
    /// Handler for <see cref="AddPidorGameMessageCommand"/>.
    /// </summary>
    internal class AddPidorGameMessageCommandHandler : AsyncRequestHandler<AddPidorGameMessageCommand>
    {
        private readonly IAppDbContext dbContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AddPidorGameMessageCommandHandler(IAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <inheritdoc/>
        protected override async Task Handle(AddPidorGameMessageCommand request, CancellationToken cancellationToken)
        {
            long? telegramUserId = default;
            if (!string.IsNullOrEmpty(request.TelegramUserName))
            {
                var registration = await dbContext.PidorRegistrations.FirstOrDefaultAsync(u => u.OriginalTelegramUserName == request.TelegramUserName, cancellationToken);
                if (registration == null)
                {
                    throw new DomainException("Could not find a user signed up for a game with the specified name.");
                }
                telegramUserId = registration.TelegramUserId;
            }

            var message = new PidorMessage
            {
                GameDay = request.TriggerDay,
                MessageRaw = request.Message,
                TelegramUserId = telegramUserId,
                Weight = request.MessageWeight,
                DayOfWeek = request.DayOfWeek,
                IsActive = true
            };
            dbContext.PidorResultMessages.Add(message);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
