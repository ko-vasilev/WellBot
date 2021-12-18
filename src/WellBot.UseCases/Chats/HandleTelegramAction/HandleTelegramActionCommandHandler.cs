using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.UseCases.Chats.Data.DeleteChatData;
using WellBot.UseCases.Chats.Data.SearchData;
using WellBot.UseCases.Chats.Data.SetChatData;
using WellBot.UseCases.Chats.Data.ShowData;
using WellBot.UseCases.Chats.Data.ShowKeys;
using WellBot.UseCases.Chats.Pidor.PidorGameRegister;
using WellBot.UseCases.Chats.Pidor.PidorGameRun;
using WellBot.UseCases.Chats.Pidor.PidorList;
using WellBot.UseCases.Chats.Pidor.PidorRules;
using WellBot.UseCases.Chats.Pidor.PidorStats;
using WellBot.UseCases.Chats.Slap;

namespace WellBot.UseCases.Chats.HandleTelegramAction
{
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

        /// <summary>
        /// Constructor.
        /// </summary>
        public HandleTelegramActionCommandHandler(ITelegramBotClient botClient, ITelegramBotSettings telegramBotSettings, IMediator mediator, ILogger<HandleTelegramActionCommandHandler> logger, TelegramMessageService telegramMessageService)
        {
            this.botClient = botClient;
            this.telegramBotSettings = telegramBotSettings;
            this.mediator = mediator;
            this.logger = logger;
            this.telegramMessageService = telegramMessageService;
        }

        /// <inheritdoc/>
        protected override async Task Handle(HandleTelegramActionCommand request, CancellationToken cancellationToken)
        {
            if (await HandleInlineQueryAsync(request.Action.InlineQuery, cancellationToken))
            {
                return;
            }

            var isMessage = request.Action.Message != null;
            if (!isMessage)
            {
                return;
            }
            var isDirectMessage = request.Action.Message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private;
            if (isDirectMessage)
            {
                // Ignore direct messages.
                return;
            }

            var plainMessageText = request.Action.Message.Text ?? request.Action.Message.Caption;
            var textFormatted = telegramMessageService.GetMessageTextHtml(request.Action.Message);
            ChatId chatId = null;
            try
            {
                if (!string.IsNullOrEmpty(plainMessageText))
                {
                    chatId = request.Action.Message.Chat.Id;
                    var command = ParseCommand(plainMessageText, textFormatted, out string arguments, out string argumentsHtml, ref isDirectMessage);
                    if (string.IsNullOrEmpty(command))
                    {
                        return;
                    }

                    await botClient.SendChatActionAsync(request.Action.Message.Chat.Id, Telegram.Bot.Types.Enums.ChatAction.Typing);
                    await HandleCommandAsync(command, arguments, argumentsHtml, isDirectMessage, chatId, request.Action.Message.From, request.Action.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling command {text}", textFormatted);
                if (chatId != null)
                {
                    await botClient.SendTextMessageAsync(chatId, "Что-то пошло не так.");
                }
            }
        }

        private async Task HandleCommandAsync(string command, string arguments, string argumentsHtml, bool isDirectMessage, ChatId chatId, User sender, Message message)
        {
            long senderId = sender.Id;
            Task action = command switch
            {
                "pidoreg" => mediator.Send(new PidorGameRegisterCommand()
                {
                    ChatId = chatId,
                    TelegramUserId = senderId,
                    TelegramUserName = telegramMessageService.GetUserFullName(sender)
                }),
                "pidorlist" => mediator.Send(new PidorListCommand
                {
                    ChatId = chatId,
                    TelegramUserId = senderId,
                    Arguments = arguments,
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
                    SenderUserId = senderId,
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
                "шлёп" => mediator.Send(new SlapCommand
                {
                    ChatId = chatId,
                    MessageId = message.MessageId
                }),
                _ => isDirectMessage
                    ? botClient.SendTextMessageAsync(chatId, "Неизвестная команда")
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
            string botUsername = telegramBotSettings?.TelegramBotUsername;
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

        private async Task<bool> HandleInlineQueryAsync(InlineQuery inlineQuery, CancellationToken cancellationToken)
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
                    .Where(res => res != null);
                await botClient.AnswerInlineQueryAsync(inlineQuery.Id, inlineResults, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling inline query {text}");
            }

            return true;
        }

        private InlineQueryResult MapQueryResult(DataItem data)
        {
            var uniqueId = data.Id.ToString();
            switch (data.DataType)
            {
                case Domain.Chats.Entities.DataType.Audio:
                    return new InlineQueryResultCachedAudio(uniqueId, data.FileId);
                case Domain.Chats.Entities.DataType.Document:
                    return new InlineQueryResultCachedDocument(uniqueId, data.FileId, data.Key);
                case Domain.Chats.Entities.DataType.Photo:
                    return new InlineQueryResultCachedPhoto(uniqueId, data.FileId);
                case Domain.Chats.Entities.DataType.Sticker:
                    return new InlineQueryResultCachedSticker(uniqueId, data.FileId);
                case Domain.Chats.Entities.DataType.Text:
                    {
                        var content = new InputTextMessageContent(data.Text)
                        {
                            ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html
                        };
                        return new InlineQueryResultArticle(uniqueId, data.Key, content);
                    }
                case Domain.Chats.Entities.DataType.Video:
                    return new InlineQueryResultCachedVideo(uniqueId, data.FileId, data.Key);
                case Domain.Chats.Entities.DataType.Voice:
                    return new InlineQueryResultCachedVoice(uniqueId, data.FileId, data.Key);
                default:
                    return null;
            }
        }
    }
}
