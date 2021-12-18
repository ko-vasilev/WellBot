using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WellBot.Domain.Chats.Entities;
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
            SlapReplyOption.FromAnimation(MessageWeight.Normal, "CgACAgIAAx0CXXsy7AAD52G9owHWTIwekhpiN7lGOFy_evF1AAKmBAACQFygS6s18RHFx_BFIwQ"),
            SlapReplyOption.FromAnimation(MessageWeight.Normal, "CgACAgIAAx0CXXsy7AAD6GG9o5lGsp9V4H2KDjWRB7o5MfO5AAIUBwACgTVYSbul7RqQrg2MIwQ"),
            SlapReplyOption.FromAnimation(MessageWeight.Normal, "CgACAgIAAx0CXXsy7AAD6WG9o8UrTkq4-_9xg93abgynkUXIAAJHBAACIS54SonLR5pDRvz1IwQ"),
            SlapReplyOption.FromAnimation(MessageWeight.Normal, "CgACAgQAAx0CXXsy7AAD6mG9pDk0grma7Rah3_5RpYcF_UtaAAKOAgACiXlNUfB23c3XNxiUIwQ"),
            SlapReplyOption.FromAnimation(MessageWeight.Normal, "CgACAgQAAx0CXXsy7AAD7GG9pH0QGT2z4LDKrr8ndbvZlI2wAALHAgACnbUUUYW9K1XdO5tjIwQ", true),
            SlapReplyOption.FromAnimation(MessageWeight.Normal, "CgACAgQAAx0CXXsy7AAD7WG9pMD4CYIVCNBrKP6J2AmtzVI0AAJlAgACXoudUqu9IG82XNr0IwQ"),
            SlapReplyOption.FromAnimation(MessageWeight.Normal, "CgACAgIAAx0CXXsy7AAD7mG9pR9V-VPaNQZ-XU6xa8agG9EoAAKuEgACYnZ5SOZrkhwFC0BqIwQ"),
        };

        private readonly TelegramMessageService telegramMessageService;
        private readonly RandomService randomService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SlapCommandHandler(TelegramMessageService telegramMessageService, RandomService randomService)
        {
            this.telegramMessageService = telegramMessageService;
            this.randomService = randomService;
        }

        /// <inheritdoc/>
        protected override async Task Handle(SlapCommand request, CancellationToken cancellationToken)
        {
            var replyOption = randomService.PickWeighted(replies);
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
