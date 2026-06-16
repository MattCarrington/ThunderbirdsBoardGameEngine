# Development Setup

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## First-time setup

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

Swagger UI is available at `/swagger`.

If you want to exercise the Blazor UI against this API instance, make sure
the UI client configuration points at the same API URL you started. The repo
defaults to `https://localhost:<HOST_PORT>/` in the UI development settings, so that
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

- `http://localhost:<HOST_PORT>`

The override file sets `ASPNETCORE_ENVIRONMENT=Development` and mounts
the `TestData/` directory into the container.

In this mode the UI and API are co-hosted on the same origin, so the UI's
`RulesClient:EndpointMode` is `CoHosted` and it resolves the API base URL
from the browser origin at runtime.

To stop and remove containers:

```powershell
docker compose -f Docker/docker-compose.yml -f Docker/docker-compose.override.yml down
```

---

## Notes

- There is no separate front-end dev server.
- When running via `dotnet run`, the API is hosted locally on the ports from
  `launchSettings.json`, and the UI configuration must point at that API URL if
  you want to use the UI in that mode.
- When running via Docker, the UI and API share the same origin, and the UI
  resolves the API base URL from the browser origin at runtime.
