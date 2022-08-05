using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WellBot.Infrastructure;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.Infrastructure.DataAccess;
using WellBot.UseCases.Users.AuthenticateUser;
using WellBot.Web.Infrastructure.Jwt;
using WellBot.Web.Infrastructure.Settings;
using WellBot.Web.Infrastructure.Web;

namespace WellBot.Web.Infrastructure.DependencyInjection
{
    /// <summary>
    /// System specific dependencies.
    /// </summary>
    internal static class SystemModule
    {
        /// <summary>
        /// Register dependencies.
        /// </summary>
        /// <param name="services">Services.</param>
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IJsonHelper, SystemTextJsonHelper>();
            services.AddScoped<IAuthenticationTokenService, SystemJwtTokenService>();
            services.AddScoped<IAppDbContext, AppDbContext>();
            services.AddScoped<ILoggedUserAccessor, LoggedUserAccessor>();

            services.AddScoped<IImageSearcher, GoogleImageSearcher>();
            services.AddTransient<IVideoConverter, VideoConverter>();
            services.AddTransient<GoogleImageSearcherSettings>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
                return new GoogleImageSearcherSettings
                {
                    ApiKey = settings.Value.SerpApiKey
                };
            });
        }
    }
}
