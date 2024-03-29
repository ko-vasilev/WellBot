﻿using WellBot.Infrastructure.Abstractions.Interfaces;
using WellBot.Infrastructure.Abstractions.Interfaces.Dtos;
using WellBot.Infrastructure.Dtos;

namespace WellBot.Infrastructure;

/// <summary>
/// Settings for the google image searcher.
/// </summary>
public class GoogleImageSearcherSettings
{
    /// <summary>
    /// API key for the SerpApi service.
    /// </summary>
    public required string ApiKey { get; set; }
}

/// <summary>
/// Implements image search in Google.
/// </summary>
public class GoogleImageSearcher : IImageSearcher
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly GoogleImageSearcherSettings settings;

    /// <summary>
    /// Constructor.
    /// </summary>
    public GoogleImageSearcher(IHttpClientFactory httpClientFactory, GoogleImageSearcherSettings settings)
    {
        this.httpClientFactory = httpClientFactory;
        this.settings = settings;
    }

    /// <inheritdoc />
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

        var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        queryString.Add("engine", "google");
        queryString.Add("q", term ?? string.Empty);
        queryString.Add("api_key", settings.ApiKey);
        queryString.Add("tbm", "isch");
        // Exclude autocorrect results
        queryString.Add("nfpr", "1");

        var queryUrl = "https://serpapi.com/search.json?" + queryString.ToString();
        var result = await httpClient.GetAsync(queryUrl, cancellationToken);
        var json = await result.Content.ReadAsStringAsync(cancellationToken);

        var images = System.Text.Json.JsonSerializer.Deserialize<SerpApiImageResponseDto>(json);
        if (images?.Images == null)
        {
            return new ImagesSearchResult
            {
                Images = Enumerable.Empty<ImageData>()
            };
        }

        return new ImagesSearchResult
        {
            Images = images.Images
                .Where(image => image.Original != null && image.Title != null)
                .Select(image => new ImageData
                {
                    Source = image.Source,
                    Title = image.Title!,
                    Url = image.Original!
                })
        };
    }
}
