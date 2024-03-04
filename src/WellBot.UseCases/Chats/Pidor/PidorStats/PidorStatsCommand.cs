using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.Pidor.PidorStats;

/// <summary>
/// Get statistics for the daily pidor game.
/// </summary>
public record PidorStatsCommand : IRequest<Unit>, IChatInfo
{
    /// <summary>
    /// Telegram chat id.
    /// </summary>
    public ChatId ChatId { get; init; }

    /// <summary>
    /// Command arguments.
    /// </summary>
    public string Arguments { get; init; }
}
