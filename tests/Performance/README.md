# First performance experiment: movement validation

See [the first local run report](FirstRun.md) for measured results and limitations.

Question: how does response time change between one, five and ten simultaneous
users validating the same movement? This is an exploratory API experiment, not
a production capacity claim or an isolated algorithm benchmark.

The test calls the real stateless rules API: Thunderbird 2 moves from
`south-pacific` to `europe`. It checks the result as well as HTTP status, using
the same expected values as the API component test. No database or game creation
is needed. The HTTP pipeline, reference catalogs, rules and serialization are included.

## Run it

Requires .NET 8 (or a newer SDK able to build net8.0) and running Docker Desktop
with Linux containers. The runner downloads the pinned k6 image on first use.
Run commands from the repository root.

1. In a terminal, start a Release API on a dedicated local port:

   ```powershell
   dotnet run --project src/Api/ThunderbirdsBoardGameEngine.Api -c Release --no-launch-profile -- --urls http://localhost:5198 --RateLimiting:PublicApi:PermitLimit 100000
   ```

   Keep this terminal open. The high limiter allowance applies only to this
   process. It prevents the default 100 requests per 60 seconds (no queue) from
   masking capacity with quick 429 responses. It does not change appsettings.
   This HTTP-only local launch avoids development HTTPS certificates; if your
   environment redirects HTTP to HTTPS, remove that environment override or
   target an appropriately trusted HTTPS listener instead. Redirects fail the test.

2. In another terminal, check the script and API with a short run:

   ```powershell
   ./tests/Performance/run.ps1 -Profile smoke
   ```

3. Establish a baseline and then apply a small stepped load:

   ```powershell
   ./tests/Performance/run.ps1 -Profile baseline
   ./tests/Performance/run.ps1 -Profile load
   ```

4. Stop the API with Ctrl+C when finished. The k6 containers remove themselves.

The Docker runner reaches the host API through `host.docker.internal` on Docker
Desktop. `-BaseUrl` can select another test instance. Neither Docker Compose nor
`Docker/.env` is used. Results are timestamped JSON files under ignored `results/`.

If k6 is already installed locally, the equivalent smoke command is:

```powershell
k6 run -e PROFILE=smoke -e BASE_URL=http://localhost:5198 -e SUMMARY_PATH=tests/Performance/results/local-smoke.json tests/Performance/movement.js
```

Create `tests/Performance/results` first for the native command.

## Workload and measurements

Each run makes five preflight/warm-up requests, one second apart. These are
excluded from the custom `movement_*` metrics, but included in k6's built-in HTTP
metrics. This is a short warm-up, not proof that runtime behaviour has stabilised.

| Profile | Measured workload |
| --- | --- |
| smoke | One virtual user for 10 seconds |
| baseline | One virtual user for 30 seconds |
| load | Five users for 30 seconds, ten for 30 seconds, then one for 30 seconds; five-second gaps between phases |

A virtual user sends one request, waits for the response, then pauses one second.
This is a **closed workload**: throughput falls if responses slow down. User
counts and pacing are learning assumptions, not observed player behaviour. The
load profile is a modest concurrency probe, not a stress test that finds a limit.

- `movement_success_duration`: milliseconds for correct HTTP 200 responses only;
  separate phase metrics expose p95 at each user count.
- `movement_successes`: correct response count and rate. Phase rates in the k6
  summary may use the whole run duration; use each phase count / 30 seconds as an
  approximate phase throughput, accounting for requests finishing in graceful stop.
- `movement_rate_limited`: fraction of measured requests returning 429.
- `movement_unexpected_response`: fraction returning another status or a transport error.
- `movement_incorrect_body`: fraction returning HTTP 200 with an incorrect result.

Thresholds require at least one correct response per phase, no rejected/unexpected/
incorrect responses, and successful p95 below **500 ms** per phase. The latency
budget is explicitly provisional; override it with `-P95Ms`. Passing it does not
establish a user requirement. A breached threshold makes the runner fail.

To observe the default limiter separately, restart the API without the
`--RateLimiting:PublicApi:PermitLimit 100000` arguments and run the load profile.
429s should fail the capacity thresholds; inspect their separate rate rather than
treating them as fast successful requests. Restart between experiments to reset
the limiter window. This is observation, not a precise limiter correctness test.

## Interpret and report

Compare baseline, five-user, ten-user and recovery p95 alongside errors and
successful throughput. A low overall average can hide slow responses or quick
rejections. Watch API CPU and memory in Task Manager at each phase and record
them if investigating a change; this script does not collect server telemetry.

For an interview report, record:

- Date, commit (`git rev-parse HEAD`), any uncommitted test changes, machine/OS,
  .NET and k6 versions, Release configuration, target and limiter settings.
- Profile, request count, successful p95 by phase, rejection/error rates and any
  resource observations. Keep the raw JSON locally.
- What changed as load increased; evidence for any suspected bottleneck; the next
  experiment that would test that explanation.

Repeat a surprising result under the same conditions before drawing conclusions.
The generator and API share a machine; background activity, Docker networking,
JIT/GC and the small sample can influence results. This fixed route does not
represent all movement inputs, cold startup, browser rendering or production load.
No CI gate or optimisation is justified by this first experiment alone.

References: [k6 getting started](https://grafana.com/docs/k6/latest/get-started/),
[response classification](https://grafana.com/docs/k6/latest/javascript-api/k6-http/set-response-callback/),
[custom summaries](https://grafana.com/docs/k6/latest/results-output/end-of-test/custom-summary/).
