# Reference Data

## Overview

The ReferenceData subsystem provides immutable, in-memory reference data used by the Thunderbirds Board Game Engine.

Examples include:

- Disaster definitions
- Character definitions
- Location definitions
- Thunderbird definitions
- Pod vehicle definitions

Reference data is generated at development/build time from source spreadsheets and embedded into the application as a versioned snapshot.

The runtime does not call a separate Catalog API for static game data.

---

## Architecture

The system consists of two main parts:

### Compiler

The ReferenceData compiler:

1. Reads source spreadsheets
2. Validates the source data
3. Builds a strongly typed snapshot
4. Serializes the snapshot to JSON

### Runtime

At startup the runtime:

1. Loads the embedded snapshot
2. Validates snapshot compatibility
3. Builds immutable in-memory catalogs
4. Exposes read-only lookup interfaces to consumers

---

## Runtime Catalogs

The following catalogs are publicly exposed to consumers:

- `IDisasterDefinitionCatalog`
- `ICharacterDefinitionCatalog`
- `ILocationDefinitionCatalog`
- `IDisasterBonusKeyDefinitionCatalog`

Catalogs provide:

- `GetAll()`
- `GetByCode(...)`

All catalogs are immutable and registered as singletons.

---

## Snapshot Metadata

Snapshots contain the following metadata:

| Field | Purpose |
|---|---|
| `schemaVersion` | Defines the snapshot contract shape |
| `contentVersion` | Defines the reference data version |
| `generatedAt` | Snapshot generation timestamp |
| `generatorVersion` | Version of the compiler |

### Schema Version

The schema version changes when the snapshot structure changes.

Examples:

- Adding a new field
- Renaming a field
- Changing serialization shape

### Content Version

The content version changes when the reference data changes without changing the schema.

Examples:

- Adding a new disaster
- Correcting display text
- Updating balance values

---

## Bonus Location Semantics

Disaster bonuses may optionally define a location.

If the location is omitted (`null`), the bonus applies at the disaster location itself.

This avoids duplicated source data and prevents location drift between disasters and bonuses.

The compiler validates that a bonus location is not explicitly set to the same location as the owning disaster.

---

## Future Evolution

The current implementation uses embedded snapshots.

Future phases may introduce:

- Snapshot archives
- Snapshot publishing
- Cloud-backed snapshot providers
- Snapshot checksum manifests

These concerns are intentionally out of scope for the current implementation.