using MediatR;
using Microsoft.EntityFrameworkCore;
using Saritasa.Tools.Domain.Exceptions;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Pidor.DeleteGameMessage;

/// <summary>
/// Handler for <see cref="DeleteGameMessageCommand"/>.
/// </summary>
internal class DeleteGameMessageCommandHandler : AsyncRequestHandler<DeleteGameMessageCommand>
{
    private readonly IAppDbContext dbContext;

    /// <summary>
    /// Constructor.
    /// </summary>
    public DeleteGameMessageCommandHandler(IAppDbContext dbContext) => this.dbContext = dbContext;

    /// <inheritdoc/>
    protected override async Task Handle(DeleteGameMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await dbContext.PidorResultMessages.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (message == null)
        {
            throw new NotFoundException();
        }

        message.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
