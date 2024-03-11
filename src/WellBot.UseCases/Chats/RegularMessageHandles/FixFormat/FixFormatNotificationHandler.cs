using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
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
    private readonly IHttpClientFactory httpClientFactory;

    /// <summary>
    /// Constructor.
    /// </summary>
    public FixFormatNotificationHandler(ITelegramBotClient botClient,
        IVideoConverter videoConverter,
        ILogger<FixFormatNotificationHandler> logger,
        IHttpClientFactory httpClientFactory)
    {
        this.botClient = botClient;
        this.videoConverter = videoConverter;
        this.logger = logger;
        this.httpClientFactory = httpClientFactory;
    }

    /// <inheritdoc/>
    public async Task Handle(MessageNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Message.Document == null)
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

        using (var httpClient = httpClientFactory.CreateClient())
        {
            using var fileStream = await httpClient.GetStreamAsync(filePath.FilePath, cancellationToken);
            if (fileStream == null)
            {
                throw new Exception("Error downloading file");
            }

            using var mp4FileStream = await videoConverter.ConvertWebmToMp4Async(fileStream, cancellationToken);
            var newFileName = Path.ChangeExtension(document.FileName, "mp4");
            var doc = new InputFile(mp4FileStream, newFileName ?? string.Empty);
            await botClient.SendDocumentAsync(originalMessage.Chat.Id,
                doc,
                replyParameters: new ReplyParameters()
                {
                    MessageId = originalMessage.MessageId
                },
                caption: "Fixed",
                cancellationToken: cancellationToken);
        }
    }
}
