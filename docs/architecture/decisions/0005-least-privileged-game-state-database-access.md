# ADR 0005: Separate GameState Runtime and Database Provisioning Access

- Status: Accepted for this increment, with a temporary database-log risk; implementation verification remains outstanding
- Date: 2026-10-22

## Context

The first Azure slice supplies the same PostgreSQL administrator connection
string to the EF migrator init container and the API container. This was an
accepted temporary risk, but compromising the API could expose database
administration privileges.

The API needs to read and persist game state, not administer databases, create
roles, or change table structures. Infrastructure provisioning and schema
migrations still require elevated access. New environments, including UAT, must
be reproducible without manual account creation or permission grants.

Local exploration established the permission model and repeatable setup. The
selected design keeps account management separate from migrations and avoids
requiring direct database connectivity from the Pulumi deployment runner.

## Decision

Separate responsibilities as follows:

| Component | Responsibility | Database credential |
| --- | --- | --- |
| Pulumi Azure Native provider | Provision Azure resources and supply secret configuration | Administrator for server provisioning |
| EF migrator init container | Apply committed schema migrations | Administrator for this increment |
| Dedicated .NET provisioner init container | Create/configure the runtime role and reconcile grants | Administrator |
| API container | Read and persist game state | Dedicated restricted runtime account |

The API must not receive the administrator connection string. Credentials are
supplied through Container App secrets, with Pulumi configuration read as
secrets and no plaintext credentials committed or emitted as ordinary outputs.

Execution order is:

```text
PostgreSQL available
        |
        v
EF migrations succeed
        |
        v
Runtime account provisioning and permission reconciliation succeed
        |
        v
API starts using its runtime account
```

Docker Compose expresses this order through health and successful-completion
dependencies. The Azure implementation will put the migrator first and
the provisioner second in the Container App's init-container array. Azure
documents sequential execution in array order, with successful completion
required before the main container starts.

Init containers run on replica initialization, not once per deployment. Both
operations must tolerate repeat execution. The provisioner uses a transaction
and a transaction-level advisory lock to coordinate cooperating provisioning
runs against the same database. This lock does not coordinate migrations or
provisioners connected to different databases, and is not a global deployment
lock.

### Permission Boundary

- Application tables belong to the `game_state` schema; EF migration history
  remains in `public`.
- The runtime role can log in and connect to the GameState database, use the
  application schema, and perform `SELECT`, `INSERT`, and `UPDATE` on its tables.
- Delete access is not currently required by the persistence implementation.
- The runtime role does not own the database, schema, or application tables and
  does not receive schema-creation or administrative privileges.
- Existing elevated roles or roles with memberships are rejected rather than
  silently adopted as runtime identities. This is not a complete audit of
  pre-existing ownership or direct grants.
- Provisioning rejects the connected administrator as the runtime identity and
  rejects maintenance/template databases as application targets.
- Existing-table grants are reconciled after migrations. Default privileges
  cover future tables created by the migration identity in `game_state`.
  Moving an existing table into a schema does not apply its default privileges.
- The local implementation revokes shared temporary-table access on the
  application database and shared connection access to `postgres`. These affect
  `PUBLIC`, not only the runtime role, and require Azure ownership/permission
  verification. Other databases and inherited access must be inspected before
  claiming database isolation.

This policy assumes a dedicated application server. Sharing the server with
other applications requires revisiting these shared permission changes.

### Relationship to ADR 0003

[ADR 0003](0003-deployment-managed-database-migrations.md) originally selected a
migration bundle run once as a deployment job. This ADR supersedes that artifact
and execution choice for this increment: use the existing .NET migrator
executable and ordered init containers. This accepts execution on replica
initialization rather than exactly once per deployment. The remaining ADR 0003
principles, including reviewed migrations and keeping schema changes out of the
API process, remain in force.

## Alternatives Considered

### Pulumi PostgreSQL Provider

Manage roles, schemas, grants, and default privileges declaratively alongside
Azure resources. This reduces custom account-management code and provides
Pulumi state tracking and previews.

It requires direct PostgreSQL connectivity wherever Pulumi runs, including the
developer workstation and GitHub-hosted deployment runner. Firewall access,
fresh-environment bootstrap, migration/grant ordering, and cleanup need a
concrete design. Provider password handling must also be verified; using a
provider does not by itself eliminate password-bearing SQL or database logging
risks. It is not selected for this increment, but can be reconsidered if the
maintenance cost or deployment requirements change.

### Account Provisioning Inside the EF Migrator

Reuse its administrator connection and existing startup ordering. This adds
less packaging but combines schema evolution and account administration. The
current preference is to keep those responsibilities separate.

### Separate Azure Provisioning Job

Run controlled provisioning inside Azure without direct database access from
the deployment runner. This separates execution from replica initialization,
but requires explicit job invocation, completion checks, and deployment gating.
Sequential init containers avoid that extra orchestration for this slice; a
separate job remains an option if initialization costs or scaling warrant it.

## Credential Handling and Accepted Temporary Risk

The current provisioner supplies the runtime password as a parameter to a
`SELECT format(...)` query, then executes the returned `ALTER ROLE` command.
Identifier/literal quoting protects SQL construction, but the final command
contains the plaintext password. Parameter masking does not protect a password
already embedded in command text.

The reviewed application code logs operational messages without credentials and
does not configure Npgsql command or parameter logging. Unhandled database
exceptions still reach container logs; failure behaviour needs verification.

Azure server settings reported during refinement are:

```ini
log_statement = NONE
log_min_error_statement = ERROR
log_min_duration_statement = -1
log_parameter_max_length = -1
log_parameter_max_length_on_error = 0
pgaudit.log = NONE
pgaudit.log_parameter = OFF
```

These disable routine statement/duration and audit logging, but a failing
password-bearing statement can still be recorded by error logging. Future
logging changes could introduce additional exposure.

The application owner accepts this PostgreSQL server-log exposure temporarily
for the current hobby environment, in order to deliver the restricted runtime
account without adding native interop or a custom cryptographic implementation.
This is a risk acceptance, not a claim that password-bearing SQL is safe under
all logging configurations. It does not automatically extend to future shared,
public, or higher-risk environments; reassess before creating those environments.

The story's credential acceptance boundary is explicit:

- Plaintext passwords must not appear in committed source, deployment/container
  logs, or ordinary Pulumi outputs. Verify both success and failure paths before
  completing the story, using disposable credentials rather than real secrets.
- PostgreSQL server statement/error logs are a separate, temporarily accepted
  exception. Treat them as sensitive and verify that access to them and any
  exported copies is restricted to authorized operators.
- Retain useful error diagnostics. Do not broadly suppress error logging to
  conceal password-setting statements. Review any logging/audit configuration
  changes for their effect on credential exposure.
- Key Vault changes secret storage and retrieval, not this SQL-logging exposure.

### Follow-up: Harden Password Provisioning Against Database Log Exposure

Track a separate hardening story before treating this risk as resolved:

- Select and document a supported approach that keeps plaintext passwords out of
  database statement/error logs, including failed password-setting operations.
- Compare client-generated SCRAM verifiers, available tooling/providers, and
  targeted logging/redaction controls without assuming any option meets every
  requirement. Protect verifiers as sensitive material too.
- Verify successful provisioning, repeat execution, failure behaviour, and API
  authentication with disposable credentials under the relevant logging modes.
- Include diagnostic visibility, Linux/Windows dependencies, operational cost,
  and Azure compatibility in the decision. Avoid bespoke cryptography.
- Update this ADR to remove or replace the temporary risk acceptance only after
  the selected approach is verified.

These follow-up criteria are recorded here for backlog capture; this ADR does
not imply that a Trello card has already been created.

## Validation Evidence and Acceptance Gates

Manual local checks completed during refinement:

- Existing games and machine positions survived the schema migration and restart.
- Compose sequencing succeeded with both an existing and a separate fresh volume.
- The API was configured with `game_state_runtime`; create/read/move operations
  worked and moves persisted after reload/restart.
- A direct runtime connection authenticated and read application data.
- Role attributes and effective grants matched the intended local restrictions;
  creating an application-schema table was rejected.
- Runtime access to temporary tables, migration-history writes, and connection
  to `postgres` was absent after reconciliation.
- Repeat provisioning succeeded; invalid identity/database and elevated-role
  probes were rejected.
- A deliberate failure rolled back probe-role creation.
- A held advisory lock blocked provisioning; releasing it allowed progress,
  while retaining it caused a timeout before account changes.

These are manual observations, not automated regression coverage or evidence of
Azure compatibility. Architecture acceptance does not mean the story is done.
Before completing the story:

- Verify the source, deployment/container-log, and Pulumi-output boundary on
  success and failure without exposing real credentials; verify restricted access
  to PostgreSQL logs and their exports under the accepted temporary risk.
- Verify operations with Azure's non-superuser administrator, including database
  ownership, shared permission changes, and repeated role management.
- Confirm future-object permissions under the actual migration identity and
  inspect effective access beyond the intended database/schema.
- Complete the ordered init-container deployment wiring selected by this ADR.
- Run deployed API integration/user-journey checks and inspect deployed identity
  and secret references; retire old revisions carrying administrator API config.

## Consequences and Future Progression

The restricted runtime account is a lasting permission boundary, independent
of where credentials are stored. A dedicated provisioner adds an executable,
image, deployment configuration, and security-sensitive code to maintain; it
must remain proportionate compared with the provider alternative.

Future stories can independently:

1. Move credential storage to Key Vault, with Container App secret references
   and managed identity access, subject to the platform's identity constraints.
2. Introduce a dedicated migration account with schema-changing permissions,
   removing administrator credentials from the migrator. Its account must be
   provisioned before migrations; runtime grants are reconciled afterwards, and
   default privileges must target the new object-creating identity.

Automated rotation, passwordless authentication, private networking, backups,
and Pulumi ESC remain outside this story. No native-library solution or switch
to the PostgreSQL provider is selected for this increment.

## References

- [ADR 0002: PostgreSQL persistence](0002-postgresql-for-game-state.md)
- [Azure Container Apps init containers](https://learn.microsoft.com/en-us/azure/container-apps/containers#init-containers)
- [Pulumi PostgreSQL provider](https://www.pulumi.com/registry/packages/postgresql/)
- [Azure PostgreSQL audit logging and password exposure](https://learn.microsoft.com/en-us/azure/postgresql/security/security-audit)
- [Npgsql logging](https://www.npgsql.org/doc/diagnostics/logging.html)
