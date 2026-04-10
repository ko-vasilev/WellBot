# Chat Commands

## Routed Commands

The webhook router currently dispatches these command names:

- `set`
- `get`
- `getall`
- `del`
- `шлёп` aka `шлеп` aka `slap`
- `admin`
- `ememe`
- `prikol`
- `recap` aka `рекап`
- `nocap`
- `cap`

## Behavior: `CMD-SET-CHAT-DATA`

- `Surface`: Telegram command `/set`
- `Trigger`: save a reusable chat data item
- `Inputs`: HTML-formatted arguments, original message, reply target if present
- `Preconditions`: a non-empty key must be parseable from the arguments
- `Main Flow`:
  - Parse the first token as the key and the remainder as text.
  - Normalize the stored key to lowercase.
  - Remove any existing record for the same chat and key.
  - If the command replies to another message, use the replied message as the data source instead of the command body.
  - Save text plus any detected attached file id and media type.
  - Mark whether the source message contains a Telegram user mention.
  - Send a confirmation message.
- `Outputs / Side Effects`: chat data becomes available for `/get` and inline search
- `Persistence`:
  - Replaces `ChatData`
- `Edge Cases`:
  - Mention detection inspects only `message.Entities`, not caption entities on replied media.
  - If only a key is provided, an empty text value is still allowed.
  - Invalid or empty keys are rejected.
- `Source References`:
  - `src/WellBot.UseCases/Chats/Data/SetChatData/SetChatDataCommandHandler.cs`
  - `src/WellBot.UseCases/Chats/TelegramMessageService.cs`

## Behavior: `CMD-GET-CHAT-DATA`

- `Surface`: Telegram command `/get`
- `Trigger`: retrieve a saved item by key
- `Inputs`: chat id, key, sender Telegram id, command message id, replied message id
- `Preconditions`: none
- `Main Flow`:
  - Lookup the lowercased key in chat-scoped `ChatData`.
  - If missing, reply with a not-found message.
  - If the item contains a user mention, enforce a per-user per-key 10 minute rate limit within the chat.
  - Send the stored text/media/reaction payload.
  - If rate limited content was sent, store a new rate-limit entry.
- `Outputs / Side Effects`: sends the saved payload
- `Persistence`:
  - Reads `ChatData`
  - Updates in-memory rate-limit state
- `Edge Cases`:
  - Mention-based entries are rate limited even when the mention came from previously saved source content.
  - Reaction data requires a reply target to exist when sending; otherwise TelegramMessageService only logs a warning.
- `Source References`:
  - `src/WellBot.UseCases/Chats/Data/ShowData/ShowDataCommandHandler.cs`
  - `src/WellBot.UseCases/Chats/MessageRateLimitingService.cs`
  - `src/WellBot.UseCases/Chats/TelegramMessageService.cs`

## Behavior: `CMD-GETALL-CHAT-DATA`

- `Surface`: Telegram command `/getall`
- `Trigger`: request the list of saved keys
- `Inputs`: chat id
- `Preconditions`: none
- `Main Flow`:
  - Load all saved keys for the current chat, ordered alphabetically.
  - If empty, send a no-data message.
  - Otherwise send a comma-separated summary.
- `Outputs / Side Effects`: informational list
- `Persistence`:
  - Reads `ChatData`
- `Edge Cases`:
  - No pagination or chunking is applied.
- `Source References`:
  - `src/WellBot.UseCases/Chats/Data/ShowKeys/ShowKeysCommandHandler.cs`

## Behavior: `CMD-DEL-CHAT-DATA`

- `Surface`: Telegram command `/del`
- `Trigger`: delete a saved item by key
- `Inputs`: chat id, key, message id
- `Preconditions`: none
- `Main Flow`:
  - Lookup the lowercased key in the current chat.
  - If missing, send a not-found reply to the command.
  - Otherwise delete it and react with a success emoji.
- `Outputs / Side Effects`: removes a saved item
- `Persistence`:
  - Deletes `ChatData`
- `Edge Cases`:
  - No authorization checks beyond chat scope.
- `Source References`:
  - `src/WellBot.UseCases/Chats/Data/DeleteChatData/DeleteChatDataCommandHandler.cs`

## Behavior: `CMD-SLAP`

- `Surface`: Telegram commands `/шлёп`, `/шлеп`, `/slap`
- `Trigger`: request a slap reaction
- `Inputs`: chat id, message id
- `Preconditions`: none
- `Main Flow`:
  - Choose a weighted slap reply option.
  - Some options send one or more text messages.
  - One option reacts directly to the command message.
  - One option attempts to send a random stored slap animation.
- `Outputs / Side Effects`: text, reaction, or animation output
- `Persistence`:
  - Reads `SlapOption`
- `Edge Cases`:
  - If the animation option is chosen and no slap animations exist, the command produces no fallback output.
- `Source References`:
  - `src/WellBot.UseCases/Chats/Slap/SlapCommandHandler.cs`

## Behavior: `CMD-ADMIN`

- `Surface`: Telegram command `/admin`
- `Trigger`: superadmin-only maintenance action
- `Inputs`: raw arguments, replied message, sender Telegram id, chat settings
- `Preconditions`: sender id must be in configured `SuperadminIds`
- `Main Flow`:
  - Unauthorized users receive a shrug reaction and nothing else.
  - Supported actions:
    - `add slap`: save the replied animation as a slap option
    - `passive add ...`: save passive reply options under selected topics
    - `passive probability ...`: update a topic probability and refresh topic cache
    - `meme`: mark the replied forwarded channel post as the meme channel
    - `broadcast`: send the replied message to every known chat
    - `user`: show metadata about the replied or forwarded user
- `Outputs / Side Effects`: depends on subcommand
- `Persistence`:
  - Reads and writes `SlapOption`, `PassiveReplyOption`, `PassiveTopic`, `MemeChannelInfo`, and known `Chat` ids
- `Edge Cases`:
  - Some invalid usages fail silently instead of sending guidance.
  - `passive add` defaults to the topic named `regular` when no topic matches.
  - `passive add batch` splits text by newline into multiple text-only replies.
  - `passive add react` validates the reaction by attempting to send it immediately.
  - `meme` only works when replying to a forwarded channel message.
  - `broadcast` is best effort and logs per-chat failures.
- `Source References`:
  - `src/WellBot.UseCases/Chats/AdminControl/AdminControlCommandHandler.cs`

## Behavior: `CMD-EMEME`

- `Surface`: Telegram command `/ememe`
- `Trigger`: request a random forwarded meme
- `Inputs`: chat id, stored meme channel id, latest message id
- `Preconditions`: a meme channel must be configured
- `Main Flow`:
  - Load the configured meme channel record.
  - Randomly choose a message id from `1..LatestMessageId`.
  - Try up to 3 times to forward a message from that channel.
  - If all attempts fail, send a fallback text.
- `Outputs / Side Effects`: forwards a channel post into the chat
- `Persistence`:
  - Reads `MemeChannelInfo`
- `Edge Cases`:
  - Deleted or unsupported source messages are tolerated by retrying.
  - Sampling is uniform over ids, not over currently accessible posts.
- `Source References`:
  - `src/WellBot.UseCases/Chats/Ememe/EmemeCommandHandler.cs`

## Behavior: `CMD-PRIKOL`

- `Surface`: Telegram command `/prikol`
- `Trigger`: request a random joke
- `Inputs`: external HTML page from `anekdot.ru`
- `Preconditions`: external site must be reachable and parseable
- `Main Flow`:
  - Download the random joke page.
  - Parse the first `.topicbox .text` block.
  - Replace HTML line breaks with newline characters.
  - Send the result to Telegram.
- `Outputs / Side Effects`: joke text
- `Persistence`: none
- `Edge Cases`:
  - Any network or parsing failure produces a generic fallback message.
- `Source References`:
  - `src/WellBot.UseCases/Chats/Prikol/PrikolCommandHandler.cs`

## Behavior: `CMD-RECAP`

- `Surface`: Telegram commands `/recap`, `/рекап`
- `Trigger`: request an AI-generated summary of recent chat history
- `Inputs`: chat id, optional duration or message-count argument, stored message logs, OpenAI API key
- `Preconditions`: message logs must exist; OpenAI call must succeed for a normal recap
- `Main Flow`:
  - Parse arguments:
    - empty input means last 4 hours
    - `Nч` or `Nh` means last `N` hours capped at 24
    - `Nм` or `Nm` means last `N` minutes capped at 1440
    - integer means last `1..10000` messages
  - Invalid input returns usage help.
  - With a 1-in-20 chance, send a joke insult instead of generating a recap.
  - Load relevant non-null message logs for the chat in chronological order.
  - If nothing matches, send a no-messages response.
  - Trigger Telegram typing status.
  - Chunk large histories and summarize them with `gpt-4o-mini`.
  - If multiple chunks were summarized, summarize the summaries again.
  - Send the final recap with a duration label.
- `Outputs / Side Effects`: AI summary text
- `Persistence`:
  - Reads `MessageLog`
- `Edge Cases`:
  - OpenAI failures return a generic failure response.
  - Chunking uses a rough token estimate based on message length, not exact tokenization.
  - Logs with anonymized or null message content after opt-out are excluded from recap content.
- `Source References`:
  - `src/WellBot.UseCases/Chats/Summarization/Recap/RecapCommandHandler.cs`

## Behavior: `CMD-NOCAP`

- `Surface`: Telegram command `/nocap`
- `Trigger`: user opts out of recap logging for the current chat
- `Inputs`: chat id, sender Telegram id, message id
- `Preconditions`: sender must be present in the update
- `Main Flow`:
  - If no opt-out exists, create one.
  - Find existing message logs in the chat from that Telegram user.
  - Anonymize those log rows by replacing sender with `somebody` and removing message text and sender Telegram id.
  - React with a success emoji.
- `Outputs / Side Effects`: future recap logging excludes the user; past stored content is scrubbed
- `Persistence`:
  - Writes `MessageLogOptOut`
  - Updates `MessageLog`
- `Edge Cases`:
  - Repeating the command after opt-out does not duplicate rows and still returns success.
- `Source References`:
  - `src/WellBot.UseCases/Chats/Summarization/LogOptOut/LogOptOutCommandHandler.cs`

## Behavior: `CMD-CAP`

- `Surface`: Telegram command `/cap`
- `Trigger`: user opts back into recap logging for the current chat
- `Inputs`: chat id, sender Telegram id, message id
- `Preconditions`: sender must be present in the update
- `Main Flow`:
  - Remove any opt-out rows for that user and chat.
  - React with a success emoji.
- `Outputs / Side Effects`: future messages are again eligible for recap logging
- `Persistence`:
  - Deletes `MessageLogOptOut`
- `Edge Cases`:
  - Past anonymized logs are not restored.
- `Source References`:
  - `src/WellBot.UseCases/Chats/Summarization/LogOptIn/LogOptInCommandHandler.cs`
