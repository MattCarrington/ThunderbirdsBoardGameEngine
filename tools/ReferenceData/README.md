# Reference Data Compiler

## Overview

The ReferenceData compiler generates the runtime snapshot consumed by the application.

The compiler transforms spreadsheet-based source data into a validated JSON snapshot.

---

## Responsibilities

The compiler is responsible for:

- Reading source spreadsheets
- Normalizing source data
- Validating source invariants
- Building the reference data snapshot
- Writing the serialized snapshot

---

## Running the Compiler

Run the compiler with:

```bash
dotnet run --project tools/ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData.Compiler
```

The generated snapshot is written to the configured output location.

---

## Source of Truth

Spreadsheet source files are the authoritative source of reference data.

Generated snapshot files should not be manually edited.

## Typed Reference Relationship Policy

The reference data snapshot is a graph of immutable game definitions connected
by typed references. The compiler establishes the integrity of those references
before writing the snapshot.

### Definitions Create Identities

When an entity is defined, the compiler may derive its canonical, strongly typed
identifier from its authored name. For example, the location name `New York` may
define `LocationCode("new-york")`.

Identifier generation belongs to the entity being defined.

### Relationships Resolve Identities

When one entity refers to another, the compiler must not independently generate
the target identifier from the authored relationship value. It must resolve the
authored value against the authoritative target set permitted by that field and
use the matched entity's canonical identifier.

For example, a disaster location authored as `New York` must resolve against the
defined locations and reuse the existing `LocationCode("new-york")`.

Definitions create their own identities. Relationships resolve and reuse those
identities.

### Resolution Rules

Every explicit relationship must resolve to exactly one permitted target:

- No matches cause compilation to fail because the relationship is missing.
- One match supplies the canonical typed identifier stored in the snapshot.
- Multiple matches cause compilation to fail because the relationship is
  ambiguous.

Normalization may assist matching, but normalization and slugification are not
evidence that a target exists.

### Permitted Target Sets

Each relationship defines the types it is permitted to target:

- Disaster locations resolve against locations.
- Optional disaster bonus locations resolve against locations.
- Both endpoints of a map edge resolve against locations.
- Disaster bonus targets resolve against the union of characters, Thunderbirds,
  and pod vehicles.

An entity existing elsewhere in reference data does not make it a valid target
for a particular field.

Future reference-data elements must declare their permitted target sets when
their relationships are introduced. They should not be implemented before the
game requires them.

### Optional and Inherited Relationships

An absent relationship is valid only when the game rules explicitly define it
as optional or inherited.

A disaster bonus location follows this rule:

```text
effective bonus location = explicit bonus location ?? disaster location
```

An empty bonus location therefore means that the asset must be at the disaster
location. A populated bonus location must resolve to an existing location and
must differ from the disaster location.

### Snapshot Integrity

Invalid relationships must not enter the constructed snapshot. Snapshot models
store canonical typed identifiers rather than unresolved authored names or
duplicated related definitions.

---

## Validation Rules

The compiler validates:

- Duplicate codes
- Invalid references
- Invalid enum values
- Invalid bonus relationships
- Missing or ambiguous relationship targets
- Snapshot metadata
- Bonus location normalization rules

Compilation fails if invalid data is detected.

---

## Versioning

### Schema Version

Increment when the snapshot contract changes.

Examples:

- New fields
- Changed serialization shape
- Renamed properties

### Content Version

Increment when reference data changes without schema changes.

Examples:

- New disasters
- Balance updates
- Corrected display names

---

## Runtime Relationship

The compiler produces the snapshot consumed by the ReferenceData runtime subsystem.

The runtime treats generated snapshots as trusted artifacts.
