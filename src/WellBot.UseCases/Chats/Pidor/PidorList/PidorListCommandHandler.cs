using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using WellBot.DomainServices.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Pidor.PidorList;

/// <summary>
/// Handler for <see cref="PidorListCommand"/>.
/// </summary>
internal class PidorListCommandHandler : AsyncRequestHandler<PidorListCommand>
{
    private readonly ITelegramBotClient botClient;
    private readonly IAppDbContext dbContext;
    private readonly CurrentChatService currentChatService;
    private readonly PidorGameService pidorGameService;
    private readonly TelegramMessageService telegramMessageService;

    /// <summary>
    /// Constructor.
    /// </summary>
    public PidorListCommandHandler(ITelegramBotClient botClient, IAppDbContext dbContext, CurrentChatService currentChatService, PidorGameService pidorGameService, TelegramMessageService telegramMessageService)
    {
        this.botClient = botClient;
        this.dbContext = dbContext;
        this.currentChatService = currentChatService;
        this.pidorGameService = pidorGameService;
        this.telegramMessageService = telegramMessageService;
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

        if (IsDeleteRequest(request.Arguments, out var deleteUsername))
        {
            var deleteUser = users.FirstOrDefault(u => u.User.Username == deleteUsername);

            if (deleteUser == null)
            {
                await botClient.SendTextMessageAsync(request.ChatId, "Пользователь не зарегистрирован в игре.");
                return;
            }

            var user = await dbContext.PidorRegistrations.FirstOrDefaultAsync(u => u.TelegramUserId == deleteUser.User.Id);
            if (user != null)
            {
                dbContext.PidorRegistrations.Remove(user);
                await dbContext.SaveChangesAsync();
            }
            await telegramMessageService.SendSuccessAsync(request.ChatId);
            return;
        }

        string reply;
        if (users.Any())
        {
            reply = "Список участвующих:\n" + string.Join("\n", users.Select(user => FormatUserName(user.User)));
        }
        else
        {
            reply = "Пока никто не зарегистрировался на игру.";
        }
        await botClient.SendTextMessageAsync(request.ChatId, reply, Telegram.Bot.Types.Enums.ParseMode.Html);
    }

    private string FormatUserName(User user)
    {
        var userName = telegramMessageService.GetUserFullName(user);
        if (!string.IsNullOrEmpty(user.Username))
        {
            userName += $" (<b>{user.Username}</b>)";
        }
        return userName;
    }

    private bool IsDeleteRequest(string arguments, out string deleteUsername)
    {
        deleteUsername = string.Empty;

        if (string.IsNullOrEmpty(arguments))
        {
            return false;
        }

        var argumentParams = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (argumentParams.Length <= 1)
        {
            return false;
        }

        var subCommand = argumentParams[0];
        if (subCommand.Equals("del", StringComparison.InvariantCultureIgnoreCase) || subCommand.Equals("delete", StringComparison.InvariantCultureIgnoreCase))
        {
            deleteUsername = argumentParams[1].TrimStart('@');
            return true;
        }

        return false;
    }
}
