﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WellBot.UseCases.Chats.Pidor.AddPidorGameMessage;

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
        /// Get current logged user info.
        /// </summary>
        /// <returns>Current logged user info.</returns>
        /// <param name="command">Request parameters.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
        [HttpPost]
        public async Task AddPidorMessage(AddPidorGameMessageCommand command, CancellationToken cancellationToken)
        {
            await mediator.Send(command, cancellationToken);
        }
    }
}
