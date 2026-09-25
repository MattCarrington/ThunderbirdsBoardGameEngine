# Development Setup

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), meeting the baseline in [`global.json`](../global.json)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## First-time setup

`global.json` selects the build SDK. It permits stable .NET 10.0 SDKs at or
above the recorded version, including newer feature bands, but not .NET 11.
Run `dotnet --version` from the repository root to check the selected SDK.
GitHub Actions installs the version recorded in this file; Docker builds also
read it, so their images must contain a compatible SDK. The application target
framework remains configured separately in `Directory.Build.props`.

After cloning the repository, install the Git hooks:

```powershell
.\scripts\install-hooks.ps1
```

---

## Running the application locally

There are two supported approaches for local development.

---

### Option 1: dotnet run

Runs the API directly on the host. Best for day-to-day API development and
debugging in an IDE.

```powershell
dotnet run --project src/Api/ThunderbirdsBoardGameEngine.Api
```

The API will be available at:

- `http://localhost:5197`
- `https://localhost:7032`

Swagger UI is available at `/swagger` when the API runs in `Development`.
The document is served at `/swagger/v1/swagger.json`. Versioned API requests
require the `X-API-Version` header (currently `1`).

If you want to exercise the Blazor UI against this API instance, make sure
the UI client configuration points at the same API URL you started. The repo
defaults to `https://localhost:8080/` in the UI development settings, so that
value may need to be adjusted for a direct `dotnet run` workflow.

---

### Option 2: Docker Compose

Runs the application in a container, matching the production image more
closely. Best for validating Docker-specific behaviour or running
integration, smoke, and end-to-end tests against a containerised build.

From the repository root:

```powershell
docker compose -f Docker/docker-compose.yml -f Docker/docker-compose.override.yml up --build
```

The API will be available at:

- `http://localhost:8000`

The override file sets `ASPNETCORE_ENVIRONMENT=Development` and mounts
the `TestData/` directory into the container.

In this mode the UI and API are co-hosted on the same origin, so the UI's
`RulesClient:EndpointMode` is `CoHosted` and it resolves the API base URL
from the browser origin at runtime.

To stop and remove containers:

```powershell
docker compose -f Docker/docker-compose.yml -f Docker/docker-compose.override.yml down
```

### Test Compose configuration

For integration and browser tests, use the test override instead:

```powershell
docker compose -f Docker/docker-compose.yml -f Docker/docker-compose.test.yml up --build
```

| Configuration | Environment | API and UI address | Swagger |
|---|---|---|---|
| Base + development override | `Development` | `http://localhost:8000` | Enabled |
| Base + test override | `Docker` | `http://localhost:8080` | Disabled |

Both configurations serve the published Blazor UI and API from the same origin.
A 404 at `/swagger/v1/swagger.json` in the test configuration is expected:
Swagger endpoints are only enabled in `Development`.

Stop the current configuration with its matching `down` command before
switching, because both use the same container name. To stop the test setup:

```powershell
docker compose -f Docker/docker-compose.yml -f Docker/docker-compose.test.yml down
```

---

## Notes

- Compose serves the published UI without a separate front-end dev server.
  For separate local UI development, the Blazor project has its own launch
  profiles; configure its API address to match the running API.
- When running via `dotnet run`, the API is hosted locally on the ports from
  `launchSettings.json`, and the UI configuration must point at that API URL if
  you want to use the UI in that mode.
- When running via Docker, the UI and API share the same origin, and the UI
  resolves the API base URL from the browser origin at runtime.
