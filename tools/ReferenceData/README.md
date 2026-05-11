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
dotnet run --project src/ReferenceData.Compiler
```

The generated snapshot is written to the configured output location.

---

## Source of Truth

Spreadsheet source files are the authoritative source of reference data.

Generated snapshot files should not be manually edited.

---

## Validation Rules

The compiler validates:

- Duplicate codes
- Invalid references
- Invalid enum values
- Invalid bonus relationships
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