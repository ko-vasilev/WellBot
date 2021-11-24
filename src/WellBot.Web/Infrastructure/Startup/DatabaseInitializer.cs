using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using WellBot.Infrastructure.DataAccess;

namespace WellBot.Web.Infrastructure.Startup
{
    /// <summary>
    /// Contains database migration helper methods.
    /// </summary>
    internal sealed class DatabaseInitializer : IAsyncInitializer
    {
        private readonly AppDbContext appDbContext;

        /// <summary>
        /// Database initializer. Performs migration and data seed.
        /// </summary>
        /// <param name="appDbContext">Data context.</param>
        public DatabaseInitializer(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            await appDbContext.Database.MigrateAsync();
        }
    }
}
