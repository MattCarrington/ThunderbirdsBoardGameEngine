# Pulumi Azure DEV Walkthrough

This folder contains a minimal Pulumi + .NET infrastructure project for learning IaC with Azure.

## Architecture

The deployed application is **co-hosted**: both the UI (Blazor WebAssembly) and API (ASP.NET Core) run in the same container.

- The API is the main application (port 8080)
- The UI is published and served as static files from the API's `wwwroot` folder
- The UI endpoint selection is explicit via `RulesClient:EndpointMode`
- When deployed, both components share the same public URL

This means:
- No CORS complications (same origin)
- Single container to manage
- Co-hosted mode uses runtime browser origin for API calls at deployment URL

Rules client endpoint modes:
- `CoHosted`: Uses browser origin at runtime (`builder.HostEnvironment.BaseAddress`). Best for same-container deployments.
- `External`: Uses configured `RulesClient:BaseAddress`. Best for local development with separate API host.

## What it provisions

- Azure Resource Group
- Azure Container App Environment
- Azure Container App
- External HTTPS ingress for the Container App

The stack exports an application URL so you can quickly verify deployment success.

## Prerequisites

1. Install a .NET 10 SDK meeting the baseline in [global.json](../../../global.json).
2. Install Pulumi CLI.
3. Install Azure CLI.
4. Sign in and select subscription:

	az login
	az account set --subscription "<your-subscription-name-or-id>"

## Local deployment flow

From this folder:

1. Initialize or select the stack:

	pulumi stack init dev

	If it already exists:

	pulumi stack select dev

2. Restore and preview:

	dotnet restore
	pulumi preview

3. Deploy:

	pulumi up

	The `game_state` table move was completed in DEV with release `0.17.4`.
	See the maintenance record below before deploying to an older environment;
	do not repeat the outage merely for an already-applied migration.

4. Check outputs:

	pulumi stack output resourceGroupName
	pulumi stack output containerAppEnvironmentName
	pulumi stack output containerAppName
	pulumi stack output containerImage
	pulumi stack output applicationUrl

## Planned downtime for the Game State schema move

The migration from `public` tables to the `game_state` schema is incompatible
with the previous API image. A running old revision can fail as soon as the
tables move, even if the replacement revision has not started successfully.
The application owner accepts a planned outage for this transition, as recorded
in [ADR 0005](../../../docs/architecture/decisions/0005-least-privileged-game-state-database-access.md).

**DEV status:** the one-time table move was deployed on 2026-10-23 as release
`0.17.4`, using a manual stop-old-revision gate. It is no longer pending for this
database. The workflow has a GitHub environment approval checkpoint, not a
reusable automatic maintenance mechanism. An ordinary repeat deployment does
not reapply a recorded EF migration.

### Completed DEV maintenance record

- Builds, tests, and publication completed for the merged release. Deployment
  waited for the required review on GitHub environment `dev`.
- Before approval, the operator switched Azure revision mode to `Multiple` and
  deactivated `tbbge-dev-app96f32a3e--0000043`.
- A direct revision query confirmed `Active: false` and `Replicas: 0`. This was
  the evidence used to release deployment. A separate replica-list command
  failed due to Windows CLI parsing and supplied no additional evidence.
- The operator approved deployment only after that confirmation. Revision
  `tbbge-dev-app96f32a3e--0000044` became active and healthy with one replica;
  its migrator and provisioner used release `0.17.4`.
- Azure resumed service as the replacement became ready; smoke tests passed,
  and existing gameplay/read-update behaviour against `game_state` was checked.
  There was no additional traffic hold pending all post-deployment checks.
- The operator restored `Single` revision mode and verified the API's runtime
  secret reference. Subsequent direct database permission checks are recorded
  in ADR 0005.

This establishes that the old revision was stopped before the table-move
deployment proceeded. It does not prove Azure failure/cancellation recovery,
database downgrade, or independent session draining. Those paths were not
exercised; reusable maintenance and disposable upgrade/recovery rehearsal are
follow-up work. Historical init-container console logs were not retrieved.

### Procedure for another pre-migration environment

The following is retained for an environment still on the old schema, or as a
starting point for a future incompatible migration. It is not a pending action
for the completed DEV release. Review the actual schema/version and rehearse
recovery for the target environment; do not claim the successful DEV deployment
validated every step below.

### Before migration

1. Record the current image references, revision/traffic configuration, and
   applied migration state without exporting secrets. Identify the matching
   new API, migrator, and provisioner images and review the migration SQL.
2. Establish a recovery point appropriate to the existing data and identify the
   previous migration to which a downgrade would return. This is deployment
   preparation, not implementation of a new backup service in this story.
3. Prevent overlapping deployments and enter maintenance. Stop all old
   application replicas, allow in-flight work to end, and prevent old revisions
   from automatically restarting. Merely routing new requests elsewhere does
   not prove old database activity has stopped.
4. Confirm old application database activity has ended before allowing the
   replacement's migrator to execute. The maintenance mechanism must permit
   replacement initialization without also restarting an incompatible old API.

### Deploy and verify

1. Apply the replacement configuration with matching images. Its init containers
   must execute **migrator -> provisioner -> API**; only the API receives the
   runtime connection string.
2. Verify setup-container completion, replacement readiness, the configured
   runtime identity, and restricted permissions. Check logs without displaying
   credentials. If service must remain blocked until all functional checks pass,
   arrange and verify a separate traffic gate; revision deactivation and GitHub
   approval alone do not provide that hold after the replacement starts.
3. Retrieve an existing game, create a game, move a machine, and retrieve the
   updated state. Verify persistence and any relevant concurrency checks.
4. If using a separate traffic gate, restore public service after checks succeed;
   otherwise verify the endpoint after Azure resumes service on readiness.
   Retire old revisions that retain administrator API
   configuration; do not reactivate them against the new schema.

### If deployment fails

- Keep public service in maintenance and old revisions stopped. Determine
  whether migrations committed, which grants were applied, and whether the
  failure occurred during provisioning or API startup.
- Prefer fixing the replacement and completing forward. Provisioning failure
  does not roll back an earlier successful EF migration.
- Do not restore the previous API image alone: it expects the original tables
  in `public` and is incompatible after the move.
- If reverting is necessary, stop replacement replicas and prevent competing
  initialization. Generate and review the EF downgrade SQL for the actual
  current migration state and intended previous migration. Apply it through a
  controlled administrator operation, verify tables/data and permissions in the
  old schema, then restore the compatible prior image and verify its game
  journey before restoring service. Inspect provisioner-managed account/grant
  changes separately; EF downgrade does not reverse them.
- Do not automatically downgrade on deployment errors, delete database volumes,
  or edit previously applied migrations as a recovery shortcut.

### Follow-up verification

The completed DEV record supplies the successful manual stop/deploy evidence
for this release. General failure/cancellation handling and schema downgrade
remain unverified and belong to the maintenance and upgrade-rehearsal backlog
stories. Rehearse these against disposable data before relying on them for a
future environment requiring stronger recovery guarantees. Existing local
restart tests alone do not establish old/new revision compatibility.

If uninterrupted deployment becomes necessary, use a separately reviewed
compatibility transition (for example, verified updatable views at the old
table names, removed in a later migration) instead of this outage procedure.

## Configuration

### Runtime-account permission scope

The API uses a dedicated runtime role with the application-data permissions it
needs in the `game_state` schema. The migrator and provisioner retain
administrator access for this increment. See
[ADR 0005](../../../docs/architecture/decisions/0005-least-privileged-game-state-database-access.md)
for the accepted boundary and database-log risk.

This is not server-wide hardening: inherited PostgreSQL/platform defaults may
permit connections to other databases. `CONNECT` alone does not grant access
to their tables or administration. Do not revoke shared `PUBLIC` privileges on
Azure-managed databases merely to make a runtime-role check pass. Such changes
affect other roles and require a separate impact assessment.

The remaining provisioner statement revoking `TEMPORARY` from `PUBLIC` also
requires scope review. Existing database ACL changes persist even if a statement
is removed from code; do not assume removal restores earlier defaults.

### Container Image

The **containerImage** is the core of your deployment. It's a pre-built Docker image containing both the API and UI (as static files).

Build and push your image before deploying:

```bash
docker build -t ghcr.io/mattcarrington/thunderbirds-app:sha-6fc39bc .
docker push ghcr.io/mattcarrington/thunderbirds-app:sha-6fc39bc
```

Then configure Pulumi to use it:

```bash
pulumi config set thunderbirds-azure-dev:containerImage ghcr.io/mattcarrington/thunderbirds-app:sha-6fc39bc
```

### All Config Keys

- azure-native:location
- thunderbirds-azure-dev:baseName
- thunderbirds-azure-dev:containerImage (default: ghcr.io/mattcarrington/thunderbirds-app:latest)
- thunderbirds-azure-dev:containerPort (default: 8080)
- thunderbirds-azure-dev:registryUsername (optional, for private registries)
- thunderbirds-azure-dev:registryPassword (optional, for private registries; must be set as a Pulumi secret)

When using registry credentials, set both registryUsername and registryPassword together, and ensure containerImage includes an explicit registry server (for example `ghcr.io/...` or `myregistry.azurecr.io/...`). The Pulumi program derives the registry server from containerImage.

### Examples

pulumi config set azure-native:location westeurope
pulumi config set thunderbirds-azure-dev:baseName tbbge2
pulumi config set thunderbirds-azure-dev:containerImage myregistry.azurecr.io/thunderbirds-api:1.0.0
pulumi config set thunderbirds-azure-dev:containerPort 8080

# For private container registries:
pulumi config set thunderbirds-azure-dev:registryUsername <username>
pulumi config set --secret thunderbirds-azure-dev:registryPassword <password>

If you are using a public image, you do not need to set registryUsername or registryPassword.

All CLI commands in this guide work in PowerShell as well as Bash.

## How the Co-Hosted Deployment Works

When the app is deployed to Azure:

1. The container starts and the API runs on port 8080
2. The browser loads the UI from the API's `wwwroot` folder
3. `RulesClient:EndpointMode` is `CoHosted`, so the UI resolves API base URL from the **browser's current origin**
   - In Azure: `https://tbbge-dev-app96f32a3e--0000002.victorioussand-830bb169.uksouth.azurecontainerapps.io`
4. The UI makes API calls to that same origin (no CORS needed)

For local development with separately hosted API:

- Set `RulesClient:EndpointMode` to `External`
- Set `RulesClient:BaseAddress` to your local API URL (for example `https://localhost:8080/`)

This endpoint policy is handled in `src/Ui/ThunderbirdsBoardGameEngine.UI/Program.cs` and makes deployment behavior explicit.

## CI/CD readiness

This is ready to check in and use in CI/CD for DEV.

Recommended approach:

1. Use Pulumi Service as backend.
2. Authenticate to Azure in GitHub Actions with OpenID Connect (OIDC) using azure/login.
3. Run Pulumi non-interactively:

	pulumi stack select dev
	pulumi preview --non-interactive
	pulumi up --yes --non-interactive

4. Store sensitive values as secrets (do not hard-code in source). Use:

	pulumi config set --secret <key> <value>

## Cleanup

Destroy all resources created by this stack:

pulumi destroy

Optionally remove local stack metadata:

pulumi stack rm dev
