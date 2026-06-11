$repoRoot = git rev-parse --show-toplevel
if ($LASTEXITCODE -ne 0) {
    Write-Host "Could not determine Git repository root."
    exit 1
}

$hookPath = Join-Path $repoRoot ".git/hooks/pre-commit"
$scriptPath = Join-Path $repoRoot "scripts/pre-commit.ps1"

$hookContent = @"
#!/bin/sh
powershell.exe -ExecutionPolicy Bypass -NoProfile -File "$scriptPath"
"@

[System.IO.File]::WriteAllText($hookPath, $hookContent, [System.Text.Encoding]::ASCII)

# Make executable where supported
git update-index --chmod=+x scripts/pre-commit.ps1 2>$null

Write-Host "Pre-commit hook installed at $hookPath"