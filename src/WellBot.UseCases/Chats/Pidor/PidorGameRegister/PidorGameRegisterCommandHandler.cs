using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace WellBot.UseCases.Chats.Pidor.PidorGameRegister
{
    /// <summary>
    /// Handler for <see cref="PidorGameRegisterCommand"/>.
    /// </summary>
    internal class PidorGameRegisterCommandHandler : AsyncRequestHandler<PidorGameRegisterCommand>
    {
        /// <inheritdoc/>
        protected override async Task Handle(PidorGameRegisterCommand request, CancellationToken cancellationToken)
        {
        }
    }
}
