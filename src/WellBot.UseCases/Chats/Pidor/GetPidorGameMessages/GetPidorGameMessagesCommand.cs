using MediatR;

namespace WellBot.UseCases.Chats.Pidor.GetPidorGameMessages;

/// <summary>
/// Get list of all pidor game messages.
/// </summary>
public class GetPidorGameMessagesCommand : IRequest<IEnumerable<PidorGameMessageDto>>
{
}
