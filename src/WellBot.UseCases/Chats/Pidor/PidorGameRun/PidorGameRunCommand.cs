using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.Pidor.PidorGameRun;

/// <summary>
/// Run the daily pidor game.
/// </summary>
public record PidorGameRunCommand : IRequest<Unit>, IChatInfo
{
    /// <summary>
    /// Id of the chat for which the game should be run.
    /// </summary>
    public ChatId ChatId { get; init; }
}
