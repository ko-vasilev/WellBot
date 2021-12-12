using AutoMapper;
using WellBot.Domain.Chats.Entities;
using WellBot.UseCases.Chats.Pidor.GetPidorGameMessages;

namespace WellBot.UseCases.Chats
{
    /// <summary>
    /// Mapping profile for Chat entities.
    /// </summary>
    public class ChatMappingProfile : Profile
    {
        /// <summary>
        /// Constructor, registers mapping settings.
        /// </summary>
        public ChatMappingProfile()
        {
            CreateMap<PidorMessage, PidorGameMessageDto>();
        }
    }
}
