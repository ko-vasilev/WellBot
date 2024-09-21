namespace WellBot.UseCases.Chats.Summarization;

/// <summary>
/// Settings for OpenAI.
/// </summary>
public class OpenAiSettings
{
    /// <summary>
    /// API key for OpenAI.
    /// </summary>
    public required string OpenaiKey { get; set; }
}
