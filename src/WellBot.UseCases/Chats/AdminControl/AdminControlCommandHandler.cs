using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.BotAPI.AvailableTypes;
using WellBot.Domain.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.UseCases.Chats.Dtos;
using WellBot.UseCases.Chats.RegularMessageHandles.Reply;

namespace WellBot.UseCases.Chats.AdminControl;

/// <summary>
/// Handler for <see cref="AdminControlCommand"/>.
/// </summary>
internal class AdminControlCommandHandler : AsyncRequestHandler<AdminControlCommand>
{
    private readonly IAppDbContext appDbContext;
    private readonly TelegramMessageService telegramMessageService;
    private readonly ILogger<AdminControlCommandHandler> logger;
    private readonly MemeChannelService memeChannelService;
    private readonly Lazy<PassiveTopicService> passiveTopicService;
    private readonly IEnumerable<long> superadminUserIds;

    /// <summary>
    /// Constructor.
    /// </summary>
    public AdminControlCommandHandler(
        IAppDbContext appDbContext,
        TelegramMessageService telegramMessageService,
        ILogger<AdminControlCommandHandler> logger,
        MemeChannelService memeChannelService,
        Lazy<PassiveTopicService> passiveTopicService,
        IOptions<ChatSettings> chatSettings)
    {
        this.appDbContext = appDbContext;
        this.telegramMessageService = telegramMessageService;
        this.logger = logger;
        this.memeChannelService = memeChannelService;
        this.passiveTopicService = passiveTopicService;
        superadminUserIds = chatSettings.Value.SuperadminIds;
    }

    /// <inheritdoc/>
    protected override async Task Handle(AdminControlCommand request, CancellationToken cancellationToken)
    {
        // Only superadmin user can trigger admin commands.
        if (request.Message.From == null || !superadminUserIds.Contains(request.Message.From.Id))
        {
            await telegramMessageService.SendMessageAsync(new GenericMessage()
                {
                    DataType = DataType.Reaction,
                    Text = "🤷"
                },
                request.Message.Chat.Id,
                request.Message.MessageId);
            return;
        }

        try
        {
            if (request.Arguments == "add slap")
            {
                if (await AddSlapOptionAsync(request.Message))
                {
                    await telegramMessageService.SendSuccessAsync(request.Message.Chat.Id, request.Message.MessageId);
                }
                return;
            }

            const string PassiveAdd = "passive add";
            if (request.Arguments.StartsWith(PassiveAdd))
            {
                var arguments = request.Arguments.Substring(PassiveAdd.Length).Trim();
                if (await AddPassiveReplyOptionAsync(request.Message, arguments))
                {
                    await telegramMessageService.SendSuccessAsync(request.Message.Chat.Id, request.Message.MessageId);
                }
                return;
            }

            const string PassiveProbability = "passive probability";
            if (request.Arguments.StartsWith(PassiveProbability))
            {
                var arguments = request.Arguments.Substring(PassiveProbability.Length).Trim();
                if (await SetTopicProbabilityAsync(arguments))
                {
                    await telegramMessageService.SendSuccessAsync(request.Message.Chat.Id, request.Message.MessageId);
                }
                return;
            }

            if (request.Arguments == "meme")
            {
                await SetMemeChannelAsync(request.Message);
                await telegramMessageService.SendSuccessAsync(request.Message.Chat.Id, request.Message.MessageId);
                return;
            }

            if (request.Arguments == "broadcast")
            {
                await BroadcastMessageAsync(request.Message);
                return;
            }

            if (request.Arguments == "user")
            {
                await ShowUserInfoAsync(request.Message);
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling admin command {command}", request.Arguments);
        }

        return;
    }

    private async Task<bool> AddSlapOptionAsync(Message message)
    {
        var replyMessage = message.ReplyToMessage;
        if (replyMessage?.Animation == null)
        {
            await telegramMessageService.SendMessageAsync("Можно добавить только гифку", message.Chat.Id, message.MessageId);
            return false;
        }

        var slapOption = new SlapOption
        {
            FileId = replyMessage.Animation.FileId
        };
        appDbContext.SlapOptions.Add(slapOption);
        await appDbContext.SaveChangesAsync();
        return true;
    }

    private async Task<bool> AddPassiveReplyOptionAsync(Message message, string arguments)
    {
        var replyMessage = message.ReplyToMessage;
        if (replyMessage == null)
        {
            return false;
        }

        var options = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var topicIds = passiveTopicService.Value.SearchTopics(options).ToList();
        if (!topicIds.Any())
        {
            const string defaultTopic = "regular";
            topicIds = passiveTopicService.Value.SearchTopics(new[] { defaultTopic }).ToList();
        }
        var topics = await appDbContext.PassiveTopics.Where(t => topicIds.Contains(t.Id))
            .ToListAsync();
        bool isBatchMode = options.Contains("batch");

        var text = telegramMessageService.GetMessageTextHtml(replyMessage);
        if (isBatchMode)
        {
            foreach (var line in text.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                var lineOption = new PassiveReplyOption
                {
                    Text = line,
                    DataType = DataType.Text,
                    PassiveTopics = topics
                };
                appDbContext.PassiveReplyOptions.Add(lineOption);
            }
            await appDbContext.SaveChangesAsync();
            return true;
        }

        var isReaction = options.Contains("react");

        var replyOption = new PassiveReplyOption
        {
            Text = text,
            DataType = DataType.Text,
            PassiveTopics = topics
        };
        if (isReaction)
        {
            replyOption.DataType = DataType.Reaction;
            // Try to use this reaction first.
            try
            {
                await telegramMessageService.SendMessageAsync(new GenericMessage
                {
                    Text = replyOption.Text,
                    FileId = replyOption.FileId,
                    DataType = replyOption.DataType
                },
                    message.Chat.Id,
                    message.MessageId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while trying to add a reaction {reaction}", text);
                await telegramMessageService.SendMessageAsync($"Не удалось добавить реакцию '{text}'", message.Chat.Id, message.MessageId);
                return false;
            }
        }
        else
        {
            var dataType = telegramMessageService.GetFileId(replyMessage, out var attachedFileId);
            if (dataType != null && attachedFileId != null)
            {
                replyOption.DataType = dataType.Value;
                replyOption.FileId = attachedFileId;
            }
        }

        appDbContext.PassiveReplyOptions.Add(replyOption);
        await appDbContext.SaveChangesAsync();
        return true;
    }

    private async Task SetMemeChannelAsync(Message message)
    {
        var replyMessage = message.ReplyToMessage;
        if (replyMessage == null)
        {
            return;
        }

        var forwardChannel = replyMessage.ForwardOrigin as MessageOriginChannel;
        if (forwardChannel == null)
        {
            return;
        }

        var currentChannelInfo = await appDbContext.MemeChannels.FirstOrDefaultAsync();
        if (currentChannelInfo == null)
        {
            currentChannelInfo = new MemeChannelInfo();
            appDbContext.MemeChannels.Add(currentChannelInfo);
        }

        currentChannelInfo.ChannelId = forwardChannel.Chat.Id;
        currentChannelInfo.LatestMessageId = forwardChannel.MessageId;
        await appDbContext.SaveChangesAsync();
        memeChannelService.CurrentMemeChatId = currentChannelInfo.ChannelId;
    }

    private async Task BroadcastMessageAsync(Message originalMessage)
    {
        var messageToSend = originalMessage.ReplyToMessage;
        if (messageToSend == null)
        {
            return;
        }

        var chats = await appDbContext.Chats
            .Select(c => c.TelegramId)
            .ToListAsync();
        var text = telegramMessageService.GetMessageTextHtml(messageToSend);
        var messageData = new GenericMessage
        {
            Text = text,
            DataType = DataType.Text
        };

        var dataType = telegramMessageService.GetFileId(messageToSend, out var attachedFileId);
        if (dataType != null && attachedFileId != null)
        {
            messageData.DataType = dataType.Value;
            messageData.FileId = attachedFileId;
        }

        foreach (var chatId in chats)
        {
            try
            {
                await telegramMessageService.SendMessageAsync(messageData, chatId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error broadcasting to {chatId} chat", chatId);
            }
        }
    }

    private async Task<bool> SetTopicProbabilityAsync(string arguments)
    {
        var parsedArguments = arguments.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        if (parsedArguments.Length != 2)
        {
            return false;
        }

        if (!int.TryParse(parsedArguments[1], out var probability))
        {
            return false;
        }

        var topicName = parsedArguments[0];
        var topic = await appDbContext.PassiveTopics.FirstOrDefaultAsync(t => t.Name == topicName);
        if (topic == null)
        {
            return false;
        }

        topic.Probability = probability;
        await appDbContext.SaveChangesAsync();

        // Reload topics cache.
        var existingTopics = await appDbContext.PassiveTopics
            .AsNoTracking()
            .ToListAsync();
        passiveTopicService.Value.Update(existingTopics);

        return true;
    }

    private async Task ShowUserInfoAsync(Message originalMessage)
    {
        if (originalMessage.ReplyToMessage == null)
        {
            await telegramMessageService.SendMessageAsync(
                "Reply to a user message to get information about his author.",
                originalMessage.Chat.Id,
                originalMessage.MessageId);
            return;
        }

        User? user = null;
        if (originalMessage.ReplyToMessage?.ForwardOrigin is MessageOriginUser originUser)
        {
            user = originUser.SenderUser;
        }
        if (originalMessage.ReplyToMessage?.ForwardOrigin is MessageOriginHiddenUser hiddenUser)
        {
            await telegramMessageService.SendMessageAsync(
                $"User {hiddenUser.SenderUserName} account is hidden.",
                originalMessage.Chat.Id,
                originalMessage.MessageId);
            return;
        }

        if (user == null)
        {
            user = originalMessage.ReplyToMessage?.From;
        }

        if (user == null)
        {
            await telegramMessageService.SendMessageAsync(
                "User information is unavailable",
                originalMessage.Chat.Id,
                originalMessage.MessageId);
            return;
        }

        var information = $"Name: {user.FirstName} {user.LastName}\n" +
            $"username: {user.Username}\n" +
            $"user id: {user.Id}";
        await telegramMessageService.SendMessageAsync(information, originalMessage.Chat.Id, originalMessage.MessageId);
    }
}
