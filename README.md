# ThunderbirdsBoardGameEngine

This repository contains an experimental ASP.NET based application built
around the *Thunderbirds* board game domain, with a strong emphasis on
**testability, contract-driven design, and automation**.

The project is primarily intended to demonstrate **SDET-focused
engineering practices** rather than to deliver a finished game product.

---

## What this project demonstrates

- End-to-end quality engineering practices, not just unit testing
- Contract-driven architecture across bounded contexts
- Automated, repeatable test environments with minimal hidden coupling
- Clear separation between domain, contracts, clients, and infrastructure
- CI-aware versioning and publishing discipline

This is intentionally an **engineering-quality demonstration** rather than
a fully featured game product.

For a detailed breakdown of the test layers and the reasoning behind them,
see [docs/TestStrategy.md](docs/TestStrategy.md).

---

## Getting started

See [docs/DevelopmentSetup.md](docs/DevelopmentSetup.md) for prerequisites and instructions on running the application locally via `dotnet run` or Docker Compose.

---

## Quick orientation for non-contributors

In plain terms:

- The backend is an ASP.NET API.
- The frontend is a Blazor WebAssembly UI.
- They are shipped together as a single Docker image.
- The UI exists mainly to support integration and end-to-end validation.

The current implementation focuses on the **Rules** bounded context as a
realistic vertical slice.

---

## Architecture overview

The application consists of:

- **ASP.NET API**  
  Exposes game-related endpoints and domain data.

- **Blazor WebAssembly UI**  
  A lightweight front-end hosted by the API, used to exercise and
  validate end-to-end behaviour.

Both are built and shipped together as a **single Docker image** for
simplicity and repeatability.

For a detailed breakdown of the bounded contexts, dependency rules, and
package structure, see [docs/Architecture.md](docs/Architecture.md).

---

## Current focus

The current vertical slice focuses on the **Rules bounded context**.

This slice exists to demonstrate:

- clear bounded context separation
- explicit **Contracts**, **Client**, and **WireMock** packages
- contract-driven and consumer-focused testing
- realistic test data generation
- disciplined CI-driven versioning and publishing
- deterministic test environments (no hidden dependencies)

The UI is intentionally minimal and exists mainly to support
integration and end-to-end testing scenarios.

---

## Reference data

Static game data — disasters, characters, locations, Thunderbirds, and pod
vehicles — is managed as **reference data**: compiled from spreadsheets at
build time into a versioned JSON snapshot, then embedded into the application
and loaded into read-only in-memory catalogs at startup.

This avoids a dependency on a live catalog service and keeps the runtime
fully self-contained.

For more detail on the compiler, snapshot versioning, and catalog interfaces,
see [src/ReferenceData/Readme.md](src/ReferenceData/Readme.md).

---

## Testing

This project uses Playwright for end to end testing. See [tests/EndToEnd/Readme.md](tests/EndToEnd/Readme.md)
for more detail.

There are also containerised smoke tests that are run as part of the deployment process.
For more detail, see [tests/SmokeTests/Readme.md](tests/SmokeTests/Readme.md).

## What is included

- **CI & versioning discipline**  
  - manual version bumps
  - prereleases only for unreleased versions
  - publishing only from `main`
  - no duplicate or zombie packages

---

## Why the scope is intentionally narrow

This repository prioritizes confidence and engineering discipline over
feature breadth.

What is intentionally out of scope right now:

- Complete game rules coverage
- Advanced UI/UX
- Long-term persistence strategy
- Multiplayer or real-time gameplay
- Production-grade hosting concerns

Narrow scope allows deeper focus on verification strategy, contracts, and
automation quality.

---

## Status

Early-stage and evolving.

Breaking changes are expected until a `1.0.0` release is declared.
