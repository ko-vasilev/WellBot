using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.Prikol;

/// <summary>
/// Get a random prikol.
/// </summary>
public class PrikolCommand : IRequest<Unit>
{
    /// <summary>
    /// Id of the chat.
    /// </summary>
    public required ChatId ChatId { get; init; }
}
