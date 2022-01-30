using AutoMapper;
using WellBot.Domain.Chats.Entities;
using WellBot.UseCases.Chats.Data.SearchData;
using WellBot.UseCases.Chats.Pidor.GetPidorGameMessages;
using WellBot.UseCases.Chats.Topics.GetTopicList;
using WellBot.UseCases.Chats.Topics.UpsertTopic;

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
            CreateMap<ChatData, DataItem>();
            CreateMap<PassiveTopic, TopicDto>();
            CreateMap<UpsertTopicCommand, PassiveTopic>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
