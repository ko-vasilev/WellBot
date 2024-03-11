using WellBot.UseCases.Common.Collections;

namespace WellBot.UseCases.Chats;

/// <summary>
/// Allows limiting user's access to bot features.
/// </summary>
public class MessageRateLimitingService
{
    private readonly ExpirableCollection<string> rateLimits = new();
    private static readonly TimeSpan rateLimitDuration = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Check if user is rate limited.
    /// </summary>
    /// <param name="chatId">Id of the telegram chat.</param>
    /// <param name="userId">Id of the user.</param>
    /// <param name="key">Additional key to distinguish different limits within a single chat.</param>
    /// <param name="allowedIn">If rate is limited, returns how much time until rate limit falls off.</param>
    /// <returns>True if rate is limited.</returns>
    public bool IsRateLimited(long chatId, long userId, string key, out TimeSpan allowedIn)
    {
        var requestItemKey = FormatItemKey(chatId, userId, key);
        return rateLimits.Contains(requestItemKey, out allowedIn);
    }

    /// <summary>
    /// Limit ability of a user to interact with the bot.
    /// </summary>
    /// <param name="chatId">Id of the telegram chat.</param>
    /// <param name="userId">Id of the user.</param>
    /// <param name="key">Additional key to distinguish different limits within a single chat.</param>
    public void LimitRate(long chatId, long userId, string key)
    {
        var requestItemKey = FormatItemKey(chatId, userId, key);
        var expirationUtc = DateTime.UtcNow.Add(rateLimitDuration);
        rateLimits.AddOrUpdate(requestItemKey, expirationUtc);
    }

    private string FormatItemKey(long chatId, long userId, string key) => $"{chatId}_{userId}_{key}";
}
