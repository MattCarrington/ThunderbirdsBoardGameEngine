# Test Strategy

## Purpose

Testing in this project is not an afterthought — it is the primary demonstration
goal. The test suite is designed to show how a well-structured quality engineering
practice looks across a realistic application: multiple layers, clear responsibilities,
and no hidden coupling between environments.

---

## Test layers

The following layers exist, roughly in order from fastest/most isolated to
slowest/most integrated.

### Unit tests

**What they test:** Individual classes and functions in isolation, with
dependencies faked or stubbed.

**Where they live:**

| Project | Focus |
|---|---|
| `tests/Rules/UnitTests/` | Domain logic, application services, client, infrastructure — one project per layer |
| `tests/ReferenceData/UnitTests/` | Compiler validation logic and runtime snapshot loading |
| `tests/Api/UnitTests/` | API-layer concerns in isolation |
| `tests/Ui/UnitTests/` | Blazor component logic |
| `tests/Common/` | Shared test utilities |

**Characteristics:** No I/O, no network, no Docker. Fast and deterministic.

---

### Component tests

**What they test:** A single component (a service, client, or bounded context)
exercised end-to-end internally, with external dependencies replaced by fakes
or WireMock stubs.

**Where they live:**

| Project | Focus |
|---|---|
| `tests/Rules/ComponentTests/ThunderbirdsBoardGameEngine.Rules.ComponentTests/` | Rules domain exercised through its full internal stack |
| `tests/Rules/ComponentTests/ThunderbirdsBoardGameEngine.Rules.Client.ComponentTests/` | Rules HTTP client exercised against a WireMock stub |
| `tests/Rules/ComponentTests/ThunderbirdsBoardGameEngine.Rules.WireMock.ComponentTests/` | WireMock stub correctness — verifies the stub matches real API behaviour |
| `tests/ReferenceData/ComponentTests/` | ReferenceData runtime exercised against a real embedded snapshot |
| `tests/Api/ComponentTests/` | API endpoints exercised via `WebApplicationFactory` with faked dependencies |
| `tests/Ui/ComponentTests/` | Blazor components rendered in isolation with bUnit |

**Characteristics:** Still fast and in-process, but exercises more of the real
stack. The WireMock component tests are a deliberate design choice: they ensure
the stub stays honest so that any consumer relying on it gets reliable isolation.

---

### Integration tests

**What they test:** A client or consumer exercised against a real running
instance of the service it depends on.

**Where they live:**

| Project | Focus |
|---|---|
| `tests/Api/IntegrationTests/ThunderbirdsBoardGameEngine.Rules.Client.IntegrationTests/` | Rules HTTP client exercised against a live API |
| `tests/Ui/IntegrationTests/` | UI components exercised against real backend calls |

**Characteristics:** Require a running environment. Driven by environment
variables (`RulesBaseUrl`, etc.) so they are not tied to a specific host or
port. These are the primary validation that the real client and real API
agree with the contract.

---

### End-to-end tests

**What they test:** Full user journeys through the Blazor UI, running in a
real browser via Playwright.

**Where they live:** `tests/EndToEnd/ThunderbirdsBoardGameEngine.PlaywrightTests/`

**Scenarios covered** (Reqnroll BDD feature files):

- Calculate rescue bonus
- Disaster cards
- Validate movement

**Characteristics:** Require a fully running application. Driven by the
`BASE_URL` environment variable. Slowest layer but gives the highest
confidence that the system works as a whole from a user's perspective.

See [tests/EndToEnd/Readme.md](../tests/EndToEnd/Readme.md) for setup and
run instructions.

---

### Smoke tests

**What they test:** A minimal set of health and key-journey checks against
a deployed (or locally running) application.

**Where they live:** `tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/`

**Characteristics:** Designed to run post-deployment to verify the environment
is live and responsive. Packaged as a Docker image so they can be executed
in CI without requiring a .NET SDK on the runner.

See [tests/SmokeTests/Readme.md](../tests/SmokeTests/Readme.md) for setup and
run instructions.

---

## Key design decisions

### WireMock as a first-class package

The `tools/Rules/ThunderbirdsBoardGameEngine.Rules.WireMock/` project is a
published WireMock stub for the Rules API. Consumers can reference it directly
rather than maintaining their own stubs, ensuring consistency. The WireMock
component tests verify this stub stays aligned with the real API.

### Contract-driven isolation

The Rules bounded context ships explicit **Contracts**, **Client**, and
**WireMock** packages. This means consumers can be tested in full isolation
(using the WireMock stub) and the real integration is validated separately in
the integration test layer — rather than blurring the two concerns together.

### Environment-variable-driven configuration

Integration, end-to-end, and smoke tests are all configured via environment
variables rather than hard-coded URLs. This makes them portable across local
development, CI pipelines, and any deployed environment without code changes.

### Shared test utilities

`tests/Common/ThunderbirdsBoardGameEngine.TestUtils/` provides shared builders,
fakes, and stubs used across test layers. This keeps test code DRY without
leaking test concerns into production code.

---

## Test pyramid summary

```
          [ Smoke ]           ← post-deployment, Docker image
        [ End-to-End ]        ← full browser journeys, Playwright + BDD
      [ Integration ]         ← real client vs real service
    [ Component ]             ← isolated component, WireMock stubs
  [ Unit ]                    ← fast, in-process, fully isolated
```

The pyramid is intentionally weighted toward the lower layers for speed and
reliability, with the upper layers providing confidence at key system boundaries.
