using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.Slap
{
    /// <summary>
    /// Slap a bot.
    /// </summary>
    public class SlapCommand : IRequest
    {
        /// <summary>
        /// Id of the chat.
        /// </summary>
        public ChatId ChatId { get; init; }

        /// <summary>
        /// Id of the sent message.
        /// </summary>
        public int MessageId { get; init; }
    }
}
