using System.Threading;
using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using Microsoft.Extensions.Logging;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.Web.Infrastructure.Startup
{
    /// <summary>
    /// Class for initializing video conversion service.
    /// </summary>
    public class VideoConversionInitializer : IAsyncInitializer
    {
        private readonly IVideoConverter videoConverter;
        private readonly ILogger<VideoConversionInitializer> logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public VideoConversionInitializer(IVideoConverter videoConverter, ILogger<VideoConversionInitializer> logger)
        {
            this.videoConverter = videoConverter;
            this.logger = logger;
        }

        /// <summary>
        /// Initialize the video converter.
        /// </summary>
        public async Task InitializeAsync()
        {
            logger.LogInformation("Starting video converter initialization...");
            await videoConverter.InitializeAsync(CancellationToken.None);
            logger.LogInformation("Finished video converter initialization.");
        }
    }
}
