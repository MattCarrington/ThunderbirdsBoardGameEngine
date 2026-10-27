# Package releases

Package versions are chosen manually in each package project. The declared
version is the next intended stable version; CI does not decide whether a
change is major, minor, or patch.

## Compatibility and versioning

Published packages now target .NET 10 and require consumers to target .NET 10
or a compatible later framework. They no longer support .NET 8 consumers.

Package versions are independent of the application's maturity:

- For stable packages (`1.x` and later), dropping a supported framework is a
  breaking change and requires a major version bump.
- For pre-1.0 packages, this repository uses a minor bump for breaking changes,
  resetting the patch to zero (for example, `0.4.1` to `0.5.0`).
- ReferenceData.Core and ReferenceData.Runtime moved to `2.0.0` for the
  .NET 10 migration and subsequently to `3.0.0` with the required character-capacity
  constructor change. Project files hold current declared versions. Other packages
  can remain on their own `0.x` release lines.

NuGet package versions are separate from reference-data snapshot metadata.
Changing the target framework does not require regenerating the snapshot or
bumping its schema or content version. See
[Reference Data](../src/ReferenceData/Readme.md#snapshot-metadata).

[ADR 0008](architecture/decisions/0008-reference-data-versioning-and-compatibility.md)
defines the separate package, schema, and content responsibilities. Core and
Runtime versions need not match: release Runtime when its code or required Core
dependency changes; a Runtime-only change does not require releasing Core.
Snapshot content increments describe content impact and are not a substitute
for package API compatibility assessment.

### Reference Data construction boundary

Core and Runtime declare `4.0.0` for the construction-boundary change described
in [ADR 0007](architecture/decisions/0007-reference-data-owned-construction.md).
Removing public definition constructors breaks source and binary callers of
those constructors. Public init setters on specific-token rewards and derived
bonus-key definitions are also removed; their read and deconstruction APIs
remain. Consumers obtain definitions through Runtime catalogs. Tests migrate
to the shared test-only factories/builders, and previously compiled callers of
removed APIs must rebuild after migration.

This release changes package APIs, not snapshot structure or content, so no
snapshot version increment or regeneration is required.

The package graph is defined in `.github/packages.json`. When package source, an
explicitly packed file, or a centrally managed external dependency changes, CI
selects that package and every downstream package that consumes it. Packages
outside that closure are ignored, even when their currently declared release
line is already closed.

For example, a change to `Rules.Contracts` selects:

```text
Rules.Contracts
|- Rules.Client
`- Rules.WireMock
```

`Client.Core` and `WireMock.Hosting` remain on their stable versions unless
their own source changes. Reference Data packages are outside the Rules graph.

The catalog's `source_paths` include files outside a project directory when
they are packed into its nupkg. Its `central_packages` list identifies entries
from `Directory.Packages.props` that affect each published package. Changes to
test-only or application-only package versions do not trigger publication.

## Release-line behaviour

- On a feature branch, an affected package whose stable version does not exist
  is published as `<version>-beta.<run-number>`.
- On a feature branch, an affected package whose stable version already exists
  fails with a request to increment its project version manually.
- On `main`, an affected open version is published as the stable version.
- On `main`, an already-published stable version is an idempotent skip.
- Unaffected packages are neither queried nor published.

Selected project versions are applied together in the CI checkout before any
package is packed. Consequently, selected project references become dependencies
on the beta packages from the same run, while unselected project references keep
their stable versions.
