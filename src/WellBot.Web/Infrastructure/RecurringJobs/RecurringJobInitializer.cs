using Extensions.Hosting.AsyncInitialization;
using Hangfire;

namespace WellBot.Web.Infrastructure.RecurringJobs;

/// <summary>
/// Initializes recurring jobs.
/// </summary>
public class RecurringJobInitializer : IAsyncInitializer
{
    /// <summary>
    /// Id of the background job that will be automatically sending messages in telegram.
    /// </summary>
    private const string SendAutomaticMessagesJobId = "SendAutomaticMessages";

    private readonly IRecurringJobManager recurringJobManager;

    /// <summary>
    /// Constructor.
    /// </summary>
    public RecurringJobInitializer(IRecurringJobManager recurringJobManager)
    {
        this.recurringJobManager = recurringJobManager;
    }

    /// <inheritdoc />
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task InitializeAsync(CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        recurringJobManager.AddOrUpdate<SendAutomaticMessages>(SendAutomaticMessagesJobId, job => job.SendAsync(CancellationToken.None), Cron.Hourly);

        recurringJobManager.Trigger(SendAutomaticMessagesJobId);
    }
}
