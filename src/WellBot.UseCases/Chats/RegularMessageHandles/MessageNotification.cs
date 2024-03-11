using MediatR;
using Telegram.BotAPI.AvailableTypes;

namespace WellBot.UseCases.Chats.RegularMessageHandles;

/// <summary>
/// Notifies about a regular message received.
/// </summary>
public record MessageNotification : INotification
{
    /// <summary>
    /// Message data.
    /// </summary>
    public required Message Message { get; init; }
}
