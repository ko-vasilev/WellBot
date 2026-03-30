using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellBot.UseCases.Chats.AutomaticMessages.CreateAutomaticMessage;
using WellBot.UseCases.Chats.AutomaticMessages.DeleteAutomaticMessage;
using WellBot.UseCases.Chats.AutomaticMessages.GetAutomaticMessages;
using WellBot.UseCases.Chats.Topics.GetTopicList;
using WellBot.UseCases.Chats.Topics.UpsertTopic;

namespace WellBot.Web.Controllers;

/// <summary>
/// Provides methods for accessing and modifying chat information.
/// </summary>
[Authorize]
[ApiController]
[Route("api/chat")]
[ApiExplorerSettings(GroupName = "chat")]
public class ChatController : ControllerBase
{
    private readonly IMediator mediator;

    /// <summary>
    /// Constructor.
    /// </summary>
    public ChatController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Get list of existing passive topics and their settings.
    /// </summary>
    /// <returns>List of topics.</returns>
    [HttpGet("topics")]
    public async Task<IEnumerable<TopicDto>> Topics(CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetTopicListQuery(), cancellationToken);
    }

    /// <summary>
    /// Create or update a topic.
    /// </summary>
    /// <returns>Id of the topic.</returns>
    [HttpPost("topic")]
    public async Task<int> Topic(UpsertTopicCommand request, CancellationToken cancellationToken)
    {
        return await mediator.Send(request, cancellationToken);
    }

    /// <summary>
    /// Get list of all automatic messages.
    /// </summary>
    /// <returns>All existing automatic messages.</returns>
    [HttpGet("automatic-messages")]
    public async Task<IEnumerable<AutomaticMessageDto>> GetAutomaticMessages(CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetAutomaticMessagesCommand(), cancellationToken);
    }

    /// <summary>
    /// Delete an automatic message.
    /// </summary>
    /// <param name="messageId">Id of the message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("automatic-message/{messageId}")]
    public async Task DeleteAutomaticMessage(int messageId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteAutomaticMessageCommand()
        {
            Id = messageId
        }, cancellationToken);
    }

    /// <summary>
    /// Create a new automatic message.
    /// </summary>
    /// <param name="request">Request parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Id of the created message.</returns>
    [HttpPost("automatic-message")]
    public async Task<int> CreateAutomaticMessage(CreateAutomaticMessageCommand request, CancellationToken cancellationToken)
    {
        return await mediator.Send(request, cancellationToken);
    }
}
