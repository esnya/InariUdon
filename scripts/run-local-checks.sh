#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PACKAGE_DIR="$ROOT_DIR/Packages/com.nekometer.esnya.inari-udon"
RUN_UNITY=0

while [ "$#" -gt 0 ]; do
  case "$1" in
    --unity)
      RUN_UNITY=1
      ;;
    *)
      echo "Unknown argument: $1" >&2
      exit 1
      ;;
  esac
  shift
done

export DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}"
export PATH="$ROOT_DIR/.codex/bin:$DOTNET_ROOT:$HOME/.dotnet/tools:$PATH"

cd "$ROOT_DIR"

git diff --check

ruby -e 'require "yaml"; YAML.load_file(".releaserc.yml"); Dir[".github/workflows/*.yml"].sort.each { |path| YAML.load_file(path) }'

node <<'NODE'
const fs = require("fs");
const path = require("path");

const manifests = new Set([
  "package.json",
  "Packages/com.nekometer.esnya.inari-udon/package.json",
]);

for (const file of manifests) {
  JSON.parse(fs.readFileSync(path.join(process.cwd(), file), "utf8"));
}
NODE

dotnet tool restore
dotnet tool run udonsharp-lint "$PACKAGE_DIR"

if [ "$RUN_UNITY" -eq 1 ]; then
  : "${UNITY_EXE:?Set UNITY_EXE to your Unity executable path before using --unity.}"
  : "${UNITY_PROJECT_PATH:?Set UNITY_PROJECT_PATH to a VRChat Creator Companion host project that includes this package before using --unity.}"
  "$UNITY_EXE" \
    -batchmode \
    -quit \
    -nographics \
    -logFile - \
    -projectPath "$UNITY_PROJECT_PATH"
fi
