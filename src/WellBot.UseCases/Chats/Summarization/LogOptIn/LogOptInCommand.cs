using MediatR;

namespace WellBot.UseCases.Chats.Summarization.LogOptIn;

/// <summary>
/// Removes an opt out of a user for logging their messages.
/// </summary>
public class LogOptInCommand : IRequest<Unit>, IChatInfo
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
    /// Id of the message that triggered the command.
    /// </summary>
    public int MessageId { get; init; }
}
