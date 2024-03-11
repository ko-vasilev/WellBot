using AngleSharp;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;

namespace WellBot.UseCases.Chats.Prikol;

/// <summary>
/// Handler for <see cref="PrikolCommand"/>.
/// </summary>
internal class PrikolCommandHandler : AsyncRequestHandler<PrikolCommand>
{
    private readonly ITelegramBotClient botClient;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<PrikolCommandHandler> logger;

    public PrikolCommandHandler(ITelegramBotClient botClient, IHttpClientFactory httpClientFactory, ILogger<PrikolCommandHandler> logger)
    {
        this.botClient = botClient;
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
    }

    /// <inheritdoc/>
    protected override async Task Handle(PrikolCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var anekdot = await GetRandomAnekdotAsync(cancellationToken);
            await botClient.SendMessageAsync(request.ChatId, anekdot);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting a random anekdot");
            await botClient.SendMessageAsync(request.ChatId, "Приколы закончились, приходите завтра.");
        }
    }

    private async Task<string> GetRandomAnekdotAsync(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Get,
            "https://www.anekdot.ru/random/anekdot/");
        using var client = httpClientFactory.CreateClient();
        using var response = await client.SendAsync(request, cancellationToken);
        using var content = await response.Content.ReadAsStreamAsync();
        response.EnsureSuccessStatusCode();

        var context = BrowsingContext.New(Configuration.Default);
        var document = await context.OpenAsync(req => req.Content(content), cancellationToken);

        var anekdotes = document.All
            .Where(m => m.ClassList.Contains("topicbox"))
            .SelectMany(m => m.GetElementsByClassName("text"))
            .FirstOrDefault();

        if (anekdotes == null)
        {
            throw new Exception("Could not find an anekdot");
        }

        return anekdotes.InnerHtml
            .Replace("<br>", "\n")
            .Replace("<br/>", "\n");
    }
}
