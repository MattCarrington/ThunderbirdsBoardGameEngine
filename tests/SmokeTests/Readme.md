# Smoke Tests

The smoke tests validate that a deployed application is reachable and that key user journeys function correctly.

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
  -t thunderbirds-smoke-tests:local `
  -f tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/Dockerfile `
  .
```

## Running the Smoke Test Docker Image

Start the application locally and note the URL.

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
SMOKE_TEST_BASE_URL=http://host.docker.internal:8080
```

is being used rather than:

```text
http://localhost:8080
```

as `localhost` inside the container refers to the container itself.

### Rebuild the image after Dockerfile changes

Any changes to:

* Playwright version
* Smoke test project dependencies
* Smoke test Dockerfile

should trigger a new smoke image build.
