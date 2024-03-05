using MediatR;
using Microsoft.EntityFrameworkCore;
using WellBot.Domain.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.UseCases.Chats.Dtos;

namespace WellBot.UseCases.Chats.Slap;

/// <summary>
/// Handler for <see cref="SlapCommand"/>.
/// </summary>
internal class SlapCommandHandler : AsyncRequestHandler<SlapCommand>
{
    private static readonly IReadOnlyCollection<SlapReplyOption> replies = new List<SlapReplyOption>
    {
        new SlapReplyOption(3, "Нихуя себе блять"),
        new SlapReplyOption(3, "Нихуя соби блять", "Уебал..."),
        new SlapReplyOption(2, "Нихуя себе блять", "Ты въебал мне 😥"),
        new SlapReplyOption(1, "👊")
        {
            ShouldReply = true
        },
        new SlapReplyOption(1)
        {
            IsAnimation = true
        },
    };

    private readonly TelegramMessageService telegramMessageService;
    private readonly RandomService randomService;
    private readonly IAppDbContext dbContext;

    /// <summary>
    /// Constructor.
    /// </summary>
    public SlapCommandHandler(TelegramMessageService telegramMessageService, RandomService randomService, IAppDbContext dbContext)
    {
        this.telegramMessageService = telegramMessageService;
        this.randomService = randomService;
        this.dbContext = dbContext;
    }

    /// <inheritdoc/>
    protected override async Task Handle(SlapCommand request, CancellationToken cancellationToken)
    {
        var replyOption = randomService.PickWeightedRaw(replies);

        if (replyOption.IsAnimation)
        {
            var animationOptions = await dbContext.SlapOptions.ToListAsync(cancellationToken);
            if (animationOptions.Count > 0)
            {
                var animation = randomService.PickRandom(animationOptions);
                await telegramMessageService.SendMessageAsync(new GenericMessage
                {
                    DataType = DataType.Animation,
                    FileId = animation.FileId,
                },
                request.ChatId);
                return;
            }
        }

        var replyMessageId = default(int?);
        if (replyOption.ShouldReply)
        {
            replyMessageId = request.MessageId;
        }

        foreach (var message in replyOption.Messages)
        {
            await telegramMessageService.SendMessageAsync(message, request.ChatId, replyMessageId);
            await Task.Delay(TimeSpan.FromSeconds(0.5));
        }
    }

    private class SlapReplyOption : RandomService.IWeightedRaw
    {
        public SlapReplyOption(int weight, params string[] replies) : this(weight)
        {
            Messages = replies.Select(message => new GenericMessage
            {
                DataType = DataType.Text,
                Text = message
            })
                .ToList();
        }

        public SlapReplyOption(int weight)
        {
            Weight = weight;
            Messages = Enumerable.Empty<GenericMessage>();
        }

        public int Weight { get; init; }

        public IEnumerable<GenericMessage> Messages { get; init; }

        public bool ShouldReply { get; init; }

        public bool IsAnimation { get; init; }
    }
}
