using System.Text.Json.Serialization;

namespace WellBot.Infrastructure.Dtos;

/// <summary>
/// Contains information about an image.
/// </summary>
internal class SerpApiImageResultDto
{
    /// <summary>
    /// Image thumbnail.
    /// </summary>
    [JsonPropertyName("thumbnail")]
    public string? Thumbnail { get; set; }

    /// <summary>
    /// Website where this image is located.
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>
    /// Image title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Original image URL.
    /// </summary>
    [JsonPropertyName("original")]
    public string? Original { get; set; }
}
