# ADR 0006: Separate Cancellable Validation from DEV Deployment

- Status: Accepted design for the DEV deployment increment; not yet implemented
- Date: 2026-10-23

## Context

The current CI workflow builds, tests, publishes, deploys to DEV after merges
to `main`, and runs smoke tests. Workflow-level concurrency cancels superseded
runs, including any deployment in those runs. Setting different concurrency on
a called reusable workflow does not protect it from its parent being cancelled.

DEV is currently the only Azure application environment. Optional feature-branch
deployments will share it; isolated feature environments are outside this
increment. Persistent PostgreSQL schema history survives application changes,
so replacing an image is not a database rollback.

The target release flow is PR validation, automatic deployment of integrated
`main` releases to DEV, and eventually manual promotion to a stable PRD
environment. Operating-hours automation and PRD do not exist yet and are
separate increments.

## Decision

### Cancellation and Execution Boundary

Separate validation/artifact preparation from an independent DEV release
workflow. Keep the existing push and pull-request validation triggers. Early
work may be automatically cancelled when superseded. Once the deployment stage
starts, newer runs must not automatically cancel it, including environment
wake-up, infrastructure updates, migrations, provisioning, readiness, and smoke
verification.

Manual emergency cancellation remains possible and requires recovery awareness.
This design does not promise that infrastructure failures or timeouts cannot
interrupt deployment. Failed deployment must not automatically downgrade the
database or reactivate an incompatible application revision.

The independent release workflow may be triggered by successful CI completion
for `main` and by an authorized manual branch-release request. A reusable
workflow called directly from cancellable CI is not an independent boundary.
The exact trigger and handoff implementation must be reviewed and tested before
the existing CI deploy/smoke jobs are removed.

### Release Selection and Trust

- Automatic DEV deployment follows successful push CI on `main`, after required
  tests and all three application images are published. Failed, cancelled, PR,
  or no-release runs must not accidentally initiate deployment.
- Manual branch deployment selects a commit with successful required validation
  and available artifacts. It does not deploy arbitrary unvalidated image tags.
- Bind the release to its CI run, source commit, and API/migrator/provisioner
  artifact identities. Prefer immutable image digests. Never resolve a queued
  release by checking out whatever a mutable branch points to at execution time.
- Do not rebuild application images during deployment. Record what was tested
  and deployed; identify trusted infrastructure/workflow code separately from
  the selected application artifacts.
- Privileged release triggers must validate repository, event, ref, run result,
  and artifact provenance before using credentials. A successful untrusted PR
  run must not gain deployment access through the handoff.
- Manual deployment is restricted to authorized project contributors. The exact
  GitHub permission/environment controls must be verified in implementation.

### Shared DEV and Deployment Coordination

- Serialize all DEV release operations through one environment-specific lock or
  equivalent concurrency boundary, including post-deployment verification.
- Preserve an active deployment when newer validation finishes. Define queue
  behaviour explicitly; do not assume basic concurrency provides commit-order
  execution or unlimited pending runs. Reject stale automatic candidates before
  deployment starts so late-finishing CI cannot silently roll DEV backwards.
- An explicit manual branch deployment may replace shared DEV; display the
  deployed commit, branch, image identities, and results clearly. It stays until
  another deliberate branch deployment or automatic `main` release replaces it.
  There is no automatic restoration of the previous application version.
- A subsequent `main` release can replace a branch deployment only subject to
  schema compatibility. Application rollback does not remove branch migrations.
- Routine DEV deployment should not require approval after successful `main`
  validation. Removing the existing `dev` approval rule is an explicit cutover
  action, not something workflow YAML alone achieves. The rule was introduced
  for the completed one-time schema-move maintenance operation in ADR 0005.
- Incompatible changes still require a deliberate maintenance decision and
  recovery procedure. General maintenance automation is not part of this story.

### Shared Database Policy

Branch deployments without migrations follow the normal manual release path.
Branch deployments introducing migrations require a compatibility review before
deployment, with the applied migration history recorded for shared DEV.

Keep branch schema changes compatible with current `main`; do not deploy
conflicting or incompatible migrations to shared DEV. Review the ability of a
subsequent `main` release to operate against the resulting schema. Never
automatically downgrade to make a branch fit. Incompatible experiments remain
local or use future disposable-environment support. The initial review policy
may be procedural; automated schema-conflict analysis is not required here.

### Contract with Future DEV Operating Hours

The separate cost-control story proposes DEV readiness by 10:00 and shutdown
from 22:00 every day in `Europe/London`, including daylight-saving changes.

When that story is implemented, deployment must wake DEV and wait for database
readiness before initialization. Shutdown must not race with startup, deployment,
or verification. If deployment crosses shutdown time or occurs out of hours,
defer shutdown until one hour after deployment/verification finishes. Provide
manual wake-up and a time-limited keep-awake override. The scheduler must account
for failed/cancelled deployment and avoid indefinite accidental keep-awake.

These are agreed integration requirements, not implemented scheduling. Cost
assessment precedes scheduling automation; PRD has its own availability policy.

## Alternatives

- Disable cancellation for all CI: protects deployment but wastes superseded
  validation work and does not provide the requested stage boundary.
- Add only job-level concurrency inside cancellable CI: serializes some work
  but cannot protect it from cancellation of the parent run.
- Provision per-branch environments: better isolation, but adds costs and
  lifecycle work outside the shared-DEV increment.
- Build a general release/maintenance platform now: deferred until the basic
  release path and manual procedures are proven.

## Validation and Cutover

Before claiming this design is implemented:

- Demonstrate that superseded early validation can cancel, while a newer push
  cannot cancel an independent active deployment or its smoke verification.
- Exercise automatic success, failed/cancelled/no-release CI, authorized manual
  branch release, and rejection of untrusted or mismatched candidates.
- Verify exact artifact selection, environment serialization, pending/stale
  candidate policy, and release identity evidence.
- Review shared database migration compatibility and record branch deployment
  history. Use disposable/local data for destructive failure tests.
- Cut over the triggers and remove the old deploy/smoke jobs together, avoiding
  duplicate deployment paths. Verify the GitHub environment approval policy.
- Keep DEV usable; this ADR does not authorize an immediate Azure change.

## Consequences

The release handoff gains explicit provenance and a separate lifecycle. There
is additional workflow configuration, but no new permanent environment is
required. Shared DEV remains replaceable and branch schema changes need human
coordination. Scheduling, isolated DEV/PRD, promotion, upgrade/recovery rehearsal,
and reusable maintenance remain linked backlog stories rather than prerequisites
for every part of this increment.

## References

- [Workflow current and target states](../../DeploymentWorkflow.md)
- [ADR 0005: Runtime database access and completed maintenance](0005-least-privileged-game-state-database-access.md)
- [GitHub workflow completion triggers](https://docs.github.com/en/actions/reference/workflows-and-actions/events-that-trigger-workflows#workflow_run)
- [GitHub concurrency](https://docs.github.com/en/actions/concepts/workflows-and-actions/concurrency)
