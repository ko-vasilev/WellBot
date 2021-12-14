using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.Data.ShowData
{
    /// <summary>
    /// Output a stored data by key <see cref="Arguments"/> to a chat.
    /// </summary>
    public class ShowDataCommand : IRequest, IChatInfo
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
        /// Id of the message sent by user.
        /// </summary>
        public int MessageId { get; init; }
    }
}
