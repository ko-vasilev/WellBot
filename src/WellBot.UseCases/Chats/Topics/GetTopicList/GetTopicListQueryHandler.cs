using MediatR;
using Microsoft.EntityFrameworkCore;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.UseCases.Chats;

namespace WellBot.UseCases.Chats.Topics.GetTopicList;

/// <summary>
/// Handler for <see cref="GetTopicListQuery"/>.
/// </summary>
internal class GetTopicListQueryHandler : IRequestHandler<GetTopicListQuery, IEnumerable<TopicDto>>
{
    private readonly IAppDbContext dbContext;
    private readonly ChatMapper mapper;

    public GetTopicListQueryHandler(IAppDbContext dbContext, ChatMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TopicDto>> Handle(GetTopicListQuery request, CancellationToken cancellationToken)
    {
        return await mapper
            .ProjectToTopicDtos(dbContext.PassiveTopics)
            .ToListAsync(cancellationToken);
    }
}
