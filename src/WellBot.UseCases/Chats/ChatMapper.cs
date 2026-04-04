using Riok.Mapperly.Abstractions;
using WellBot.Domain.Chats;
using WellBot.UseCases.Chats.AutomaticMessages.GetAutomaticMessages;
using WellBot.UseCases.Chats.Data.SearchData;
using WellBot.UseCases.Chats.Topics.GetTopicList;
using WellBot.UseCases.Chats.Topics.UpsertTopic;

namespace WellBot.UseCases.Chats;

/// <summary>
/// Mapper for chat-related models.
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ChatMapper
{
    internal partial AutomaticMessageDto ToAutomaticMessageDto(AutomaticMessageTemplate template);

    internal partial DataItem ToDataItem(ChatData data);

    internal partial TopicDto ToTopicDto(PassiveTopic topic);

    internal partial IQueryable<AutomaticMessageDto> ProjectToAutomaticMessageDtos(IQueryable<AutomaticMessageTemplate> query);

    internal partial IQueryable<DataItem> ProjectToDataItems(IQueryable<ChatData> query);

    internal partial IQueryable<TopicDto> ProjectToTopicDtos(IQueryable<PassiveTopic> query);

    [MapperIgnoreTarget(nameof(PassiveTopic.Id))]
    [MapperIgnoreTarget(nameof(PassiveTopic.ReplyOptions))]
    internal partial void UpdatePassiveTopic(UpsertTopicCommand source, [MappingTarget] PassiveTopic target);

    [UserMapping(Default = true)]
    private static string MapNullableString(string? value) => value!;
}
