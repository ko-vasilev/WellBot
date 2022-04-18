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
        private readonly TelegramMessageService telegramMessageService;
        private readonly RandomService randomService;

        public PidorGameRunCommandHandler(ITelegramBotClient botClient, IAppDbContext dbContext, PidorGameService pidorGameService, CurrentChatService currentChatService, TelegramMessageService telegramMessageService, RandomService randomService)
        {
            this.botClient = botClient;
            this.dbContext = dbContext;
            this.pidorGameService = pidorGameService;
            this.currentChatService = currentChatService;
            this.telegramMessageService = telegramMessageService;
            this.randomService = randomService;
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
                var user = await pidorGameService.GetPidorMemberAsync(request.ChatId, selectedPidor.Registration.TelegramUserId, cancellationToken);
                if (user == null)
                {
                    await botClient.SendTextMessageAsync(request.ChatId, "Пидор дня вышел из чата :(");
                    return;
                }

                var username = telegramMessageService.GetPersonNameHtml(user.User, mention: false);
                await botClient.SendTextMessageAsync(request.ChatId, $"По моей информации пидор дня — {username}", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, disableNotification: true);
                return;
            }

            var users = await pidorGameService.GetPidorUsersAsync(currentChatService.ChatId, request.ChatId, cancellationToken);
            if (!users.Any())
            {
                await botClient.SendTextMessageAsync(request.ChatId, "Никто ещё не зарегистрировался на игру.");
                return;
            }

            var pidor = randomService.PickRandom(users);
            var notification = await GetNotificationMessageAsync(pidor.User.Id, pidorGameDay);
            var pidorData = new ChatPidor
            {
                ChatId = currentChatService.ChatId,
                GameDay = pidorGameDay,
                RegistrationId = pidor.PidorRegistration.Id,
                UsedMessageId = notification.Id
            };
            dbContext.ChatPidors.Add(pidorData);
            await dbContext.SaveChangesAsync();

            var pidorUsername = telegramMessageService.GetPersonNameHtml(pidor.User, mention: true);
            foreach (var text in notification.Message)
            {
                var message = text.Replace(PidorMessage.UsernamePlaceholder, pidorUsername);
                await botClient.SendTextMessageAsync(request.ChatId,
                    message,
                    disableNotification: true,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                await Task.Delay(TimeSpan.FromSeconds(0.7));
            }
        }

        private async Task<(int Id, IEnumerable<string> Message)> GetNotificationMessageAsync(long winnerTelegramId, DateTime gameDay)
        {
            var previousWin = await dbContext.ChatPidors
                .Where(p => p.ChatId == currentChatService.ChatId)
                .OrderByDescending(p => p.GameDay)
                .FirstOrDefaultAsync();

            var availableMessages = dbContext.PidorResultMessages
                .AsNoTracking()
                .Where(m => m.IsActive);
            if (previousWin != null)
            {
                availableMessages = availableMessages.Where(m => m.Id != previousWin.Id);
            }

            var regularMessages = availableMessages.Where(m => m.GameDay == null && m.TelegramUserId == null && m.DayOfWeek == null);
            var userMessages = availableMessages.Where(m => m.TelegramUserId == winnerTelegramId);
            var dayMessages = availableMessages.Where(m => m.GameDay != null && m.GameDay.Value.Month == gameDay.Month && m.GameDay.Value.Day == gameDay.Day);
            var dayOfWeekMessages = availableMessages.Where(m => m.DayOfWeek == gameDay.DayOfWeek);

            var messages = await regularMessages.Union(userMessages)
                .Union(dayMessages)
                .Union(dayOfWeekMessages)
                .Select(m => new MessageData
                {
                    MessageRaw = m.MessageRaw,
                    Weight = m.Weight,
                    Id = m.Id
                })
                .ToListAsync();

            var randomMessage = randomService.PickWeighted(messages);
            return (randomMessage.Id, randomMessage.MessageRaw.Split(PidorMessage.MessagesSeparator));
        }

        private class MessageData : RandomService.IWeighted
        {
            public int Id { get; init; }

            public string MessageRaw { get; init; }

            public MessageWeight Weight { get; init; }
        }
    }
}
