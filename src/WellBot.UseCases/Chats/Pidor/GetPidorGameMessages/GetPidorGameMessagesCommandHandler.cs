using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Pidor.GetPidorGameMessages
{
    /// <summary>
    /// Handler for <see cref="GetPidorGameMessagesCommand"/>.
    /// </summary>
    internal class GetPidorGameMessagesCommandHandler : IRequestHandler<GetPidorGameMessagesCommand, IEnumerable<PidorGameMessageDto>>
    {
        private readonly IAppDbContext dbContext;
        private readonly IMapper mapper;

        public GetPidorGameMessagesCommandHandler(IAppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PidorGameMessageDto>> Handle(GetPidorGameMessagesCommand request, CancellationToken cancellationToken)
        {
            return await mapper.ProjectTo<PidorGameMessageDto>(dbContext.PidorResultMessages)
                .ToListAsync(cancellationToken);
        }
    }
}
