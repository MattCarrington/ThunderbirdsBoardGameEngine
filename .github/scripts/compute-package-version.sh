#!/usr/bin/env bash

set -euo pipefail

BASE_VERSION=$(dotnet msbuild "$PROJECT_PATH" -getProperty:Version -nologo)
DEFAULT_BRANCH="${DEFAULT_BRANCH:-main}"
RELEASE_REF="refs/heads/${DEFAULT_BRANCH}"

echo "Base version from csproj: $BASE_VERSION"
echo "Package ID: $PACKAGE_ID"

STABLE_EXISTS=false
PAGE=1
MAX_PAGES=50
RESPONSE_FILE=$(mktemp)
trap 'rm -f "$RESPONSE_FILE"' EXIT

while [ "$PAGE" -le "$MAX_PAGES" ]; do
  echo "Fetching package versions (page $PAGE)..."
  URL="https://api.github.com/users/${OWNER}/packages/nuget/${PACKAGE_ID}/versions?per_page=100&page=${PAGE}"

  HTTP_STATUS=$(curl -sS -w "%{http_code}" -o "$RESPONSE_FILE" \
    -H "Authorization: Bearer $GITHUB_TOKEN" \
    -H "Accept: application/vnd.github+json" \
    "$URL")

  if [ "$HTTP_STATUS" = "404" ]; then
    echo "Package not found — first publish allowed"
    STABLE_EXISTS=false
    break
  fi

  if [ "$HTTP_STATUS" != "200" ]; then
    echo "Unexpected HTTP status $HTTP_STATUS when querying package versions"
    echo "Response body from GitHub API:"
    cat "$RESPONSE_FILE"
    exit 1
  fi

  COUNT=$(jq 'length' "$RESPONSE_FILE")

  if [ "$COUNT" -eq 0 ]; then
    break
  fi

  jq -r '.[].name' "$RESPONSE_FILE"

  if jq -e --arg version "$BASE_VERSION" '.[] | select(.name == $version)' "$RESPONSE_FILE" >/dev/null; then
    STABLE_EXISTS=true
    break
  fi

  PAGE=$((PAGE + 1))
done

if [ "$PAGE" -gt "$MAX_PAGES" ]; then
  echo "Exceeded maximum pagination limit (${MAX_PAGES}) while scanning package versions"
  exit 1
fi

if [ "$STABLE_EXISTS" = true ]; then
  echo "Stable version $BASE_VERSION already exists."

  if [ "$GITHUB_REF" != "$RELEASE_REF" ]; then
    echo "ERROR: The release line for $PACKAGE_ID $BASE_VERSION is closed." >&2
    echo "Increment the package version manually before publishing another prerelease." >&2
    exit 1
  fi

  echo "Stable package already published. Skipping this idempotent main-branch publish."
  echo "version=$BASE_VERSION" >> "$GITHUB_OUTPUT"
  echo "should_publish=false" >> "$GITHUB_OUTPUT"
  exit 0
fi

echo "No stable version found. Publishing allowed."
echo "should_publish=true" >> "$GITHUB_OUTPUT"

if [ "$GITHUB_REF" = "$RELEASE_REF" ]; then
  FINAL_VERSION="$BASE_VERSION"
  echo "main branch — stable version $FINAL_VERSION"
else
  FINAL_VERSION="$BASE_VERSION-beta.${GITHUB_RUN_NUMBER}"
  echo "branch — prerelease version $FINAL_VERSION"
fi

echo "version=$FINAL_VERSION" >> "$GITHUB_OUTPUT"
