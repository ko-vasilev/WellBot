using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using WellBot.Domain.Chats.Entities;
using WellBot.UseCases.Chats.Dtos;

namespace WellBot.UseCases.Chats
{
    /// <summary>
    /// Service for sending some common messages to user.
    /// </summary>
    public class TelegramMessageService
    {
        private readonly ITelegramBotClient botClient;
        private readonly RandomService randomService;
        private readonly ILogger<TelegramMessageService> logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TelegramMessageService(ITelegramBotClient botClient, RandomService randomService, ILogger<TelegramMessageService> logger)
        {
            this.botClient = botClient;
            this.randomService = randomService;
            this.logger = logger;
        }

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
            var reply = randomService.PickRandom(successReplies);
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
        /// <param name="mention">Indicates if user should be mentioned in the text.</param>
        /// <returns>String to use for tagging a user.</returns>
        public string GetPersonNameHtml(User user, bool mention)
        {
            if (!string.IsNullOrEmpty(user.Username))
            {
                if (mention)
                {
                    return "@" + user.Username;
                }
                return user.Username;
            }

            var userFullName = GetUserFullName(user);
            if (mention)
            {
                return $"<a href=\"tg://user?id={user.Id}\">{HttpUtility.HtmlEncode(userFullName)}</a>";
            }
            return userFullName;
        }

        /// <summary>
        /// Send a generic message to a chat.
        /// </summary>
        /// <param name="message">Message data.</param>
        /// <param name="chatId">Id of the chat.</param>
        /// <param name="replyMessageId">Id of the message to reply to.</param>
        public async Task SendMessageAsync(GenericMessage message, ChatId chatId, int? replyMessageId = null)
        {
            Telegram.Bot.Types.InputFiles.InputOnlineFile file = null;
            if (message.FileId != null)
            {
                file = new Telegram.Bot.Types.InputFiles.InputOnlineFile(message.FileId);
            }
            switch (message.DataType)
            {
                case Domain.Chats.Entities.DataType.Animation:
                    await botClient.SendAnimationAsync(chatId, file, caption: message.Text, replyToMessageId: replyMessageId);
                    break;
                case Domain.Chats.Entities.DataType.Audio:
                    await botClient.SendAudioAsync(chatId, file, caption: message.Text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: replyMessageId);
                    break;
                case Domain.Chats.Entities.DataType.Document:
                    await botClient.SendDocumentAsync(chatId, file, caption: message.Text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: replyMessageId);
                    break;
                case Domain.Chats.Entities.DataType.Photo:
                    await botClient.SendPhotoAsync(chatId, file, caption: message.Text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: replyMessageId);
                    break;
                case Domain.Chats.Entities.DataType.Sticker:
                    await botClient.SendStickerAsync(chatId, file, replyToMessageId: replyMessageId);
                    break;
                case Domain.Chats.Entities.DataType.Text:
                    await botClient.SendTextMessageAsync(chatId, message.Text, Telegram.Bot.Types.Enums.ParseMode.Html, disableNotification: true, replyToMessageId: replyMessageId);
                    break;
                case Domain.Chats.Entities.DataType.Video:
                    await botClient.SendVideoAsync(chatId, file, caption: message.Text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: replyMessageId);
                    break;
                case Domain.Chats.Entities.DataType.VideoNote:
                    await botClient.SendVideoNoteAsync(chatId, file, replyToMessageId: replyMessageId);
                    break;
                case Domain.Chats.Entities.DataType.Voice:
                    await botClient.SendVoiceAsync(chatId, file, caption: message.Text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: replyMessageId);
                    break;
                default:
                    await botClient.SendTextMessageAsync(chatId, "Неожиданный формат файла " + message.DataType);
                    break;
            }
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
                return ApplyTextModifiers(message.Caption, message.CaptionEntities);
            }

            return ApplyTextModifiers(message.Text, message.Entities);
        }

        private string ApplyTextModifiers(string text, IEnumerable<MessageEntity> entities)
        {
            if (entities == null)
            {
                return text;
            }

            var previousOffset = int.MaxValue;
            void AddTextMarkup(string format, int startPosition, int length)
            {
                var textPart = text.Substring(startPosition, length);
                logger.LogDebug("Applying markup {0} to text {1}", format, textPart);
                var newText = string.Format(format, HttpUtility.HtmlEncode(textPart));

                text = text.Substring(0, startPosition)
                    + newText
                    + text.Substring(startPosition + length);
            }
            // Start from last entities to first to simplify the string replacement
            foreach (var formatEntity in entities.Reverse())
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
                    var link = "{0}";
                    if (formatEntity.Url != null)
                    {
                        // Escape curly braces to avoid string.Format exception
                        link = formatEntity.Url.Replace("{", "{{").Replace("}", "}}");
                    }
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

        /// <summary>
        /// Get a main file data from a message.
        /// </summary>
        /// <param name="message">Message data.</param>
        /// <param name="file">Found file.</param>
        /// <returns>Type of file data.</returns>
        public DataType? GetFile(Message message, out FileBase file)
        {
            if (message.Photo != null)
            {
                file = message.Photo.OrderByDescending(p => p.Height).First();
                return DataType.Photo;
            }

            if (message.Audio != null)
            {
                file = message.Audio;
                return DataType.Audio;
            }

            if (message.Animation != null)
            {
                file = message.Animation;
                return DataType.Animation;
            }

            if (message.Sticker != null)
            {
                file = message.Sticker;
                return DataType.Sticker;
            }

            if (message.Video != null)
            {
                file = message.Video;
                return DataType.Video;
            }

            if (message.VideoNote != null)
            {
                file = message.VideoNote;
                return DataType.VideoNote;
            }

            if (message.Voice != null)
            {
                file = message.Voice;
                return DataType.Voice;
            }

            if (message.Document != null)
            {
                file = message.Document;
                return DataType.Document;
            }

            file = null;
            return null;
        }
    }
}
