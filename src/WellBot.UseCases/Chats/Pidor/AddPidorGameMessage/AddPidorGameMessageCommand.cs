﻿using MediatR;
using WellBot.Domain.Chats;

namespace WellBot.UseCases.Chats.Pidor.AddPidorGameMessage;

/// <summary>
/// Add a new message option for the daily pidor game.
/// </summary>
public record AddPidorGameMessageCommand : IRequest<Unit>
{
    /// <summary>
    /// Message to show. Separated by <see cref="PidorMessage.MessagesSeparator"/>.
    /// Use <see cref="PidorMessage.UsernamePlaceholder"/> where you want to mention the chosen pidor user.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Optional name of the user. The message will only be used when this user is chosen as a pidor.
    /// </summary>
    public required string TelegramUserName { get; init; }

    /// <summary>
    /// Optional date. Will be used only when on this month/day.
    /// </summary>
    public DateTime? TriggerDay { get; init; }

    /// <summary>
    /// Message weight.
    /// </summary>
    public MessageWeight MessageWeight { get; init; }

    /// <summary>
    /// Optional, on which day of week this message should be used.
    /// </summary>
    public DayOfWeek? DayOfWeek { get; init; }
}
