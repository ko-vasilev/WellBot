using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.Data.DeleteChatData;

/// <summary>
/// Delete a data by key from chat.
/// </summary>
public record DeleteChatDataCommand : IRequest<Unit>, IChatInfo
{
    /// <summary>
    /// Id of the associated chat.
    /// </summary>
    public ChatId ChatId { get; init; }

    /// <summary>
    /// Data key to delete.
    /// </summary>
    public string Key { get; init; }

    /// <summary>
    /// Id of the message sent by user.
    /// </summary>
    public int MessageId { get; init; }
}
