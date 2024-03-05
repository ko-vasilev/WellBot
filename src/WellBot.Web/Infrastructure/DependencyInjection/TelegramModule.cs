using Microsoft.Extensions.Options;
using Telegram.Bot;
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

        // Register named HttpClient to get benefits of IHttpClientFactory
        // and consume it with ITelegramBotClient typed client.
        services.AddHttpClient("tgwebhook")
            .AddTypedClient<ITelegramBotClient>((httpClient, serviceProvider) =>
            {
                var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
                return new TelegramBotClient(appSettings.Value.BotToken, httpClient);
            });
    }
}
