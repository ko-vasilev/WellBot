using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using WellBot.Infrastructure.Abstractions.Interfaces;
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

        /// <summary>
        /// Constructor.
        /// </summary>
        public HandleTelegramActionCommandHandler(ITelegramBotClient botClient, ITelegramBotSettings telegramBotSettings, IMediator mediator, ILogger<HandleTelegramActionCommandHandler> logger)
        {
            this.botClient = botClient;
            this.telegramBotSettings = telegramBotSettings;
            this.mediator = mediator;
            this.logger = logger;
        }

        /// <inheritdoc/>
        protected override async Task Handle(HandleTelegramActionCommand request, CancellationToken cancellationToken)
        {
            var messageText = request.Action.Message?.Text;
            ChatId chatId = null;
            try
            {
                if (!string.IsNullOrEmpty(messageText))
                {
                    chatId = request.Action.Message.Chat.Id;
                    var isDirectMessage = request.Action.Message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private;
                    var command = ParseCommand(messageText, out string arguments, ref isDirectMessage);
                    if (string.IsNullOrEmpty(command))
                    {
                        return;
                    }

                    await HandleCommandAsync(command, arguments, isDirectMessage, chatId, request.Action.Message.From);
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

        private async Task HandleCommandAsync(string command, string arguments, bool isDirectMessage, ChatId chatId, User sender)
        {
            long senderId = sender.Id;
            Task action = command switch
            {
                "pidoreg" => mediator.Send(new PidorGameRegisterCommand()
                {
                    ChatId = chatId,
                    TelegramUserId = senderId,
                    TelegramUserName = $"{sender.FirstName} {sender.LastName}"
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
