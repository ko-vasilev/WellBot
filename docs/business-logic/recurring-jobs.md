# Recurring Jobs

## Behavior: `JOB-REGISTRATION`

- `Surface`: startup recurring-job registration
- `Trigger`: application startup
- `Inputs`: Hangfire recurring job manager
- `Preconditions`: application startup reaches `RecurringJobInitializer`
- `Main Flow`:
  - Register hourly automatic-message sending.
  - Register daily message-log cleanup.
  - Trigger the automatic-message job once immediately after registration.
- `Outputs / Side Effects`: jobs become active without waiting for the next scheduler cycle
- `Persistence`: none directly
- `Edge Cases`:
  - Immediate trigger means automatic messages can run during startup if they pass their own time-window checks.
- `Source References`:
  - `src/WellBot.Web/Infrastructure/RecurringJobs/RecurringJobInitializer.cs`

## Behavior: `JOB-SEND-AUTOMATIC-MESSAGES-WINDOW`

- `Surface`: hourly recurring job
- `Trigger`: `SendAutomaticMessages.SendAsync`
- `Inputs`: current UTC hour
- `Preconditions`: current UTC hour must be between `06:00` inclusive and `18:00` exclusive
- `Main Flow`:
  - Treat the allowed window as `10:00` through `21:59` in GMT+4 by subtracting 4 hours from local targets.
  - Skip execution outside that window.
  - Invoke `SendAutomaticMessagesCommand` inside the window.
- `Outputs / Side Effects`: enables template-driven Telegram sends during the allowed hours only
- `Persistence`: none directly
- `Edge Cases`:
  - The time-window rule is a hard-coded GMT+4 approximation, not a timezone-aware conversion.
- `Source References`:
  - `src/WellBot.Web/Infrastructure/RecurringJobs/SendAutomaticMessages.cs`

## Behavior: `JOB-SEND-AUTOMATIC-MESSAGES`

- `Surface`: recurring command handler
- `Trigger`: `SendAutomaticMessagesCommand`
- `Inputs`: current UTC time, automatic message templates, optional image search results
- `Preconditions`: template `LastTriggeredDate.Date` must be before the current UTC date, and its cron schedule must have an occurrence between `LastTriggeredDate` and now
- `Main Flow`:
  - Load all templates not yet triggered today.
  - For each template, compute the next cron occurrence after `LastTriggeredDate`.
  - Skip templates whose next occurrence is absent or still in the future.
  - If the template has no image search query, send the text message directly.
  - Otherwise search for images, choose one at random, and try up to 3 candidate URLs.
  - If all image attempts fail or no images are available, fall back to text-only sending.
  - After a send attempt succeeds, update `LastTriggeredDate` to `now`.
- `Outputs / Side Effects`: Telegram messages or photos sent to configured chats
- `Persistence`:
  - Reads and updates `AutomaticMessageTemplate`
- `Edge Cases`:
  - `LastTriggeredDate` is updated after the template send succeeds, not before.
  - The handler processes at most one send per template per UTC date because of the initial date filter.
  - Image retries handle Telegram send failures, not image search failures separately.
- `Source References`:
  - `src/WellBot.UseCases/Chats/AutomaticMessages/SendAutomaticMessages/SendAutomaticMessagesCommandHandler.cs`

## Behavior: `JOB-CLEANUP-MESSAGE-LOGS`

- `Surface`: daily recurring job
- `Trigger`: `CleanupMessageLogs.CleanupAsync`
- `Inputs`: current UTC time
- `Preconditions`: none
- `Main Flow`:
  - Compute `DateTime.UtcNow.AddDays(-1)`.
  - Delete all `MessageLog` rows with `MessageDate` older than or equal to that threshold.
- `Outputs / Side Effects`: recap history older than one day is removed
- `Persistence`:
  - Deletes `MessageLog`
- `Edge Cases`:
  - Retention is strict UTC-based age, not per-chat local date.
- `Source References`:
  - `src/WellBot.Web/Infrastructure/RecurringJobs/CleanupMessageLogs.cs`

## Behavior: `STARTUP-CHAT-CACHES`

- `Surface`: startup initializer
- `Trigger`: application startup
- `Inputs`: database state
- `Preconditions`: database access succeeds
- `Main Flow`:
  - Apply EF Core migrations.
  - Load current meme-channel configuration into `MemeChannelService`.
  - Load all passive topics into the in-memory `PassiveTopicService` cache.
  - Remove newline characters from any persisted `ChatData.Key` values and save the cleanup.
- `Outputs / Side Effects`: runtime caches are hydrated and malformed chat-data keys are normalized
- `Persistence`:
  - Reads `MemeChannelInfo`, `PassiveTopic`, and `ChatData`
  - Updates `ChatData`
- `Edge Cases`:
  - Cache state at runtime depends on this startup initializer and later explicit refreshes from admin/API topic updates.
- `Source References`:
  - `src/WellBot.Web/Infrastructure/Startup/DatabaseInitializer.cs`
