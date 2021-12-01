using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using WellBot.Web.Infrastructure.Telegram;

namespace WellBot.Web.Controllers
{
    /// <summary>
    /// Handles bot messages.
    /// </summary>
    public class BotController : ControllerBase
    {
        private TelegramHandler actionHandler;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BotController(TelegramHandler actionHandler)
        {
            this.actionHandler = actionHandler;
        }

        /// <summary>
        /// Handle telegram webhook action.
        /// </summary>
        /// <param name="update">Information about an action.</param>
        [HttpPost]
        public async Task<IActionResult> Telegram([FromBody] Update update)
        {
            await actionHandler.HandleAsync(update);
            return Ok();
        }
    }
}
