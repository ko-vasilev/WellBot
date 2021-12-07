using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using WellBot.UseCases.Common.Extensions;

namespace WellBot.UseCases.Chats
{
    /// <summary>
    /// Service for sending some common messages to user.
    /// </summary>
    public class ReplyService
    {
        private readonly ITelegramBotClient botClient;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReplyService(ITelegramBotClient botClient) => this.botClient = botClient;

        private static readonly string[] successReplies = new string[]
        {
            "Ок",
            "ОК!",
            "👌",
            "Принято",
        };

        /// <summary>
        /// Send a "success" type of response to user.
        /// </summary>
        public async Task SendSuccessAsync(ChatId chatId)
        {
            var reply = successReplies.PickRandom();
            await botClient.SendTextMessageAsync(chatId, reply);
        }
    }
}
