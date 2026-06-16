# Architecture Overview

## Deployment shape

The application is deployed as a **single Docker image** containing both the
ASP.NET Core API and the Blazor WebAssembly UI. The UI is published as static
files and served directly from the API's `wwwroot` folder, making them
co-hosted on the same origin with no CORS complications.

```
[ Browser ]
    │
    ▼
[ ASP.NET Core API ]  ← hosts Blazor WASM static files
    │
    ├── Rules bounded context (in-process)
    └── ReferenceData bounded context (in-process, embedded snapshot)
```

---

## Bounded contexts

### Rules

Responsible for evaluating game rules: calculating rescue targets and
validating movement routes.

Internally layered as:

| Layer | Project | Responsibility |
|---|---|---|
| Domain | `Rules.Domain` | Core logic — `RescueTargetCalculator`, `MovementEvaluator`, `Topography` |
| Application | `Rules.Application` | MediatR handlers, orchestration, interface definitions |
| Infrastructure | `Rules.Infrastructure` | Adapters — lookup implementations that bridge ReferenceData catalogs into domain interfaces |
| Contracts | `Rules.Contracts` | Public request/response DTOs shared between API and client |
| Client | `Rules.Client` | Typed HTTP client for consuming the Rules API |

The domain layer has **no dependency on ReferenceData**. The infrastructure
layer owns that bridge via lookup adapters, keeping the domain pure.

**Published packages:** `Rules.Contracts`, `Rules.Client`, and the
`Rules.WireMock` stub (see below) are all published NuGet packages so
consumers can reference them independently.

### ReferenceData

Provides immutable, in-memory static game data: disasters, characters,
locations, Thunderbirds, and pod vehicles.

Data is compiled from spreadsheets at build time into a versioned JSON
snapshot, embedded into the application, and loaded into read-only singleton
catalogs at startup. The runtime never calls an external service.

Exposed via catalog interfaces:
`IDisasterDefinitionCatalog`, `ICharacterDefinitionCatalog`,
`ILocationDefinitionCatalog`, `IThunderbirdDefinitionCatalog`,
`IMapEdgeDefinitionCatalog`, and others.

See [src/ReferenceData/Readme.md](../src/ReferenceData/Readme.md) for detail.

---

## Dependency rules

```
Rules.Domain
    ↑
Rules.Application  ←→  Rules.Infrastructure  →  ReferenceData.Runtime
    ↑
    API
    ↑
Rules.Client  (consumers / UI)
```

- **Domain** has no external dependencies — pure C# logic only.
- **Application** defines interfaces; it does not depend on infrastructure.
- **Infrastructure** depends on both Application (implements its interfaces)
  and ReferenceData.Runtime (as the data source).
- **Contracts** and **Client** are independently publishable; they do not
  depend on domain or application internals.

---

## Contract-driven consumer isolation

The Rules bounded context ships three consumer-facing packages:

| Package | Purpose |
|---|---|
| `Rules.Contracts` | Shared DTOs — request/response shapes |
| `Rules.Client` | Typed HTTP client for calling the Rules API |
| `Rules.WireMock` (`tools/Rules/`) | WireMock stub of the Rules API for use in consumer tests |

This means a consumer can be fully tested in isolation using the WireMock
stub, with the real integration validated separately. The WireMock stub has
its own component tests to keep it honest against the real API.

---

## API

The API layer (`src/Api/`) is thin: it receives HTTP requests, maps them via
MediatR into the Rules application layer, and returns responses. It registers
both `AddReferenceData()` and `AddRules()` at startup.

Controllers are versioned under `V1/` and use kebab-case URL routing. Swagger
is available in Development at `/swagger`.

---

## UI

The Blazor WebAssembly UI (`src/Ui/`) is intentionally minimal and exists
primarily to support integration and end-to-end testing.

It communicates with the API via the published `Rules.Client`. Endpoint
resolution is controlled by `RulesClient:EndpointMode`:

- **`CoHosted`** — resolves the API base URL from the browser origin at
  runtime. Used in Docker and production deployments.
- **`External`** — uses an explicitly configured `BaseAddress`. Used for
  local development with `dotnet run`.
