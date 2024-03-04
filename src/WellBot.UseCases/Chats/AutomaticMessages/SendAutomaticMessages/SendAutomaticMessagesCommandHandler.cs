using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using WellBot.Domain.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.AutomaticMessages.SendAutomaticMessages;

/// <summary>
/// Handler for <see cref="SendAutomaticMessagesCommand"/>.
/// </summary>
internal class SendAutomaticMessagesCommandHandler : AsyncRequestHandler<SendAutomaticMessagesCommand>
{
    private readonly IAppDbContext dbContext;
    private readonly ITelegramBotClient botClient;
    private readonly IImageSearcher imageSearcher;
    private readonly RandomService randomService;
    private readonly ILogger<SendAutomaticMessagesCommandHandler> logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    public SendAutomaticMessagesCommandHandler(IAppDbContext dbContext, ITelegramBotClient telegramBotClient, IImageSearcher imageSearcher, RandomService randomService, ILogger<SendAutomaticMessagesCommandHandler> logger)
    {
        this.dbContext = dbContext;
        this.botClient = telegramBotClient;
        this.imageSearcher = imageSearcher;
        this.randomService = randomService;
        this.logger = logger;
    }

    /// <inheritdoc/>
    protected override async Task Handle(SendAutomaticMessagesCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var currentDay = now.Date;
        var messagesToHandle = await dbContext.AutomaticMessageTemplates
            .Include(m => m.Chat)
            .Where(m => m.LastTriggeredDate.Date < currentDay)
            .ToListAsync(cancellationToken);

        logger.LogDebug("{count} potential messages to be processed", messagesToHandle.Count);
        foreach (var messageTemplate in messagesToHandle)
        {
            var interval = Cronos.CronExpression.Parse(messageTemplate.CronInterval);
            // Check if the job should have run between the last time it ran and current day.
            var lastTriggeredDateUtc = new DateTime(messageTemplate.LastTriggeredDate.Ticks, DateTimeKind.Utc);
            var nextOccurrence = interval.GetNextOccurrence(lastTriggeredDateUtc);
            if (nextOccurrence == null || nextOccurrence > now)
            {
                logger.LogDebug("Next occurrence should be on {nextDate}, skipping", nextOccurrence);
                continue;
            }

            logger.LogInformation("Starting sending messages for template {templateId}", messageTemplate.Id);
            await SendMessageAsync(messageTemplate, cancellationToken);

            // Save the fact that we have sent the message right away
            // In case if there are errors in sending next messages
            messageTemplate.LastTriggeredDate = now;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task SendMessageAsync(AutomaticMessageTemplate messageTemplate, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(messageTemplate.ImageSearchQuery))
        {
            await SendTextMessageAsync(messageTemplate, cancellationToken);
            return;
        }

        var attempt = 0;
        var availableImages = (await SearchImagesAsync(messageTemplate.ImageSearchQuery, cancellationToken))
            .ToList();
        while (attempt < 3)
        {
            if (!availableImages.Any())
            {
                logger.LogWarning("No images to send using query {query}", messageTemplate.ImageSearchQuery);
                await SendTextMessageAsync(messageTemplate, cancellationToken);
                return;
            }

            var image = randomService.PickRandom(availableImages);
            availableImages.Remove(image);
            try
            {
                await botClient.SendPhotoAsync(messageTemplate.Chat.TelegramId,
                    new Telegram.Bot.Types.InputFiles.InputOnlineFile(image),
                    messageTemplate.Message,
                    Telegram.Bot.Types.Enums.ParseMode.Html,
                    cancellationToken: cancellationToken);

                return;
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                ++attempt;
                logger.LogWarning(ex, "Could not send an image {url}", image);
            }
        }
    }

    private async Task SendTextMessageAsync(AutomaticMessageTemplate messageTemplate, CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(messageTemplate.Chat.TelegramId,
            messageTemplate.Message,
            Telegram.Bot.Types.Enums.ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    private async Task<IEnumerable<string>> SearchImagesAsync(string query, CancellationToken cancellationToken)
    {
        const int maxImagesResult = 25;

        var imageResult = await imageSearcher.SearchAsync(query, cancellationToken);
        return imageResult
            .Images
            .Select(image => image.Url)
            .Take(maxImagesResult);
    }
}
