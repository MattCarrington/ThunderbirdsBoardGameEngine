param(
    [ValidateSet('smoke', 'baseline', 'load')]
    [string]$Profile = 'smoke',
    [string]$BaseUrl = 'http://host.docker.internal:5198',
    [ValidateRange(1, 60000)]
    [int]$P95Ms = 500
)

$ErrorActionPreference = 'Stop'
$resultDirectory = Join-Path $PSScriptRoot 'results'
New-Item -ItemType Directory -Path $resultDirectory -Force | Out-Null
$runName = '{0}-{1}' -f $Profile, (Get-Date -Format 'yyyyMMdd-HHmmss-fff')

# Mount only this test directory. Docker/.env and other local config are not used.
& docker run --rm `
    --mount "type=bind,source=$PSScriptRoot,target=/scripts,readonly" `
    --mount "type=bind,source=$resultDirectory,target=/results" `
    grafana/k6:2.2.0 run `
    --env "PROFILE=$Profile" `
    --env "BASE_URL=$BaseUrl" `
    --env "P95_MS=$P95Ms" `
    --env "SUMMARY_PATH=/results/$runName.json" `
    /scripts/movement.js

if ($LASTEXITCODE -ne 0) {
    throw "Performance run failed (exit $LASTEXITCODE). Review the output and results/$runName.json if generated."
}
Write-Host "Results: $resultDirectory/$runName.json"
