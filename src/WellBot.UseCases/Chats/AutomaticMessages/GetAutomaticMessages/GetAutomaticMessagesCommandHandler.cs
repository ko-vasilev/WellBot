using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.AutomaticMessages.GetAutomaticMessages
{
    /// <summary>
    /// Handler for <see cref="GetAutomaticMessagesCommand"/>.
    /// </summary>
    internal class GetAutomaticMessagesCommandHandler : IRequestHandler<GetAutomaticMessagesCommand, IEnumerable<AutomaticMessageDto>>
    {
        private readonly IAppDbContext dbContext;
        private readonly IMapper mapper;

        public GetAutomaticMessagesCommandHandler(IAppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<AutomaticMessageDto>> Handle(GetAutomaticMessagesCommand request, CancellationToken cancellationToken)
        {
            return await mapper.ProjectTo<AutomaticMessageDto>(dbContext.AutomaticMessageTemplates)
                .ToListAsync(cancellationToken);
        }
    }
}
