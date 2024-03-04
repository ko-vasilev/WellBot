using Microsoft.EntityFrameworkCore;
using WellBot.Domain.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.DomainServices.Chats
{
    /// <summary>
    /// Service for storing information about the current chat that is being handled.
    /// </summary>
    public class CurrentChatService
    {
        private readonly IAppDbContext dbContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CurrentChatService(IAppDbContext dbContext) => this.dbContext = dbContext;

        /// <summary>
        /// Id of the chat (<see cref="Chat.Id"/>).
        /// </summary>
        public int ChatId { get; private set; }

        /// <summary>
        /// Store information about the current chat.
        /// </summary>
        /// <param name="telegramChatId">Id of the chat in Telegram.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task SetCurrentChatIdAsync(long telegramChatId, CancellationToken cancellationToken)
        {
            var chat = await dbContext.Chats.FirstOrDefaultAsync(c => c.TelegramId == telegramChatId, cancellationToken);
            if (chat == null)
            {
                chat = new Chat()
                {
                    TelegramId = telegramChatId
                };
                dbContext.Chats.Add(chat);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            ChatId = chat.Id;
        }
    }
}
