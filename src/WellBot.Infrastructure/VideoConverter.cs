using Microsoft.Extensions.Logging;
using WellBot.Infrastructure.Abstractions.Interfaces;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace WellBot.Infrastructure;

/// <summary>
/// Contains logic for converting between video formats.
/// </summary>
public class VideoConverter : IVideoConverter
{
    private readonly ILogger<VideoConverter> logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    public VideoConverter(ILogger<VideoConverter> logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Stream> ConvertWebmToMp4Async(Stream inputStream, CancellationToken cancellationToken)
    {
        var tempFile = Path.GetTempFileName();
        var mediaFilePath = Path.ChangeExtension(tempFile, "webm");
        var mp4FilePath = Path.ChangeExtension(mediaFilePath, "mp4");
        try
        {
            logger.LogInformation("Converting webm to mp4, saving file to a temporary directory");
            await using (var fileStream = new FileStream(mediaFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                inputStream.Position = 0;
                await inputStream.CopyToAsync(fileStream, cancellationToken);
                await fileStream.FlushAsync(cancellationToken);
            }

            logger.LogInformation("Starting conversion");
            var conversion = await FFmpeg.Conversions.FromSnippet.ToMp4(mediaFilePath, mp4FilePath);
            conversion.OnProgress += (sender, args) => logger.LogDebug("Conversion progress {percent}%", args.Percent);
            var result = await conversion.Start(cancellationToken);
            logger.LogInformation("Finished conversion in {duration}", result.Duration);

            return new AutoCleanupFileStream(mp4FilePath);
        }
        finally
        {
            if (File.Exists(mediaFilePath))
            {
                File.Delete(mediaFilePath);
                File.Delete(tempFile);
            }
        }
    }

    private void Conversion_OnProgress(object sender, Xabe.FFmpeg.Events.ConversionProgressEventArgs args) => throw new NotImplementedException();

    /// <inheritdoc/>
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var ffmpegDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FFmpeg");
        Directory.CreateDirectory(ffmpegDirectory);

        FFmpeg.SetExecutablesPath(ffmpegDirectory);
        await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ffmpegDirectory);
    }
}
