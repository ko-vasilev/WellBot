using Microsoft.Extensions.Options;
using Telegram.BotAPI;
using WellBot.Web.Infrastructure.Settings;
using WellBot.Web.Infrastructure.Telegram;

namespace WellBot.Web.Infrastructure.DependencyInjection;

/// <summary>
/// Module for registering telegram-specific services.
/// </summary>
internal static class TelegramModule
{
    /// <summary>
    /// Register dependencies.
    /// </summary>
    /// <param name="services">Services.</param>
    public static void Register(IServiceCollection services)
    {
        services.AddHostedService<TelegramWebhookInitializer>();

        services.AddScoped<ITelegramBotClient>(serviceProvider =>
        {
            var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            return new TelegramBotClient(appSettings.Value.BotToken, httpClientFactory.CreateClient());
        });
    }
}
