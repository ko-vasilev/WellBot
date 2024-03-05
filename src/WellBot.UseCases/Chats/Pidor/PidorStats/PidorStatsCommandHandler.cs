using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using WellBot.DomainServices.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Pidor.PidorStats;

/// <summary>
/// Handler for <see cref="PidorStatsCommand"/>.
/// </summary>
internal class PidorStatsCommandHandler : AsyncRequestHandler<PidorStatsCommand>
{
    private readonly ITelegramBotClient botClient;
    private readonly IAppDbContext dbContext;
    private readonly CurrentChatService currentChatService;
    private readonly PidorGameService pidorGameService;
    private readonly TelegramMessageService telegramMessageService;

    /// <summary>
    /// Constructor.
    /// </summary>
    public PidorStatsCommandHandler(PidorGameService pidorGameService, CurrentChatService currentChatService, IAppDbContext dbContext, ITelegramBotClient botClient, TelegramMessageService telegramMessageService)
    {
        this.pidorGameService = pidorGameService;
        this.currentChatService = currentChatService;
        this.dbContext = dbContext;
        this.botClient = botClient;
        this.telegramMessageService = telegramMessageService;
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

        string message = stats.IntroMessage;
        var statsData = topUsers.Select(user => new
        {
            Victories = user.VictoriesCount,
            User = gameParticipants.First(p => p.PidorRegistration.TelegramUserId == user.TelegramUserId)
        });
        message += "\n"
            + string.Join('\n', statsData.Select((u, index) =>
            {
                var userName = telegramMessageService.GetUserFullName(u.User.User);
                return $"#{index + 1} - {userName} ({u.Victories}-кратный пидор)";
            }));

        await botClient.SendTextMessageAsync(request.ChatId, message);
    }

    private async Task<(IEnumerable<UserGameStats> Stats, string IntroMessage)> GetUserStats(IList<long> telegramUserIds, string argument, CancellationToken cancellationToken)
    {
        argument = (argument ?? string.Empty).Trim();
        var filteredUsers = dbContext.PidorRegistrations
            .Where(r => r.ChatId == currentChatService.ChatId)
            .Where(r => telegramUserIds.Contains(r.TelegramUserId));
        if (argument.Equals("all", System.StringComparison.InvariantCultureIgnoreCase))
        {
            var globalStats = await filteredUsers.Select(u => new UserGameStats
            {
                TelegramUserId = u.TelegramUserId,
                VictoriesCount = u.Wins.Count
            })
                .ToListAsync(cancellationToken);
            return (globalStats, "Топ за всё время:");
        }

        var year = pidorGameService.GetCurrentGameDay().Year;
        var introMessage = "Топ за текущий год:";
        if (int.TryParse(argument, out var paramYear))
        {
            year = paramYear;
            introMessage = $"Топ за {year} год:";
        }
        var stats = await filteredUsers.Select(u => new UserGameStats
        {
            TelegramUserId = u.TelegramUserId,
            VictoriesCount = u.Wins
                .Where(w => w.GameDay.Year == year)
                .Count()
        })
            .ToListAsync(cancellationToken);

        return (stats, introMessage);
    }

    private class UserGameStats
    {
        public long TelegramUserId { get; set; }

        public int VictoriesCount { get; set; }
    }
}
