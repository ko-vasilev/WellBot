using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using WellBot.Infrastructure.DataAccess;
using WellBot.UseCases.Chats.RegularMessageHandles.Reply;
using WellBot.UseCases.Chats;

namespace WellBot.Web.Infrastructure.Startup;

/// <summary>
/// Contains database migration helper methods.
/// </summary>
internal sealed class DatabaseInitializer : IAsyncInitializer
{
    private readonly AppDbContext appDbContext;
    private readonly MemeChannelService memeChannelService;
    private readonly PassiveTopicService passiveTopicService;

    /// <summary>
    /// Database initializer. Performs migration and data seed.
    /// </summary>
    public DatabaseInitializer(AppDbContext appDbContext, MemeChannelService memeChannelService, PassiveTopicService passiveTopicService)
    {
        this.appDbContext = appDbContext;
        this.memeChannelService = memeChannelService;
        this.passiveTopicService = passiveTopicService;
    }

    /// <inheritdoc />
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await appDbContext.Database.MigrateAsync(cancellationToken);

        var currentMemeChannel = await appDbContext.MemeChannels
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        if (currentMemeChannel != null)
        {
            memeChannelService.CurrentMemeChatId = currentMemeChannel.ChannelId;
        }

        var existingTopics = await appDbContext.PassiveTopics
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        passiveTopicService.Update(existingTopics);

        var chatDataRecords = await appDbContext.ChatDatas
            .ToListAsync(cancellationToken);

        var invalidChatDataRecords = chatDataRecords.Where(d => d.Key.Contains('\n'));
        foreach (var chatData in invalidChatDataRecords)
        {
            chatData.Key = chatData.Key.Replace("\n", string.Empty);
        }
        await appDbContext.SaveChangesAsync(cancellationToken);
    }
}
