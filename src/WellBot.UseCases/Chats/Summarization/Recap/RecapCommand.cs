using System;
using MediatR;

namespace WellBot.UseCases.Chats.Summarization.Recap;

/// <summary>
/// Request a recap in the chat.
/// </summary>
public class RecapCommand : IRequest<Unit>, IChatInfo
{
    /// <summary>
    /// Id of the telegram chat.
    /// </summary>
    public required long ChatId { get; init; }

    /// <summary>
    /// Additional command arguments.
    /// </summary>
    public required string Arguments { get; init; }

    /// <summary>
    /// Id of the message that triggered the command.
    /// </summary>
    public int MessageId { get; init; }
}
