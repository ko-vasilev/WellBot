using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.Pidor.PidorList
{
    /// <summary>
    /// Get list of users signed up for daily pidor game.
    /// </summary>
    public record PidorListCommand : IRequest, IChatInfo
    {
        /// <summary>
        /// Id of the telegram chat.
        /// </summary>
        public ChatId ChatId { get; init; }

        /// <summary>
        /// Id of the user who has sent the request.
        /// </summary>
        public long TelegramUserId { get; init; }
    }
}
