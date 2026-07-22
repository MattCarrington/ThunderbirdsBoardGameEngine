# Package releases

Package versions are chosen manually in each package project. The declared
version is the next intended stable version; CI does not decide whether a
change is major, minor, or patch.

The package graph is defined in `.github/packages.json`. When package source changes,
CI selects that package and every downstream package that consumes it. Packages
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
