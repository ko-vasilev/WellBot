using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.Data.ShowKeys;

/// <summary>
/// Show the list of exiting keys.
/// </summary>
public class ShowKeysCommand : IRequest<Unit>, IChatInfo
{
    /// <summary>
    /// Id of the current chat.
    /// </summary>
    public ChatId ChatId { get; init; }
}
