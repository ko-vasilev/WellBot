using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.Ememe
{
    /// <summary>
    /// Send a random meme to the chat.
    /// </summary>
    public record EmemeCommand : IRequest
    {
        /// <summary>
        /// Id of the chat.
        /// </summary>
        public ChatId ChatId { get; init; }
    }
}
