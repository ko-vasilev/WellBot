using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using WellBot.DomainServices.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Data.ShowKeys
{
    /// <summary>
    /// Handler for <see cref="ShowKeysCommand"/>.
    /// </summary>
    internal class ShowKeysCommandHandler : AsyncRequestHandler<ShowKeysCommand>
    {
        private readonly IAppDbContext dbContext;
        private readonly ITelegramBotClient botClient;
        private readonly CurrentChatService currentChatService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ShowKeysCommandHandler(IAppDbContext dbContext, ITelegramBotClient botClient, CurrentChatService currentChatService)
        {
            this.dbContext = dbContext;
            this.botClient = botClient;
            this.currentChatService = currentChatService;
        }

        /// <inheritdoc/>
        protected override async Task Handle(ShowKeysCommand request, CancellationToken cancellationToken)
        {
            var items = await dbContext.ChatDatas
                .Where(d => d.ChatId == currentChatService.ChatId)
                .OrderBy(d => d.Key)
                .Select(d => d.Key)
                .ToListAsync();
            if (!items.Any())
            {
                await botClient.SendTextMessageAsync(request.ChatId, "Пока ещё ничего не сохранено");
                return;
            }

            var keysSummary = string.Join(", ", items);
            await botClient.SendTextMessageAsync(request.ChatId, keysSummary, disableNotification: true);
        }
    }
}
