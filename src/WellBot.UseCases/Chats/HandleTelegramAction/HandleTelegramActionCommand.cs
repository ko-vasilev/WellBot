﻿using MediatR;
using Telegram.BotAPI.GettingUpdates;

namespace WellBot.UseCases.Chats.HandleTelegramAction;

/// <summary>
/// Command for handling a telegram action.
/// </summary>
public record HandleTelegramActionCommand : IRequest<Unit>
{
    /// <summary>
    /// Description of an action.
    /// </summary>
    public required Update Action { get; init; }
}
