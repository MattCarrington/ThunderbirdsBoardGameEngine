# ThunderbirdsBoardGameEngine

This repository contains an experimental ASP.NET�based application built
around the *Thunderbirds* board game domain, with a strong emphasis on
**testability, contract-driven design, and automation**.

The project is primarily intended to demonstrate **SDET-focused
engineering practices** rather than to deliver a finished game product.

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

## What�s included

- **CI & versioning discipline**  
  - manual version bumps
  - prereleases only for unreleased versions
  - publishing only from `main`
  - no duplicate or zombie packages

---

## What�s intentionally out of scope (for now)

- Complete game rules engine
- Sophisticated UI/UX
- Persistence beyond generated data
- Multiplayer or real-time gameplay
- Production hosting concerns

These may be explored in later slices if useful for testing scenarios.

---

## Status

Early-stage and evolving.

Breaking changes are expected until a `1.0.0` release is declared.
