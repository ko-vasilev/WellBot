# Admin And API

## Behavior: `API-TOPICS-LIST`

- `Surface`: `GET /api/chat/topics`
- `Trigger`: authenticated HTTP request
- `Inputs`: none
- `Preconditions`: caller must be authorized
- `Main Flow`:
  - Return passive topic DTOs.
- `Outputs / Side Effects`: list response
- `Persistence`:
  - Reads `PassiveTopic`
- `Edge Cases`:
  - Topic DTOs are the management surface for passive reply routing.
- `Source References`:
  - `src/WellBot.Web/Controllers/ChatController.cs`
  - `src/WellBot.UseCases/Chats/Topics/GetTopicList/GetTopicListQueryHandler.cs`

## Behavior: `API-TOPIC-UPSERT`

- `Surface`: `POST /api/chat/topic`
- `Trigger`: authenticated HTTP request
- `Inputs`: topic id, name, regex, probability, direct/meme/audio flags, exclusivity
- `Preconditions`: caller must be authorized
- `Main Flow`:
  - If `Id` is present, update the existing topic.
  - Otherwise create a new topic.
  - Save changes.
  - Reload the in-memory passive topic cache from the full topic table.
- `Outputs / Side Effects`: topic definition becomes active for future passive matching
- `Persistence`:
  - Writes `PassiveTopic`
  - Refreshes in-memory topic cache
- `Edge Cases`:
  - Updating a non-existent topic throws a not-found exception.
- `Source References`:
  - `src/WellBot.Web/Controllers/ChatController.cs`
  - `src/WellBot.UseCases/Chats/Topics/UpsertTopic/UpsertTopicCommandHandler.cs`

## Behavior: `API-AUTOMATIC-MESSAGES-LIST`

- `Surface`: `GET /api/chat/automatic-messages`
- `Trigger`: authenticated HTTP request
- `Inputs`: none
- `Preconditions`: caller must be authorized
- `Main Flow`:
  - Return all automatic message templates.
- `Outputs / Side Effects`: list response
- `Persistence`:
  - Reads `AutomaticMessageTemplate`
- `Edge Cases`:
  - Templates are not filtered by time or last-run status.
- `Source References`:
  - `src/WellBot.Web/Controllers/ChatController.cs`
  - `src/WellBot.UseCases/Chats/AutomaticMessages/GetAutomaticMessages/GetAutomaticMessagesCommandHandler.cs`

## Behavior: `API-AUTOMATIC-MESSAGE-CREATE`

- `Surface`: `POST /api/chat/automatic-message`
- `Trigger`: authenticated HTTP request
- `Inputs`: chat id, cron expression, run-from date, text, optional image search query
- `Preconditions`: caller must be authorized; cron expression must be valid
- `Main Flow`:
  - Validate the cron expression with `Cronos`.
  - Persist an `AutomaticMessageTemplate`.
- `Outputs / Side Effects`: new template becomes eligible for recurring sends
- `Persistence`:
  - Writes `AutomaticMessageTemplate`
- `Edge Cases`:
  - Invalid cron expressions raise validation errors.
  - `RunFrom` seeds `LastTriggeredDate`, which controls when the first recurring send becomes eligible.
- `Source References`:
  - `src/WellBot.Web/Controllers/ChatController.cs`
  - `src/WellBot.UseCases/Chats/AutomaticMessages/CreateAutomaticMessage/CreateAutomaticMessageCommandHandler.cs`

## Behavior: `API-AUTOMATIC-MESSAGE-DELETE`

- `Surface`: `DELETE /api/chat/automatic-message/{messageId}`
- `Trigger`: authenticated HTTP request
- `Inputs`: template id
- `Preconditions`: caller must be authorized
- `Main Flow`:
  - Load and delete the automatic message template.
- `Outputs / Side Effects`: template is no longer eligible for recurring sends
- `Persistence`:
  - Deletes `AutomaticMessageTemplate`
- `Edge Cases`:
  - Missing ids will fail through the entity fetch helper.
- `Source References`:
  - `src/WellBot.Web/Controllers/ChatController.cs`
  - `src/WellBot.UseCases/Chats/AutomaticMessages/DeleteAutomaticMessage/DeleteAutomaticMessageCommandHandler.cs`

## Behavior: `ADMIN-SUPERADMIN-CONTROLS`

- `Surface`: Telegram command `/admin`
- `Trigger`: superadmin Telegram user issues a supported subcommand
- `Inputs`: sender Telegram id, reply target, configured `SuperadminIds`
- `Preconditions`: sender must match configured superadmin ids
- `Main Flow`:
  - See `CMD-ADMIN` in `chat-commands.md` for the user-facing command behavior.
  - This surface is included here because it mutates reference data also managed through API endpoints.
- `Outputs / Side Effects`: updates slap options, passive reply data, meme channel, or cross-chat broadcast state
- `Persistence`:
  - Writes multiple chat-domain tables depending on subcommand
- `Edge Cases`:
  - Authorization is independent from HTTP auth and based solely on Telegram sender id.
- `Source References`:
  - `src/WellBot.UseCases/Chats/AdminControl/AdminControlCommandHandler.cs`
