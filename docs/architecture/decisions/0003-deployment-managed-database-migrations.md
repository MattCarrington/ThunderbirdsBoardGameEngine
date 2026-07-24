# ADR 0003: Apply Database Migrations as a Deployment Job

- Status: Accepted for the planned persisted-game slice
- Date: 2026-07-24

## Context

The first persisted slice introduces a new schema, and later slices will evolve
it while retaining existing games. Schema creation and evolution must be
repeatable locally, in CI, and in deployed environments.

Applying migrations during application startup couples availability to schema
changes and can allow multiple application instances to compete to update the
database.

## Decision

EF Core migrations will be committed to source control and packaged as an
executable migration bundle in a one-shot container.

Deployment order will be:

```text
Provision or update PostgreSQL
        |
        v
Run the migration job once
        |
        v
Deploy the application only after migration succeeds
        |
        v
Run readiness and smoke checks
```

The application will not call `EnsureCreated`, `Migrate`, or otherwise alter
the database schema during startup.

CI will:

- apply all migrations to an empty PostgreSQL database;
- detect model changes that have no corresponding migration;
- run persistence integration tests after migration;
- build and exercise the deployable migration artifact;
- add upgrade-path tests once more than one released schema version exists.

Generated migrations will be reviewed before commit. A generated SQL script may
be retained as a CI artifact for inspection.

## Consequences

- The same migration mechanism can run locally, in CI, Azure, or AWS.
- Migration failure stops deployment before incompatible application code
  receives traffic.
- The migration container is a short-lived job, not another production service.
- Database credentials are supplied at runtime and are not stored in the image.
- Once released, applied migrations are not edited; later changes use new,
  preferably additive migrations.

