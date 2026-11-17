# Active Character Turn Order

## Status

In development.

## User story

As a group starting a game, we want to choose the participating player
characters in turn order, so that the companion application knows who starts
and can support turn-based actions later.

## Scope

This slice records an ordered selection of two to four player-controlled
characters when a game is created. The first character in that order is the
initial active character.

All six characters still exist in GameState and remain aboard Thunderbird
Machines. The turn order identifies the player-controlled characters; it does
not remove non-player characters from the game.

The selected characters also complete the standard setup rule for Thunderbird
3: when John is selected, Thunderbird 3 starts at Geo-Stationary Orbit;
otherwise it starts at the South Pacific.

Ending a turn and restricting existing actions by the active character are
separate follow-on slices.

## Acceptance criteria

```gherkin
Feature: Create a game with an active-character turn order

Scenario: Create a game with the minimum turn order
    Given two distinct known characters are selected in order
    When a standard game is created
    Then the supplied turn order should be persisted unchanged
    And the first selected character should be active
    And the active turn index should be zero

Scenario: Create a game with the maximum turn order
    Given four distinct known characters are selected in order
    When a standard game is created
    Then the supplied turn order should be persisted unchanged
    And the first selected character should be active

Scenario: Resume a game with its turn state
    Given a game has been created with an ordered character selection
    When the game is retrieved again
    Then its turn order should match the original order
    And its active character should match the original active character

Scenario: Start Thunderbird 3 in orbit when John is selected
    Given John is included in the selected turn order
    When a standard game is created
    Then Thunderbird 3 should start at Geo-Stationary Orbit

Scenario: Start Thunderbird 3 in the South Pacific when John is not selected
    Given John is not included in the selected turn order
    When a standard game is created
    Then Thunderbird 3 should start at the South Pacific

Scenario: Reject too few selected characters
    Given fewer than two characters are selected
    When a standard game is requested
    Then the request should be rejected
    And no game should be persisted

Scenario: Reject too many selected characters
    Given more than four characters are selected
    When a standard game is requested
    Then the request should be rejected
    And no game should be persisted

Scenario: Reject a duplicate selected character
    Given the same character appears more than once in the turn order
    When a standard game is requested
    Then the request should be rejected
    And no game should be persisted

Scenario: Reject an unknown selected character
    Given the turn order contains a character unknown to ReferenceData
    When a standard game is requested
    Then the request should be rejected
    And no game should be persisted
```

## Domain model

GameState records:

- an immutable ordered `TurnOrder` containing two to four distinct characters;
- an `ActiveTurnIndex`, initially zero; and
- an `ActiveCharacter`, derived from `TurnOrder[ActiveTurnIndex]`.

The aggregate protects structural invariants that can be established from its
own state: the order has the supported cardinality, contains no duplicates,
contains only characters participating in the game, and has a valid active
index. The application validates selected codes against ReferenceData before
creating the aggregate.

`TurnOrder` is immutable after setup. `ActiveTurnIndex` is mutable game state,
although this slice does not yet provide an operation that changes it.

## Persistence

The active index is stored on the game record. The ordered character codes are
stored as child rows with an explicit position:

```text
GameId + Position -> CharacterCode
```

The database prevents duplicate positions and duplicate characters within one
game. Repository retrieval orders the rows by position before rehydrating the
aggregate. Updates to unrelated game state must preserve the immutable turn
order.

Existing development games created before this schema do not contain a
truthful turn order. They are disposable and must be recreated rather than
being assigned an invented order.

## HTTP outcomes

| Condition | Outcome |
|---|---:|
| Game created | `201 Created` |
| Missing, fewer than two, or more than four selections | `400 Bad Request` |
| Duplicate or unknown selected character | `422 Unprocessable Entity` |

Problem responses must not expose private game identifiers, internal state, or
implementation details.

## Testing strategy

- Domain tests cover turn-order invariants and the derived active character.
- Setup-factory tests cover both Thunderbird 3 starting positions.
- Application tests cover ReferenceData validation and prove invalid requests
  are not persisted.
- PostgreSQL integration tests cover the ordered representation, complete
  round-trip, active index, active character, and preservation during unrelated
  updates.
- API component tests cover request validation and controlled error outcomes.
- Client and UI tests cover selection order and successful creation.
- A user journey creates and resumes a game with the same turn state.

## Non-goals

- Ending a turn or advancing the active index
- Action points or turn completion rules
- Player identities, accounts, names, or ownership
- Enforcing active-character restrictions on movement or transfers
- Digitally recording another player's permission to move their character peg

## Follow-on stories

### End the active turn

Advance `ActiveTurnIndex`, wrap from the final selected character to zero,
persist the change with optimistic concurrency, and retain it across reloads
and application restarts.

### Enforce active-character actions

- A Thunderbird Machine may move only when the active character is aboard it.
- Transfer Characters may move any characters, including another player's
  character, but both participating machines must be in the active character's
  current space.
- Asking another player for permission before transferring their physical peg
  remains a social table rule and is not enforced without player identity and
  consent modelling.
- The UI should guide the player by disabling unavailable actions, while
  GameState remains authoritative when a client bypasses that guidance.

