using MediatR;

namespace WellBot.UseCases.Chats.Pidor.PidorList;

/// <summary>
/// Get list of users signed up for daily pidor game.
/// </summary>
public record PidorListCommand : IRequest<Unit>, IChatInfo
{
    /// <summary>
    /// Id of the telegram chat.
    /// </summary>
    public required long ChatId { get; init; }

    /// <summary>
    /// Id of the user who has sent the request.
    /// </summary>
    public long TelegramUserId { get; init; }

    /// <summary>
    /// Additional command arguments.
    /// </summary>
    public required string Arguments { get; init; }
}
