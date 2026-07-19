#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIRECTORY=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
COMPUTE_VERSION_SCRIPT="$SCRIPT_DIRECTORY/compute-version.sh"
TEST_ROOT=$(mktemp -d)
trap 'rm -rf "$TEST_ROOT"' EXIT

PASSED=0

run_test() {
  local name="$1"
  local title="$2"
  local git_ref="$3"
  local tag_prefix="$4"
  local expected_version="$5"
  local expected_release_type="$6"
  shift 6

  local test_directory="$TEST_ROOT/test-$PASSED"
  local output_file="$test_directory/github-output"
  mkdir -p "$test_directory"

  git -C "$test_directory" init --quiet
  git -C "$test_directory" config user.name "CI Test"
  git -C "$test_directory" config user.email "ci-test@example.com"
  git -C "$test_directory" commit --quiet --allow-empty -m "Initial commit"

  local tag
  for tag in "$@"; do
    git -C "$test_directory" tag "$tag"
  done

  (
    cd "$test_directory"
    PR_TITLE="$title" \
      TAG_PREFIX="$tag_prefix" \
      GITHUB_REF="$git_ref" \
      GITHUB_RUN_NUMBER="42" \
      GITHUB_OUTPUT="$output_file" \
      bash "$COMPUTE_VERSION_SCRIPT" >/dev/null
  )

  local actual_version
  local actual_release_type
  actual_version=$(sed -n 's/^version=//p' "$output_file")
  actual_release_type=$(sed -n 's/^release_type=//p' "$output_file")

  if [ "$actual_version" != "$expected_version" ]; then
    echo "FAIL: $name: expected version '$expected_version', got '$actual_version'" >&2
    exit 1
  fi

  if [ "$actual_release_type" != "$expected_release_type" ]; then
    echo "FAIL: $name: expected release type '$expected_release_type', got '$actual_release_type'" >&2
    exit 1
  fi

  PASSED=$((PASSED + 1))
  echo "PASS: $name"
}

run_test "no existing tag" "feat: first feature" "refs/heads/main" "server/v" "0.1.0" "minor"
run_test "feature increments minor" "feat: add feature" "refs/heads/main" "server/v" "1.3.0" "minor" "server/v1.2.3"
run_test "fix increments patch" "fix: correct defect" "refs/heads/main" "server/v" "1.2.4" "patch" "server/v1.2.3"
run_test "performance increments patch" "perf: improve lookup" "refs/heads/main" "server/v" "1.2.4" "patch" "server/v1.2.3"
run_test "chore increments patch" "chore: update dependency" "refs/heads/main" "server/v" "1.2.4" "patch" "server/v1.2.3"
run_test "breaking change increments major" "feat: replace API BREAKING CHANGE" "refs/heads/main" "server/v" "2.0.0" "major" "server/v1.2.3"
run_test "breaking marker increments major" "feat!: replace API" "refs/heads/main" "server/v" "2.0.0" "major" "server/v1.2.3"
run_test "unknown type does not release" "docs: improve guide" "refs/heads/feature/docs" "server/v" "1.2.3" "none" "server/v1.2.3"
run_test "branch release is beta" "fix: correct defect" "refs/heads/feature/fix" "server/v" "1.2.4-beta.42" "patch" "server/v1.2.3"
run_test "configured prefix isolates stream" "fix: correct defect" "refs/heads/main" "server/v" "1.2.4" "patch" "server/v1.2.3" "smoke/v9.9.9"
run_test "semantic versions sort naturally" "fix: correct defect" "refs/heads/main" "server/v" "1.10.1" "patch" "server/v1.9.0" "server/v1.10.0"

unsafe_marker="$TEST_ROOT/unsafe-message-executed"
unsafe_title=$(printf 'fix: preserve "quotes", `commands`, and $(touch %s)\nsecond line' "$unsafe_marker")
run_test "shell-like title is treated as data" "$unsafe_title" "refs/heads/main" "server/v" "1.2.4" "patch" "server/v1.2.3"

if [ -e "$unsafe_marker" ]; then
  echo "FAIL: shell syntax in the title was executed" >&2
  exit 1
fi

echo "All $PASSED compute-version tests passed."
