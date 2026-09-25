# Smoke Tests

The smoke tests validate that a deployed application is reachable and that key user journeys function correctly.

## Prerequisites

For source runs, install a .NET 10 SDK meeting the baseline in
[global.json](../../global.json). For container runs, install Docker Desktop.
The target application must already be running and serving both the API and UI.

Set your GitHub username and a PAT with `read:packages` permission:

    $env:GITHUB_PACKAGES_USERNAME = "your-github-username"
    $env:GITHUB_PACKAGES_TOKEN = "<your-pat>"

## Running Smoke Tests from Source

From the repository root, build the tests, install their browser dependencies,
and set the application URL before running them. This example targets test
Compose on port `8080`:

```powershell
dotnet build tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests.csproj -c Release
pwsh tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/bin/Release/net10.0/playwright.ps1 install
$env:SMOKE_TEST_BASE_URL = "http://localhost:8080"
dotnet test `
  tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests.csproj `
  -c Release
```

## Building the Smoke Test Docker Image

Build the smoke test image locally:

```powershell
$env:DOCKER_BUILDKIT = "1"
docker build `
  --build-arg GITHUB_PACKAGES_USERNAME="$env:GITHUB_PACKAGES_USERNAME" `
  --secret id=github_packages_token,env=GITHUB_PACKAGES_TOKEN `
  -t thunderbirds-smoke-tests:local `
  -f tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/Dockerfile `
  .
```

The Docker build uses BuildKit secrets to authenticate against GitHub Packages during package restore. The token is only available during the restore step and is not baked into the final image.

The image keeps Playwright's browsers and system dependencies, and copies the
SDK from Microsoft's .NET 10 SDK image. This lets the build meet `global.json`
without depending on the SDK bundled with a particular Playwright release.

## Running the Smoke Test Docker Image

Start the application locally and note the URL. This is required for 
`SMOKE_TEST_BASE_URL` variable used to run the tests and needs to be set before running the tests.

For example:

- Development Docker Compose: `http://localhost:8000`
- Test Docker Compose: `http://localhost:8080`

See [Development Setup](../../docs/DevelopmentSetup.md) for the commands.
An API-only `dotnet run` does not provide the published UI needed by browser
smoke tests.

Run the smoke test container:

```powershell
docker run --rm `
  -e SMOKE_TEST_BASE_URL="http://host.docker.internal:<HOST_PORT>" `
  thunderbirds-smoke-tests:local
```

> Note: `host.docker.internal` is required when running from inside a Docker container. Using `localhost` will refer to the container itself rather than the host machine.

## GitHub Actions: building versus running

The workflows have separate responsibilities:

| Workflow | Trigger | Result |
|---|---|---|
| `Smoke Image` | Manual dispatch, or relevant changes pushed to `main` | Builds and publishes the smoke-test container; does not execute tests |
| `Smoke Tests` | Called by the main CI pipeline after deployment | Runs the published container against the deployment URL |

To build on a branch, open **Actions → Smoke Image → Run workflow**, select
the branch, and wait for publication to succeed. Record the exact image tag
from that run. Branch builds publish an image but do not create a `smoke/v…`
Git tag, update `latest`, or update the `SMOKE_IMAGE_TAG` repository variable.
Those updates happen on successful `main` runs.

`Smoke Tests` currently has no manual dispatch trigger. It can still be run
independently by executing a published image locally. After authenticating
Docker to GHCR if required, replace the owner and tag below with your values:

```powershell
docker run --rm `
  -e SMOKE_TEST_BASE_URL="http://host.docker.internal:8080" `
  ghcr.io/<owner>/thunderbirds-smoke-tests:<image-tag>
```

Use the deployed application's URL instead when testing a deployment.
The main pipeline selects `SMOKE_IMAGE_TAG`, falling back to `latest`. Because
the image build runs independently, deployment tests can select the previous
image before the new one is published. Check the image tag in the test job's
output; if it is old, run the new image explicitly before considering the new
smoke image verified. A successful image build alone does not prove tests pass.

## Troubleshooting

### Playwright version mismatch

If smoke tests suddenly fail after a Playwright package update, rebuild the smoke test image.

The Playwright package version and the Playwright Docker base image must remain aligned.

### Application reachable in browser but not from container

Verify that:

```text
SMOKE_TEST_BASE_URL=http://host.docker.internal:<HOST_PORT>
```

is being used rather than:

```text
http://localhost:<HOST_PORT>
```

as `localhost` inside the container refers to the container itself.

### Rebuild the image after Dockerfile changes

Any changes to:

* Playwright version
* Smoke test project dependencies
* Smoke test Dockerfile
* `global.json` or shared build settings

should trigger a new smoke image build.
