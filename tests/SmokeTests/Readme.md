# Smoke Tests

The smoke tests validate that a deployed application is reachable and that key user journeys function correctly.

## Prerequisites

Set your GitHub username and a PAT with read:packages permission:

$env:GITHUB_PACKAGES_USERNAME = "your-github-username"
$env:GITHUB_PACKAGES_TOKEN = "<your-pat>"

## Running Smoke Tests from Source

Run the smoke tests directly using the .NET test runner:

```powershell
dotnet test `
  tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests.csproj `
  -c Release
```

## Building the Smoke Test Docker Image

Build the smoke test image locally:

```powershell
docker build `
  --build-arg GITHUB_PACKAGES_USERNAME="$env:GITHUB_PACKAGES_USERNAME" `
  --secret id=github_packages_token,env=GITHUB_PACKAGES_TOKEN `
  -t thunderbirds-smoke-tests:local `
  -f tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/Dockerfile `
  .
```

The Docker build uses BuildKit secrets to authenticate against GitHub Packages during package restore. The token is only available during the restore step and is not baked into the final image.

## Running the Smoke Test Docker Image

Start the application locally and note the URL. This is required for 
`SMOKE_TEST_BASE_URL` variable used to run the tests and needs to be set before running the tests.

For example:

- `dotnet run` (default): `http://localhost:5197`
- Docker Compose (default): `http://localhost:8000`

Run the smoke test container:

```powershell
docker run --rm `
  -e SMOKE_TEST_BASE_URL="http://host.docker.internal:<HOST_PORT>" `
  thunderbirds-smoke-tests:local
```

> Note: `host.docker.internal` is required when running from inside a Docker container. Using `localhost` will refer to the container itself rather than the host machine.

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

should trigger a new smoke image build.
