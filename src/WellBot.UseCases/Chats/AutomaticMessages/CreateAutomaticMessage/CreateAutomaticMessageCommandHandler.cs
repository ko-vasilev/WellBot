using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WellBot.Domain.Chats.Entities;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.AutomaticMessages.CreateAutomaticMessage
{
    /// <summary>
    /// Handler for <see cref="CreateAutomaticMessageCommand"/>.
    /// </summary>
    internal class CreateAutomaticMessageCommandHandler : IRequestHandler<CreateAutomaticMessageCommand, int>
    {
        private readonly IAppDbContext dbContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CreateAutomaticMessageCommandHandler(IAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<int> Handle(CreateAutomaticMessageCommand request, CancellationToken cancellationToken)
        {
            var messageTemplate = new AutomaticMessageTemplate()
            {
                ChatId = request.ChatId,
                CronInterval = request.CronInterval,
                ImageSearchQuery = request.ImageSearchQuery,
                LastTriggeredDate = DateTime.MinValue,
                Message = request.Message,
            };
            dbContext.AutomaticMessageTemplates.Add(messageTemplate);

            await dbContext.SaveChangesAsync(cancellationToken);

            return messageTemplate.Id;
        }
    }
}
