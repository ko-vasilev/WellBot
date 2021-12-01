using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WellBot.Web.Infrastructure.Telegram
{
    /// <summary>
    /// Handles Telegram actions.
    /// </summary>
    public class TelegramHandler
    {
        private readonly ITelegramBotClient botClient;
        private readonly ILogger<TelegramHandler> logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TelegramHandler(ITelegramBotClient botClient, ILogger<TelegramHandler> logger)
        {
            this.botClient = botClient;
            this.logger = logger;
        }

        /// <summary>
        /// Handle a Telegram action.
        /// </summary>
        /// <param name="action">Contains information about what has happened: message received or anything else.</param>
        public async Task HandleAsync(Update action)
        {
            if (!string.IsNullOrEmpty(action.Message?.Text))
            {
                await botClient.SendTextMessageAsync(action.Message.Chat.Id, action.Message.Text);
            }
            // TODO
            await Task.CompletedTask;
        }
    }
}
