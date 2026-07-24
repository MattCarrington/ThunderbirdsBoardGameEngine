# Persisted Game Dashboard

## Status

Planned production vertical slice.

This specification supersedes the `spike/game-state` implementation as the
description of the behaviour to build. The spike remains a source of product
and technical learning; it is not intended to be deployed or promoted directly
to production.

## User story

As a player using the Thunderbirds companion app, I want to create a game and
resume it through its private game URL, so that the app retains the state of my
board game.

## Scope

The first slice creates the currently supported standard setup, persists it,
and presents its Thunderbird Machines as a dashboard segment. It proves the
complete production path through domain construction, PostgreSQL persistence,
API, client, UI, deployment, security controls, testing, and observability.

The shared code currently uses `ThunderbirdCode` as the identity for all
Thunderbird Machines, including FAB 1. New product language should use
**Thunderbird Machine**, while the shared type name remains temporarily for
compatibility and to avoid coupling this slice to a repository-wide rename.

## Acceptance criteria

```gherkin
Feature: Create and resume a persisted game

Scenario: Create the supported standard game setup
    When a player creates a standard game
    Then a random game Guid should be assigned by the server
    And the game should be persisted
    And Thunderbirds 1, 2, 3, and 4 should be at the South Pacific
    And Thunderbird 5 should be at Geostationary Orbit
    And FAB 1 should be in Europe
    And Scott should be aboard Thunderbird 1
    And Virgil should be aboard Thunderbird 2
    And Alan should be aboard Thunderbird 3
    And Gordon should be aboard Thunderbird 4
    And John should be aboard Thunderbird 5
    And Lady Penelope should be aboard FAB 1
    And the player should be taken to the private URL for the game

Scenario: Display the Thunderbird Machines dashboard segment
    Given a standard game has been created
    When its dashboard is displayed
    Then each Thunderbird Machine should be shown with its current location
    And each character aboard it should be shown

Scenario: Resume an existing game
    Given a standard game has been created
    When its private game URL is opened
    Then the persisted game should be retrieved
    And its Thunderbird Machine locations and character assignments should be displayed

Scenario: Refresh the game dashboard
    Given an existing game is displayed
    When the dashboard is refreshed
    Then the same game should be retrieved
    And its state should be unchanged

Scenario: A newly created game survives an application restart
    Given a standard game has been created
    And its game Guid has been retained
    When the application is restarted
    And the game is retrieved using its Guid
    Then the game should still exist
    And its Thunderbird Machine locations should match their original locations
    And its character assignments should match their original assignments

Scenario: Retrieve an unknown game
    Given no game exists with a supplied game Guid
    When that game is requested
    Then a not-found response should be returned
    And no game should be created
    And no storage or implementation details should be disclosed
```

## Supported setup

All six characters participate in every game, including characters not
controlled by a player. Every character starts aboard exactly one Thunderbird
Machine.

The initial slice does not record player-controlled character selection.
Thunderbird 3 therefore always starts at the South Pacific. When John is
player-controlled, players may manually correct Thunderbird 3's location after
machine movement is available. This setup correction does not consume an
action point because turns and action-point accounting are outside the current
scope.

Player-character selection and the John-dependent starting position are
deferred to a separate story.

## Access model

The game Guid in the private URL is an anonymous read/write capability. Anyone
who possesses the URL can view and modify that one game. This is an accepted
MVP limitation because the application stores no personal, financial,
confidential, or safety-critical information; the expected impact of
unauthorised access is disruption of one helper-app game.

The slice must nevertheless:

- generate the Guid on the server using `Guid.NewGuid()`;
- reject `Guid.Empty` and malformed identifiers;
- provide no operation that enumerates games;
- use HTTPS in the deployed environment;
- avoid recording raw game URLs or Guids in logs, metrics, traces, and analytics;
- return `Referrer-Policy: no-referrer` for the application;
- return `Cache-Control: no-store` for game-state responses;
- retain normal input validation, safe problem responses, output encoding,
  dependency scanning, security headers, and rate limiting;
- ensure possession of one game Guid cannot expose another game.

There is no access revocation, token rotation, lost-link recovery, ownership,
or attribution of actions to people in this slice.

## Production definition of done

### Domain and application

- Standard setup is owned by a named GameState domain factory or policy rather
  than by an application handler or ReferenceData snapshot.
- The game protects its state and exposes no externally mutable collections.
- Every participating character is assigned to exactly one Thunderbird
  Machine.
- Character location is derived from the assigned machine; it is not persisted
  independently.
- ReferenceData remains the source of reusable machine, character, and location
  definitions and display names.

### Persistence and deployment

- Production persistence uses PostgreSQL through EF Core and Npgsql.
- Schema changes are represented by reviewed EF Core migrations committed to
  source control.
- A one-shot migration job applies migrations before application deployment.
- Application startup does not create or migrate the database schema.
- Migration failure prevents deployment of application code that requires the
  new schema.
- PostgreSQL readiness is reported separately from application liveness.
- Provider-managed default operational backups are accepted; custom backup
  scheduling and long-term retention are outside scope.

### User experience

- Creating a game navigates to `/games/{gameId}`.
- Opening or refreshing that route loads the persisted game.
- Create and load operations expose appropriate loading and error states.
- The create action is disabled while a creation request is in progress.
- Display names come from ReferenceData rather than exposing raw codes.
- The dashboard is keyboard operable and meets the project's applicable
  accessibility expectations.

### Testing

- Domain tests cover standard setup and character-assignment invariants.
- Application tests cover creation and retrieval orchestration.
- Repository integration tests run against real PostgreSQL.
- API component tests cover creation, retrieval, and not-found behaviour.
- Client tests cover the published HTTP contract.
- UI component tests cover the Thunderbird Machines segment.
- A browser test covers create, navigate, and refresh.
- Deployment verification proves that replacing or restarting the application
  does not remove a created game.
- CI applies all migrations to an empty PostgreSQL database before running
  persistence tests.

### Observability

- Structured logs and traces correlate requests without using the game Guid as
  the correlation identifier.
- Metrics cover creation and retrieval outcomes and duration.
- Database failures are observable without exposing connection details.
- Game payloads and private game URLs are not logged.

## Lifecycle

For the first slice, a game either exists or does not exist. Games are retained
indefinitely and have no user-facing status.

The persisted game records its identifier, creation time, setup version, and a
state/concurrency version. An update timestamp may be added if it serves an
identified query or operational requirement.

Completion, success, failure, abandonment, archive, deletion, and automatic
expiry are deferred. Completion may initially be an explicit player action;
the application cannot derive it until it tracks the relevant gameplay.

## Non-goals

- Selecting or recording player-controlled characters
- Applying John's alternative Thunderbird 3 setup automatically
- Moving Thunderbird Machines
- Transfer Characters
- Turn or action-point accounting
- Hood, scheme, disaster, deck, card, token, dice, or starting-player state
- Alternative setup variants
- Game listing, discovery, search, or lost-link recovery
- User accounts, ownership, invitations, or access roles
- Completion, archive, deletion, or expiry
- Revocable or separate read/write access tokens

## Follow-on stories

- Select player-controlled characters and apply the John-dependent setup rule.
- Move a Thunderbird Machine and persist the updated state.
- Transfer Characters atomically between Thunderbird Machines.
- Introduce lifecycle behaviour when the application can meaningfully record or
  derive game completion.
- Adopt `ThunderbirdMachine` consistently as the shared domain terminology.
