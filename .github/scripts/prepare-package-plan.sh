#!/usr/bin/env bash

set -euo pipefail

required_env() {
  local name="$1"
  if [ -z "${!name:-}" ]; then
    echo "ERROR: $name must be set" >&2
    exit 1
  fi
}

required_env "PACKAGE_PLAN"
required_env "OWNER"
required_env "GITHUB_TOKEN"
required_env "GITHUB_REF"
required_env "GITHUB_RUN_NUMBER"

SCRIPT_DIRECTORY=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
VERSION_SCRIPT="$SCRIPT_DIRECTORY/compute-package-version.sh"
WORK_DIR=$(mktemp -d)
trap 'rm -rf "$WORK_DIR"' EXIT

prepared_plan="$PACKAGE_PLAN"

while IFS= read -r package; do
  package_id=$(jq -r '.package_id' <<<"$package")
  project_path=$(jq -r '.project_path' <<<"$package")
  package_output="$WORK_DIR/${package_id}.output"

  echo "Computing version for $package_id"

  PROJECT_PATH="$project_path" \
    PACKAGE_ID="$package_id" \
    OWNER="$OWNER" \
    GITHUB_TOKEN="$GITHUB_TOKEN" \
    GITHUB_REF="$GITHUB_REF" \
    GITHUB_RUN_NUMBER="$GITHUB_RUN_NUMBER" \
    GITHUB_OUTPUT="$package_output" \
    bash "$VERSION_SCRIPT"

  version=$(sed -n 's/^version=//p' "$package_output")
  should_publish=$(sed -n 's/^should_publish=//p' "$package_output")

  if [ -z "$version" ] || { [ "$should_publish" != true ] && [ "$should_publish" != false ]; }; then
    echo "ERROR: Version computation returned invalid output for $package_id" >&2
    exit 1
  fi

  # All selected project versions are applied before packing. Project references
  # therefore become dependencies on betas selected in this run, while references
  # outside the plan retain their stable versions.
  sed -i "0,/<Version>.*<\/Version>/s//<Version>${version}<\/Version>/" "$project_path"

  prepared_plan=$(jq -c \
    --arg id "$package_id" \
    --arg version "$version" \
    --argjson should_publish "$should_publish" \
    'map(if .package_id == $id then . + {version: $version, should_publish: $should_publish} else . end)' \
    <<<"$prepared_plan")
done < <(jq -c '.[]' <<<"$PACKAGE_PLAN")

echo "Prepared package plan:"
jq -r '.[] | "  \(.package_id) \(.version) (publish: \(.should_publish))"' <<<"$prepared_plan"

if [ -n "${GITHUB_OUTPUT:-}" ]; then
  echo "plan=$prepared_plan" >> "$GITHUB_OUTPUT"
else
  echo "$prepared_plan"
fi
