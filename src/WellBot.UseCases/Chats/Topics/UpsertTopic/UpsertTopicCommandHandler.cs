using MediatR;
using Microsoft.EntityFrameworkCore;
using Saritasa.Tools.Domain.Exceptions;
using WellBot.Domain.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.UseCases.Chats;
using WellBot.UseCases.Chats.RegularMessageHandles.Reply;

namespace WellBot.UseCases.Chats.Topics.UpsertTopic;

/// <summary>
/// Handler for <see cref="UpsertTopicCommand"/>.
/// </summary>
internal class UpsertTopicCommandHandler : IRequestHandler<UpsertTopicCommand, int>
{
    private readonly IAppDbContext dbContext;
    private readonly ChatMapper mapper;
    private readonly PassiveTopicService passiveTopicService;

    public UpsertTopicCommandHandler(IAppDbContext dbContext, ChatMapper mapper, PassiveTopicService passiveTopicService)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.passiveTopicService = passiveTopicService;
    }

    /// <inheritdoc/>
    public async Task<int> Handle(UpsertTopicCommand request, CancellationToken cancellationToken)
    {
        PassiveTopic? topic;
        if (request.Id != null)
        {
            topic = await dbContext.PassiveTopics.FirstOrDefaultAsync(t => t.Id == request.Id.Value, cancellationToken);
            if (topic == null)
            {
                throw new NotFoundException();
            }
        }
        else
        {
            topic = new PassiveTopic()
            {
                Name = request.Name,
                Regex = request.Regex
            };
            dbContext.PassiveTopics.Add(topic);
        }

        mapper.UpdatePassiveTopic(request, topic);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Update topics cache.
        var allTopics = await dbContext.PassiveTopics
            .AsNoTracking()
            .ToListAsync();
        passiveTopicService.Update(allTopics);

        return topic.Id;
    }
}
