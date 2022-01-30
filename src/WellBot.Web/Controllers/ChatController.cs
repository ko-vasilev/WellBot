using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("Topics")]
        public async Task<IEnumerable<TopicDto>> Topics(CancellationToken cancellationToken)
        {
            return await mediator.Send(new GetTopicListQuery(), cancellationToken);
        }

        /// <summary>
        /// Create or update a topic.
        /// </summary>
        /// <returns>Id of the topic.</returns>
        [HttpPost("Topic")]
        public async Task<int> Topic(UpsertTopicCommand request, CancellationToken cancellationToken)
        {
            return await mediator.Send(request, cancellationToken);
        }
    }
}
