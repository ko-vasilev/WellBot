using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using WellBot.DomainServices.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Pidor.PidorList
{
    /// <summary>
    /// Handler for <see cref="PidorListCommand"/>.
    /// </summary>
    internal class PidorListCommandHandler : AsyncRequestHandler<PidorListCommand>
    {
        private readonly ITelegramBotClient botClient;
        private readonly IAppDbContext dbContext;
        private readonly CurrentChatService currentChatService;
        private readonly PidorGameService pidorGameService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PidorListCommandHandler(ITelegramBotClient botClient, IAppDbContext dbContext, CurrentChatService currentChatService, PidorGameService pidorGameService)
        {
            this.botClient = botClient;
            this.dbContext = dbContext;
            this.currentChatService = currentChatService;
            this.pidorGameService = pidorGameService;
        }

        /// <inheritdoc/>
        protected override async Task Handle(PidorListCommand request, CancellationToken cancellationToken)
        {
            var chatInfo = await botClient.GetChatAsync(request.ChatId, cancellationToken);
            if (chatInfo.Type == Telegram.Bot.Types.Enums.ChatType.Group || chatInfo.Type == Telegram.Bot.Types.Enums.ChatType.Supergroup)
            {
                var chatAdmins = await botClient.GetChatAdministratorsAsync(request.ChatId, cancellationToken);
                var isSenderAdmin = chatAdmins.Any(admin => admin.User.Id == request.TelegramUserId);
                if (!isSenderAdmin)
                {
                    // Don't do anything.
                    return;
                }
            }

            var users = await pidorGameService.GetPidorUsersAsync(currentChatService.ChatId, request.ChatId, cancellationToken);

            string reply;
            if (users.Any())
            {
                reply = "Список участвующих:\n" + string.Join("\n", users.Select(user => FormatUserName(user.User)));
            }
            else
            {
                reply = "Пока никто не зарегистрировался на игру.";
            }
            await botClient.SendTextMessageAsync(request.ChatId, reply, Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }

        private string FormatUserName(User user)
        {
            var userName = $"{user.FirstName} {user.LastName}";
            if (!string.IsNullOrEmpty(user.Username))
            {
                userName += $" (*{user.Username}*)";
            }
            return userName;
        }
    }
}
