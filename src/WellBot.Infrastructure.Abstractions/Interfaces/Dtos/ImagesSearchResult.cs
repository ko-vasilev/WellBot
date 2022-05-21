using System.Collections.Generic;

namespace WellBot.Infrastructure.Abstractions.Interfaces.Dtos
{
    /// <summary>
    /// Contains search result of images.
    /// </summary>
    public class ImagesSearchResult
    {
        /// <summary>
        /// Contains list of found images.
        /// </summary>
        public IEnumerable<ImageData> Images { get; set; }
    }
}
