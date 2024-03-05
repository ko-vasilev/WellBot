using MediatR;

namespace WellBot.UseCases.Chats.Pidor.DeleteGameMessage;

/// <summary>
/// Command to delete a game message.
/// </summary>
public record DeleteGameMessageCommand : IRequest<Unit>
{
    /// <summary>
    /// Id of the message.
    /// </summary>
    public int Id { get; init; }
}
