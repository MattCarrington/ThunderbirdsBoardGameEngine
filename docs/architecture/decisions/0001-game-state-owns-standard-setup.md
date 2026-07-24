# ADR 0001: GameState Owns Standard Game Setup

- Status: Accepted for the planned persisted-game slice
- Date: 2026-07-24

## Context

ReferenceData models reusable game elements compiled into an immutable,
versioned snapshot. The initial arrangement of those elements creates the first
valid mutable state of an individual game.

The rulebook setup includes fixed positions and assignments as well as a
conditional rule: Thunderbird 3 starts in Geostationary Orbit when John is
player-controlled and at the South Pacific otherwise. Extending the spreadsheet
compiler or attaching this relationship to a Thunderbird, character, or
location definition would give ReferenceData responsibility for GameState
behaviour.

## Decision

GameState will own a named standard-game factory or setup policy. It will
construct the resolved initial state using strongly typed machine, character,
and location identities.

ReferenceData remains the authority for reusable definitions and display names.
GameState integration tests or startup validation will ensure that identities
used by the setup policy exist in the ReferenceData catalogs.

The first slice supports one deterministic setup in which Thunderbird 3 starts
at the South Pacific. Player-controlled character selection and the John
override are deferred.

## Consequences

- Creation handlers orchestrate setup and persistence but do not contain setup
  tables or rules.
- The domain setup policy is deterministic and can be unit tested without I/O.
- ReferenceData does not acquire a niche setup-rule language or compiler input.
- Adding setup variants later requires an explicit GameState design rather than
  changing reusable element definitions.
- The persisted game stores resolved current state, not unresolved setup rules.

