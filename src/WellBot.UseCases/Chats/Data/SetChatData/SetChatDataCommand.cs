using MediatR;
using Telegram.BotAPI.AvailableTypes;

namespace WellBot.UseCases.Chats.Data.SetChatData;

/// <summary>
/// Store a data in chat.
/// </summary>
public record SetChatDataCommand : IRequest<Unit>, IChatInfo
{
    /// <summary>
    /// Id of the associated chat.
    /// </summary>
    public required long ChatId { get; init; }

    /// <summary>
    /// Command arguments.
    /// </summary>
    public required string Arguments { get; init; }

    /// <summary>
    /// Telegram message.
    /// </summary>
    public required Message Message { get; init; }
}
