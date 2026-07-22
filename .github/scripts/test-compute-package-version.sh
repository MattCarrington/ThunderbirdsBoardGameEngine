#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIRECTORY=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
COMPUTE_PACKAGE_VERSION_SCRIPT="$SCRIPT_DIRECTORY/compute-package-version.sh"
TEST_ROOT=$(mktemp -d)
MOCK_BIN="$TEST_ROOT/bin"
mkdir -p "$MOCK_BIN"
trap 'rm -rf "$TEST_ROOT"' EXIT

cat > "$MOCK_BIN/dotnet" <<'MOCK'
#!/usr/bin/env bash
echo "$MOCK_BASE_VERSION"
MOCK

cat > "$MOCK_BIN/curl" <<'MOCK'
#!/usr/bin/env bash
set -eu
output_file=""
url=""

while [ "$#" -gt 0 ]; do
  case "$1" in
    -o)
      output_file="$2"
      shift 2
      ;;
    -H|-w)
      shift 2
      ;;
    -sS)
      shift
      ;;
    *)
      url="$1"
      shift
      ;;
  esac
done

page=${url##*page=}
status=200

case "$MOCK_API_SCENARIO" in
  not-found)
    status=404
    body='{"message":"Not Found"}'
    ;;
  stable-first-page)
    body='[{"name":"1.2.3"}]'
    ;;
  stable-second-page)
    if [ "$page" = "1" ]; then
      body='[{"name":"1.2.2"}]'
    else
      body='[{"name":"1.2.3"}]'
    fi
    ;;
  no-stable)
    if [ "$page" = "1" ]; then
      body='[{"name":"1.2.3-beta.1"}]'
    else
      body='[]'
    fi
    ;;
  api-error)
    status=500
    body='{"message":"Server error"}'
    ;;
  max-pages)
    body="[{\"name\":\"0.0.$page\"}]"
    ;;
  *)
    echo "Unknown mock API scenario: $MOCK_API_SCENARIO" >&2
    exit 1
    ;;
esac

printf '%s' "$body" > "$output_file"
printf '%s' "$status"
MOCK

cat > "$MOCK_BIN/jq" <<'MOCK'
#!/usr/bin/env bash
set -eu
mode=""
version=""

while [ "$#" -gt 0 ]; do
  case "$1" in
    -r|-e)
      mode="$1"
      shift
      ;;
    --arg)
      version="$3"
      shift 3
      ;;
    *)
      expression="$1"
      file="$2"
      break
      ;;
  esac
done

case "$expression" in
  length)
    grep -o '"name"' "$file" | wc -l | tr -d ' '
    ;;
  '.[].name')
    sed -n 's/.*"name":"\([^"]*\)".*/\1/p' "$file"
    ;;
  '.[] | select(.name == $version)')
    grep -Fq "\"name\":\"$version\"" "$file"
    ;;
  *)
    echo "Unsupported jq expression: $expression ($mode)" >&2
    exit 1
    ;;
esac
MOCK

chmod +x "$MOCK_BIN/dotnet" "$MOCK_BIN/curl" "$MOCK_BIN/jq"

PASSED=0

run_success_test() {
  local name="$1"
  local scenario="$2"
  local git_ref="$3"
  local expected_version="$4"
  local expected_should_publish="$5"
  local default_branch="${6:-main}"
  local output_file="$TEST_ROOT/output-$PASSED"

  PATH="$MOCK_BIN:$PATH" \
    MOCK_BASE_VERSION="1.2.3" \
    MOCK_API_SCENARIO="$scenario" \
    PROJECT_PATH="package.csproj" \
    PACKAGE_ID="Example.Package" \
    OWNER="example" \
    GITHUB_TOKEN="test-token" \
    GITHUB_REF="$git_ref" \
    GITHUB_RUN_NUMBER="42" \
    DEFAULT_BRANCH="$default_branch" \
    GITHUB_OUTPUT="$output_file" \
    bash "$COMPUTE_PACKAGE_VERSION_SCRIPT" >/dev/null

  local actual_version
  local actual_should_publish
  actual_version=$(sed -n 's/^version=//p' "$output_file")
  actual_should_publish=$(sed -n 's/^should_publish=//p' "$output_file")

  if [ "$actual_version" != "$expected_version" ]; then
    echo "FAIL: $name: expected version '$expected_version', got '$actual_version'" >&2
    exit 1
  fi

  if [ "$actual_should_publish" != "$expected_should_publish" ]; then
    echo "FAIL: $name: expected should_publish '$expected_should_publish', got '$actual_should_publish'" >&2
    exit 1
  fi

  PASSED=$((PASSED + 1))
  echo "PASS: $name"
}

run_failure_test() {
  local name="$1"
  local scenario="$2"
  local git_ref="${3:-refs/heads/main}"
  local default_branch="${4:-main}"
  local output_file="$TEST_ROOT/output-$PASSED"

  if PATH="$MOCK_BIN:$PATH" \
    MOCK_BASE_VERSION="1.2.3" \
    MOCK_API_SCENARIO="$scenario" \
    PROJECT_PATH="package.csproj" \
    PACKAGE_ID="Example.Package" \
    OWNER="example" \
    GITHUB_TOKEN="test-token" \
    GITHUB_REF="$git_ref" \
    GITHUB_RUN_NUMBER="42" \
    DEFAULT_BRANCH="$default_branch" \
    GITHUB_OUTPUT="$output_file" \
    bash "$COMPUTE_PACKAGE_VERSION_SCRIPT" >/dev/null 2>&1; then
    echo "FAIL: $name: expected the script to fail" >&2
    exit 1
  fi

  PASSED=$((PASSED + 1))
  echo "PASS: $name"
}

run_success_test "missing package permits stable publish" "not-found" "refs/heads/main" "1.2.3" "true"
run_success_test "missing package permits beta publish" "not-found" "refs/heads/feature/package" "1.2.3-beta.42" "true"
run_success_test "stable version on main skips publish" "stable-first-page" "refs/heads/main" "1.2.3" "false"
run_success_test "pagination finds stable version" "stable-second-page" "refs/heads/main" "1.2.3" "false"
run_success_test "exhausted versions permit publish" "no-stable" "refs/heads/main" "1.2.3" "true"
run_failure_test "unexpected API status fails" "api-error"
run_failure_test "pagination safety cap fails" "max-pages"
run_failure_test "closed release line rejects another beta" "stable-first-page" "refs/heads/feature/package"
run_success_test "custom default branch publishes stable" "not-found" "refs/heads/trunk" "1.2.3" "true" "trunk"
run_success_test "custom default branch feature publishes beta" "not-found" "refs/heads/feature/package" "1.2.3-beta.42" "true" "trunk"
run_success_test "custom default branch skips existing stable" "stable-first-page" "refs/heads/trunk" "1.2.3" "false" "trunk"
run_failure_test "custom default branch rejects closed beta" "stable-first-page" "refs/heads/feature/package" "trunk"

echo "All $PASSED compute-package-version tests passed."
