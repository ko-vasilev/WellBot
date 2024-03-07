using MediatR;
using Microsoft.EntityFrameworkCore;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.Ememe;

/// <summary>
/// Handler for <see cref="EmemeCommand"/>.
/// </summary>
internal class EmemeCommandHandler : AsyncRequestHandler<EmemeCommand>
{
    private readonly ITelegramBotClient botClient;
    private readonly RandomService randomService;
    private readonly IAppDbContext dbContext;

    public EmemeCommandHandler(ITelegramBotClient botClient, RandomService randomService, IAppDbContext dbContext)
    {
        this.botClient = botClient;
        this.randomService = randomService;
        this.dbContext = dbContext;
    }

    /// <inheritdoc/>
    protected override async Task Handle(EmemeCommand request, CancellationToken cancellationToken)
    {
        var memeChannel = await dbContext.MemeChannels.FirstOrDefaultAsync(cancellationToken);
        if (memeChannel == null)
        {
            await botClient.SendMessageAsync(request.ChatId, "Не настроен канал с мемами :(");
            return;
        }

        var attempts = 0;
        // Try multiple times because some messages might not be available because they were deleted or it is special kinds of messages.
        while (attempts < 3)
        {
            var messageId = randomService.GetRandom(memeChannel.LatestMessageId);
            // Generated random starts from 0 so increase it by 1.
            ++messageId;
            try
            {
                await botClient.ForwardMessageAsync(request.ChatId, memeChannel.ChannelId, messageId);
                return;
            }
            catch
            {
                ++attempts;
            }
        }

        await botClient.SendMessageAsync(request.ChatId, "Не сегодня");
    }
}
