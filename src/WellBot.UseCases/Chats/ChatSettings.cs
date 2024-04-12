namespace WellBot.UseCases.Chats;

/// <summary>
/// Contains global settings for chats.
/// </summary>
public class ChatSettings
{
    /// <summary>
    /// Ids of the superadmin users in Telegram.
    /// </summary>
    public IEnumerable<long> SuperadminIds { get; set; } = Enumerable.Empty<long>();
}
