using MediatR;
using Saritasa.Tools.Domain.Exceptions;
using WellBot.Domain.Chats;
using WellBot.Infrastructure.Abstractions.Interfaces;

namespace WellBot.UseCases.Chats.AutomaticMessages.CreateAutomaticMessage;

/// <summary>
/// Handler for <see cref="CreateAutomaticMessageCommand"/>.
/// </summary>
internal class CreateAutomaticMessageCommandHandler : IRequestHandler<CreateAutomaticMessageCommand, int>
{
    private readonly IAppDbContext dbContext;

    /// <summary>
    /// Constructor.
    /// </summary>
    public CreateAutomaticMessageCommandHandler(IAppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <inheritdoc/>
    public async Task<int> Handle(CreateAutomaticMessageCommand request, CancellationToken cancellationToken)
    {
        // Validate the Cron format
        try
        {
            Cronos.CronExpression.Parse(request.CronInterval);
        }
        catch (Cronos.CronFormatException ex)
        {
            throw new ValidationException($"Invalid Cron format in field {nameof(request.CronInterval)}", ex);
        }

        var messageTemplate = new AutomaticMessageTemplate()
        {
            ChatId = request.ChatId,
            CronInterval = request.CronInterval,
            ImageSearchQuery = request.ImageSearchQuery,
            LastTriggeredDate = request.RunFrom,
            Message = request.Message,
        };
        dbContext.AutomaticMessageTemplates.Add(messageTemplate);

        await dbContext.SaveChangesAsync(cancellationToken);

        return messageTemplate.Id;
    }
}
