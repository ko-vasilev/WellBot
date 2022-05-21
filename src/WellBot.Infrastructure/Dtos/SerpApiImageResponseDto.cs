using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WellBot.Infrastructure.Dtos
{
    /// <summary>
    /// Contains images API reply.
    /// </summary>
    internal class SerpApiImageResponseDto
    {
        /// <summary>
        /// Found images.
        /// </summary>
        [JsonPropertyName("images_results")]
        public IEnumerable<SerpApiImageResultDto> Images { get; set; }
    }
}
