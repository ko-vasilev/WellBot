using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WellBot.DomainServices.Chats;
using WellBot.UseCases.Chats;

namespace WellBot.Web.Infrastructure.Telegram
{
    /// <summary>
    /// Stores information about current action chat.
    /// </summary>
    public class TelegramChatSetterPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly CurrentChatService currentChatService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TelegramChatSetterPipelineBehavior(CurrentChatService currentChatService) => this.currentChatService = currentChatService;

        /// <inheritdoc />
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is IChatInfo chatInfo)
            {
                await currentChatService.SetCurrentChatIdAsync(chatInfo.ChatId.Identifier.Value, cancellationToken);
            }

            return await next();
        }
    }
}
