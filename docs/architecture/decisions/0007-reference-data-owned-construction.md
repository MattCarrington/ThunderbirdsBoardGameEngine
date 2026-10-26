# ADR 0007: Reference Data Infrastructure Owns Definition Construction

- Status: Accepted
- Date: 2026-10-22
- Implemented: 2026-10-26

## Context

Applications consume immutable Reference Data definitions through Runtime catalogs. Explicit production construction currently occurs in the compiler; Runtime also creates definitions through snapshot deserialization. Automated tests construct representative definitions and invalid inputs.

Core nevertheless exposes public definition constructors. PR #156 added `CharacterCapacity` and replaced the Thunderbird definition constructor with one requiring another argument. Catalogue readers could continue using the existing properties, but the public constructor change broke the exposed construction contract. Project references allowed owned callers to be updated together, making this consumer-facing compatibility obligation less visible.

Adding required construction inputs will remain normal as definitions evolve. Applications should not acquire that obligation solely because they read definitions.

## Decision

Restrict definition construction to owned Reference Data infrastructure and supported test tooling. Consumers obtain definitions through Runtime catalogs and retain public read access to supported immutable properties.

Apply the boundary consistently across reference definitions. Do not replace public constructors with an unrestricted public factory exposing equivalent construction inputs.

Retain supported paths for complete compiler construction, Runtime deserialization, and representative test data. Production constructors will not remain public solely for tests. Core construction-validation tests must still be able to exercise invalid inputs through an appropriate owned path.

Use internal constructors with explicit `JsonConstructor` attributes on Core
definitions and their bonus/reward values. System.Text.Json on the supported
.NET 10 target can hydrate those constructors without making them public or
introducing a separate serialization model. Existing snapshot JSON remains
compatible; this change does not alter its schema.

Core grants narrowly scoped `InternalsVisibleTo` access to the compiler, Core
unit tests, and shared TestUtils assembly. Runtime grants access to TestUtils
for its derived disaster-bonus-key definition, in addition to its existing
Runtime unit-test access. Other test assemblies use
`ReferenceDefinitionFactory`, shared fixtures, or `ReferenceDataSnapshotBuilder`
from TestUtils. No production factory is introduced and Runtime does not need
friend access to Core for deserialization.

Keep constructors public on typed identities, which consumers use for lookups.
`ReferenceDataSnapshot` remains a public serialization/compilation envelope;
it is not a catalogue definition. This boundary controls the supported API,
not hostile reflection or arbitrary use of JSON deserialization.

Definitions expose get-only properties. Disaster definitions copy bonus and
reward collections into immutable arrays rather than retaining mutable input
lists. The formerly positional reward and Runtime bonus-key records retain
read/deconstruction support but no longer expose public init setters.

## Consequences

- Removing existing public constructors is itself a source and binary compatibility change and requires an explicit package-release decision.
- Core and Runtime move from `3.0.1` to `4.0.0`. Existing binaries calling
  removed public constructors must migrate and rebuild; callers using the
  removed init setters must also migrate. Catalogue readers retain their
  property and deconstruction APIs. Snapshot metadata is unchanged.
- Future required construction inputs can change within owned infrastructure without exposing constructor changes to catalogue consumers.
- Changes to public properties, semantics, snapshot schemas, or gameplay can still require coordinated or breaking releases.
- Compiler generation, Runtime hydration, public API visibility, and test-data creation must be verified before release.
- The work does not require Blob storage, pipeline replacement, package-reference migration, or repository extraction.

## Related documentation

- [Reference Data overview](../../../src/ReferenceData/Readme.md)
- [Reference Data evolution plan](../reference-data-evolution.md)
- [PR #156](https://github.com/MattCarrington/ThunderbirdsBoardGameEngine/pull/156)
