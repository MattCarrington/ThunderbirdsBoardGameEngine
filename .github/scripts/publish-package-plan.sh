#!/usr/bin/env bash

set -euo pipefail

if [ -z "${PACKAGE_PLAN:-}" ]; then
  echo "ERROR: PACKAGE_PLAN must be set" >&2
  exit 1
fi

ARTIFACT_ROOT="${ARTIFACT_ROOT:-./artifacts/packages}"
mkdir -p "$ARTIFACT_ROOT"

while IFS= read -r package; do
  should_publish=$(jq -r '.should_publish' <<<"$package")

  if [ "$should_publish" != true ]; then
    continue
  fi

  package_id=$(jq -r '.package_id' <<<"$package")
  project_path=$(jq -r '.project_path' <<<"$package")
  version=$(jq -r '.version' <<<"$package")
  package_dir="$ARTIFACT_ROOT/$package_id"

  mkdir -p "$package_dir"

  echo "Packing $package_id $version"
  dotnet pack "$project_path" \
    --configuration Release \
    --output "$package_dir" \
    -p:ContinuousIntegrationBuild=true

  package_file="$package_dir/$package_id.$version.nupkg"
  if [ ! -f "$package_file" ]; then
    echo "ERROR: Expected package was not produced: $package_file" >&2
    exit 1
  fi

  echo "Publishing $package_id $version"
  dotnet nuget push "$package_file" \
    --source "https://nuget.pkg.github.com/${GITHUB_REPOSITORY_OWNER}/index.json" \
    --api-key "$GITHUB_TOKEN" \
    --skip-duplicate
done < <(jq -c '.[]' <<<"$PACKAGE_PLAN")
