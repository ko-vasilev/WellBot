using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WellBot.Domain.Chats.Entities;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.UseCases.Chats.Dtos;

namespace WellBot.UseCases.Chats.Slap
{
    /// <summary>
    /// Handler for <see cref="SlapCommand"/>.
    /// </summary>
    internal class SlapCommandHandler : AsyncRequestHandler<SlapCommand>
    {
        private static readonly IReadOnlyCollection<SlapReplyOption> replies = new List<SlapReplyOption>
        {
            new SlapReplyOption(MessageWeight.High, "Нихуя себе блять"),
            new SlapReplyOption(MessageWeight.High, "Нихуя соби блять", "Уебал..."),
            new SlapReplyOption(MessageWeight.Normal, "Нихуя себе блять", "Ты въебал мне 😥"),
            new SlapReplyOption(MessageWeight.Normal, "👊")
            {
                ShouldReply = true
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
            var animationOptions = await dbContext.SlapOptions.ToListAsync(cancellationToken);
            var allOptions = animationOptions.Select(opt => SlapReplyOption.FromAnimation(MessageWeight.Normal, opt.FileId))
                .ToList();
            allOptions.AddRange(replies);

            var replyOption = randomService.PickWeighted(allOptions);
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

        private class SlapReplyOption : RandomService.IWeighted
        {
            public SlapReplyOption(MessageWeight weight, params string[] replies) : this(weight)
            {
                Messages = replies.Select(message => new GenericMessage
                {
                    DataType = DataType.Text,
                    Text = message
                })
                    .ToList();
            }

            public SlapReplyOption(MessageWeight weight)
            {
                Weight = weight;
            }

            public static SlapReplyOption FromAnimation(MessageWeight weight, string fileId, bool isReply = false) =>
                new SlapReplyOption(weight)
                {
                    Messages = new List<GenericMessage>
                    {
                        new GenericMessage
                        {
                            DataType = DataType.Animation,
                            FileId = fileId
                        }
                    },
                    ShouldReply = isReply
                };

            public MessageWeight Weight { get; init; }

            public IEnumerable<GenericMessage> Messages { get; init; }

            public bool ShouldReply { get; init; }
        }
    }
}
