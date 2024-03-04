using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Pidor;

/// <summary>
/// Service containing common logic for Pidor game.
/// </summary>
public class PidorGameService
{
    private readonly ITelegramBotClient botClient;
    private readonly IAppDbContext dbContext;

    /// <summary>
    /// Constructor.
    /// </summary>
    public PidorGameService(ITelegramBotClient botClient, IAppDbContext dbContext)
    {
        this.botClient = botClient;
        this.dbContext = dbContext;
    }

    /// <summary>
    /// Get list of Telegram users who has signed up for the daily pidor game.
    /// </summary>
    /// <param name="chatId">Id of the chat in the database.</param>
    /// <param name="telegramChatId">Id of the telegram chat.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of active chat members.</returns>
    public async Task<IList<PidorParticipant>> GetPidorUsersAsync(int chatId, ChatId telegramChatId, CancellationToken cancellationToken)
    {
        var registrations = dbContext.PidorRegistrations
            .AsNoTracking()
            .Where(reg => reg.ChatId == chatId)
            .AsAsyncEnumerable();
        var users = new List<PidorParticipant>();
        await foreach (var registration in registrations)
        {
            var telegramUser = await GetPidorMemberAsync(telegramChatId, registration.TelegramUserId, cancellationToken);
            if (telegramUser == null)
            {
                continue;
            }
            users.Add(new PidorParticipant
            {
                PidorRegistration = registration,
                User = telegramUser.User
            });
        }

        return users;
    }

    /// <summary>
    /// Get information about an active user in a chat.
    /// </summary>
    /// <param name="telegramChatId">Id of the chat.</param>
    /// <param name="telegramUserId">Id of the telegram user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>User object or null if user is not found or not active.</returns>
    public async Task<ChatMember> GetPidorMemberAsync(ChatId telegramChatId, long telegramUserId, CancellationToken cancellationToken)
    {
        ChatMember telegramUser;
        try
        {
            telegramUser = await botClient.GetChatMemberAsync(telegramChatId, telegramUserId, cancellationToken);
        }
        catch (Telegram.Bot.Exceptions.ApiRequestException)
        {
            // User is not in the chat anymore.
            return null;
        }

        if (!IsActiveUser(telegramUser))
        {
            return null;
        }
        return telegramUser;
    }

    /// <summary>
    /// Get the current day of the game.
    /// </summary>
    /// <returns>Current game day.</returns>
    public DateTime GetCurrentGameDay()
    {
        var resetTimeZone = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/Moscow") ?? NodaTime.DateTimeZone.Utc;
        var offset = resetTimeZone.GetUtcOffset(NodaTime.Instant.FromDateTimeOffset(DateTimeOffset.UtcNow));
        var currentMoscowTime = DateTime.UtcNow.AddTicks(offset.Ticks);
        return currentMoscowTime.Date;
    }

    private bool IsActiveUser(ChatMember chatMember) => chatMember.Status == ChatMemberStatus.Administrator
        || chatMember.Status == ChatMemberStatus.Creator
        || chatMember.Status == ChatMemberStatus.Member;
}
