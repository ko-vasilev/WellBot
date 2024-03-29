﻿using MediatR;

namespace WellBot.UseCases.Chats.AutomaticMessages.DeleteAutomaticMessage;

/// <summary>
/// Delete an automatic message.
/// </summary>
public record DeleteAutomaticMessageCommand : IRequest<Unit>
{
    /// <summary>
    /// Id of the message.
    /// </summary>
    public int Id { get; set; }
}
