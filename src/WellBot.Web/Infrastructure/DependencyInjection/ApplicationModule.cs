using Hangfire;
using Hangfire.MemoryStorage;
using WellBot.DomainServices.Chats;
using WellBot.UseCases.Chats;
using WellBot.UseCases.Chats.Pidor;

namespace WellBot.Web.Infrastructure.DependencyInjection;

/// <summary>
/// Application specific dependencies.
/// </summary>
internal static class ApplicationModule
{
    /// <summary>
    /// Register dependencies.
    /// </summary>
    /// <param name="services">Services.</param>
    public static void Register(IServiceCollection services)
    {
        services.AddScoped<CurrentChatService>();
        services.AddTransient<TelegramMessageService>();
        services.AddTransient<PidorGameService>();
        services.AddSingleton<MessageRateLimitingService>();
        services.AddSingleton<RandomService>();
        services.AddSingleton<MemeChannelService>();
        services.AddSingleton<UseCases.Chats.RegularMessageHandles.Reply.PassiveTopicService>();
        services.AddTransient(typeof(Lazy<>), typeof(Lazier<>));

        ConfigureHangfire(services);
    }

    private static void ConfigureHangfire(IServiceCollection services)
    {
        services.AddHangfire(configuration => configuration
           .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
           .UseSimpleAssemblyNameTypeSerializer()
           .UseRecommendedSerializerSettings()
           .UseMemoryStorage());

        services.AddHangfireServer();
    }

    /// <summary>
    /// DI implementation for resolving dependencies as Lazy.
    /// </summary>
    internal class Lazier<T> : Lazy<T> where T : class
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Lazier(IServiceProvider provider)
            : base(() => provider.GetRequiredService<T>())
        {
        }
    }
}
