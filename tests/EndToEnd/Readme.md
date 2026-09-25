# End-to-End Tests

The end-to-end tests use [Playwright](https://playwright.dev/dotnet/) with
[Reqnroll](https://reqnroll.net/) (BDD) to exercise key user journeys through
the Blazor UI against a running application.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), meeting the baseline in [global.json](../../global.json)
- A running instance of the application (see
  [docs/DevelopmentSetup.md](../../docs/DevelopmentSetup.md))

## First-time setup

Build the test project and install the Playwright browsers:

```powershell
cd tests/EndToEnd/ThunderbirdsBoardGameEngine.PlaywrightTests
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install
```

> The `playwright install` step downloads the browser binaries used by the
> tests. It only needs to be run once, or after a Playwright package version
> update.

## Configuration

The tests require a `BASE_URL` environment variable pointing at the running
application. Set this to whatever URL your local instance is served on.

**PowerShell:**

```powershell
$env:BASE_URL = "http://localhost:8080"
```

**bash/zsh:**

```bash
export BASE_URL="http://localhost:8080"
```

The example uses the test Docker Compose configuration, which serves both the
API and published UI on port `8080`. The development Compose configuration uses
port `8000`. Use the UI's address when running it separately from the API; an
API-only `dotnet run` does not publish the Blazor UI. See
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
pwsh bin/Debug/net10.0/playwright.ps1 install
```

### Playwright version mismatch

If browser behaviour changes unexpectedly after a package update, re-run
`playwright install` to ensure the installed browser binaries match the
current package version.
