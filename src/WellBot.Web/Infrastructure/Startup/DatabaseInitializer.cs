using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using WellBot.Infrastructure.DataAccess;
using WellBot.UseCases.Chats;
using WellBot.UseCases.Chats.RegularMessageHandles.Reply;

namespace WellBot.Web.Infrastructure.Startup
{
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
        public async Task InitializeAsync()
        {
            await appDbContext.Database.MigrateAsync();

            var currentMemeChannel = await appDbContext.MemeChannels
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (currentMemeChannel != null)
            {
                memeChannelService.CurrentMemeChatId = currentMemeChannel.ChannelId;
            }

            var existingTopics = await appDbContext.PassiveTopics
                .AsNoTracking()
                .ToListAsync();
            passiveTopicService.Update(existingTopics);
        }
    }
}
