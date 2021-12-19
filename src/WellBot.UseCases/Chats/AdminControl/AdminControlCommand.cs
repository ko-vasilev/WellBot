using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.AdminControl
{
    /// <summary>
    /// Set some bot settings.
    /// </summary>
    public record AdminControlCommand : IRequest
    {
        /// <summary>
        /// Command arguments.
        /// </summary>
        public string Arguments { get; init; }

        /// <summary>
        /// Telegram message.
        /// </summary>
        public Message Message { get; init; }
    }
}
