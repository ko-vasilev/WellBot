using MediatR;

namespace WellBot.UseCases.Chats.Summarization.LogOptOut;

/// <summary>
/// Command for opting out from message logging.
/// </summary>
public class LogOptOutCommand : IRequest<Unit>, IChatInfo
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
