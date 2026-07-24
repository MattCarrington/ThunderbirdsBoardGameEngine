# GameState Domain

## Status

Planned model for the persisted game dashboard vertical slice.

## Responsibility

GameState owns the lifecycle and mutable state of an individual game. This
includes constructing the supported standard setup and recording the resolved
positions and assignments for that game.

ReferenceData owns reusable definitions such as valid Thunderbird Machines,
characters, locations, disasters, and display names. It does not own how pieces
are arranged when a game is created.

```text
ReferenceData definitions
          |
          v
GameState standard setup policy
          |
          v
Valid Game aggregate
          |
          v
Persistent current state
```

## Initial aggregate

Conceptually, the game contains:

```text
Game
|- GameId
|- CreatedAtUtc
|- SetupVersion
|- StateVersion
|- Thunderbird Machine positions
|  `- ThunderbirdCode -> LocationCode
`- Character assignments
   `- CharacterCode -> ThunderbirdCode
```

`ThunderbirdCode` is the existing shared identity type. It currently represents
all Thunderbird Machines, including FAB 1. The intended ubiquitous-language
name is `ThunderbirdMachineCode`; changing the shared public type is deferred.

## Invariants

- Every Thunderbird Machine participating in a game has exactly one location.
- All six standard characters participate in the supported standard game.
- Every participating character is aboard exactly one Thunderbird Machine.
- A character cannot exist independently of a machine.
- A character cannot be aboard two machines at once.
- Every assigned machine participates in the same game.
- A character's location is derived from the location of their assigned
  machine and is not stored separately.
- Callers cannot mutate positions or assignments without using a domain
  operation.

The canonical assignment direction is:

```text
CharacterCode -> ThunderbirdCode
```

The dashboard derives the inverse grouping:

```text
Thunderbird Machine -> characters aboard
```

This allows a future **Transfer Characters** operation to replace each selected
character's assignment atomically, without an observable intermediate state in
which a character is aboard no machine or two machines.

## Supported standard setup

The GameState domain owns a named standard-game factory or setup policy. For
the first slice it resolves:

| Thunderbird Machine | Initial location | Initial character |
|---|---|---|
| Thunderbird 1 | South Pacific | Scott |
| Thunderbird 2 | South Pacific | Virgil |
| Thunderbird 3 | South Pacific | Alan |
| Thunderbird 4 | South Pacific | Gordon |
| Thunderbird 5 | Geostationary Orbit | John |
| FAB 1 | Europe | Lady Penelope |

The rulebook places Thunderbird 3 in Geostationary Orbit when John is
player-controlled. Player-character selection is not recorded in the first
slice, so the supported MVP setup uses the South Pacific default. That
limitation is explicit and is not encoded into ReferenceData.

## Layer responsibilities

### Domain

- Define and validate a game and its invariants.
- Resolve the supported standard setup deterministically.
- Expose controlled future operations such as moving a machine or transferring
  characters.

### Application

- Orchestrate game creation and retrieval.
- Obtain a new game from the standard-game factory.
- Persist and retrieve through a repository abstraction.
- Enrich query results with reusable ReferenceData where appropriate.

### Infrastructure

- Persist and reconstruct games through PostgreSQL.
- Validate integration between setup identities and ReferenceData catalogs.
- Adapt Rules operations to GameState through application-defined gateways.

### API and client contracts

- Expose stable resource-oriented create and retrieve operations.
- Use the correct consumer-facing term **Thunderbird Machine** even while the
  existing shared code type remains `ThunderbirdCode`.

### UI

- Render a Thunderbird Machine-centred dashboard projection.
- Display each machine's location and grouped occupants.
- Never become the authoritative source of game state.

## Future operations

Two distinct operations are anticipated but not included in the first slice:

- **Move Thunderbird Machine** changes a machine's location and therefore the
  derived location of everyone aboard it.
- **Transfer Characters** atomically changes the machine assignments of one or
  more characters while preserving all assignment invariants.
