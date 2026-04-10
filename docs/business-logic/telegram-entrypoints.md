# Telegram Entrypoints

## Behavior: `ENTRY-WEBHOOK-UPDATE`

- `Surface`: Telegram webhook
- `Trigger`: `POST` to `BotController.Telegram`
- `Inputs`: Telegram `Update`
- `Preconditions`: none
- `Main Flow`:
  - Forward the update to `HandleTelegramActionCommand`.
  - Return `200 OK` regardless of command output.
- `Outputs / Side Effects`: asynchronous bot behavior happens inside MediatR handlers
- `Persistence`: none at controller level
- `Edge Cases`:
  - Controller does not validate update subtype beyond model binding.
- `Source References`:
  - `src/WellBot.Web/Controllers/BotController.cs`

## Behavior: `ENTRY-HANDLE-UPDATE`

- `Surface`: Telegram update router
- `Trigger`: `HandleTelegramActionCommandHandler`
- `Inputs`: inline query, message, chat type, sender, formatted text, bot username
- `Preconditions`: update must contain either an inline query or a message
- `Main Flow`:
  - Handle inline queries first; if present, stop.
  - Ignore updates without `Message`.
  - Ignore private chats entirely.
  - If the message came from the configured meme channel, update the stored latest meme message id and stop.
  - Extract plain text from `Text` or `Caption`.
  - Parse slash commands, including `@botusername` suffixes.
  - Route known commands to their handlers.
  - If the message is not a command, publish a fire-and-forget `MessageNotification`.
  - On exception, log it and send a generic failure message to the chat if chat id is known.
- `Outputs / Side Effects`:
  - Telegram command execution
  - Inline query answers
  - Meme channel state update
  - Background notification publication
- `Persistence`:
  - Reads and writes `MemeChannelInfo` for meme-channel messages
- `Edge Cases`:
  - Direct messages are ignored before command parsing, so normal private-chat command usage is disabled.
  - `@botusername` suffix marks a command as direct-command context even in groups.
  - Unknown commands receive a reply only in direct-command context.
  - Notification publishing is fire-and-forget; failures are logged but not surfaced to Telegram.
- `Source References`:
  - `src/WellBot.UseCases/Chats/HandleTelegramAction/HandleTelegramActionCommandHandler.cs`

## Behavior: `ENTRY-PARSE-COMMAND`

- `Surface`: Telegram command parser
- `Trigger`: message text or caption starts with `/`
- `Inputs`: raw text, HTML-formatted text, configured Telegram bot username
- `Preconditions`: input starts with `/`
- `Main Flow`:
  - Split command name from arguments on the first space only.
  - Preserve raw and HTML-formatted argument variants.
  - Remove `@botusername` suffix from the command when present.
  - Remove the leading slash before dispatch.
- `Outputs / Side Effects`: command name plus raw and HTML argument payloads
- `Persistence`: none
- `Edge Cases`:
  - Newline is not treated as a command delimiter here; only the first space splits the command token.
  - Non-command text returns an empty command string and keeps the full message as arguments.
- `Source References`:
  - `src/WellBot.UseCases/Chats/HandleTelegramAction/HandleTelegramActionCommandHandler.cs`

## Behavior: `ENTRY-INLINE-SEARCH`

- `Surface`: Telegram inline query
- `Trigger`: update contains `InlineQuery`
- `Inputs`: inline query text
- `Preconditions`: none
- `Main Flow`:
  - Run `SearchDataQuery`.
  - Map matching saved chat data to Telegram inline result types by stored `DataType`.
  - Answer the inline query with the mapped items.
- `Outputs / Side Effects`: inline results using cached Telegram file ids or HTML text content
- `Persistence`:
  - Reads `ChatData`
- `Edge Cases`:
  - Empty query text returns no results.
  - `Reaction` and unsupported data types are skipped because they do not map to inline result objects.
  - Errors are logged and swallowed.
- `Source References`:
  - `src/WellBot.UseCases/Chats/HandleTelegramAction/HandleTelegramActionCommandHandler.cs`
  - `src/WellBot.UseCases/Chats/Data/SearchData/SearchDataQueryHandler.cs`

## Behavior: `ENTRY-MEME-CHANNEL-TRACKING`

- `Surface`: Telegram message short-circuit
- `Trigger`: inbound message from the configured meme channel id
- `Inputs`: message chat id, message id, cached current meme channel id
- `Preconditions`: `MemeChannelService.CurrentMemeChatId` is set and matches the message chat id
- `Main Flow`:
  - Load the stored meme-channel record.
  - Replace its `LatestMessageId` with the current message id.
  - Stop all other message handling.
- `Outputs / Side Effects`: future `/ememe` requests can sample from a larger message id range
- `Persistence`:
  - Writes `MemeChannelInfo.LatestMessageId`
- `Edge Cases`:
  - Messages in the meme channel are not processed as commands or notifications.
  - The handler expects the meme-channel row to exist once the cache points to a channel.
- `Source References`:
  - `src/WellBot.UseCases/Chats/HandleTelegramAction/HandleTelegramActionCommandHandler.cs`
  - `src/WellBot.UseCases/Chats/MemeChannelService.cs`

## Behavior: `ENTRY-CHAT-CONTEXT-SETUP`

- `Surface`: MediatR pipeline behavior
- `Trigger`: request implements `IChatInfo`
- `Inputs`: Telegram chat id from command/query
- `Preconditions`: `ChatId` must be non-default
- `Main Flow`:
  - Ensure a `Chat` row exists for the Telegram chat id.
  - Store the internal `Chat.Id` on `CurrentChatService`.
  - Continue handler execution.
- `Outputs / Side Effects`: handlers can use internal chat ids without repeating lookup logic
- `Persistence`:
  - Reads or creates `Chat`
- `Edge Cases`:
  - Requests with default `ChatId` only log a warning.
- `Source References`:
  - `src/WellBot.Web/Infrastructure/Telegram/TelegramChatSetterPipelineBehavior.cs`
  - `src/WellBot.DomainServices/Chats/CurrentChatService.cs`
