using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using WellBot.Domain.Chats;
using WellBot.DomainServices.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Pidor.PidorGameRegister;

/// <summary>
/// Handler for <see cref="PidorGameRegisterCommand"/>.
/// </summary>
internal class PidorGameRegisterCommandHandler : AsyncRequestHandler<PidorGameRegisterCommand>
{
    private readonly IAppDbContext dbContext;
    private readonly ITelegramBotClient botClient;
    private readonly CurrentChatService currentChatService;
    private readonly TelegramMessageService telegramMessageService;

    public PidorGameRegisterCommandHandler(IAppDbContext dbContext, ITelegramBotClient botClient, CurrentChatService currentChatService, TelegramMessageService telegramMessageService)
    {
        this.dbContext = dbContext;
        this.botClient = botClient;
        this.currentChatService = currentChatService;
        this.telegramMessageService = telegramMessageService;
    }

    /// <inheritdoc/>
    protected override async Task Handle(PidorGameRegisterCommand request, CancellationToken cancellationToken)
    {
        var registrationExists = await dbContext.PidorRegistrations
            .Where(reg => reg.TelegramUserId == request.TelegramUserId)
            .Where(reg => reg.ChatId == currentChatService.ChatId)
            .AnyAsync(cancellationToken);
        if (registrationExists)
        {
            await botClient.SendTextMessageAsync(request.ChatId, "Эй, ты уже в игре!");
            return;
        }

        var registration = new PidorRegistration()
        {
            ChatId = currentChatService.ChatId,
            TelegramUserId = request.TelegramUserId,
            OriginalTelegramUserName = request.TelegramUserName
        };
        dbContext.PidorRegistrations.Add(registration);
        await dbContext.SaveChangesAsync(cancellationToken);
        await telegramMessageService.SendSuccessAsync(request.ChatId);
    }
}
