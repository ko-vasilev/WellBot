using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.Infrastructure.Abstractions.Interfaces.Dtos;
using WellBot.Infrastructure.Dtos;

namespace WellBot.Infrastructure
{
    /// <summary>
    /// Settings for the google image searcher
    /// </summary>
    public class GoogleImageSearcherSettings
    {
        /// <summary>
        /// API key for the SerpApi service.
        /// </summary>
        public string ApiKey { get; set; }
    }

    public class GoogleImageSearcher : IImageSearcher
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly GoogleImageSearcherSettings settings;

        public GoogleImageSearcher(IHttpClientFactory httpClientFactory, GoogleImageSearcherSettings settings)
        {
            this.httpClientFactory = httpClientFactory;
            this.settings = settings;
        }

        public async Task<ImagesSearchResult> SearchAsync(string term, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(term))
            {
                return new ImagesSearchResult
                {
                    Images = Enumerable.Empty<ImageData>()
                };
            }

            using var httpClient = httpClientFactory.CreateClient();
            // Using the SerpApi for image search
            // Documentation https://serpapi.com/search-api

            var queryBuilder = new QueryBuilder();
            queryBuilder.Add("engine", "google");
            queryBuilder.Add("q", term ?? string.Empty);
            queryBuilder.Add("api_key", settings.ApiKey);
            queryBuilder.Add("tbm", "isch");
            // Exclude autocorrect results
            queryBuilder.Add("nfpr", "1");

            var queryUrl = "https://serpapi.com/search.json" + queryBuilder.ToString();
            var result = await httpClient.GetAsync(queryUrl, cancellationToken);
            var json = await result.Content.ReadAsStringAsync(cancellationToken);

            var images = System.Text.Json.JsonSerializer.Deserialize<SerpApiImageResponseDto>(json);

            return new ImagesSearchResult
            {
                Images = images.Images.Select(image => new ImageData
                {
                    Source = image.Source,
                    Title = image.Title,
                    Url = image.Original
                })
            };
        }
    }
}
