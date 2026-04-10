# Message Behaviors

## Behavior: `MSG-PASSIVE-REPLY`

- `Surface`: message notification
- `Trigger`: non-command group message reaches `ReplyNotificationHandler`
- `Inputs`: message text, caption metadata, replied message, configured passive topics, random source
- `Preconditions`: at least one topic must match and the probability roll must pass
- `Main Flow`:
  - Ask `PassiveTopicService` for matching topics.
  - Choose the topic with the smallest numeric probability value.
  - Roll `GetRandom(probability)` and only continue when the result is `0`.
  - Gather all distinct reply option ids across matching topics.
  - Retry up to 5 times:
    - choose a random option id
    - load the reply option
    - send its stored text/media/reaction payload as a reply to the triggering message
- `Outputs / Side Effects`: sends a text, media file, or reaction back into the chat
- `Persistence`:
  - Reads `PassiveTopic` and `PassiveReplyOption`
- `Edge Cases`:
  - Lower probability numbers cause more frequent replies because success is `1 / probability`.
  - If a send fails, the handler logs a warning and retries with another random option.
  - The selection order uses the smallest numeric probability, which may not be the most permissive topic.
- `Source References`:
  - `src/WellBot.UseCases/Chats/RegularMessageHandles/Reply/ReplyNotificationHandler.cs`

## Behavior: `MSG-PASSIVE-TOPIC-MATCHING`

- `Surface`: topic matcher
- `Trigger`: `PassiveTopicService.GetMatchingTopics`
- `Inputs`: message text, media type, caption entities, reply target, configured bot username, cached topic definitions
- `Preconditions`: topics must have been loaded into memory during startup or after admin/API updates
- `Main Flow`:
  - Determine whether the message is direct-to-bot:
    - reply to the bot
    - mention of `@botusername` in message text
    - mention of `@botusername` in caption text
  - Determine whether the message is a meme (`Photo` or `Video`) or audio.
  - Evaluate topics in cache order, with exclusive topics loaded first.
  - Filter each topic by `IsDirect`, `IsMeme`, `IsAudio`, and regex match.
  - Add matching topics to the result.
  - Stop immediately when an exclusive topic matches.
- `Outputs / Side Effects`: ordered list of matching topic ids and probabilities
- `Persistence`: none at runtime; uses in-memory topic cache
- `Edge Cases`:
  - Regex matching checks `message.Text` only, not captions.
  - Audio detection looks only at `Audio`, not voice notes.
  - Exclusive topics prevent later topic matches from being considered.
- `Source References`:
  - `src/WellBot.UseCases/Chats/RegularMessageHandles/Reply/PassiveTopicService.cs`
  - `src/WellBot.Web/Infrastructure/Startup/DatabaseInitializer.cs`

## Behavior: `MSG-FIX-WEBM`

- `Surface`: message notification
- `Trigger`: non-command message contains `Document` with MIME type `video/webm`
- `Inputs`: Telegram file id, Telegram file URL, HTTP download stream, video converter
- `Preconditions`: document must be downloadable and convertible
- `Main Flow`:
  - Fetch the Telegram file path.
  - Download the source stream with `HttpClient`.
  - Convert WebM to MP4 through `IVideoConverter`.
  - Send the converted file back to the same chat as a reply with caption `Fixed`.
- `Outputs / Side Effects`: converted MP4 document is posted in chat
- `Persistence`: none
- `Edge Cases`:
  - Conversion errors are only logged.
  - Only `video/webm` documents are handled.
- `Source References`:
  - `src/WellBot.UseCases/Chats/RegularMessageHandles/FixFormat/FixFormatNotificationHandler.cs`

## Behavior: `MSG-LOG-FOR-RECAP`

- `Surface`: message notification
- `Trigger`: non-command message reaches `LogMessageNotificationHandler`
- `Inputs`: message content, message metadata, sender info, chat id
- `Preconditions`: none
- `Main Flow`:
  - Convert Telegram unix timestamp to UTC datetime.
  - Decide whether to log message content based on per-chat opt-out state for the sender.
  - Build a human-readable event description for supported message types.
  - Ensure a `Chat` row exists.
  - Persist a `MessageLog` row for every processed message, even when the detailed message body is null.
- `Outputs / Side Effects`: recap history is expanded
- `Persistence`:
  - Reads `MessageLogOptOut`
  - Reads or creates `Chat`
  - Writes `MessageLog`
- `Edge Cases`:
  - Unsupported message types produce a log row with `Message = null`.
  - Forwarded messages are prefixed as forwarded events.
  - Replies store a reference to the replied message id in the generated text.
  - Sender display name falls back to `somebody`.
- `Source References`:
  - `src/WellBot.UseCases/Chats/Summarization/LogMessageNotificationHandler.cs`

## Behavior: `MSG-MEME-CHANNEL-STATE`

- `Surface`: webhook short-circuit for meme-channel messages
- `Trigger`: any message in the configured meme channel
- `Inputs`: message id and chat id
- `Preconditions`: meme channel must have been configured earlier
- `Main Flow`:
  - Update the stored latest meme message id.
  - Skip command parsing and message notifications.
- `Outputs / Side Effects`: `/ememe` sampling range advances
- `Persistence`:
  - Writes `MemeChannelInfo`
- `Edge Cases`:
  - The bot effectively treats the meme channel as a data source, not an interactive chat.
- `Source References`:
  - `src/WellBot.UseCases/Chats/HandleTelegramAction/HandleTelegramActionCommandHandler.cs`

## Behavior: `MSG-NOTIFICATION-DISPATCH`

- `Surface`: asynchronous notification fan-out
- `Trigger`: non-command message after webhook routing
- `Inputs`: raw Telegram `Message`
- `Preconditions`: message must not have been consumed by meme-channel handling
- `Main Flow`:
  - Create a new DI scope.
  - Publish `MessageNotification` in a background task.
  - Catch and log any notification exception.
- `Outputs / Side Effects`: enables passive replies, format fixing, and recap logging without blocking the webhook response
- `Persistence`: indirect only through downstream handlers
- `Edge Cases`:
  - Fire-and-forget execution means there is no delivery guarantee to the Telegram caller.
- `Source References`:
  - `src/WellBot.UseCases/Chats/HandleTelegramAction/HandleTelegramActionCommandHandler.cs`
  - `src/WellBot.UseCases/Chats/RegularMessageHandles/MessageNotification.cs`
