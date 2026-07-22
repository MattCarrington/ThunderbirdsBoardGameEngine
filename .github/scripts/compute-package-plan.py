#!/usr/bin/env python3

import json
import os
from pathlib import Path
import sys


def fail(message: str) -> None:
    print(f"ERROR: {message}", file=sys.stderr)
    raise SystemExit(1)


script_directory = Path(__file__).resolve().parent
catalog_path = Path(
    os.environ.get("PACKAGE_CATALOG_PATH", script_directory.parent / "packages.json")
)

if not catalog_path.is_file():
    fail(f"Package catalog not found: {catalog_path}")

changed_files_path = os.environ.get("CHANGED_FILES_PATH")
changed_files_value = os.environ.get("CHANGED_FILES")

if changed_files_path:
    changed_files = Path(changed_files_path).read_text(encoding="utf-8").splitlines()
elif changed_files_value is not None:
    changed_files = changed_files_value.splitlines()
else:
    fail("CHANGED_FILES or CHANGED_FILES_PATH must be set")

with catalog_path.open(encoding="utf-8") as catalog_file:
    catalog = json.load(catalog_file)

selected = {
    package["package_id"]
    for package in catalog
    if any(
        changed_file == package["source_path"]
        or changed_file.startswith(f'{package["source_path"]}/')
        for changed_file in changed_files
    )
}

while True:
    consumers = {
        package["package_id"]
        for package in catalog
        if package["package_id"] not in selected
        and any(dependency in selected for dependency in package["dependencies"])
    }

    if not consumers:
        break

    selected.update(consumers)

# Catalog order is topological, so filtering it preserves dependency order.
plan = [package for package in catalog if package["package_id"] in selected]
compact_plan = json.dumps(plan, separators=(",", ":"))

print("Package publish plan:")
if plan:
    for package in plan:
        print(f'  {package["package_id"]}')
else:
    print("  (no packages)")

github_output = os.environ.get("GITHUB_OUTPUT")
if github_output:
    with Path(github_output).open("a", encoding="utf-8") as output_file:
        output_file.write(f"plan={compact_plan}\n")
        output_file.write(f"has_packages={str(bool(plan)).lower()}\n")
else:
    print(compact_plan)
