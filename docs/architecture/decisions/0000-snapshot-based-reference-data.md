# ADR 0000: Snapshot-based Reference Data for Static Game World

- Status: Accepted
- Original decision date: Not recorded in the source document
- Imported into the repository: 2026-10-26

## Context

The Thunderbirds Board Game Engine contains a significant amount of static
game data, such as disaster cards, characters, and board topology. This data
defines the game world and does not change during gameplay.

Previously, this data was delivered via the Catalog bounded context as a
runtime service (APIs, SDK calls, in-memory reference sources). This introduced
unnecessary runtime indirection, asynchronous loading, failure modes, and test
complexity for data that is immutable by nature.

The UI and Rules contexts also require different representations of this data:

- Rules require only the data necessary for decision-making.
- UI requires human-readable, presentable shapes.

Treating this static data as "live" runtime data blurred responsibilities and
increased complexity.

## Decision

We will treat all static game world data as reference data snapshots.

The Catalog bounded context remains responsible for authoring, validating,
and generating JSON representations of the game world.

A new ReferenceData library represents the runtime form of the Catalog
bounded context.

ReferenceData embeds the generated JSON, loads it at application startup,
validates it, and exposes read-only, in-memory catalogs.

Runtime contexts (Rules, UI, etc.) consume this reference data in-process,
without HTTP calls or SDKs.

Each consuming context derives its own projections from the reference data:

- Rules project the data into decision-oriented shapes.
- UI projects the data into presentation-oriented shapes.

The reference data itself remains in a single canonical shape that represents
facts, not behaviour or presentation.

## Consequences

- Static game data is loaded once at startup and is immutable during runtime.
- Rules become deterministic: given the same snapshot and inputs, outcomes
  are repeatable.
- UI complexity is reduced by removing asynchronous loading and failure
  handling for static data.
- Tests can rely on real reference data snapshots instead of duplicating
  data in fakes.
- The Catalog bounded context is no longer a runtime dependency, but a
  producer of versioned snapshots.
- Future separation into multiple deployables remains possible without
  changing the model.

## Record history and related documentation

This record was imported from the existing Word document
*Snapshot-based Reference Data for Static Game World*, originally labelled
ADR-001. It is numbered 0000 here to retain the repository's existing ADR
numbers. The decision above preserves the original rationale and terminology;
the import does not constitute a new decision or approval of future changes.

The [Reference Data overview](../../../src/ReferenceData/Readme.md) describes
the current implementation, including compiler ownership and loading when
catalogs are first resolved. The original startup wording above is retained
as part of the historical decision.

The [evolution plan](../reference-data-evolution.md) proposes an independent
release lifecycle and Blob delivery. A future delivery ADR must explicitly
identify which embedded-delivery and loading assumptions it revises while
retaining the snapshot and catalogue consumption model.
