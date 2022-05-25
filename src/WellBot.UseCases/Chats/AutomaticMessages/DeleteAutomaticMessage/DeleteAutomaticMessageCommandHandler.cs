using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Saritasa.Tools.EFCore;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.AutomaticMessages.DeleteAutomaticMessage
{
    /// <summary>
    /// Handler for <see cref="DeleteAutomaticMessageCommand"/>.
    /// </summary>
    internal class DeleteAutomaticMessageCommandHandler : AsyncRequestHandler<DeleteAutomaticMessageCommand>
    {
        private readonly IAppDbContext dbContext;

        public DeleteAutomaticMessageCommandHandler(IAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <inheritdoc/>
        protected override async Task Handle(DeleteAutomaticMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await dbContext.AutomaticMessageTemplates.GetAsync(cancellationToken, request.Id);
            dbContext.AutomaticMessageTemplates.Remove(message);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
