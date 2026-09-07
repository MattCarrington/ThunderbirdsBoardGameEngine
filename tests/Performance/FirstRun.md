# First local movement performance run

## Question and finding

On 7 September 2026, the fixed movement-validation workload showed no clear
latency degradation between one, five and ten paced virtual users. All 520
measured requests across smoke, baseline and load runs returned the expected
movement result. There were no 429s, unexpected responses or incorrect bodies.
All exploratory thresholds passed.

This establishes a runnable baseline for this scenario, not maximum capacity.
The experiment deliberately offers only about 10 requests/second at its highest
level, and does not attempt to saturate the API.

## Environment

- Application commit: `c4526a75c0da468d6fd2c12e54d58b23b378ced5` (local main).
- Branch: `codex/rules-performance-testing`; performance scripts and docs were
  uncommitted additions when run. Application source was unchanged.
- Windows 11 Pro, version 10.0.26200; AMD Ryzen 7 PRO 8845HS, 16 logical processors;
  approximately 27.8 GiB physical memory visible to Windows.
- Release net8.0 API, built with .NET SDK 10.0.400; installed .NET 8 runtime 8.0.30.
- Docker Engine 29.7.2; Linux `grafana/k6:2.2.0` image, digest
  `sha256:9bd01d6941fca969cb61bb57d2da5ee9b385fe2aa8881df3798c196564d6ace6`.
- API hosted directly on Windows at `http://localhost:5198`, reached from Docker
  through `http://host.docker.internal:5198`. API and generator shared the machine.
- Production ASP.NET environment, no launch profile; limiter overridden to 100000
  permits per 60 seconds for this process only. No database or browser involved.
- Five warm-up requests per run, excluded from the table. Each measured iteration
  waits for its response and then sleeps one second. API remained running between runs.
- CPU/GC/memory usage during phases was not sampled. Background activity was not
  controlled, so resource saturation and the cause of small timing differences
  cannot be inferred.

## Results

Successful-response timing is client-observed HTTP request duration, in milliseconds.

| Phase | Users | Duration | Correct responses | Approx. responses/s | p95 ms | Maximum ms |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Smoke | 1 | 10 s | 10 | 1 | 4.53 | 4.60 |
| Baseline | 1 | 30 s | 30 | 1 | 4.40 | 5.78 |
| Load: five users | 5 | 30 s | 150 | 5 | 3.42 | 4.26 |
| Load: ten users | 10 | 30 s | 300 | 10 | 3.91 | 6.29 |
| Recovery | 1 | 30 s | 30 | 1 | 3.72 | 3.97 |

Approximate phase throughput is count divided by planned phase duration; k6's
summary counter rates use the whole run duration, including warm-up/waiting.
There were no interrupted iterations. All phases were below the provisional
500 ms p95 budget, which is a learning threshold, not an agreed service objective.

Raw JSON files are retained locally in the ignored `results/` directory:

- `smoke-20260907-113034-206.json`
- `baseline-20260907-113129-719.json`
- `load-20260907-113216-031.json`

## Interpretation and next experiment

The lower five-user p95 does not demonstrate that adding users improves
performance: samples are small, runs are sequential, and runtime warm-up and
background activity can affect these measurements. The results provide no
evidence that this workload needs optimisation.

A useful next experiment is to repeat baseline and load under the same conditions
while sampling API CPU, memory and GC. If exploring capacity after that, increase
the offered load deliberately and consider a fixed arrival-rate workload, since
this closed model slows request generation when responses slow down. Broaden
movement inputs before making claims about the whole rules engine.

The default limiter's rejection behaviour was not exercised in these runs; the
separate 429 metric exists but its positive case remains to be verified. The
[run guide](README.md) describes that follow-up experiment.
