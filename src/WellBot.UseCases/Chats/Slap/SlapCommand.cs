using MediatR;

namespace WellBot.UseCases.Chats.Slap;

/// <summary>
/// Slap a bot.
/// </summary>
public class SlapCommand : IRequest<Unit>, IChatInfo
{
    /// <summary>
    /// Id of the chat.
    /// </summary>
    public required long ChatId { get; init; }

    /// <summary>
    /// Id of the sent message.
    /// </summary>
    public int MessageId { get; init; }
}
