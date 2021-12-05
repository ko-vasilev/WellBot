using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using WellBot.Domain.Chats.Entities;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Pidor.PidorGameRegister
{
    /// <summary>
    /// Handler for <see cref="PidorGameRegisterCommand"/>.
    /// </summary>
    internal class PidorGameRegisterCommandHandler : AsyncRequestHandler<PidorGameRegisterCommand>
    {
        private readonly IAppDbContext dbContext;
        private readonly ITelegramBotClient botClient;

        public PidorGameRegisterCommandHandler(IAppDbContext dbContext, ITelegramBotClient botClient)
        {
            this.dbContext = dbContext;
            this.botClient = botClient;
        }

        /// <inheritdoc/>
        protected override async Task Handle(PidorGameRegisterCommand request, CancellationToken cancellationToken)
        {
            var registrationExists = await dbContext.PidorRegistrations
                .Where(reg => reg.TelegramUserId == request.TelegramUserId)
                .Where(reg => reg.Chat.TelegramId == request.ChatId.Identifier.Value)
                .AnyAsync(cancellationToken);
            if (registrationExists)
            {
                await botClient.SendTextMessageAsync(request.ChatId, "Эй, ты уже в игре!");
                return;
            }

            var chat = await dbContext.Chats.FirstOrDefaultAsync(c => c.TelegramId == request.ChatId.Identifier.Value, cancellationToken);
            if (chat == null)
            {
                chat = new Chat()
                {
                    TelegramId = request.ChatId.Identifier.Value
                };
                dbContext.Chats.Add(chat);
            }

            var registration = new PidorRegistration()
            {
                Chat = chat,
                TelegramUserId = request.TelegramUserId
            };
            dbContext.PidorRegistrations.Add(registration);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
