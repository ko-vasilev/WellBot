﻿using System.Collections.Generic;
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
        public async Task<IEnumerable<ChatMember>> GetPidorUsersAsync(int chatId, ChatId telegramChatId, CancellationToken cancellationToken)
        {
            var registrations = dbContext.PidorRegistrations
                .AsNoTracking()
                .Where(reg => reg.ChatId == chatId)
                .AsAsyncEnumerable();
            var users = new List<ChatMember>();
            await foreach (var registration in registrations)
            {
                var telegramUser = await botClient.GetChatMemberAsync(telegramChatId, registration.TelegramUserId, cancellationToken);
                if (!IsActiveUser(telegramUser))
                {
                    continue;
                }
                users.Add(telegramUser);
            }

            return users;
        }

        private bool IsActiveUser(ChatMember chatMember) => chatMember.Status == ChatMemberStatus.Administrator
            || chatMember.Status == ChatMemberStatus.Creator
            || chatMember.Status == ChatMemberStatus.Member;
    }
}
