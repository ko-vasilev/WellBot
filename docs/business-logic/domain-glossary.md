# Domain Glossary

## Persistent Entities

- `Chat`
  - Internal chat row keyed by Telegram chat id.
  - Used as the anchor for most chat-scoped data.

- `ChatData`
  - Saved reusable content keyed per chat.
  - Can store text, media file ids, and whether the saved source contained a user mention.
  - Used by `/set`, `/get`, `/getall`, `/del`, and inline queries.

- `SlapOption`
  - Stored animation candidates used by `/slap`.

- `PassiveTopic`
  - Matching rule for passive replies.
  - Includes name, regex, probability, direct/meme/audio flags, and exclusivity.

- `PassiveReplyOption`
  - Stored passive response payload linked to one or more passive topics.
  - Can be text, media, or reaction.

- `MemeChannelInfo`
  - Stores the configured meme source channel and the latest observed message id.
  - Used by meme-channel tracking and `/ememe`.

- `AutomaticMessageTemplate`
  - Scheduled outbound message definition.
  - Stores target chat, cron interval, message text, optional image query, and last trigger date.

- `MessageLog`
  - Stored recap event record for a chat.
  - Contains sender display name, optional sender Telegram id, UTC event time, and optional textual event description.

- `MessageLogOptOut`
  - Per-user per-chat recap opt-out marker.
  - Prevents future detailed logging and triggers anonymization of prior logs on opt-out.

### Obsolete data

These entities are no longer used in the bot and kept only for historic purposes.

- `PidorRegistration`
  - Participant enrollment for the pidor game.
  - Scoped to a chat and Telegram user id.

- `ChatPidor`
  - Historical daily winner record for a chat.
  - Stores the game day, winning registration, and the result-message template used.

- `PidorMessage`
  - Result-message template for `/pidor`.
  - Supports weighting and optional specialization by Telegram user, specific date, or day of week.

## Cross-Cutting Services

- `CurrentChatService`
  - Maps Telegram chat ids to internal `Chat.Id`.
  - Populated by MediatR pipeline behavior for requests implementing `IChatInfo`.

- `TelegramMessageService`
  - Central helper for sending text, media, stickers, voice notes, and reactions.
  - Also converts Telegram message entities into HTML-formatted text for stored content reuse.

- `PassiveTopicService`
  - In-memory matcher for passive reply topics.
  - Cache is hydrated at startup and refreshed when topics change.

- `MessageRateLimitingService`
  - In-memory 10 minute rate limiter for `/get` responses whose saved content includes a user mention.
  - Keyed by Telegram chat id, Telegram user id, and chat-data key.

- `MemeChannelService`
  - In-memory holder of the active meme channel id.
  - Populated at startup and updated by `/admin meme`.

- `RandomService`
  - Shared source for random selection and weighted random behavior.
  - Affects slap responses, passive replies, recap joke branch, and automatic image choice.

## Observable Rules

- Chat scope is usually enforced through `CurrentChatService` rather than repeating chat lookups in handlers.
- Private chats are ignored at webhook routing level.
- A message in the configured meme channel is treated as source data, not normal bot input.
- Some behavior depends on runtime caches and will not change until startup or an explicit refresh path runs.
- Several features are best effort and log failures instead of surfacing them to users.

## Known Time Rules

- Automatic-message scheduling uses UTC timestamps plus a hard-coded GMT+4 sending window.
- Recap log cleanup removes entries older than one day using UTC time.

## Known Inconsistencies To Preserve In Documentation

- Passive-topic regex checks message text but not captions.
- `/slap` can produce no visible output when the animation branch is chosen with no stored animations.
