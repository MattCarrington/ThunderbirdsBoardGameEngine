$hookPath = ".git/hooks/pre-commit"

@"
#!/bin/sh
powershell.exe -ExecutionPolicy Bypass -NoProfile -File scripts/pre-commit.ps1
"@ | Set-Content -NoNewline $hookPath

Write-Host "Pre-commit hook installed."