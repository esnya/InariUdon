#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

git diff --check

ruby -e 'require "yaml"; YAML.load_file(".releaserc.yml"); Dir[".github/workflows/*.yml"].sort.each { |path| YAML.load_file(path) }'

node <<'NODE'
const fs = require("fs");
const path = require("path");

const manifests = [
  "package.json",
  "Packages/com.nekometer.esnya.inari-udon/package.json",
  "Packages/com.nekometer.esnya.inari-udon/InariUdonRuntime.asmdef",
  "Packages/com.nekometer.esnya.inari-udon/Editor/InariUdonEditor.asmdef",
];

for (const relativePath of manifests) {
  JSON.parse(fs.readFileSync(path.join(process.cwd(), relativePath), "utf8"));
}

const runtimeAsmdef = JSON.parse(fs.readFileSync("Packages/com.nekometer.esnya.inari-udon/InariUdonRuntime.asmdef", "utf8"));
if (Array.isArray(runtimeAsmdef.includePlatforms) && runtimeAsmdef.includePlatforms.includes("Editor")) {
  throw new Error("Runtime asmdef must not be editor-only.");
}

const packageRoot = path.join(process.cwd(), "Packages/com.nekometer.esnya.inari-udon");
const editorGuardPattern = /#if\s+(?:!COMPILER_UDONSHARP\s*&&\s*)?UNITY_EDITOR|#if\s+UNITY_EDITOR/;
const forbiddenUsingPattern = /^(using UnityEditor;|using UdonSharpEditor;)$/m;
const errors = [];

function walk(dir) {
  for (const entry of fs.readdirSync(dir, { withFileTypes: true })) {
    const fullPath = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      walk(fullPath);
      continue;
    }
    if (!entry.name.endsWith(".cs")) continue;
    const relativePath = path.relative(process.cwd(), fullPath).replaceAll(path.sep, "/");
    if (relativePath.includes("/Editor/")) continue;
    const source = fs.readFileSync(fullPath, "utf8");
    if (forbiddenUsingPattern.test(source) && !editorGuardPattern.test(source)) {
      errors.push(`${relativePath}: runtime script references editor-only namespaces without an editor preprocessor guard.`);
    }
  }
}

walk(packageRoot);

if (errors.length > 0) {
  console.error(errors.join("\n"));
  process.exit(1);
}
NODE
