# ADR 0005: Reference Data Infrastructure Owns Definition Construction

- Status: Proposed
- Date: 2026-10-22

## Context

Applications consume immutable Reference Data definitions through Runtime catalogs. Explicit production construction currently occurs in the compiler; Runtime also creates definitions through snapshot deserialization. Automated tests construct representative definitions and invalid inputs.

Core nevertheless exposes public definition constructors. PR #156 added `CharacterCapacity` and replaced the Thunderbird definition constructor with one requiring another argument. Catalogue readers could continue using the existing properties, but the public constructor change broke the exposed construction contract. Project references allowed owned callers to be updated together, making this consumer-facing compatibility obligation less visible.

Adding required construction inputs will remain normal as definitions evolve. Applications should not acquire that obligation solely because they read definitions.

## Decision

Restrict definition construction to owned Reference Data infrastructure and supported test tooling. Consumers obtain definitions through Runtime catalogs and retain public read access to supported immutable properties.

Apply the boundary consistently across reference definitions. Do not replace public constructors with an unrestricted public factory exposing equivalent construction inputs.

Retain supported paths for complete compiler construction, Runtime deserialization, and representative test data. Production constructors will not remain public solely for tests. Core construction-validation tests must still be able to exercise invalid inputs through an appropriate owned path.

The implementation must assess Core-owned internal factories, explicit serialization configuration, and narrowly scoped `InternalsVisibleTo`. This ADR establishes ownership; the concrete mechanism remains to be selected after checking serializer and assembly requirements.

## Consequences

- Removing existing public constructors is itself a source and binary compatibility change and requires an explicit package-release decision.
- Future required construction inputs can change within owned infrastructure without exposing constructor changes to catalogue consumers.
- Changes to public properties, semantics, snapshot schemas, or gameplay can still require coordinated or breaking releases.
- Compiler generation, Runtime hydration, public API visibility, and test-data creation must be verified before release.
- The work does not require Blob storage, pipeline replacement, package-reference migration, or repository extraction.

## Related documentation

- [Reference Data overview](../../../src/ReferenceData/Readme.md)
- [Reference Data evolution plan](../reference-data-evolution.md)
- [PR #156](https://github.com/MattCarrington/ThunderbirdsBoardGameEngine/pull/156)
