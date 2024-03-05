using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace WellBot.Web.Infrastructure.Startup;

/// <summary>
/// JSON options setup.
/// </summary>
internal class JsonOptionsSetup
{
    /// <summary>
    /// Setup method.
    /// </summary>
    /// <param name="options">JSON options.</param>
    public void Setup(JsonOptions options)
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
}
