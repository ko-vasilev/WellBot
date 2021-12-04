using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.HandleTelegramAction
{
    /// <summary>
    /// Command for handling a telegram action.
    /// </summary>
    public record HandleTelegramActionCommand : IRequest
    {
        /// <summary>
        /// Description of an action.
        /// </summary>
        public Update Action { get; init; }
    }
}
