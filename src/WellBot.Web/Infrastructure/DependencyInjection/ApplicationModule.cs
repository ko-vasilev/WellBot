using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WellBot.DomainServices.Chats;

namespace WellBot.Web.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Application specific dependencies.
    /// </summary>
    internal static class ApplicationModule
    {
        /// <summary>
        /// Register dependencies.
        /// </summary>
        /// <param name="services">Services.</param>
        /// <param name="configuration">Configuration.</param>
#pragma warning disable CA1801 // Review unused parameters
        public static void Register(IServiceCollection services, IConfiguration configuration)
#pragma warning restore CA1801 // Review unused parameters
        {
            services.AddScoped<CurrentChatService>();
        }
    }
}
