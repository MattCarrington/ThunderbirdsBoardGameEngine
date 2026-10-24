#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIRECTORY=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
node --test "$SCRIPT_DIRECTORY/test-release-selection.cjs"