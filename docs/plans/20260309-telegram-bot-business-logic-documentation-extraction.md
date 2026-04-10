# Telegram Bot Business Logic Documentation Extraction

## Overview

Create a source-of-truth documentation set in `/docs` that captures the current Telegram chat business logic, plus related admin/API and recurring-job behavior that changes chat outcomes. The documentation should be readable by humans, structured enough for future machine consumption, and focused on behavior rather than implementation details.

The immediate goal is not to refactor code. The goal is to extract the existing behavior into stable documentation that can later guide a full rewrite without losing rules, flows, and edge cases.

## Context

The current behavior is spread across several layers:

- Telegram webhook ingress in `src/WellBot.Web/Controllers/BotController.cs`
- Primary command dispatch in `src/WellBot.UseCases/Chats/HandleTelegramAction/HandleTelegramActionCommandHandler.cs`
- Chat command handlers in `src/WellBot.UseCases/Chats/**/*CommandHandler.cs`
- Non-command message behaviors in `src/WellBot.UseCases/Chats/RegularMessageHandles/**/*NotificationHandler.cs`
- Summarization and log-related behavior in `src/WellBot.UseCases/Chats/Summarization/**/*`
- Admin/API surfaces in `src/WellBot.Web/Controllers/ChatController.cs`
- Recurring jobs in `src/WellBot.Web/Infrastructure/RecurringJobs/**/*`
- Shared chat-domain entities in `src/WellBot.Domain/Chats/**/*`

Constraints and expectations:

- Documentation should be readable by both humans and machines.
- It should not be excessively detailed, but must cover meaningful edge cases.
- It should describe current behavior as implemented, including inconsistent or surprising behavior when relevant.
- It should become the primary reference during later refactoring.

## Development Approach

Recommended approach: create one canonical documentation format, then extract behavior by surface area.

Proposed documentation shape:

- `docs/business-logic/README.md`
  Defines the documentation contract, terminology, and how the files relate.
- `docs/business-logic/telegram-entrypoints.md`
  Describes webhook ingestion, command parsing, command routing, and non-command dispatch.
- `docs/business-logic/chat-commands.md`
  Captures slash commands and their observable behavior.
- `docs/business-logic/message-behaviors.md`
  Captures passive replies, media format fixes, meme-channel handling, and message logging.
- `docs/business-logic/admin-and-api.md`
  Captures admin-only Telegram controls plus authenticated API endpoints that alter chat behavior.
- `docs/business-logic/recurring-jobs.md`
  Captures scheduled message sending and cleanup behavior.
- `docs/business-logic/domain-glossary.md`
  Defines persistent entities and key concepts used across behaviors.

Each behavior entry should use a consistent, parse-friendly template such as:

- `Behavior ID`
- `Surface`
- `Trigger`
- `Inputs`
- `Preconditions`
- `Main Flow`
- `Outputs / Side Effects`
- `Persistence`
- `Edge Cases`
- `Open Questions / Ambiguities`
- `Source References`

This format keeps the files human-readable while remaining regular enough for future automated parsing.

## Testing Strategy

This task is documentation-first, but every extraction step still needs verification against code.

- Cross-check each documented behavior against the current handler/controller/job implementation before marking it complete.
- Verify every user-facing Telegram command routed in `HandleTelegramActionCommandHandler` is represented exactly once in documentation.
- Verify every chat-affecting authenticated endpoint in `ChatController` is represented exactly once.
- Verify every recurring job that changes chat behavior is documented.
- Verify non-command message handlers are documented, including log-only and best-effort behaviors.
- Run `dotnet test src` after documentation changes only if the repo already expects documentation updates to coexist with build/test checks; otherwise note that no code behavior changed and tests are not strictly required for the plan phase.

## Implementation Steps

1. Create the documentation container and schema.
   Files to create:
   - `docs/business-logic/README.md`
   - `docs/business-logic/domain-glossary.md`
   - optional shared template file if repetition becomes excessive
   Test checklist:
   - Confirm the schema is consistent across all planned documents.
   - Confirm section names are stable and suitable for future machine parsing.

2. Document Telegram ingress and routing behavior.
   Files to create or modify:
   - `docs/business-logic/telegram-entrypoints.md`
   Source files to inspect:
   - `src/WellBot.Web/Controllers/BotController.cs`
   - `src/WellBot.UseCases/Chats/HandleTelegramAction/HandleTelegramActionCommandHandler.cs`
   - `src/WellBot.UseCases/Chats/TelegramMessageService.cs`
   Test checklist:
   - Confirm direct-message behavior, inline-query behavior, meme-channel short-circuiting, and command parsing rules are captured.
   - Confirm unknown-command handling and error handling are captured.

3. Document all Telegram slash-command behaviors.
   Files to create or modify:
   - `docs/business-logic/chat-commands.md`
   Source files to inspect:
   - `src/WellBot.UseCases/Chats/Pidor/**/*`
   - `src/WellBot.UseCases/Chats/Data/**/*`
   - `src/WellBot.UseCases/Chats/Slap/*`
   - `src/WellBot.UseCases/Chats/Ememe/*`
   - `src/WellBot.UseCases/Chats/Prikol/*`
   - `src/WellBot.UseCases/Chats/Summarization/**/*`
   - `src/WellBot.UseCases/Chats/AdminControl/*`
   Test checklist:
   - Confirm every routed command from `HandleTelegramActionCommandHandler` appears in the document.
   - Confirm command aliases, required reply context, persistence changes, and user-visible responses are captured.
   - Confirm edge cases such as empty registration pools, repeated daily runs, missing reply targets, and opt-in/opt-out state are captured.

4. Document non-command message behaviors.
   Files to create or modify:
   - `docs/business-logic/message-behaviors.md`
   Source files to inspect:
   - `src/WellBot.UseCases/Chats/RegularMessageHandles/Reply/ReplyNotificationHandler.cs`
   - `src/WellBot.UseCases/Chats/RegularMessageHandles/Reply/PassiveTopicService.cs`
   - `src/WellBot.UseCases/Chats/RegularMessageHandles/FixFormat/FixFormatNotificationHandler.cs`
   - `src/WellBot.UseCases/Chats/Summarization/LogMessageNotificationHandler.cs`
   - any meme-channel helper logic used during webhook handling
   Test checklist:
   - Confirm probabilistic replies, topic matching, retry behavior, and reaction/media reply support are captured.
   - Confirm message logging scope, opt-out handling, and log retention dependency are captured.
   - Confirm automatic media conversion triggers and failure behavior are captured.

5. Document admin/API behaviors that mutate chat logic.
   Files to create or modify:
   - `docs/business-logic/admin-and-api.md`
   Source files to inspect:
   - `src/WellBot.Web/Controllers/ChatController.cs`
   - `src/WellBot.UseCases/Chats/Topics/**/*`
   - `src/WellBot.UseCases/Chats/AutomaticMessages/**/*`
   - `src/WellBot.UseCases/Chats/Pidor/AddPidorGameMessage/*`
   - `src/WellBot.UseCases/Chats/Pidor/DeleteGameMessage/*`
   - `src/WellBot.UseCases/Chats/Pidor/GetPidorGameMessages/*`
   Test checklist:
   - Confirm every authenticated endpoint that changes chat behavior or reference data is documented.
   - Confirm topic-cache refresh behavior and administrative constraints are captured.

6. Document recurring jobs and timing-sensitive behavior.
   Files to create or modify:
   - `docs/business-logic/recurring-jobs.md`
   Source files to inspect:
   - `src/WellBot.Web/Infrastructure/RecurringJobs/RecurringJobInitializer.cs`
   - `src/WellBot.Web/Infrastructure/RecurringJobs/SendAutomaticMessages.cs`
   - `src/WellBot.Web/Infrastructure/RecurringJobs/CleanupMessageLogs.cs`
   - `src/WellBot.UseCases/Chats/AutomaticMessages/SendAutomaticMessages/SendAutomaticMessagesCommandHandler.cs`
   Test checklist:
   - Confirm schedule frequency, time-window rules, daily-send semantics, and fallback-to-text behavior are captured.
   - Confirm log cleanup retention rules are captured.

7. Build a glossary and dependency map for later refactoring.
   Files to create or modify:
   - `docs/business-logic/domain-glossary.md`
   - `docs/business-logic/README.md`
   Source files to inspect:
   - `src/WellBot.Domain/Chats/**/*`
   - selected shared services such as `CurrentChatService`, `RandomService`, `TelegramMessageService`, and `MemeChannelService`
   Test checklist:
   - Confirm persistent entities, cross-cutting services, and naming inconsistencies are explained.
   - Confirm future refactoring teams can identify where behavior depends on database state, caches, randomness, or Telegram metadata.

8. Run a completeness review of the documentation set.
   Files to modify:
   - all files under `docs/business-logic/`
   Test checklist:
   - Confirm every chat-affecting handler/controller/job in scope is referenced.
   - Confirm duplicate or conflicting behavior descriptions are resolved.
   - Confirm each file contains source references back to implementation files.
   - Confirm edge cases are recorded without drifting into rewrite proposals.

## Technical Details

Documentation conventions:

- Prefer stable identifiers such as `CMD-PIDOR-RUN` or `MSG-PASSIVE-REPLY` for each behavior.
- Use explicit file references to current implementation so later refactoring can trace back to code.
- Keep sections ordered consistently across files.
- Keep behavior descriptions implementation-faithful, even when the current code has quirks.

Edge cases to explicitly capture:

- Inline queries vs ordinary messages.
- Private chats ignored by default.
- Commands addressed to `@botusername`.
- Unknown commands only answered in direct-command contexts.
- Reply-dependent commands that silently fail or return guidance when used incorrectly.
- Randomized behavior in passive replies, pidor winner selection, and automatic image selection.
- State-dependent behavior such as one-pidor-per-day, user opt-out from recap logging, and daily automatic-message suppression after send.
- Best-effort behavior when Telegram API calls fail.
- Time-based behavior using UTC and hard-coded GMT+4 assumptions for automatic messages.

Out-of-scope for this plan:

- Rewriting handlers or normalizing current architecture.
- Designing the target refactor architecture.
- Documenting authentication or user-management logic that does not affect chat behavior.

## Post-Completion

After the documentation set is created:

- Review the docs for missing behavior and contradictions against the current code.
- Decide whether to freeze the docs as the refactor baseline or add one more pass for examples and scenario tables.
- Use the documentation set to define the refactor backlog by behavior group rather than by current file structure.
- Optionally add lightweight linting or schema checks later if the team wants stricter machine-readability.
