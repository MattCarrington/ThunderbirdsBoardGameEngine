#!/usr/bin/env bash

set -euo pipefail

if [ -z "${PACKAGE_PLAN:-}" ]; then
  echo "ERROR: PACKAGE_PLAN must be set" >&2
  exit 1
fi

SCRIPT_DIRECTORY=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
VALIDATION_SCRIPT="$SCRIPT_DIRECTORY/validate-nuget-package-consumption.sh"

while IFS= read -r package; do
  if [ "$(jq -r '.should_publish' <<<"$package")" != true ]; then
    continue
  fi

  PACKAGE_ID=$(jq -r '.package_id' <<<"$package") \
    PACKAGE_VERSION=$(jq -r '.version' <<<"$package") \
    GITHUB_OWNER="$GITHUB_REPOSITORY_OWNER" \
    GITHUB_TOKEN="$GITHUB_TOKEN" \
    bash "$VALIDATION_SCRIPT"
done < <(jq -c '.[]' <<<"$PACKAGE_PLAN")
