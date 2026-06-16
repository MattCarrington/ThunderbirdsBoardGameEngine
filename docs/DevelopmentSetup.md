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

Runs the API directly on the host. Best for day-to-day development and
debugging in an IDE.

```powershell
dotnet run --project src/Api/ThunderbirdsBoardGameEngine.Api
```

The API will be available at:

- `http://localhost:5197`
- `https://localhost:7032`

Swagger UI is available at `/swagger`.

The Blazor WebAssembly UI is hosted by the API and will be served at
the same URL. In `Development` mode the UI is configured via
`appsettings.Development.json` to call the API at `https://localhost:8080/`
by default — update `RulesClient:BaseAddress` in that file if your port
differs.

---

### Option 2: Docker Compose

Runs the application in a container, matching the production image more
closely. Best for validating Docker-specific behaviour or running
integration/smoke tests against a containerised build.

From the repository root:

```powershell
docker compose -f Docker/docker-compose.yml -f Docker/docker-compose.override.yml up --build
```

The API will be available at:

- `http://localhost:8000`

The override file sets `ASPNETCORE_ENVIRONMENT=Development` and mounts
the `TestData/` directory into the container.

To stop and remove containers:

```powershell
docker compose -f Docker/docker-compose.yml -f Docker/docker-compose.override.yml down
```

---

## Notes

- The UI and API are co-hosted in a single process. There is no separate
  front-end dev server.
- When running via `dotnet run`, the UI uses `EndpointMode: External` and
  calls the API at the configured `BaseAddress`.
- When running via Docker, the UI uses `EndpointMode: CoHosted` and
  resolves the API base URL from the browser origin at runtime.
