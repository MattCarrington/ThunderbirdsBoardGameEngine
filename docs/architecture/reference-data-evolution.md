# Reference Data Evolution Plan

- Status: Draft roadmap; construction boundary implemented, remaining delivery planned
- Date: 2026-10-22
- Policy updated: 2026-10-27

## Purpose

Give Reference Data an independent production and release lifecycle, with automated snapshot production and versioned delivery through Azure Blob Storage. Applications continue to consume immutable definitions through Runtime catalogs.

Practical Azure experience is an explicit objective, including infrastructure as
code, authentication, artifact publishing, and reliable retrieval. A separate
Reference Data repository with its own CI and release lifecycle, and delivery
through Blob storage, are intended scope. Initially preserve embedded delivery,
then publish the same verified artifact through both paths while migrating.

## Current position

[ADR 0000](decisions/0000-snapshot-based-reference-data.md) records the original
accepted snapshot architecture. This plan builds on that model; future Blob
delivery decisions must explicitly revise its embedded-delivery assumptions.

The [Reference Data overview](../../src/ReferenceData/Readme.md) describes the implemented subsystem. The [compiler guide](../../tools/ReferenceData/README.md) owns authoring, relationship validation, and the current manual snapshot-update procedure.

The compiler generates a local JSON file. Runtime embeds a separately copied snapshot and requires an exact schema-version match. Core and Runtime are published packages, but production projects can integrate through project references. Package selection and publishing share the repository's [release infrastructure](../PackageReleases.md).

Definition construction is now restricted under ADR 0007. Compilation, snapshot
copying, and version updates still require manual coordination. Shared builds
can hide the compatibility effects of changing multiple projects together.

Application deployment verification remains a separate responsibility; see the
[smoke-test guide](../../tests/SmokeTests/Readme.md) and
[deployment ADR](decisions/0006-separate-dev-validation-and-deployment.md).
Reference Data CI must have explicit production and publication gates rather
than being coupled to application deployment smoke tests.

## Intended boundaries

- **Reference Data production:** validate authoring input, compile snapshots, verify Runtime hydration, and publish immutable versioned artifacts.
- **Application integration:** verify selected packages and snapshots support required identities, relationships, and game behaviour.
- **Deployment verification:** verify the deployed application can access its configured snapshot and serve its essential journeys.

The accepted [versioning and compatibility ADR](decisions/0008-reference-data-versioning-and-compatibility.md)
defines exact application-selected content versions, schema validation,
immutable publication, retention/provenance, and future game pinning. Publishing
does not automatically adopt a snapshot. Game pinning and historical catalogue
resolution are not implemented and are separate from repository extraction.

## Change model

| Change | Example | Expected impact |
|---|---|---|
| Existing content | Adjust a machine's speed | New snapshot; assess gameplay impact |
| Additional supported entry | FAB 2: speed 2, character capacity 2, Earth domain | New snapshot if existing consumers handle additional identities and behaviour |
| Definition contract | Add `CargoCapacity` | Coordinated model, compiler, package, and snapshot changes as needed |
| Delivery implementation | Introduce Blob retrieval | Runtime and infrastructure changes as needed |

Core and Runtime may release independently, with explicit compatible dependencies. Content can release independently when the existing code and schema support it. Independent versioning does not promise compatibility between every combination.

## Proposed delivery order

1. **Owned construction:** the [accepted construction-boundary ADR](decisions/0007-reference-data-owned-construction.md) is implemented, preserving compiler, deserialization, and test paths.
2. **Versioning and compatibility:** policy is recorded in ADR 0008. This design slice does not implement the capabilities it describes.
3. **Repository extraction:** create the independently buildable/testable Reference Data repository and basic CI, preserving embedded behaviour. Resolve shared test dependencies. Keep the application buildable until package migration is verified.
4. **Automated embedded publication:** validate and compile input, verify hydration, generate provenance and checksum evidence, retain immutable artifacts, and package the same verified snapshot without manual copying/resource changes.
5. **Package integration:** move application consumption to published packages, add consumer and API compatibility checks, and remove superseded projects and pipeline steps after verification.
6. **Blob publishing:** provision Azure resources through code and publish versioned artifacts/manifests using authenticated automation alongside embedded delivery.
7. **Runtime retrieval:** implement explicit exact-version selection, authentication, manifest/integrity and schema checks, caching, diagnostics, and failure/recovery behaviour. Resolve supported hosts, including browser-hosted consumers and access constraints.
8. **Application adoption and workflow demonstration:** switch deployment to Blob, verify a content-only update and contract change, and demonstrate compatible default rollback. Record whether embedded delivery remains supported. Game-specific rollback behaviour can only be demonstrated once the separate pinning work exists.

Each slice includes appropriate tests and documentation. The order expresses dependencies, not a requirement to implement the entire plan as one change.

## Architecture records

ADRs 0007 and 0008 record owned construction and versioning/compatibility.
Use focused later ADRs for repository/package release boundaries and Blob
delivery mechanics. Extraction is intended scope; its migration mechanics still
need design. Implementation status belongs in current architecture and
operational documentation, distinct from an accepted policy.

## Implementation follow-ups from ADR 0008

These are backlog requirements, not claims of existing support or newly created
board cards. They can be split into reviewable stories without reopening the
agreed policy.

| Follow-up | Required outcome |
|---|---|
| Publication validation and immutable identity | Block invalid releases; classify content increments; prevent reuse of a content version for different bytes; review actual identity and game-fact changes |
| CI provenance and retention | Generate the companion SHA-256 manifest with source/compiler revisions, publication/run identity, and durable validation evidence; retain the exact artifacts and referenced sources indefinitely |
| Historical snapshot availability | Distribute and retrieve exact supported historical versions; keeping files in the repository alone does not fulfil this |
| Version-specific catalogue resolution | Resolve definitions from a requested version without substituting a global default; report unavailable or unsupported versions explicitly |
| Pin persisted games to an immutable Reference Data version | Persist the creation version through a database migration; use it for relevant reads, validation, projections, and actions; define a non-guessing policy for pre-existing unversioned games |
| Schema-support changes and game retirement/migration | For each schema transition, explicitly preserve support, implement a justified conversion/migration, or report unsupported games without modifying their saved state |
| Default and code rollback verification | A changed default affects new games after pinning exists; existing games retain their versions; code rollback must retain required schema support or return a controlled outcome |

The pinning story depends on historical availability and version-specific
resolution. It is a persisted-game consistency concern that exists even with
embedded delivery, not an automatic consequence of extraction or Blob storage.
Do not claim it is complete merely because a game stores a version identifier.

## Completion evidence

- An automated process validates, compiles, verifies, and publishes a versioned snapshot to Azure Blob Storage.
- A deployed application loads an explicitly selected compatible snapshot through Runtime and exposes definitions through catalogs.
- A supported content-only addition requires no Core or Runtime rebuild.
- Incompatible artifacts fail with actionable diagnostics; retrieval failures and recovery follow documented policies.
- Snapshot rollback is demonstrated, including the agreed treatment of existing games.
- Consumer construction is restricted and package compatibility is checked.
- Reference Data, application integration, and deployment checks have explicit owners and release gates.

## Documentation ownership

Keep the versioning decision authoritative in ADR 0008, with a short consumption
and metadata summary in the Reference Data overview. Keep compiler operation and
authoring policies in its guide. Keep general package publication mechanics and
deployed smoke-test operation in their existing guides. Link these documents
rather than duplicating them. Consolidate or remove obsolete material as each
slice lands and update inbound links.
