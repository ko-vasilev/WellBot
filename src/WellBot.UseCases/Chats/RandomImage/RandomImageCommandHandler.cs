using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Telegram.Bot;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.RandomImage
{
    /// <summary>
    /// Handler for <see cref="RandomImageCommand"/>.
    /// </summary>
    internal class RandomImageCommandHandler : AsyncRequestHandler<RandomImageCommand>
    {
        private readonly IImageSearcher imageSearcher;
        private readonly TelegramMessageService telegramMessageService;
        private readonly ITelegramBotClient botClient;

        public RandomImageCommandHandler(TelegramMessageService telegramMessageService, IImageSearcher imageSearcher, ITelegramBotClient botClient)
        {
            this.telegramMessageService = telegramMessageService;
            this.imageSearcher = imageSearcher;
            this.botClient = botClient;
        }

        /// <inheritdoc/>
        protected override async Task Handle(RandomImageCommand request, CancellationToken cancellationToken)
        {
            var caption = "С днём рождения Арташес";
            var imageResult = await imageSearcher.SearchAsync(caption, cancellationToken);

            var images = imageResult.Images.ToList();

            var image = images.First();
            await botClient.SendPhotoAsync(request.ChatId, new Telegram.Bot.Types.InputFiles.InputOnlineFile(image.Url), caption, cancellationToken: cancellationToken);

            await telegramMessageService.SendSuccessAsync(request.ChatId);
        }
    }
}
