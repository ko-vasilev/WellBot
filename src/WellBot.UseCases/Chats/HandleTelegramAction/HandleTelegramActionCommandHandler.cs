﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.UseCases.Chats.Data.SetChatData;
using WellBot.UseCases.Chats.Data.ShowData;
using WellBot.UseCases.Chats.Pidor.PidorGameRegister;
using WellBot.UseCases.Chats.Pidor.PidorGameRun;
using WellBot.UseCases.Chats.Pidor.PidorList;
using WellBot.UseCases.Chats.Pidor.PidorRules;
using WellBot.UseCases.Chats.Pidor.PidorStats;

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

            var messageText = telegramMessageService.GetMessageTextHtml(request.Action.Message);
            ChatId chatId = null;
            try
            {
                if (!string.IsNullOrEmpty(messageText))
                {
                    chatId = request.Action.Message.Chat.Id;
                    var command = ParseCommand(messageText, out string arguments, ref isDirectMessage);
                    if (string.IsNullOrEmpty(command))
                    {
                        return;
                    }

                    await botClient.SendChatActionAsync(request.Action.Message.Chat.Id, Telegram.Bot.Types.Enums.ChatAction.Typing);
                    await HandleCommandAsync(command, arguments, isDirectMessage, chatId, request.Action.Message.From, request.Action.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling command {text}", messageText);
                if (chatId != null)
                {
                    await botClient.SendTextMessageAsync(chatId, "Что-то пошло не так.");
                }
            }
        }

        private async Task HandleCommandAsync(string command, string arguments, bool isDirectMessage, ChatId chatId, User sender, Message message)
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
                    Arguments = arguments,
                    Message = message
                }),
                "get" => mediator.Send(new ShowDataCommand
                {
                    ChatId = chatId,
                    Arguments = arguments,
                    MessageId = message.MessageId
                }),
                _ => isDirectMessage
                    ? botClient.SendTextMessageAsync(chatId, "Неизвестная команда")
                    : Task.CompletedTask
            };

            await action;
        }

        private string ParseCommand(string text, out string arguments, ref bool isDirectMessage)
        {
            if (!text.StartsWith('/'))
            {
                arguments = text;
                return string.Empty;
            }

            var command = SplitCommandText(text, out arguments);
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

        private string SplitCommandText(string text, out string commandArguments)
        {
            var command = text;
            commandArguments = string.Empty;
            var commandNamePart = text.IndexOf(' ');
            if (commandNamePart >= 0)
            {
                command = text.Substring(0, commandNamePart);
                // Parse arguments and ignore the space delimiter
                commandArguments = text.Substring(commandNamePart + 1);
            }

            return command;
        }
    }
}
