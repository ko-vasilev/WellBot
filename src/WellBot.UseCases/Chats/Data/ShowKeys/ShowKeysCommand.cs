using MediatR;

namespace WellBot.UseCases.Chats.Data.ShowKeys;

/// <summary>
/// Show the list of exiting keys.
/// </summary>
public class ShowKeysCommand : IRequest<Unit>, IChatInfo
{
    /// <summary>
    /// Id of the current chat.
    /// </summary>
    public required long ChatId { get; init; }
}
