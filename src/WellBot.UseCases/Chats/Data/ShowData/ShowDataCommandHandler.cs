using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using WellBot.Domain.Chats;
using WellBot.DomainServices.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Data.ShowData;

/// <summary>
/// Handler for <see cref="ShowDataCommand"/>.
/// </summary>
internal class ShowDataCommandHandler : AsyncRequestHandler<ShowDataCommand>
{
    private readonly IAppDbContext dbContext;
    private readonly ITelegramBotClient botClient;
    private readonly CurrentChatService currentChatService;
    private readonly MessageRateLimitingService messageRateLimitingService;
    private readonly TelegramMessageService telegramMessageService;

    /// <summary>
    /// Constructor.
    /// </summary>
    public ShowDataCommandHandler(IAppDbContext dbContext, ITelegramBotClient botClient, CurrentChatService currentChatService, MessageRateLimitingService messageRateLimitingService, TelegramMessageService telegramMessageService)
    {
        this.dbContext = dbContext;
        this.botClient = botClient;
        this.currentChatService = currentChatService;
        this.messageRateLimitingService = messageRateLimitingService;
        this.telegramMessageService = telegramMessageService;
    }

    /// <inheritdoc/>
    protected override async Task Handle(ShowDataCommand request, CancellationToken cancellationToken)
    {
        var key = (request.Key ?? string.Empty).ToLowerInvariant();
        var data = await dbContext.ChatDatas.FirstOrDefaultAsync(d => d.ChatId == currentChatService.ChatId && d.Key == key, cancellationToken);
        var replyParameters = new ReplyParameters()
        {
            MessageId = request.MessageId
        };
        if (data == null)
        {
            await botClient.SendMessageAsync(request.ChatId, "Не могу найти данных по этому ключу", replyParameters: replyParameters);
            return;
        }

        var rateLimit = ShouldRateLimitItem(data);
        if (rateLimit)
        {
            // Do not allow outputting the item too frequently
            if (messageRateLimitingService.IsRateLimited(request.ChatId, request.SenderUserId, data.Key, out var allowedIn))
            {
                string duration = $"{allowedIn.Seconds} секунд";
                if (allowedIn.Minutes > 0)
                {
                    duration = $"{allowedIn.Minutes} минут";
                }
                await botClient.SendMessageAsync(request.ChatId, $"Подождите " + duration, replyParameters: replyParameters);
                return;
            }
        }

        await telegramMessageService.SendMessageAsync(new Dtos.GenericMessage
        {
            DataType = data.DataType,
            FileId = data.FileId,
            Text = data.Text
        },
        request.ChatId,
        request.ReplyMessageId);

        if (rateLimit)
        {
            messageRateLimitingService.LimitRate(request.ChatId, request.SenderUserId, data.Key);
        }
    }

    /// <summary>
    /// Check if there should be a rate limit for accessing the specified chat data.
    /// </summary>
    /// <param name="chatData">Chat data.</param>
    /// <returns>True if request rate limit should be applied when displaying the data.</returns>
    private bool ShouldRateLimitItem(ChatData chatData)
    {
        return chatData.HasUserMention;
    }
}
