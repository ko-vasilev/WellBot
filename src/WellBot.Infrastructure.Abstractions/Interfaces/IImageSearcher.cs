using System.Threading;
using System.Threading.Tasks;
using WellBot.Infrastructure.Abstractions.Interfaces.Dtos;

namespace WellBot.Infrastructure.Abstractions.Interfaces
{
    /// <summary>
    /// Contains logic for searching for images.
    /// </summary>
    public interface IImageSearcher
    {
        /// <summary>
        /// Search for images.
        /// </summary>
        /// <param name="term">Search term.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Found images.</returns>
        Task<ImagesSearchResult> SearchAsync(string term, CancellationToken cancellationToken);
    }
}
