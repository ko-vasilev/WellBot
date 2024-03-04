using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.AdminControl;

/// <summary>
/// Set some bot settings.
/// </summary>
public record AdminControlCommand : IRequest<Unit>
{
    /// <summary>
    /// Command arguments.
    /// </summary>
    public required string Arguments { get; init; }

    /// <summary>
    /// Telegram message.
    /// </summary>
    public required Message Message { get; init; }
}
