# WellBot Business Logic

## Purpose

This directory is the source-of-truth description of the current chat-domain behavior implemented by WellBot.

It documents:

- Telegram webhook entrypoints and routing
- User-facing Telegram commands
- Non-command message behavior
- Admin/API surfaces that change chat behavior or its reference data
- Recurring jobs that affect chat output or recap data
- Core domain concepts and persistent state

It describes the current behavior as implemented.

## Scope

Included:

- Telegram chat behavior in `src/WellBot.UseCases/Chats`
- Chat-affecting API endpoints in `src/WellBot.Web/Controllers/ChatController.cs`
- Chat-affecting recurring jobs in `src/WellBot.Web/Infrastructure/RecurringJobs`
- Startup behavior that changes chat caches or chat data validity

Excluded:

- Authentication flows that do not affect chat behavior
- Internal framework wiring with no user-visible effect

## Document Structure

- `telegram-entrypoints.md`: webhook ingress, command parsing, inline queries, and notification dispatch
- `chat-commands.md`: slash commands and admin Telegram controls
- `message-behaviors.md`: non-command message reactions and logging behavior
- `admin-and-api.md`: authenticated HTTP surfaces that manage chat behavior
- `recurring-jobs.md`: scheduled behavior and startup initialization that changes outcomes
- `domain-glossary.md`: entities, services, and cross-cutting rules

## Behavior Record Schema

Each behavior entry should follow the same shape:

- `Behavior ID`: stable identifier
- `Surface`: Telegram command, inline query, HTTP API, recurring job, startup, or message notification
- `Trigger`: what causes the behavior to run
- `Inputs`: user input, message metadata, stored data, or time-based inputs
- `Preconditions`: conditions required for the happy path
- `Main Flow`: high-level decision path
- `Outputs / Side Effects`: user-visible output, API result, or background effect
- `Persistence`: database reads/writes or in-memory cache changes
- `Edge Cases`: notable quirks, failures, or boundary rules

## Conventions

- Behavior descriptions prioritize observable behavior over code structure.
- Known quirks are documented instead of normalized away.
- Telegram responses are paraphrased unless the exact wording is itself important.
- Time behavior should explicitly call out UTC, Moscow time, or hard-coded offsets when present.
- Randomized behavior should state both the selection source and the fallback path.
