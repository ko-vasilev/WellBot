using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.Data.SetChatData
{
    /// <summary>
    /// Store a data in chat.
    /// </summary>
    public record SetChatDataCommand : IRequest, IChatInfo
    {
        /// <summary>
        /// Id of the associated chat.
        /// </summary>
        public ChatId ChatId { get; init; }

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
