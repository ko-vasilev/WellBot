using MediatR;
using Saritasa.Tools.EntityFrameworkCore;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.AutomaticMessages.DeleteAutomaticMessage;

/// <summary>
/// Handler for <see cref="DeleteAutomaticMessageCommand"/>.
/// </summary>
internal class DeleteAutomaticMessageCommandHandler : AsyncRequestHandler<DeleteAutomaticMessageCommand>
{
    private readonly IAppDbContext dbContext;

    /// <summary>
    /// Constructor.
    /// </summary>
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
