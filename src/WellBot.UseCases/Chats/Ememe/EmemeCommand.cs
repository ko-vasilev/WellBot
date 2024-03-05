using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.Ememe;

/// <summary>
/// Send a random meme to the chat.
/// </summary>
public record EmemeCommand : IRequest<Unit>
{
    /// <summary>
    /// Id of the chat.
    /// </summary>
    public required ChatId ChatId { get; init; }
}
