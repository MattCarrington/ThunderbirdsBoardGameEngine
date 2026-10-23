# ADR 0003: Apply Database Migrations as a Deployment Job

- Status: Accepted; artifact and execution model superseded by ADR 0005
- Date: 2026-07-24
- Amended: 2026-10-23

## Amendment: Ordered Init Containers

[ADR 0005](0005-least-privileged-game-state-database-access.md) supersedes the
original migration-bundle and once-per-deployment job choice below for the
current increment. Migrations use the existing .NET migrator executable as the
first Azure Container App init container, followed by the database provisioner
init container, then the API. Docker Compose enforces the corresponding order
through successful-completion dependencies.

Init containers execute on replica initialization and are not guaranteed to run
only once per deployment. Migration and provisioning operations must tolerate
repeat execution; the provisioner's advisory lock does not coordinate migration
runs. The API process still does not apply migrations. Reviewed, committed
migrations, runtime-supplied credentials, and the validation principles below
remain applicable. Azure execution and failure behaviour still require deployed
verification.

The `public` to `game_state` table move is incompatible with the previous API
image. ADR 0005 accepts planned downtime for that transition: old replicas must
be stopped before migrations run and remain stopped on failure. Init-container
ordering alone does not establish this maintenance gate. Restoring an old image
after the table move requires a reviewed database recovery as well; an image
rollback does not reverse migrations. See the deployment runbook in the
[Azure DEV README](../../../infra/Pulumi/AzureDev/README.md#planned-downtime-for-the-game-state-schema-move).

The original decision is retained below as historical context; its job lifecycle
and executable-bundle requirements no longer describe the selected design.

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

