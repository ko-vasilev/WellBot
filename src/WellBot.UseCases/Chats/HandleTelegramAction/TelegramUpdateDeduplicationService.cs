using System.Collections.Concurrent;

namespace WellBot.UseCases.Chats.HandleTelegramAction;

/// <summary>
/// Keeps track of Telegram updates that are currently being processed.
/// </summary>
public class TelegramUpdateDeduplicationService
{
    private readonly ConcurrentDictionary<long, byte> inFlightUpdates = new();

    /// <summary>
    /// Attempt to mark an update as currently executing.
    /// </summary>
    /// <param name="updateId">Telegram update id.</param>
    /// <returns>True when the update was registered, false when it is already in-flight.</returns>
    public bool TryBegin(long updateId)
    {
        return inFlightUpdates.TryAdd(updateId, 0);
    }

    /// <summary>
    /// Stop tracking an in-flight update.
    /// </summary>
    /// <param name="updateId">Telegram update id.</param>
    public void End(long updateId)
    {
        inFlightUpdates.TryRemove(updateId, out _);
    }
}
