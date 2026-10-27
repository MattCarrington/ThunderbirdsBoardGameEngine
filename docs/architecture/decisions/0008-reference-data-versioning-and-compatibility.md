# ADR 0008: Reference Data Versioning and Compatibility

- Status: Accepted policy; implementation is incremental
- Date: 2026-10-27

## Context

Reference Data has several independent contracts. Core and Runtime expose a
package API, snapshots have a serialized structure, and their contents identify
game elements and supply game facts. A change can affect one or several of
these contracts. Their version numbers must not be treated as interchangeable.

For example, introducing required character capacity moved the snapshot from
schema 3 to schema 4. Content version 1.3.0 identifies the resulting dataset.
The public constructor change also broke the package API; restricting
construction in [ADR 0007](0007-reference-data-owned-construction.md) prevents
future infrastructure constructor changes from unnecessarily becoming consumer
API changes. Adding a readable property can remain additive for catalogue readers.

Today Runtime embeds one snapshot and requires an exact schema match. Archived
snapshots are repository files, not selectable Runtime artifacts. Games are not
pinned to historical snapshots. Package updates can therefore replace the facts
used to interpret existing persisted games.

The planned independent release lifecycle and Blob delivery need explicit
selection, publication, compatibility, and retention policies. These policies
also expose a persisted-game consistency problem that exists with embedded
delivery and is independent of repository extraction.

## Decision

### Version responsibilities

| Version | Responsibility |
|---|---|
| Core and Runtime package versions | Public code contracts and declared dependencies |
| Snapshot schema version | The structure and interpretation supported by the snapshot reader |
| Snapshot content version | Globally unique identifier of one immutable published dataset |
| Generator version | The compiler release that produced the artifact |

Content versions are unique across the Reference Data publication history,
including across different schemas. A content version must never identify two
different published artifacts. A schema change does not reset the content
version sequence. Retrying publication may verify an already published identical
artifact, but must not overwrite it with different bytes.

The existing `schemaVersion`, `contentVersion`, `generatedAt`, and
`generatorVersion` metadata remain part of the snapshot. No published snapshot
is renumbered or regenerated to fit this decision retrospectively.

### Content version increments

Apply the following project convention to releases prepared after adoption of
this ADR. Use the most significant applicable increment and reset subordinate
components to zero for major or minor increments.

| Increment | Meaning | Examples |
|---|---|---|
| Patch | Corrections to presentation or representation that preserve existing identities and gameplay facts | Correct "Tunderbird 2" without changing its code |
| Minor | Add content while preserving existing definitions' identities and gameplay facts | Add FAB 2 using existing speed, capacity, and domain fields |
| Major | Remove or replace identities, change existing gameplay facts, or introduce facts that alter existing definitions' behaviour | Remove North America; change speed or capacity; introduce character-capacity constraints |

Classify the actual compiled change, not its intention. A correction to an
incorrect capacity still changes gameplay facts and is major. A display-name
correction that also changes a generated identity is not presentation-only.
Review references and consumer assumptions when adding identities; a minor
increment does not prove that every consumer can handle the new entry.

Schema compatibility is determined separately. A new required field changes
the schema; introducing character capacity also qualifies as a major content
change because it adds a gameplay constraint. A serialization reorganisation
preserving identities and facts does not require a content major merely because
the schema changes. Any newly published artifact still receives a new globally
unique content version.

Historical content 1.3.0 remains unchanged. Under the new convention, an
equivalent future introduction of character capacity would be major. This is a
prospective policy, not a declaration that earlier numbering was erroneous.

### Application selection and Runtime compatibility

The application owns snapshot selection. For independently delivered snapshots,
configuration specifies an exact content version. Runtime locates that artifact,
validates its identity, integrity, and supported schema, then exposes definitions
through catalogues. It does not choose the latest version, resolve a floating
range such as `1.4.x`, or silently fall back to another dataset.

Publishing a snapshot makes it available; adopting it is a separate application
configuration/deployment action. That adoption may be automated after testing.
Embedded delivery currently selects the snapshot through its containing package;
this ADR does not imply that configurable snapshot selection already exists.

Keep exact-schema support initially: the installed Runtime defines the schema
it understands, not user configuration. Schema 4 is the current supported
baseline. Supporting older schemas requires a deliberate implementation and
compatibility decision; retaining their files does not make them supported.
Never infer missing game facts merely to make an older schema load.

Structural compatibility does not establish content suitability. Removing North
America can invalidate a game's location references without changing the schema;
removing `Domain` changes the serialized contract and can require model and
application changes. Consumer checks must cover required identities and facts
as well as the structural check.

### Package releases

Core and Runtime may have different versions. Runtime declares the compatible
Core dependency it requires. A Runtime release is needed when its implementation
or dependency requirements change, including adoption of new Core capabilities.
A Runtime-only implementation change does not require a Core release. A Core
release does not mechanically require a Runtime release if existing compatibility
and dependency requirements remain sufficient.

Assess package API compatibility independently from schema and content changes.
Equal-looking package and schema numbers do not establish a relationship.

### Publication and provenance

Automated publication must gate release on parsing, required structure and
metadata, permitted values, identity and relationship validation, and loading
through the intended Runtime. Review and independent behavioural expectations
remain necessary: valid data can contain an incorrect but permitted speed,
capacity, boolean, or display name.

Once published, a snapshot is immutable even if mistakes are later discovered.
Corrections produce a new version using the content-impact rules above.

Generate a companion release manifest after compilation and verification. It
must identify the content and schema versions and record:

- SHA-256 of the exact final snapshot bytes;
- authoring source repository and commit, with input paths identifying the source;
- compiler release and source repository/commit;
- publication time and CI run identity/reference;
- retained validation summary identifying the checks and their results.

The checksum lives outside the snapshot it hashes. Publish the verified bytes
and manifest as one logical release; a release must not be consumable as complete
until both are available and consistent. The publishing implementation will
choose the mechanism. Embedded and Blob outputs must derive from the same
verified artifact during dual delivery.

Retrieval must check the artifact against its manifest and Runtime's supported
contract. Publication validation does not replace retrieval checks. This policy
does not claim that current Runtime already performs manifest verification.

### Historical retention

Retain every published snapshot and its release manifest indefinitely. Preserve
the referenced authoring source, compiler source, and schema models/descriptions
through recoverable source history or release archives. Preserve formal schema
files too if they are introduced; no separate JSON Schema artifact is assumed
to exist today.

CI logs and temporary build artifacts may expire. Essential validation evidence
must remain with the release rather than depending only on an expiring run URL.
Retain the exact artifact instead of relying on future regeneration: generation
timestamps or compiler/environment differences can change its bytes.

Preserve existing archives as they are. New publication evidence applies
prospectively; do not invent missing historical provenance or claim that an old
artifact passed checks for which no evidence remains.

Retention is an audit and availability commitment, not a promise that every
Runtime release supports every historical snapshot. Storage layout, access,
and publication mechanics belong to the later delivery work.

### Persisted games and rollback

Adopt game pinning as a target policy, implemented in a separate story. A new
game records the exact content version selected at creation. Reads, integrity
validation, projections, and actions use that game's snapshot where relevant.
Do not confuse this identifier with setup version or schema version.

Changing the application's selected default, including rolling it back, affects
new games. It does not change existing games' pinned versions. Moving an
existing game to different data requires an explicit validated migration;
automatic migration is not part of this decision.

A code rollback must preserve support for the snapshots required by games that
are to remain playable. If a pinned artifact is missing, corrupt, or uses an
unsupported schema, return a controlled outcome and leave the persisted game
unchanged. Never silently substitute the default snapshot or invent facts.

As the model evolves, a schema change may make earlier games unsupported. Each
such release must explicitly decide whether to retain compatibility, provide a
meaning-preserving conversion or game migration, or retire support with a clear
outcome. Pinning data does not pin executable Rules behaviour; new rule code
may need facts that old snapshots do not contain.

Existing games without a recorded version require an explicit backfill or
unsupported-game policy. Do not assume the current snapshot was their original
one. The game-pinning story must resolve this during implementation design.

## Consequences

- Version selection and adoption are repeatable without conflating package,
  structural, and gameplay compatibility.
- A gameplay correction can require a major content release even when the JSON
  structure and package APIs are unchanged.
- Immutable publication and traceability require new CI capabilities; the
  current compiler and manual archive procedure do not supply them yet.
- Pinning requires historical snapshot distribution and version-specific
  catalogue resolution, not merely a version column in GameState.
- Supporting every historical schema indefinitely is not required. Dropping
  support must be explicit, observable, and safe for saved state.
- This policy does not implement Blob retrieval, extraction, manifests, game
  pinning, migrations, or dynamic refresh. Their implementation stories remain
  separate from this design decision.

## Related documentation

- [Foundational snapshot decision](0000-snapshot-based-reference-data.md)
- [Reference Data overview](../../../src/ReferenceData/Readme.md)
- [Compiler operation](../../../tools/ReferenceData/README.md)
- [Package releases](../../PackageReleases.md)
- [Evolution plan and implementation follow-ups](../reference-data-evolution.md)
