using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.Pidor.PidorRules;

/// <summary>
/// Show the pidor game rules.
/// </summary>
public record PidorRulesCommand : IRequest<Unit>, IChatInfo
{
    /// <summary>
    /// Id of the telegram chat.
    /// </summary>
    public required ChatId ChatId { get; init; }
}
