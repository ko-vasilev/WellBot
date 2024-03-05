using MediatR;
using WellBot.DomainServices.Chats;
using WellBot.UseCases.Chats;

namespace WellBot.Web.Infrastructure.Telegram;

/// <summary>
/// Stores information about current action chat.
/// </summary>
public class TelegramChatSetterPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly CurrentChatService currentChatService;
    private readonly ILogger<TelegramChatSetterPipelineBehavior<TRequest, TResponse>> logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    public TelegramChatSetterPipelineBehavior(CurrentChatService currentChatService, ILogger<TelegramChatSetterPipelineBehavior<TRequest, TResponse>> logger)
    {
        this.currentChatService = currentChatService;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IChatInfo chatInfo)
        {
            if (chatInfo.ChatId.Identifier.HasValue)
            {
                await currentChatService.SetCurrentChatIdAsync(chatInfo.ChatId.Identifier.Value, cancellationToken);
            }
            else
            {
                logger.LogWarning("ChatId is not set, {command}", request);
            }
        }

        return await next();
    }
}
