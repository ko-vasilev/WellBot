using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace WellBot.Web.Infrastructure.Web;

/// <summary>
/// System JSON helper that implements <see cref="IJsonHelper" /> interface.
/// </summary>
/// <remarks>
/// Source:
/// https://github.com/dotnet/aspnetcore/blob/master/src/Mvc/Mvc.ViewFeatures/src/Rendering/SystemTextJsonHelper.cs .
/// </remarks>
internal class SystemTextJsonHelper : IJsonHelper
{
    private readonly JsonSerializerOptions htmlSafeJsonSerializerOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">JSON options.</param>
    public SystemTextJsonHelper(IOptions<JsonOptions> options)
    {
        htmlSafeJsonSerializerOptions = GetHtmlSafeSerializerOptions(options.Value.SerializerOptions);
    }

    /// <inheritdoc />
    public IHtmlContent Serialize(object value)
    {
        // JsonSerializer always encodes non-ASCII chars, so we do not need
        // to do anything special with the SerializerOptions
        var json = JsonSerializer.Serialize(value, htmlSafeJsonSerializerOptions);
        return new HtmlString(json);
    }

    private static JsonSerializerOptions GetHtmlSafeSerializerOptions(JsonSerializerOptions serializerOptions)
    {
        if (serializerOptions.Encoder is null || serializerOptions.Encoder == JavaScriptEncoder.Default)
        {
            return serializerOptions;
        }

        return new JsonSerializerOptions(serializerOptions)
        {
            Encoder = JavaScriptEncoder.Default,
        };
    }
}
