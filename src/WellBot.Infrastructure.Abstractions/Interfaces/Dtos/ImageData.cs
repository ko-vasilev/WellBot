namespace WellBot.Infrastructure.Abstractions.Interfaces.Dtos;

/// <summary>
/// Contains information about an image.
/// </summary>
public class ImageData
{
    /// <summary>
    /// Website where this image is located.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Image title.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Url of the image.
    /// </summary>
    public required string Url { get; set; }
}
