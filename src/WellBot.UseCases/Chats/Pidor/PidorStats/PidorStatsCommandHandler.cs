using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using WellBot.DomainServices.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Pidor.PidorStats
{
    /// <summary>
    /// Handler for <see cref="PidorStatsCommand"/>.
    /// </summary>
    internal class PidorStatsCommandHandler : AsyncRequestHandler<PidorStatsCommand>
    {
        private readonly ITelegramBotClient botClient;
        private readonly IAppDbContext dbContext;
        private readonly CurrentChatService currentChatService;
        private readonly PidorGameService pidorGameService;
        private readonly ReplyService replyService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PidorStatsCommandHandler(PidorGameService pidorGameService, CurrentChatService currentChatService, IAppDbContext dbContext, ITelegramBotClient botClient, ReplyService replyService)
        {
            this.pidorGameService = pidorGameService;
            this.currentChatService = currentChatService;
            this.dbContext = dbContext;
            this.botClient = botClient;
            this.replyService = replyService;
        }

        /// <inheritdoc/>
        protected override async Task Handle(PidorStatsCommand request, CancellationToken cancellationToken)
        {
            var gameParticipants = await pidorGameService.GetPidorUsersAsync(currentChatService.ChatId, request.ChatId, cancellationToken);
            var participantTelegramIds = gameParticipants.Select(user => user.PidorRegistration.TelegramUserId).ToList();
            var stats = await GetUserStats(participantTelegramIds, request.Arguments, cancellationToken);

            var topUsers = stats.Stats
                .Where(u => u.VictoriesCount > 0)
                .OrderByDescending(u => u.VictoriesCount)
                .Take(10)
                .ToList();
            if (topUsers.Count == 0)
            {
                await botClient.SendTextMessageAsync(request.ChatId, "Нет статистики за выбранный период");
                return;
            }

            string message = "Топ за текущий год:";
            if (stats.IsAll)
            {
                message = "Топ за всё время:";
            }
            var statsData = topUsers.Select(user => new
            {
                Victories = user.VictoriesCount,
                User = gameParticipants.First(p => p.PidorRegistration.TelegramUserId == user.TelegramUserId)
            });
            message += "\n" + string.Join('\n', statsData.Select((u, index) => $"#{index + 1} - {replyService.GetUserFullName(u.User.User)} ({u.Victories}-кратный пидор)"));

            await botClient.SendTextMessageAsync(request.ChatId, message);
        }

        private async Task<(IEnumerable<UserGameStats> Stats, bool IsAll)> GetUserStats(IList<long> telegramUserIds, string argument, CancellationToken cancellationToken)
        {
            bool getAllStats = string.Equals(argument, "all", System.StringComparison.InvariantCultureIgnoreCase);
            var filteredUsers = dbContext.PidorRegistrations
                .Where(r => r.ChatId == currentChatService.ChatId)
                .Where(r => telegramUserIds.Contains(r.TelegramUserId));
            IEnumerable<UserGameStats> stats;
            if (getAllStats)
            {
                stats = await filteredUsers.Select(u => new UserGameStats
                {
                    TelegramUserId = u.TelegramUserId,
                    VictoriesCount = u.Wins.Count
                })
                    .ToListAsync(cancellationToken);
            }
            else
            {

                var currentDay = pidorGameService.GetCurrentGameDay();
                stats = await filteredUsers.Select(u => new UserGameStats
                {
                    TelegramUserId = u.TelegramUserId,
                    VictoriesCount = u.Wins
                        .Where(w => w.GameDay.Year == currentDay.Year)
                        .Count()
                })
                    .ToListAsync(cancellationToken);
            }

            return (stats, getAllStats);
        }

        private class UserGameStats
        {
            public long TelegramUserId { get; set; }

            public int VictoriesCount { get; set; }
        }
    }
}
