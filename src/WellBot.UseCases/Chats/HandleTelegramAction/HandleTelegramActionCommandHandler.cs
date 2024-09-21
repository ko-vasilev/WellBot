using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Saritasa.Tools.Domain.Exceptions;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.InlineMode;
using WellBot.Domain.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.UseCases.Chats.AdminControl;
using WellBot.UseCases.Chats.Data.DeleteChatData;
using WellBot.UseCases.Chats.Data.SearchData;
using WellBot.UseCases.Chats.Data.SetChatData;
using WellBot.UseCases.Chats.Data.ShowData;
using WellBot.UseCases.Chats.Data.ShowKeys;
using WellBot.UseCases.Chats.Ememe;
using WellBot.UseCases.Chats.Pidor.PidorGameRegister;
using WellBot.UseCases.Chats.Pidor.PidorGameRun;
using WellBot.UseCases.Chats.Pidor.PidorList;
using WellBot.UseCases.Chats.Pidor.PidorRules;
using WellBot.UseCases.Chats.Pidor.PidorStats;
using WellBot.UseCases.Chats.Prikol;
using WellBot.UseCases.Chats.RegularMessageHandles;
using WellBot.UseCases.Chats.Slap;
using WellBot.UseCases.Chats.Summarization.Recap;

namespace WellBot.UseCases.Chats.HandleTelegramAction;

/// <summary>
/// Handler for <see cref="HandleTelegramActionCommand"/>.
/// </summary>
internal class HandleTelegramActionCommandHandler : AsyncRequestHandler<HandleTelegramActionCommand>
{
    private readonly ITelegramBotClient botClient;
    private readonly ITelegramBotSettings telegramBotSettings;
    private readonly IMediator mediator;
    private readonly ILogger<HandleTelegramActionCommandHandler> logger;
    private readonly TelegramMessageService telegramMessageService;
    private readonly MemeChannelService memeChannelService;
    private readonly Lazy<IAppDbContext> dbContext;
    private readonly IServiceScopeFactory serviceScopeFactory;

    /// <summary>
    /// Constructor.
    /// </summary>
    public HandleTelegramActionCommandHandler(
        ITelegramBotClient botClient,
        ITelegramBotSettings telegramBotSettings,
        IMediator mediator,
        ILogger<HandleTelegramActionCommandHandler> logger,
        TelegramMessageService telegramMessageService,
        MemeChannelService memeChannelService,
        Lazy<IAppDbContext> dbContext,
        IServiceScopeFactory serviceScopeFactory)
    {
        this.botClient = botClient;
        this.telegramBotSettings = telegramBotSettings;
        this.mediator = mediator;
        this.logger = logger;
        this.telegramMessageService = telegramMessageService;
        this.memeChannelService = memeChannelService;
        this.dbContext = dbContext;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    /// <inheritdoc/>
    protected override async Task Handle(HandleTelegramActionCommand request, CancellationToken cancellationToken)
    {
        if (await HandleInlineQueryAsync(request?.Action?.InlineQuery, cancellationToken))
        {
            return;
        }

        // Check if this is a message.
        if (request?.Action?.Message == null)
        {
            return;
        }
        var isDirectMessage = request.Action.Message.Chat.Type == ChatTypes.Private;
        if (isDirectMessage)
        {
            // Ignore direct messages.
            return;
        }

        if (await HandleMemeChannelMessageAsync(request.Action.Message, cancellationToken))
        {
            return;
        }

        var plainMessageText = request.Action.Message.Text ?? request.Action.Message.Caption;
        var textFormatted = telegramMessageService.GetMessageTextHtml(request.Action.Message);
        long? chatId = null;
        try
        {
            if (!string.IsNullOrEmpty(plainMessageText))
            {
                chatId = request.Action.Message.Chat.Id;
                var command = ParseCommand(plainMessageText, textFormatted, out string arguments, out string argumentsHtml, ref isDirectMessage);
                if (!string.IsNullOrEmpty(command))
                {
                    await HandleCommandAsync(command, arguments, argumentsHtml, isDirectMessage, chatId.Value, request.Action.Message.From, request.Action.Message);
                    return;
                }
            }

            HandleMessageNotification(request.Action.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling command {text}", textFormatted);
            if (chatId.HasValue)
            {
                await botClient.SendMessageAsync(chatId.Value, "Что-то пошло не так.");
            }
        }
    }

    private async Task HandleCommandAsync(string command, string arguments, string argumentsHtml, bool isDirectMessage, long chatId, User? sender, Message message)
    {
        long GetSenderId()
        {
            if (sender == null)
            {
                throw new DomainException("Sender is not specified");
            }
            return sender.Id;
        }
        Task action = command switch
        {
            "pidoreg" => mediator.Send(new PidorGameRegisterCommand()
            {
                ChatId = chatId,
                TelegramUserId = GetSenderId(),
                TelegramUserName = telegramMessageService.GetUserFullName(sender),
                MessageId = message.MessageId
            }),
            "pidorlist" => mediator.Send(new PidorListCommand
            {
                ChatId = chatId,
                TelegramUserId = GetSenderId(),
                Arguments = arguments,
                MessageId = message.MessageId
            }),
            "pidorules" => mediator.Send(new PidorRulesCommand
            {
                ChatId = chatId,
            }),
            "pidor" => mediator.Send(new PidorGameRunCommand
            {
                ChatId = chatId,
            }),
            "pidorstats" => mediator.Send(new PidorStatsCommand
            {
                ChatId = chatId,
                Arguments = arguments
            }),
            "set" => mediator.Send(new SetChatDataCommand
            {
                ChatId = chatId,
                Arguments = argumentsHtml,
                Message = message
            }),
            "get" => mediator.Send(new ShowDataCommand
            {
                ChatId = chatId,
                Key = arguments,
                MessageId = message.MessageId,
                SenderUserId = GetSenderId(),
                ReplyMessageId = message.ReplyToMessage?.MessageId
            }),
            "getall" => mediator.Send(new ShowKeysCommand
            {
                ChatId = chatId,
            }),
            "del" => mediator.Send(new DeleteChatDataCommand
            {
                ChatId = chatId,
                Key = arguments,
                MessageId = message.MessageId
            }),
            "шлёп" or "шлеп" or "slap" => mediator.Send(new SlapCommand
            {
                ChatId = chatId,
                MessageId = message.MessageId
            }),
            "admin" => mediator.Send(new AdminControlCommand
            {
                Arguments = arguments,
                Message = message
            }),
            "ememe" => mediator.Send(new EmemeCommand
            {
                ChatId = chatId
            }),
            "prikol" => mediator.Send(new PrikolCommand
            {
                ChatId = chatId
            }),
            "recap" or "рекап" => mediator.Send(new RecapCommand
            {
                ChatId = chatId,
                Arguments = arguments,
                MessageId = message.MessageId
            }),
            _ => isDirectMessage
                ? botClient.SendMessageAsync(chatId, "Неизвестная команда")
                : Task.CompletedTask
        };

        await action;
    }

    private string ParseCommand(string text, string textFormatted, out string arguments, out string argumentsFormatted, ref bool isDirectMessage)
    {
        if (!text.StartsWith('/'))
        {
            arguments = text;
            argumentsFormatted = textFormatted;
            return string.Empty;
        }

        var command = SplitCommandText(text, out var argumentsStartIndex);
        arguments = text.Substring(argumentsStartIndex);
        argumentsFormatted = textFormatted.Substring(argumentsStartIndex);
        string botUsername = "@" + telegramBotSettings?.TelegramBotUsername;
        if (command.EndsWith(botUsername))
        {
            command = command.Substring(0, command.Length - botUsername.Length);
            isDirectMessage = true;
        }
        // Remove the leading slash
        command = command.Substring(1);

        return command;
    }

    private string SplitCommandText(string text, out int argumentsStartIndex)
    {
        var command = text;
        argumentsStartIndex = 0;
        var commandNamePart = text.IndexOf(' ');
        if (commandNamePart >= 0)
        {
            command = text.Substring(0, commandNamePart);
            // Ignore the space delimiter
            argumentsStartIndex = commandNamePart + 1;
        }

        return command;
    }

    private async Task<bool> HandleInlineQueryAsync(InlineQuery? inlineQuery, CancellationToken cancellationToken)
    {
        if (inlineQuery == null)
        {
            return false;
        }

        try
        {
            var dataItems = await mediator.Send(new SearchDataQuery
            {
                SearchText = inlineQuery.Query
            }, cancellationToken);

            var inlineResults = dataItems.Select(item => MapQueryResult(item))
                .Where(res => res != null)
                .Cast<InlineQueryResult>();
            await botClient.AnswerInlineQueryAsync(inlineQuery.Id, inlineResults, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling inline query {text}", inlineQuery.Query);
        }

        return true;
    }

    private async Task<bool> HandleMemeChannelMessageAsync(Message message, CancellationToken cancellationToken)
    {
        if (message == null)
        {
            return false;
        }

        if (message.Chat.Id != memeChannelService.CurrentMemeChatId)
        {
            return false;
        }

        var context = dbContext.Value;

        var memeChannel = await context.MemeChannels.FirstAsync(cancellationToken);
        memeChannel.LatestMessageId = message.MessageId;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private InlineQueryResult? MapQueryResult(DataItem data)
    {
        var uniqueId = data.Id.ToString();
        switch (data.DataType)
        {
            case DataType.Animation:
                return new InlineQueryResultCachedMpeg4Gif()
                {
                    Id = uniqueId,
                    Mpeg4FileId = data.FileId
                };
            case DataType.Audio:
                return new InlineQueryResultCachedAudio()
                {
                    Id = uniqueId,
                    AudioFileId = data.FileId
                };
            case DataType.Document:
                return new InlineQueryResultCachedDocument()
                {
                    Id = uniqueId,
                    DocumentFileId = data.FileId,
                    Title = data.Key
                };
            case DataType.Photo:
                return new InlineQueryResultCachedPhoto()
                {
                    Id = uniqueId,
                    PhotoFileId = data.FileId
                };
            case DataType.Sticker:
                return new InlineQueryResultCachedSticker()
                {
                    Id = uniqueId,
                    StickerFileId = data.FileId
                };
            case DataType.Text:
                {
                    var content = new InputTextMessageContent(data.Text)
                    {
                        ParseMode = FormatStyles.HTML
                    };
                    return new InlineQueryResultArticle()
                    {
                        Id = uniqueId,
                        Title = data.Key,
                        InputMessageContent = content
                    };
                }
            case DataType.Video:
                return new InlineQueryResultCachedVideo()
                {
                    Id = uniqueId,
                    VideoFileId = data.FileId,
                    Title = data.Key
                };
            case DataType.Voice:
                return new InlineQueryResultCachedVoice()
                {
                    Id = uniqueId,
                    VoiceFileId = data.FileId,
                    Title = data.Key
                };
            default:
                return null;
        }
    }

    private void HandleMessageNotification(Message message)
    {
        // TODO: ideally this logic should be encapsulated in a separate class.
        // Fire and forget for the notification.
        var serviceScope = serviceScopeFactory.CreateScope();
        Task.Run(async () =>
        {
            using (serviceScope)
            {
                var mediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<HandleTelegramActionCommandHandler>>();
                try
                {
                    await mediator.Publish(new MessageNotification
                    {
                        Message = message
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error sending message notification");
                }
            }
        });
    }
}
