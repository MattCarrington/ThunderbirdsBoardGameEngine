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

From the repository root, run the compiler from its project directory so its relative input and output paths resolve correctly:

```powershell
Push-Location tools/ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData.Compiler
try {
    dotnet run
} finally {
    Pop-Location
}
```

The current entry point reads `ReferenceData.xlsx` and writes `snapshot.json`, both relative to the process working directory. It has no command-line path configuration. Building the application does not run this compiler.

### Updating the embedded snapshot

1. Update the authoritative spreadsheet in the compiler project.
2. Assess the schema and content changes using the [shared versioning rules](../../src/ReferenceData/Readme.md#snapshot-metadata). Update the generation constants in `SnapshotVersions.cs` as appropriate; generator and content versions are currently maintained manually.
3. Run the compiler as above. If compilation fails, correct the source or compiler before proceeding; an existing output file is not evidence of a successful run.
4. Review the generated snapshot and its metadata. Copy the successfully generated `snapshot.json` to `src/ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData.Runtime/snapshot.json`, replacing the embedded source. Retain a prior snapshot under `src/ReferenceData/Archive` when needed for compatibility coverage; archive policy is not yet automated.
5. Run the relevant Reference Data unit and component tests, including Runtime loading checks, and any application tests affected by changed definitions or identities.
6. Review package compatibility and declared package versions using [package-release guidance](../../docs/PackageReleases.md). Commit the source, generated artifacts, metadata, and related code/test changes together.

The compiler does not copy its output into Runtime, archive previous snapshots, choose package versions, or publish artifacts. Those are currently separate steps. Their automation is part of the proposed [evolution plan](../../docs/architecture/reference-data-evolution.md).

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

The authoritative explanation of schema, content, and generator versions is in [Reference Data snapshot metadata](../../src/ReferenceData/Readme.md#snapshot-metadata). Package version selection follows [package-release guidance](../../docs/PackageReleases.md).

The accepted [versioning and compatibility policy](../../docs/architecture/decisions/0008-reference-data-versioning-and-compatibility.md)
applies content increment rules prospectively. Classify changes by the compiled
result: correcting an authored name can also change a generated code, making
the change an identity change rather than a display-only correction.

Future CI publication will validate and load the artifact, generate a companion
SHA-256/provenance manifest, and publish immutable versions. The current compiler
does not generate that manifest or enforce publication uniqueness. Existing
snapshots must not be edited or regenerated to apply the policy retroactively.

---

## Runtime Relationship

The compiler produces the snapshot consumed by the ReferenceData runtime subsystem.

The runtime treats generated snapshots as trusted artifacts.
