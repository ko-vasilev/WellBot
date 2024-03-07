using System.Web;
using Microsoft.Extensions.Logging;
using Saritasa.Tools.Domain.Exceptions;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Stickers;
using WellBot.Domain.Chats;
using WellBot.UseCases.Chats.Dtos;

namespace WellBot.UseCases.Chats;

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
    public async Task SendSuccessAsync(long chatId)
    {
        var reply = randomService.PickRandom(successReplies);
        await botClient.SendMessageAsync(chatId, reply);
    }

    /// <summary>
    /// Get full name of the user.
    /// </summary>
    /// <param name="user">User info.</param>
    /// <returns>User full name.</returns>
    public string GetUserFullName(User? user)
    {
        if (user == null)
        {
            return string.Empty;
        }

        var name = user.FirstName;
        if (string.IsNullOrEmpty(name))
        {
            return user.LastName ?? string.Empty;
        }

        if (!string.IsNullOrEmpty(user.LastName))
        {
            name += " " + user.LastName;
        }
        return name;
    }

    /// <summary>
    /// Get a string that allows to mention a user.
    /// The resulting string can be used with <see cref="Telegram.BotAPI.FormatStyles.HTML"/> format.
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
    public async Task SendMessageAsync(GenericMessage message, long chatId, int? replyMessageId = null)
    {
        string GetFileId()
        {
            if (message.FileId == null)
            {
                throw new DomainException("Missing file.");
            }
            return message.FileId;
        }

        ReplyParameters? reply = null;
        if (replyMessageId != null)
        {
            reply = new ReplyParameters()
            {
                MessageId = replyMessageId.Value
            };
        }

        switch (message.DataType)
        {
            case DataType.Animation:
                await botClient.SendAnimationAsync(chatId, GetFileId(), caption: message.Text, replyParameters: reply);
                break;
            case DataType.Audio:
                await botClient.SendAudioAsync(chatId, GetFileId(), caption: message.Text, parseMode: FormatStyles.HTML, replyParameters: reply);
                break;
            case DataType.Document:
                await botClient.SendDocumentAsync(chatId, GetFileId(), caption: message.Text, parseMode: FormatStyles.HTML, replyParameters: reply);
                break;
            case DataType.Photo:
                await botClient.SendPhotoAsync(chatId, GetFileId(), caption: message.Text, parseMode: FormatStyles.HTML, replyParameters: reply);
                break;
            case DataType.Sticker:
                await botClient.SendStickerAsync(chatId, GetFileId(), replyParameters: reply);
                break;
            case DataType.Text:
                await botClient.SendMessageAsync(chatId, message.Text ?? string.Empty, parseMode: FormatStyles.HTML, disableNotification: true, replyParameters: reply);
                break;
            case DataType.Video:
                await botClient.SendVideoAsync(chatId, GetFileId(), caption: message.Text, parseMode: FormatStyles.HTML, replyParameters: reply);
                break;
            case DataType.VideoNote:
                await botClient.SendVideoNoteAsync(chatId, GetFileId(), replyParameters: reply);
                break;
            case DataType.Voice:
                await botClient.SendVoiceAsync(chatId, GetFileId(), caption: message.Text, parseMode: FormatStyles.HTML, replyParameters: reply);
                break;
            default:
                await botClient.SendMessageAsync(chatId, "Неожиданный формат файла " + message.DataType);
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
            return ApplyTextModifiers(message.Caption ?? string.Empty,
                message.CaptionEntities ?? Enumerable.Empty<MessageEntity>());
        }

        return ApplyTextModifiers(message.Text, message.Entities ?? Enumerable.Empty<MessageEntity>());
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
            logger.LogDebug("Applying markup {format} to text {textPart}", format, textPart);
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

            if (formatEntity.Type == MessageEntityTypes.Bold)
            {
                AddTextMarkup("<b>{0}</b>", formatEntity.Offset, formatEntity.Length);
                continue;
            }

            if (formatEntity.Type == MessageEntityTypes.Italic)
            {
                AddTextMarkup("<i>{0}</i>", formatEntity.Offset, formatEntity.Length);
                continue;
            }

            if (formatEntity.Type == MessageEntityTypes.Code)
            {
                AddTextMarkup("<code>{0}</code>", formatEntity.Offset, formatEntity.Length);
                continue;
            }

            if (formatEntity.Type == MessageEntityTypes.Pre)
            {
                AddTextMarkup("<pre>{0}</pre>", formatEntity.Offset, formatEntity.Length);
                continue;
            }

            if (formatEntity.Type == MessageEntityTypes.Strikethrough)
            {
                AddTextMarkup("<s>{0}</s>", formatEntity.Offset, formatEntity.Length);
                continue;
            }

            if (formatEntity.Type == MessageEntityTypes.TextLink)
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

            if (formatEntity.Type == MessageEntityTypes.TextMention && formatEntity.User != null)
            {
                AddTextMarkup($"<a href=\"tg://user?id={formatEntity.User.Id}\">{{0}}</a>", formatEntity.Offset, formatEntity.Length);
                continue;
            }

            if (formatEntity.Type == MessageEntityTypes.Underline)
            {
                AddTextMarkup("<u>{0}</u>", formatEntity.Offset, formatEntity.Length);
                continue;
            }

            if (formatEntity.Type == MessageEntityTypes.Url)
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
    /// <param name="fileId">Id of the found file.</param>
    /// <returns>Type of file data.</returns>
    public DataType? GetFileId(Message message, out string? fileId)
    {
        fileId = null;
        if (message.Photo != null)
        {
            fileId = message.Photo.OrderByDescending(p => p.Height).FirstOrDefault()?.FileId;
            return DataType.Photo;
        }

        if (message.Audio != null)
        {
            fileId = message.Audio.FileId;
            return DataType.Audio;
        }

        if (message.Animation != null)
        {
            fileId = message.Animation.FileId;
            return DataType.Animation;
        }

        if (message.Sticker != null)
        {
            fileId = message.Sticker.FileId;
            return DataType.Sticker;
        }

        if (message.Video != null)
        {
            fileId = message.Video.FileId;
            return DataType.Video;
        }

        if (message.VideoNote != null)
        {
            fileId = message.VideoNote.FileId;
            return DataType.VideoNote;
        }

        if (message.Voice != null)
        {
            fileId = message.Voice.FileId;
            return DataType.Voice;
        }

        if (message.Document != null)
        {
            fileId = message.Document.FileId;
            return DataType.Document;
        }

        return null;
    }
}
