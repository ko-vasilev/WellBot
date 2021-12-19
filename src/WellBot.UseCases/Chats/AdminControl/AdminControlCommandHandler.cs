using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using WellBot.Domain.Chats.Entities;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.AdminControl
{
    /// <summary>
    /// Handler for <see cref="AdminControlCommand"/>.
    /// </summary>
    internal class AdminControlCommandHandler : AsyncRequestHandler<AdminControlCommand>
    {
        private readonly IAppDbContext appDbContext;
        private readonly TelegramMessageService telegramMessageService;
        private readonly ILogger<AdminControlCommandHandler> logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AdminControlCommandHandler(IAppDbContext appDbContext, TelegramMessageService telegramMessageService, ILogger<AdminControlCommandHandler> logger)
        {
            this.appDbContext = appDbContext;
            this.telegramMessageService = telegramMessageService;
            this.logger = logger;
        }

        /// <inheritdoc/>
        protected override async Task Handle(AdminControlCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Arguments == "add slap")
                {
                    await AddSlapOptionAsync(request.Message);
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling admin command {command}", request.Arguments);
            }
        }

        private async Task AddSlapOptionAsync(Message message)
        {
            var replyMessage = message.ReplyToMessage;
            if (replyMessage?.Animation == null)
            {
                return;
            }

            var slapOption = new SlapOption
            {
                FileId = replyMessage.Animation.FileId
            };
            appDbContext.SlapOptions.Add(slapOption);
            await appDbContext.SaveChangesAsync();
            await telegramMessageService.SendSuccessAsync(message.Chat.Id);
        }
    }
}
