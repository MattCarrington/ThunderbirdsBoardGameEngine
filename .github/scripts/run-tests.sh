#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIRECTORY=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
shopt -s nullglob
TEST_SCRIPTS=("$SCRIPT_DIRECTORY"/test-*.sh)

if [ "${#TEST_SCRIPTS[@]}" -eq 0 ]; then
  echo "No CI script tests were found." >&2
  exit 1
fi

for test_script in "${TEST_SCRIPTS[@]}"; do
  echo "Running $(basename "$test_script")"
  bash "$test_script"
done
