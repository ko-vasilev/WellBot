using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using WellBot.Domain.Chats.Entities;
using WellBot.DomainServices.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.UseCases.Common.Extensions;

namespace WellBot.UseCases.Chats.Pidor.PidorGameRun
{
    /// <summary>
    /// Handler for <see cref="PidorGameRunCommand"/>.
    /// </summary>
    internal class PidorGameRunCommandHandler : AsyncRequestHandler<PidorGameRunCommand>
    {
        private readonly ITelegramBotClient botClient;
        private readonly IAppDbContext dbContext;
        private readonly PidorGameService pidorGameService;
        private readonly CurrentChatService currentChatService;

        private static readonly IList<IEnumerable<string>> pidorSelectionTemplates = new List<IEnumerable<string>>
        {
            new List<string>
            {
                "Пидор-бригада выехала...",
                "Начинаем поиск",
                "Опа, попался, {0}!"
            },
            new List<string>
            {
                "🤔",
                "🗿",
                "А сегодняшний пидор дня {0}!"
            },
            new List<string>
            {
                "Начинаем поиск...",
                "Что вы наделали...",
                "Пидором дня оказался {0}"
            },
            new List<string>
            {
                "Так-так-так",
                "Опа Ф5, дамы и госпада!",
                "А пидор дня-то {0}!"
            },
            new List<string>
            {
                "Зачем вы меня разбудили...",
                "Сегодня в робота полезет",
                "Пидор дня - {0}!"
            }
        };

        public PidorGameRunCommandHandler(ITelegramBotClient botClient, IAppDbContext dbContext, PidorGameService pidorGameService, CurrentChatService currentChatService)
        {
            this.botClient = botClient;
            this.dbContext = dbContext;
            this.pidorGameService = pidorGameService;
            this.currentChatService = currentChatService;
        }

        /// <inheritdoc/>
        protected override async Task Handle(PidorGameRunCommand request, CancellationToken cancellationToken)
        {
            var pidorGameDay = pidorGameService.GetCurrentGameDay();

            // Make sure the game has not run yet.
            var selectedPidor = await dbContext.ChatPidors
                .Include(p => p.Registration)
                .Where(p => p.GameDay == pidorGameDay)
                .FirstOrDefaultAsync(p => p.ChatId == currentChatService.ChatId);
            if (selectedPidor != null)
            {
                var user = await botClient.GetChatMemberAsync(request.ChatId, selectedPidor.Registration.TelegramUserId);
                if (user == null)
                {
                    await botClient.SendTextMessageAsync(request.ChatId, "Пидор дня вышел из чата :(");
                    return;
                }

                await botClient.SendTextMessageAsync(request.ChatId, $"По моей информации пидор дня — @{user.User.Username}", disableNotification: true);
                return;
            }

            var users = await pidorGameService.GetPidorUsersAsync(currentChatService.ChatId, request.ChatId, cancellationToken);
            if (!users.Any())
            {
                await botClient.SendTextMessageAsync(request.ChatId, "Никто ещё не зарегистрировался на игру.");
                return;
            }

            var pidor = users.PickRandom();
            var pidorData = new ChatPidor
            {
                ChatId = currentChatService.ChatId,
                GameDay = pidorGameDay,
                RegistrationId = pidor.PidorRegistration.Id
            };
            dbContext.ChatPidors.Add(pidorData);
            await dbContext.SaveChangesAsync();

            var notification = pidorSelectionTemplates.PickRandom();
            var pidorUsername = "@" + pidor.User.Username;
            foreach (var text in notification)
            {
                var message = string.Format(text, pidorUsername);
                await botClient.SendTextMessageAsync(request.ChatId, message, disableNotification: true);
                await Task.Delay(TimeSpan.FromSeconds(0.5));
            }
        }
    }
}
