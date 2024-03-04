using MediatR;

namespace WellBot.UseCases.Chats.Topics.GetTopicList;

/// <summary>
/// Get all existing topics.
/// </summary>
public class GetTopicListQuery : IRequest<IEnumerable<TopicDto>>
{
}
