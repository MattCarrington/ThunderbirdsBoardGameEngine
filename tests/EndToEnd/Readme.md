# End-to-End Tests

The end-to-end tests use [Playwright](https://playwright.dev/dotnet/) with
[Reqnroll](https://reqnroll.net/) (BDD) to exercise key user journeys through
the Blazor UI against a running application.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A running instance of the application (see
  [docs/DevelopmentSetup.md](../../docs/DevelopmentSetup.md))

## First-time setup

Build the test project and install the Playwright browsers:

```powershell
cd tests/EndToEnd/ThunderbirdsBoardGameEngine.PlaywrightTests
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install
```

> The `playwright install` step downloads the browser binaries used by the
> tests. It only needs to be run once, or after a Playwright package version
> update.

## Configuration

The tests require a `BASE_URL` environment variable pointing at the running
application. Set this to whatever URL your local instance is served on.

**PowerShell:**

```powershell
$env:BASE_URL = "http://localhost:5197"
```

**bash/zsh:**

```bash
export BASE_URL="http://localhost:5197"
```

Replace `http://localhost:5197` with the actual URL of your running instance.
If you are running via Docker Compose the default port is `8000`
(`http://localhost:8000`). See
[docs/DevelopmentSetup.md](../../docs/DevelopmentSetup.md) for details.

## Running the tests

From the repository root, with `BASE_URL` set:

```powershell
dotnet test tests/EndToEnd/ThunderbirdsBoardGameEngine.PlaywrightTests/ThunderbirdsBoardGameEngine.PlaywrightTests.csproj
```

## Troubleshooting

### `BASE_URL is not set`

The tests will throw on startup if `BASE_URL` is missing. Set the environment
variable before running (see **Configuration** above).

### Browser binaries missing

If tests fail with a browser executable error, re-run the install step:

```powershell
cd tests/EndToEnd/ThunderbirdsBoardGameEngine.PlaywrightTests
pwsh bin/Debug/net8.0/playwright.ps1 install
```

### Playwright version mismatch

If browser behaviour changes unexpectedly after a package update, re-run
`playwright install` to ensure the installed browser binaries match the
current package version.
