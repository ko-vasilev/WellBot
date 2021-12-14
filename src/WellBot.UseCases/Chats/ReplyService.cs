using System.Threading.Tasks;
using System.Web;
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

        /// <summary>
        /// Get full name of the user.
        /// </summary>
        /// <param name="user">User info.</param>
        /// <returns>User full name.</returns>
        public string GetUserFullName(User user)
        {
            var name = user.FirstName;
            if (string.IsNullOrEmpty(name))
            {
                return user.LastName;
            }

            if (!string.IsNullOrEmpty(user.LastName))
            {
                name += " " + user.LastName;
            }
            return name;
        }

        /// <summary>
        /// Get a string that allows to mention a user.
        /// The resulting string can be used with <see cref="Telegram.Bot.Types.Enums.ParseMode.Html"/> format.
        /// </summary>
        /// <param name="user">User reference.</param>
        /// <returns>String to use for tagging a user.</returns>
        public string GetPersonMentionHtml(User user)
        {
            if (!string.IsNullOrEmpty(user.Username))
            {
                return "@" + user.Username;
            }

            var userFullName = GetUserFullName(user);
            return $"<a href=\"tg://user?id={user.Id}\">{HttpUtility.HtmlEncode(userFullName)}</a>";
        }
    }
}
