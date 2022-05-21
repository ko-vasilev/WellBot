using System;
using MediatR;
using Telegram.Bot.Types;

namespace WellBot.UseCases.Chats.RandomImage
{
    public class RandomImageCommand : IRequest
    {
        /// <summary>
        /// Id of the chat.
        /// </summary>
        public ChatId ChatId { get; init; }
    }
}
