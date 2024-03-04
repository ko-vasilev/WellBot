using WellBot.Domain.Chats;

namespace WellBot.UseCases.Chats.Data.SearchData;

/// <summary>
/// Contains information about a saved data.
/// </summary>
public class DataItem
{
    /// <inheritdoc cref="ChatData.Id"/>
    public int Id { get; set; }

    /// <inheritdoc cref="ChatData.DataType"/>
    public DataType DataType { get; set; }

    /// <inheritdoc cref="ChatData.FileId"/>
    public required string FileId { get; set; }

    /// <inheritdoc cref="ChatData.Text"/>
    public required string Text { get; set; }

    /// <inheritdoc cref="ChatData.Key"/>
    public required string Key { get; set; }
}
