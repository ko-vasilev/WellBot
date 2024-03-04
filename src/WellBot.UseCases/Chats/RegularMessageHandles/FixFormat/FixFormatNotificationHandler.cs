using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.RegularMessageHandles.FixFormat;

/// <summary>
/// Attempts to automatically convert different media messages to a better format.
/// </summary>
internal class FixFormatNotificationHandler : INotificationHandler<MessageNotification>
{
    private readonly ITelegramBotClient botClient;
    private readonly IVideoConverter videoConverter;
    private readonly ILogger<FixFormatNotificationHandler> logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    public FixFormatNotificationHandler(ITelegramBotClient botClient, IVideoConverter videoConverter, ILogger<FixFormatNotificationHandler> logger)
    {
        this.botClient = botClient;
        this.videoConverter = videoConverter;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task Handle(MessageNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Message.Type != Telegram.Bot.Types.Enums.MessageType.Document
            || notification.Message.Document == null)
        {
            return;
        }

        var document = notification.Message.Document;

        try
        {
            switch (document.MimeType)
            {
                case "video/webm":
                    await ConvertFromWebmToMp4Async(document, notification.Message, cancellationToken);
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error converting video");
        }
    }

    private async Task ConvertFromWebmToMp4Async(Document document, Message originalMessage, CancellationToken cancellationToken)
    {
        var filePath = await botClient.GetFileAsync(document.FileId, cancellationToken);
        if (filePath?.FilePath == null)
        {
            throw new Exception("File not found");
        }
        await using var tempFileStream = new MemoryStream();
        await botClient.DownloadFileAsync(filePath.FilePath, tempFileStream, cancellationToken);

        using var mp4FileStream = await videoConverter.ConvertWebmToMp4Async(tempFileStream, cancellationToken);
        var newFileName = Path.ChangeExtension(document.FileName, "mp4");
        var doc = new InputOnlineFile(mp4FileStream, newFileName);
        await botClient.SendDocumentAsync(originalMessage.Chat.Id,
            doc,
            replyToMessageId: originalMessage.MessageId,
            caption: "Fixed",
            cancellationToken: cancellationToken);
    }
}
