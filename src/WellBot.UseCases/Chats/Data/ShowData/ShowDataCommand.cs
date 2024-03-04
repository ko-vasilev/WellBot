using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.Data.ShowData;

/// <summary>
/// Output a stored data by key <see cref="Key"/> to a chat.
/// </summary>
public class ShowDataCommand : IRequest<Unit>, IChatInfo
{
    /// <summary>
    /// Id of the associated chat.
    /// </summary>
    public ChatId ChatId { get; init; }

    /// <summary>
    /// Data key.
    /// </summary>
    public string Key { get; init; }

    /// <summary>
    /// Id of the message sent by user.
    /// </summary>
    public int MessageId { get; init; }

    /// <summary>
    /// Id of the user who sent the request to show the value.
    /// </summary>
    public long SenderUserId { get; init; }

    /// <summary>
    /// Id of the message the current message was sent as a reply for.
    /// </summary>
    public int? ReplyMessageId { get; init; }
}
