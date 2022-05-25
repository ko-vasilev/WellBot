using System;
using System.Collections.Generic;
using MediatR;

namespace WellBot.UseCases.Chats.AutomaticMessages.GetAutomaticMessages
{
    /// <summary>
    /// Get all existing automatic messages.
    /// </summary>
    public class GetAutomaticMessagesCommand : IRequest<IEnumerable<AutomaticMessageDto>>
    {
    }
}
