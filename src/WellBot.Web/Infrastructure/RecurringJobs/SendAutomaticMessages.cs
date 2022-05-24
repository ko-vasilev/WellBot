using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WellBot.Web.Infrastructure.RecurringJobs
{
    /// <summary>
    /// Send automatic messages in Telegram chats.
    /// </summary>
    public class SendAutomaticMessages
    {
        private readonly ITelegramBotClient telegramBotClient;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SendAutomaticMessages(ITelegramBotClient telegramBotClient) => this.telegramBotClient = telegramBotClient;

        /// <summary>
        /// Send messages.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task SendAsync(CancellationToken cancellationToken)
        {
            await telegramBotClient.SendTextMessageAsync(new ChatId(-1001568355052), "test", cancellationToken: cancellationToken);
        }
    }
}
