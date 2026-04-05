using MediatR;
using Microsoft.EntityFrameworkCore;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.UseCases.Chats;

namespace WellBot.UseCases.Chats.AutomaticMessages.GetAutomaticMessages;

/// <summary>
/// Handler for <see cref="GetAutomaticMessagesCommand"/>.
/// </summary>
internal class GetAutomaticMessagesCommandHandler : IRequestHandler<GetAutomaticMessagesCommand, IEnumerable<AutomaticMessageDto>>
{
    private readonly IAppDbContext dbContext;
    private readonly ChatMapper mapper;

    public GetAutomaticMessagesCommandHandler(IAppDbContext dbContext, ChatMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<AutomaticMessageDto>> Handle(GetAutomaticMessagesCommand request, CancellationToken cancellationToken)
    {
        return await mapper
            .ProjectToAutomaticMessageDtos(dbContext.AutomaticMessageTemplates)
            .ToListAsync(cancellationToken);
    }
}
