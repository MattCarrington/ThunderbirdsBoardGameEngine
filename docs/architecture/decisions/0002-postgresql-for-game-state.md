# ADR 0002: Use PostgreSQL for GameState Persistence

- Status: Accepted for the planned persisted-game slice
- Date: 2026-07-24

## Context

The GameState spike used an in-memory repository because it explored what state
to present and how it could be manipulated. A production slice must retain
created and updated games when the application process or container is
replaced.

The current deployment targets Azure, but persistence should not create an
unnecessary dependency on Azure-specific data APIs. Local development and CI
also require production-representative persistence.

## Decision

GameState will use PostgreSQL through EF Core and the Npgsql provider.

- Local development will run PostgreSQL in Docker with a named volume.
- CI will run persistence tests against a PostgreSQL service container.
- Azure will use a managed PostgreSQL service.
- A future AWS deployment can use Amazon RDS for PostgreSQL without changing
  the application persistence technology.
- GameState will depend on an application repository abstraction rather than
  cloud SDKs.

An initial relational design is expected to contain games, per-game Thunderbird
Machine positions, and per-game character assignments. Exact table and index
design remains an implementation decision and is not part of the feature
contract.

## Consequences

- Development, CI, and production share PostgreSQL semantics.
- Switching cloud providers primarily affects infrastructure, networking,
  secrets, and connection configuration.
- The project supports one database engine rather than attempting
  database-engine independence.
- Repository integration tests must use real PostgreSQL rather than an EF
  in-memory provider.
- Provider-managed default operational backups are accepted for the first
  slice. Custom schedules, long-term retention, and formal recovery objectives
  are deferred.

