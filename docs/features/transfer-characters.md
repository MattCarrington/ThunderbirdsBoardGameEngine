# Transfer Characters

## Status

Proposed product vertical slice.

## User story

As a player using the Thunderbirds companion app, I want to transfer
characters between co-located Thunderbird Machines, so that the persisted
dashboard continues to match the physical board game.

## Scope

This slice allows a player to commit the board game's **Transfer Characters**
action. A request represents either one character moving into available
capacity or two characters exchanging places through a reciprocal swap.

GameState remains the source of truth for current character assignments and
Thunderbird Machine locations. Rules evaluates a complete snapshot supplied by
GameState and returns a stateless decision. ReferenceData supplies the static
character capacity of each Thunderbird Machine.

The operation updates the existing character-to-machine assignments. Character
locations remain derived from the locations of their assigned machines and are
not persisted independently.

## Behaviour

### Single transfer

A single assignment moves one character from its current Thunderbird Machine
to a different machine. The source and destination machines must occupy the
same map location, and the destination must have available character capacity.

### Reciprocal swap

Two assignments in one request must form a reciprocal swap. Each character's
destination must be the other character's current machine.

The swap is evaluated against its complete proposed final state. It is not
processed as two sequential transfers. This permits characters to exchange
places between full machines without creating an invalid intermediate state.

### Request cardinality

- Zero assignments is not a well-formed Transfer Characters request.
- One assignment represents a single transfer.
- Two assignments must form a reciprocal swap.
- Three or more assignments is not a well-formed Transfer Characters request.
- A character may occur no more than once in a request.

The request supplies each character and its destination machine. The source
machine is derived from the authoritative current GameState rather than being
provided redundantly by the client.

An indicative request is:

```json
{
  "assignments": [
    {
      "characterCode": "gordon",
      "destinationThunderbirdCode": "thunderbird-4"
    },
    {
      "characterCode": "alan",
      "destinationThunderbirdCode": "thunderbird-1"
    }
  ]
}
```

An indicative route is:

```http
POST /api/games/{gameId}/character-transfers
```

## Acceptance criteria

```gherkin
Feature: Transfer characters between Thunderbird Machines

Scenario: Transfer one character into available capacity
    Given Thunderbird 1 and Thunderbird 2 are at the same location
    And Scott is aboard Thunderbird 1
    And Thunderbird 2 has available character capacity
    When one Transfer Characters action assigns Scott to Thunderbird 2
    Then Scott should be aboard Thunderbird 2
    And Scott should no longer be aboard Thunderbird 1
    And the updated assignment should be persisted

Scenario: Reject a transfer between machines at different locations
    Given Scott is aboard Thunderbird 1
    And Thunderbird 1 and Thunderbird 2 are at different locations
    When one Transfer Characters action assigns Scott to Thunderbird 2
    Then the action should be rejected because the machines are not co-located
    And Scott should remain aboard Thunderbird 1

Scenario: Reject a transfer into a full machine
    Given Scott is aboard Thunderbird 1
    And Thunderbird 1 and Thunderbird 4 are at the same location
    And Thunderbird 4 is at its character capacity
    When one Transfer Characters action assigns Scott to Thunderbird 4
    Then the action should be rejected because Thunderbird 4 has no available character capacity
    And Scott should remain aboard Thunderbird 1

Scenario: Swap characters between full co-located machines
    Given Thunderbird 1 and Thunderbird 4 are at the same location
    And Scott and Gordon are aboard Thunderbird 1
    And Thunderbird 1 has a character capacity of 2
    And Alan is aboard Thunderbird 4
    And Thunderbird 4 has a character capacity of 1
    When one Transfer Characters action assigns Gordon to Thunderbird 4
    And assigns Alan to Thunderbird 1
    Then Scott and Alan should be aboard Thunderbird 1
    And Gordon should be aboard Thunderbird 4
    And both assignments should be persisted atomically

Scenario: Reject multiple assignments that are not a reciprocal swap
    Given Thunderbird 1, Thunderbird 2, and Thunderbird 4 are at the same location
    And Scott is aboard Thunderbird 1
    And Alan is aboard Thunderbird 4
    When one Transfer Characters action assigns Scott to Thunderbird 2
    And assigns Alan to Thunderbird 1
    Then the action should be rejected because its assignments do not form a reciprocal swap
    And Scott should remain aboard Thunderbird 1
    And Alan should remain aboard Thunderbird 4

Scenario: Transfer a character into Thunderbird 5
    Given Thunderbird 3 and Thunderbird 5 are at Geo-Stationary Orbit
    And Alan is aboard Thunderbird 3
    And Thunderbird 5 has available character capacity
    When one Transfer Characters action assigns Alan to Thunderbird 5
    Then Alan should be aboard Thunderbird 5
    And the updated assignment should be persisted

Scenario: Resume a game after transferring a character
    Given Scott has been transferred from Thunderbird 1 to Thunderbird 2
    When the game is retrieved again
    Then Scott should still be aboard Thunderbird 2
```

## Thunderbird Machine character capacities

Character capacity is an intrinsic property of a Thunderbird Machine and is
owned by ReferenceData. The property is named `CharacterCapacity` to
distinguish it from cargo and other future capacity concepts.

| Thunderbird Machine | Character capacity |
|---|---:|
| Thunderbird 1 | 2 |
| Thunderbird 2 | 3 |
| Thunderbird 3 | 3 |
| Thunderbird 4 | 1 |
| Thunderbird 5 | 5 |
| FAB 1 | 2 |

ReferenceData validation requires every Thunderbird Machine to define a
positive character capacity. The compiled snapshot and runtime catalogue expose
this value.

Thunderbird 2's cargo capability is separate from character capacity. Carrying
Thunderbird 4, FAB 1, or pod vehicles is outside this slice and must not be
represented by a generic capacity value.

## Location semantics

Transfers compare the actual map locations occupied by the source and
destination Thunderbird Machines.

Thunderbird 5 is a stationary, occupiable Thunderbird Machine located at
Geo-Stationary Orbit. Its top speed of zero prevents movement, but characters
may transfer into or out of it when another eligible machine is at the same
location.

Tracy Island is not a character destination in the currently supported game.
A character in the South Pacific remains aboard a Thunderbird Machine. Future
expansion support may introduce Tracy Island as an occupiable board asset, but
that requirement will determine its model when the expansion is implemented.

## Responsibility boundaries

### GameState

GameState owns and persists:

- the current machine assigned to every character;
- the current map location of every Thunderbird Machine;
- the atomic application of approved assignment changes; and
- the state/concurrency version protecting the aggregate.

The aggregate protects facts it can know locally: requested characters and
destination machines belong to the game, each character is assigned at most
once by the request, and either all approved assignments are applied or none
are.

The GameState integrity validator checks the completed state against
ReferenceData before persistence and after rehydration, including ensuring no
machine exceeds its character capacity.

### Rules

Rules owns the decision about whether the proposed Transfer Characters action
is permitted. It evaluates:

- whether the proposal represents a single transfer or reciprocal swap;
- whether the involved machines are co-located;
- whether the proposed final occupancies respect character capacity; and
- whether the proposal violates any other transfer rule introduced within this
  slice.

Rules remains stateless. It does not retrieve a game, receive a game Guid,
mutate assignments, persist an outcome, track a state version, or know whether
an approved proposal is subsequently committed.

Transfer validation is consumed internally through the GameState-to-Rules
gateway. There is no public Rules endpoint, Rules client method, WireMock Rules
endpoint, or experimental transfer page because the operation has no useful
standalone value without authoritative GameState.

### ReferenceData

ReferenceData owns reusable definitions, including each Thunderbird Machine's
`CharacterCapacity`. It does not own current occupancy or character
assignments.

## Application flow

The GameState application handler performs the following orchestration:

1. Load the authoritative game aggregate.
2. Validate the integrity of the current state.
3. Derive each requested character's source machine from the game.
4. Construct a transfer proposal containing current character assignments,
   current machine locations, and proposed destination assignments.
5. Ask the internal Rules gateway to evaluate the proposal.
6. Reject the request without mutation when Rules returns an invalid decision.
7. Apply every approved assignment atomically to the aggregate.
8. Validate the integrity of the resulting state.
9. Persist the aggregate using optimistic concurrency.
10. Return the updated game state.

If the game changes after Rules evaluates the proposal but before persistence,
the state version causes the stale update to fail with a conflict rather than
overwriting the newer state.

## HTTP outcomes

| Condition | Outcome |
|---|---:|
| Transfer succeeds | `200 OK` |
| Missing, zero, or more than two assignments | `400 Bad Request` |
| Duplicate character in the request | `400 Bad Request` |
| Unknown game, character, or Thunderbird Machine | `404 Not Found` |
| Machines are not co-located | `422 Unprocessable Entity` |
| Destination lacks character capacity | `422 Unprocessable Entity` |
| Two assignments do not form a reciprocal swap | `422 Unprocessable Entity` |
| Game state changed concurrently | `409 Conflict` |

Problem responses must not expose the game Guid, private URL, persisted state,
or internal implementation details.

## Testing strategy

### ReferenceData

- Validate that character capacity is positive and present for every machine.
- Compile and expose the expected capacity for each known machine.

### Rules

- Unit-test the pure transfer evaluator across single-transfer, co-location,
  capacity, reciprocal-swap, duplicate, and cardinality cases.
- Test Thunderbird 5 as a valid transfer participant.
- Evaluate capacity against a copied proposed final state rather than through
  sequential mutation.

### GameState domain and application

- Unit-test atomic assignment application and local structural protection.
- Unit-test handler orchestration with substituted Rules and persistence
  boundaries.
- Confirm rejected decisions do not mutate or persist the aggregate.
- Confirm concurrency conflicts propagate as controlled conflicts.

### Infrastructure and persistence

- Component-test the in-process Rules gateway with the real Rules and
  ReferenceData implementations.
- Extend GameState integrity-validation tests with capacity violations.
- PostgreSQL integration-test a single transfer and a reciprocal swap.
- Confirm both sides of a swap persist together and survive rehydration.

The existing database structure already stores character-to-machine
assignments, so this slice is not expected to require a GameState schema
migration.

### API, client, and UI

- Component-test success, malformed requests, rejected actions, unknown
  resources, and concurrency conflicts at the API boundary.
- Cover the published GameState client contract and WireMock support.
- Component-test the transfer interaction and refreshed dashboard presentation.
- Add an API user journey proving transfer and later retrieval.
- Add a browser journey proving transfer through the dashboard and persistence
  after reload.

## Non-goals

- Player-controlled character selection
- Active-player enforcement
- Turn order or action-point accounting
- Charging an action point for a transfer
- Character assignment to Tracy Island
- Thunderbird 2 cargo transport
- Carrying Thunderbird 4 or FAB 1
- Pod-vehicle construction, loading, transport, or deployment
- A public stateless Transfer Characters Rules endpoint
- A standalone transfer experiment page

## Follow-on stories

- Record player-controlled characters and apply John's alternative Thunderbird
  3 starting position.
- Track turn order, the active player or character, and remaining action points.
- Make committed movement and Transfer Characters actions consume action
  points.
- Transport Thunderbird 4 or FAB 1 within Thunderbird 2.
- Construct, load, transport, and deploy pod vehicles.
- Introduce Tracy Island as an occupiable board asset if required by expansion
  support.
