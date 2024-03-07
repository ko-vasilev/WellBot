namespace WellBot.UseCases.Chats;

/// <summary>
/// Should be used for commands that contain information about a chat.
/// </summary>
public interface IChatInfo
{
    /// <summary>
    /// Id of the current chat.
    /// </summary>
    long ChatId { get; }
}
