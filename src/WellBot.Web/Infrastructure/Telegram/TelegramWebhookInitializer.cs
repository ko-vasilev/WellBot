using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using WellBot.Web.Infrastructure.Settings;

namespace WellBot.Web.Infrastructure.Telegram
{
    /// <summary>
    /// Initializes the webhook configuration for the telegram bot.
    /// </summary>
    public class TelegramWebhookInitializer : IHostedService
    {
        private readonly ILogger<TelegramWebhookInitializer> logger;
        private readonly IServiceProvider services;
        private readonly AppSettings appSettings;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TelegramWebhookInitializer(ILogger<TelegramWebhookInitializer> logger,
                                IServiceProvider serviceProvider,
                                IOptions<AppSettings> appSettings)
        {
            this.logger = logger;
            services = serviceProvider;
            this.appSettings = appSettings.Value;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            // Configure custom endpoint per Telegram API recommendations:
            // https://core.telegram.org/bots/api#setwebhook
            // If you'd like to make sure that the Webhook request comes from Telegram, we recommend
            // using a secret path in the URL, e.g. https://www.example.com/<token>.
            // Since nobody else knows your bot's token, you can be pretty sure it's us.
            var webhookAddress = @$"{appSettings.HostAddress}/bot/tg/{appSettings.BotToken}";
            logger.LogInformation("Setting webhook: {webhookAddress}", webhookAddress);
            await botClient.SetWebhookAsync(
                url: webhookAddress,
                allowedUpdates: Array.Empty<UpdateType>(),
                cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            // Remove webhook upon app shutdown.
            logger.LogInformation("Removing webhook");
            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
    }
}
