Write-Host "Running pre-commit checks..."

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "dotnet CLI is not installed. Please install it to run pre-commit checks."
    exit 1
}

Write-Host "Running dotnet format..."
dotnet format --verify-no-changes
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Running dotnet build..."
dotnet build --configuration Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Pre-commit checks passed successfully."
exit 0