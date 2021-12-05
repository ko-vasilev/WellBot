using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.Pidor.PidorGameRegister
{
    /// <summary>
    /// Registers user in a Daily Pidor game.
    /// </summary>
    public record PidorGameRegisterCommand : IRequest
    {
        /// <summary>
        /// Id of the chat.
        /// </summary>
        public ChatId ChatId { get; init; }

        /// <summary>
        /// Id of the user who wants to sign up for the game.
        /// </summary>
        public long TelegramUserId { get; init; }
    }
}
