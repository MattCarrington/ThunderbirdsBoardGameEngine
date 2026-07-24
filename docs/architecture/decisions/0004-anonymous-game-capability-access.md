# ADR 0004: Use Anonymous Capability Access for Games

- Status: Accepted for the planned persisted-game slice
- Date: 2026-07-24

## Context

The companion app does not currently need user accounts, ownership, game
listing, access roles, or attribution of actions. The persisted state contains
no personal, financial, confidential, or safety-critical information.

The maximum expected impact of unauthorised access is that someone views or
changes the helper state for one board game, causing it to disagree with the
physical board.

## Decision

The server will assign each game a random .NET `Guid` using `Guid.NewGuid()`.
PostgreSQL will store it using its native `uuid` type.

The Guid in `/games/{gameId}` is an anonymous read/write capability. Possession
of the private game URL is permission to retrieve and modify that game.

The application will:

- expose no game-enumeration operation;
- use HTTPS and HSTS at the deployed boundary;
- avoid logging raw game Guids, private URLs, or payloads;
- use request or trace identifiers rather than game Guids for correlation;
- return `Referrer-Policy: no-referrer`;
- return `Cache-Control: no-store` for game-state responses;
- apply safe error handling, validation, output encoding, security headers,
  dependency scanning, request limits, and rate limiting;
- ensure a Guid grants access only to its corresponding game.

## Accepted limitations

- Anyone with the URL can read and modify the game.
- Sharing the URL shares access.
- Access cannot be revoked or rotated.
- A lost URL cannot be recovered through the application.
- Actions cannot be attributed to a person.
- The URL may remain in browser history.

These limitations are proportionate to the information and impact in the MVP.

## Revisit triggers

Reconsider this decision when the application needs any of the following:

- personal or sensitive information;
- user-owned game lists or lost-game recovery;
- revocation or token rotation;
- separate read and write permissions;
- attribution or audit by person;
- competitive or public play;
- materially valuable game history.

At that point, the game Guid can remain a stable identifier while
authentication or a separate access token supplies authorisation.

