using System;
using MediatR;

namespace WellBot.UseCases.Chats.Pidor.PidorGameRegister
{
    /// <summary>
    /// Registers user in a Daily Pidor game.
    /// </summary>
    public record PidorGameRegisterCommand : IRequest
    {
    }
}
