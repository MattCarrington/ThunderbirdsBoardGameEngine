#!/usr/bin/env bash

set -euo pipefail

echo "PR Title: $PR_TITLE"
echo "Tag prefix: $TAG_PREFIX"

# Find the latest tag in the configured version stream.
LAST_TAG=$(git tag --list "${TAG_PREFIX}*" | sort -V | tail -n1)

if [ -z "$LAST_TAG" ]; then
  LAST_VERSION="0.0.0"
  echo "No tag found, starting from 0.0.0"
else
  LAST_VERSION=${LAST_TAG#${TAG_PREFIX}}
  echo "Found last tag: $LAST_TAG"
fi

IFS='.' read -r MAJOR MINOR PATCH <<< "$LAST_VERSION"

if [[ "$PR_TITLE" =~ BREAKING\ CHANGE || "$PR_TITLE" =~ "!:" ]]; then
  RELEASE_TYPE="major"
elif [[ "$PR_TITLE" =~ ^feat ]]; then
  RELEASE_TYPE="minor"
elif [[ "$PR_TITLE" =~ ^fix || "$PR_TITLE" =~ ^perf || "$PR_TITLE" =~ ^chore ]]; then
  RELEASE_TYPE="patch"
else
  RELEASE_TYPE="none"
fi

echo "Determined release type: $RELEASE_TYPE"

if [ "$RELEASE_TYPE" = "major" ]; then
  MAJOR=$((MAJOR + 1))
  MINOR=0
  PATCH=0
elif [ "$RELEASE_TYPE" = "minor" ]; then
  MINOR=$((MINOR + 1))
  PATCH=0
elif [ "$RELEASE_TYPE" = "patch" ]; then
  PATCH=$((PATCH + 1))
else
  echo "No release required; keeping version $LAST_VERSION"
fi

NEXT_VERSION="$MAJOR.$MINOR.$PATCH"
echo "Calculated version: $NEXT_VERSION"

if [[ "$RELEASE_TYPE" != "none" && "$GITHUB_REF" != "refs/heads/main" ]]; then
  NEXT_VERSION="${NEXT_VERSION}-beta.${GITHUB_RUN_NUMBER}"
  echo "Beta prerelease version applied: $NEXT_VERSION"
else
  echo "Main branch detected — producing stable version"
fi

echo "version=$NEXT_VERSION" >> "$GITHUB_OUTPUT"
echo "release_type=$RELEASE_TYPE" >> "$GITHUB_OUTPUT"
