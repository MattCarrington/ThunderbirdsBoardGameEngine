#!/usr/bin/env python3

import argparse
import json
import os
from pathlib import Path
import re
import subprocess
import sys
import xml.etree.ElementTree as ET


def fail(message: str) -> None:
    print(f"ERROR: {message}", file=sys.stderr)
    raise SystemExit(1)


def git(*args: str) -> str:
    return subprocess.check_output(["git", *args], encoding="utf-8")


def check_versions(plan: list, base: str) -> None:
    previous_paths = set(git("ls-tree", "-r", "--name-only", base).splitlines())
    errors = []
    for package in plan:
        path = package["project_path"]
        current = ET.parse(path).getroot().findtext("./PropertyGroup/Version", "").strip()
        previous = None
        if path in previous_paths:
            previous = ET.fromstring(git("show", f"{base}:{path}")).findtext(
                "./PropertyGroup/Version", ""
            ).strip()

        if not re.fullmatch(r"[0-9]+\.[0-9]+\.[0-9]+", current):
            errors.append(f"{path}: declare a stable version in <Version> (major.minor.patch).")
        elif previous is not None:
            if not re.fullmatch(r"[0-9]+\.[0-9]+\.[0-9]+", previous):
                errors.append(f"{path}: cannot compare with base version {previous!r}.")
            elif tuple(map(int, current.split("."))) <= tuple(map(int, previous.split("."))):
                errors.append(f"{path}: bump <Version> above {previous} (found {current}).")

    if errors:
        fail("Package releases require version bumps:\n  " + "\n  ".join(errors))


parser = argparse.ArgumentParser()
parser.add_argument("--check-base", help="Check that selected package versions increase from this base commit")
args = parser.parse_args()

script_directory = Path(__file__).resolve().parent
catalog_path = Path(
    os.environ.get("PACKAGE_CATALOG_PATH", script_directory.parent / "packages.json")
)

if not catalog_path.is_file():
    fail(f"Package catalog not found: {catalog_path}")

changed_files_path = os.environ.get("CHANGED_FILES_PATH")
changed_files_value = os.environ.get("CHANGED_FILES")

if args.check_base:
    changed_files = git("diff", "--name-only", args.check_base, "HEAD", "--").splitlines()
elif changed_files_path:
    changed_files = Path(changed_files_path).read_text(encoding="utf-8").splitlines()
elif changed_files_value is not None:
    changed_files = changed_files_value.splitlines()
else:
    fail("CHANGED_FILES or CHANGED_FILES_PATH must be set")

package_props_changed = "Directory.Packages.props" in changed_files
package_props_diff_path = os.environ.get("PACKAGE_PROPS_DIFF_PATH")
package_props_diff_value = os.environ.get("PACKAGE_PROPS_DIFF")

if args.check_base:
    package_props_diff = git("diff", "--unified=0", args.check_base, "HEAD", "--", "Directory.Packages.props")
elif package_props_diff_path:
    package_props_diff = Path(package_props_diff_path).read_text(encoding="utf-8")
elif package_props_diff_value is not None:
    package_props_diff = package_props_diff_value
elif package_props_changed:
    fail("PACKAGE_PROPS_DIFF or PACKAGE_PROPS_DIFF_PATH must be set when Directory.Packages.props changes")
else:
    package_props_diff = ""

package_version_pattern = re.compile(
    r'<PackageVersion\s+(?:Include|Update)="([^"]+)"', re.IGNORECASE
)
changed_central_packages = {
    match.group(1).casefold()
    for line in package_props_diff.splitlines()
    if line.startswith(("+", "-")) and not line.startswith(("+++", "---"))
    for match in package_version_pattern.finditer(line)
}

with catalog_path.open(encoding="utf-8") as catalog_file:
    catalog = json.load(catalog_file)

selected = {
    package["package_id"]
    for package in catalog
    if (
        any(
            changed_file == source_path
            or changed_file.startswith(f"{source_path}/")
            for changed_file in changed_files
            for source_path in package["source_paths"]
        )
        or any(
            central_package.casefold() in changed_central_packages
            for central_package in package["central_packages"]
        )
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

if args.check_base and plan:
    check_versions(plan, args.check_base)

github_output = os.environ.get("GITHUB_OUTPUT")
if github_output:
    with Path(github_output).open("a", encoding="utf-8") as output_file:
        output_file.write(f"plan={compact_plan}\n")
        output_file.write(f"has_packages={str(bool(plan)).lower()}\n")
else:
    print(compact_plan)
