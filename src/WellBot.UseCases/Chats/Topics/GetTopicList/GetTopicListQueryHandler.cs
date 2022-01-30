using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Topics.GetTopicList
{
    /// <summary>
    /// Handler for <see cref="GetTopicListQuery"/>.
    /// </summary>
    internal class GetTopicListQueryHandler : IRequestHandler<GetTopicListQuery, IEnumerable<TopicDto>>
    {
        private readonly IAppDbContext dbContext;
        private readonly IMapper mapper;

        public GetTopicListQueryHandler(IAppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TopicDto>> Handle(GetTopicListQuery request, CancellationToken cancellationToken)
        {
            return await mapper.ProjectTo<TopicDto>(dbContext.PassiveTopics)
                .ToListAsync(cancellationToken);
        }
    }
}
