using MediatR;
using Microsoft.AspNetCore.Mvc;
using Telegram.BotAPI.GettingUpdates;
using WellBot.UseCases.Chats.HandleTelegramAction;

namespace WellBot.Web.Controllers;

/// <summary>
/// Handles bot messages.
/// </summary>
public class BotController : ControllerBase
{
    private readonly IMediator mediator;

    /// <summary>
    /// Constructor.
    /// </summary>
    public BotController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// Handle telegram webhook action.
    /// </summary>
    /// <param name="update">Information about an action.</param>
    [HttpPost]
    public async Task<IActionResult> Telegram([FromBody] Update update)
    {
        await mediator.Send(new HandleTelegramActionCommand
        {
            Action = update
        });
        return Ok();
    }
}
