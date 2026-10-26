# Reference Data Evolution Plan

- Status: Draft roadmap; construction boundary implemented, remaining delivery planned
- Date: 2026-10-22

## Purpose

Give Reference Data an independent production and release lifecycle, with automated snapshot production and versioned delivery through Azure Blob Storage. Applications continue to consume immutable definitions through Runtime catalogs.

Practical Azure experience is an explicit objective, including infrastructure as code, authentication, artifact publishing, and reliable retrieval. Blob delivery is intended scope. A separate solution or repository is an option to evaluate, not a prerequisite for the first change.

## Current position

[ADR 0000](decisions/0000-snapshot-based-reference-data.md) records the original
accepted snapshot architecture. This plan builds on that model; future Blob
delivery decisions must explicitly revise its embedded-delivery assumptions.

The [Reference Data overview](../../src/ReferenceData/Readme.md) describes the implemented subsystem. The [compiler guide](../../tools/ReferenceData/README.md) owns authoring, relationship validation, and the current manual snapshot-update procedure.

The compiler generates a local JSON file. Runtime embeds a separately copied snapshot and requires an exact schema-version match. Core and Runtime are published packages, but production projects can integrate through project references. Package selection and publishing share the repository's [release infrastructure](../PackageReleases.md).

Public definition constructors expose an infrastructure concern as a consumer contract. Compilation, snapshot copying, and version updates require manual coordination. Shared builds can hide the compatibility effects of changing multiple projects together.

The [smoke-test guide](../../tests/SmokeTests/Readme.md) separately documents an image-selection timing issue: deployment tests can run a previous smoke image. Reference Data evolution must clarify pipeline responsibilities without treating that existing issue as a consequence of Reference Data or automatically expanding the construction ticket to fix it.

## Intended boundaries

- **Reference Data production:** validate authoring input, compile snapshots, verify Runtime hydration, and publish immutable versioned artifacts.
- **Application integration:** verify selected packages and snapshots support required identities, relationships, and game behaviour.
- **Deployment verification:** verify the deployed application can access its configured snapshot and serve its essential journeys.

An independently published snapshot does not imply automatic adoption. Snapshot selection, compatibility checks, and rollback must be explicit. Existing games may need to retain a reference-data version; this must be decided before enabling content adoption that could alter their behaviour.

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
2. **Versioning and compatibility:** decide content/schema/package relationships, snapshot selection, existing-game version behaviour, and upgrade/rollback expectations. Record the decisions in a focused ADR.
3. **Automated production:** make validation, compilation, metadata handling, and hydration verification repeatable; produce a reviewable artifact without manual copying.
4. **Package integration:** establish package-consumer verification and API compatibility checks. Decide whether independent builds and releases justify solution or repository extraction before creating repository-specific publishing workflows.
5. **Blob publishing:** record the delivery decision, provision Azure resources through code, and publish versioned artifacts using authenticated automation.
6. **Runtime retrieval:** implement the selected version policy, authentication, validation, caching, diagnostics, and failure/recovery behaviour. Resolve which hosts retrieve snapshots, including browser-hosted consumers and their access constraints.
7. **Workflow demonstration:** verify a content-only update, a contract change, deployed retrieval, and rollback. Document release ownership and operational procedures.

Each slice includes appropriate tests and documentation. The order expresses dependencies, not a requirement to implement the entire plan as one change.

## Architecture records

Use focused ADRs for owned construction, versioning/compatibility, Blob delivery, and package/release boundaries. Record repository extraction separately if chosen. Mark proposals clearly and move them to accepted status only when their decisions are agreed. Implementation status belongs in current architecture and operational documentation.

## Completion evidence

- An automated process validates, compiles, verifies, and publishes a versioned snapshot to Azure Blob Storage.
- A deployed application loads an explicitly selected compatible snapshot through Runtime and exposes definitions through catalogs.
- A supported content-only addition requires no Core or Runtime rebuild.
- Incompatible artifacts fail with actionable diagnostics; retrieval failures and recovery follow documented policies.
- Snapshot rollback is demonstrated, including the agreed treatment of existing games.
- Consumer construction is restricted and package compatibility is checked.
- Reference Data, application integration, and deployment checks have explicit owners and release gates.

## Documentation ownership

Keep shared consumption and versioning guidance in the Reference Data overview. Keep compiler operation and authoring policies in its guide. Keep general package publication mechanics and deployed smoke-test operation in their existing guides. Link these documents rather than duplicating them. Consolidate or remove obsolete material as each slice lands and update inbound links.
