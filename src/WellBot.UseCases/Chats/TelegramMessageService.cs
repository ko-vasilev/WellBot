using System.Linq;
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
    public class TelegramMessageService
    {
        private readonly ITelegramBotClient botClient;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TelegramMessageService(ITelegramBotClient botClient) => this.botClient = botClient;

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

        /// <summary>
        /// Get a HTML content of a message text.
        /// </summary>
        /// <param name="message">Message object.</param>
        /// <returns>String representing an HTML text content of a message.</returns>
        public string GetMessageTextHtml(Message message)
        {
            if (message.Text == null)
            {
                return message.Caption;
            }

            var text = message.Text;
            if (message.Entities == null)
            {
                return text;
            }

            var previousOffset = int.MaxValue;
            void AddTextMarkup(string format, int startPosition, int length)
            {
                var textPart = text.Substring(startPosition, length);
                var newText = string.Format(format, HttpUtility.HtmlEncode(textPart));

                text = text.Substring(0, startPosition)
                    + newText
                    + text.Substring(startPosition + length);
            }
            foreach (var formatEntity in message.Entities.Reverse())
            {
                // To fix the bug when there are multiple subsequent tags like <b><i></i></b>
                // We will display only the first encountered one.
                if (previousOffset == formatEntity.Offset)
                {
                    continue;
                }
                previousOffset = formatEntity.Offset;

                if (formatEntity.Type == Telegram.Bot.Types.Enums.MessageEntityType.Bold)
                {
                    AddTextMarkup("<b>{0}</b>", formatEntity.Offset, formatEntity.Length);
                    continue;
                }

                if (formatEntity.Type == Telegram.Bot.Types.Enums.MessageEntityType.Italic)
                {
                    AddTextMarkup("<i>{0}</i>", formatEntity.Offset, formatEntity.Length);
                    continue;
                }

                if (formatEntity.Type == Telegram.Bot.Types.Enums.MessageEntityType.Code)
                {
                    AddTextMarkup("<code>{0}</code>", formatEntity.Offset, formatEntity.Length);
                    continue;
                }

                if (formatEntity.Type == Telegram.Bot.Types.Enums.MessageEntityType.Pre)
                {
                    AddTextMarkup("<pre>{0}</pre>", formatEntity.Offset, formatEntity.Length);
                    continue;
                }

                if (formatEntity.Type == Telegram.Bot.Types.Enums.MessageEntityType.Strikethrough)
                {
                    AddTextMarkup("<s>{0}</s>", formatEntity.Offset, formatEntity.Length);
                    continue;
                }

                if (formatEntity.Type == Telegram.Bot.Types.Enums.MessageEntityType.TextLink)
                {
                    var link = formatEntity.Url ?? "{0}";
                    AddTextMarkup($"<a href=\"{link}\">{{0}}</a>", formatEntity.Offset, formatEntity.Length);
                    continue;
                }

                if (formatEntity.Type == Telegram.Bot.Types.Enums.MessageEntityType.TextMention)
                {
                    AddTextMarkup($"<a href=\"tg://user?id={formatEntity.User.Id}\">{{0}}</a>", formatEntity.Offset, formatEntity.Length);
                    continue;
                }

                if (formatEntity.Type == Telegram.Bot.Types.Enums.MessageEntityType.Underline)
                {
                    AddTextMarkup("<u>{0}</u>", formatEntity.Offset, formatEntity.Length);
                    continue;
                }

                if (formatEntity.Type == Telegram.Bot.Types.Enums.MessageEntityType.Url)
                {
                    AddTextMarkup("<a href=\"{0}\">{0}</a>", formatEntity.Offset, formatEntity.Length);
                    continue;
                }
            }

            return text;
        }
    }
}
