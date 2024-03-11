using MediatR;

namespace WellBot.UseCases.Chats.Prikol;

/// <summary>
/// Get a random prikol.
/// </summary>
public class PrikolCommand : IRequest<Unit>, IChatInfo
{
    /// <summary>
    /// Id of the chat.
    /// </summary>
    public required long ChatId { get; init; }
}
