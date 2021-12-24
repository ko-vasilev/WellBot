using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using WellBot.Infrastructure.DataAccess;
using WellBot.UseCases.Chats;

namespace WellBot.Web.Infrastructure.Startup
{
    /// <summary>
    /// Contains database migration helper methods.
    /// </summary>
    internal sealed class DatabaseInitializer : IAsyncInitializer
    {
        private readonly AppDbContext appDbContext;
        private readonly MemeChannelService memeChannelService;

        /// <summary>
        /// Database initializer. Performs migration and data seed.
        /// </summary>
        public DatabaseInitializer(AppDbContext appDbContext, MemeChannelService memeChannelService)
        {
            this.appDbContext = appDbContext;
            this.memeChannelService = memeChannelService;
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
        }
    }
}
