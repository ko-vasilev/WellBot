using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Pidor
{
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
                var telegramUser = await botClient.GetChatMemberAsync(telegramChatId, registration.TelegramUserId, cancellationToken);
                if (!IsActiveUser(telegramUser))
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
        /// Get the current day of the game.
        /// </summary>
        /// <returns>Current game day.</returns>
        public DateTime GetCurrentGameDay()
        {
            var moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            var currentMoscowTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, moscowTimeZone);
            return currentMoscowTime.Date;
        }

        private bool IsActiveUser(ChatMember chatMember) => chatMember.Status == ChatMemberStatus.Administrator
            || chatMember.Status == ChatMemberStatus.Creator
            || chatMember.Status == ChatMemberStatus.Member;
    }
}
