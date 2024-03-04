namespace WellBot.Infrastructure.Abstractions.Interfaces;

/// <summary>
/// Contains logic for converting between different video formats.
/// </summary>
public interface IVideoConverter
{
    /// <summary>
    /// Convert a Webm file to an Mp4 file.
    /// </summary>
    /// <param name="inputStream">Webm file stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream with the data.</returns>
    Task<Stream> ConvertWebmToMp4Async(Stream inputStream, CancellationToken cancellationToken);

    /// <summary>
    /// Initialize the video converter.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task InitializeAsync(CancellationToken cancellationToken);
}
