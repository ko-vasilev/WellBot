using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WellBot.Infrastructure.Abstractions.Interfaces;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace WellBot.Infrastructure
{
    /// <summary>
    /// Contains logic for converting between video formats.
    /// </summary>
    public class VideoConverter : IVideoConverter
    {
        /// <inheritdoc/>
        public async Task<Stream> ConvertWebmToMp4Async(Stream inputStream, CancellationToken cancellationToken)
        {
            var mediaFilePath = Path.ChangeExtension(Path.GetTempFileName(), "webm");
            var mp4FilePath = Path.ChangeExtension(mediaFilePath, "mp4");
            try
            {
                await using (var fileStream = new FileStream(mediaFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    inputStream.Position = 0;
                    await inputStream.CopyToAsync(fileStream, cancellationToken);
                    await fileStream.FlushAsync(cancellationToken);
                }

                var conversion = await FFmpeg.Conversions.FromSnippet.ToMp4(mediaFilePath, mp4FilePath);
                var result = await conversion.Start(cancellationToken);

                return new AutoCleanupFileStream(mp4FilePath);
            }
            finally
            {
                if (File.Exists(mediaFilePath))
                {
                    File.Delete(mediaFilePath);
                }
            }
        }

        /// <inheritdoc/>
        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            var ffmpegDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FFmpeg");
            Directory.CreateDirectory(ffmpegDirectory);

            FFmpeg.SetExecutablesPath(ffmpegDirectory);
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, ffmpegDirectory);
        }
    }
}
