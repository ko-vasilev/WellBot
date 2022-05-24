using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellBot.UseCases.Chats.AutomaticMessages.CreateAutomaticMessage;
using WellBot.UseCases.Chats.AutomaticMessages.DeleteAutomaticMessage;
using WellBot.UseCases.Chats.AutomaticMessages.GetAutomaticMessages;
using WellBot.UseCases.Chats.Pidor.AddPidorGameMessage;
using WellBot.UseCases.Chats.Pidor.DeleteGameMessage;
using WellBot.UseCases.Chats.Pidor.GetPidorGameMessages;
using WellBot.UseCases.Chats.Topics.GetTopicList;
using WellBot.UseCases.Chats.Topics.UpsertTopic;

namespace WellBot.Web.Controllers
{
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
        /// Add a new pidor message.
        /// </summary>
        /// <param name="command">Request parameters.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        [HttpPost]
        public async Task AddPidorMessage(AddPidorGameMessageCommand command, CancellationToken cancellationToken)
        {
            await mediator.Send(command, cancellationToken);
        }

        /// <summary>
        /// Get all existing pidor game messages.
        /// </summary>
        /// <returns>List of pidor game messages.</returns>
        [HttpGet]
        public async Task<IEnumerable<PidorGameMessageDto>> PidorMessages(CancellationToken cancellationToken)
        {
            return await mediator.Send(new GetPidorGameMessagesCommand(), cancellationToken);
        }

        /// <summary>
        /// Delete a game message.
        /// </summary>
        /// <param name="command">Request parameters.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        [HttpDelete]
        public async Task PidorMessage(DeleteGameMessageCommand command, CancellationToken cancellationToken)
        {
            await mediator.Send(command, cancellationToken);
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
}
