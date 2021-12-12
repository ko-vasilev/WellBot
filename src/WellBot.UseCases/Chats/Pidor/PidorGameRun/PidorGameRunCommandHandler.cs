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

            var pidorUsername = "@" + pidor.User.Username;
            foreach (var text in notification.Message)
            {
                var message = text.Replace(PidorMessage.UsernamePlaceholder, pidorUsername);
                await botClient.SendTextMessageAsync(request.ChatId,
                    message,
                    disableNotification: true,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
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
                .AsQueryable();
            if (previousWin != null)
            {
                availableMessages = availableMessages.Where(m => m.Id != previousWin.Id);
            }

            var regularMessages = availableMessages.Where(m => m.GameDay == null && m.TelegramUserId == null);
            var userMessages = availableMessages.Where(m => m.TelegramUserId == winnerTelegramId);
            var dayMessages = availableMessages.Where(m => m.GameDay != null && m.GameDay.Value.Month == gameDay.Month && m.GameDay.Value.Day == gameDay.Day);

            var messages = await regularMessages.Union(userMessages)
                .Union(dayMessages)
                .Select(m => new MessageData
                {
                    MessageRaw = m.MessageRaw,
                    MessageWeight = m.Weight,
                    Id = m.Id
                })
                .ToListAsync();

            var randomMessage = PickRandom(messages);
            return (randomMessage.Id, randomMessage.MessageRaw.Split(PidorMessage.MessagesSeparator));
        }

        private MessageData PickRandom(IEnumerable<MessageData> messages)
        {
            // Prepare a weighted list.
            var weightedItems = messages.Select(m => (m, GetWeight(m.MessageWeight)))
                .ToList();

            var totalWeight = weightedItems.Sum(m => m.Item2);
            var desiredWeight = EnumerableRandom.GetRandom(totalWeight);
            var currentWeightSum = 0;
            foreach (var item in weightedItems)
            {
                var currentWeightRange = currentWeightSum + item.Item2;
                if (currentWeightSum <= desiredWeight && desiredWeight < currentWeightRange)
                {
                    return item.m;
                }
                currentWeightSum = currentWeightRange;
            }

            // Weird, shouldn't happen.
            return weightedItems.Last().m;
        }

        private int GetWeight(MessageWeight weight) => weight switch
            {
                MessageWeight.Highest => 100,
                MessageWeight.High => 10,
                _ => 1
            };

        private class MessageData
        {
            public int Id { get; init; }

            public string MessageRaw { get; init; }

            public MessageWeight MessageWeight { get; init; }
        }
    }
}
