#!/usr/bin/env bash
set -euo pipefail

DOTNET_ROOT="${HOME}/.dotnet"
DOTNET_BIN="${DOTNET_ROOT}/dotnet"

if [ ! -x "${DOTNET_BIN}" ]; then
  echo "[setup] Installing .NET SDK (channel 8.0) into ${DOTNET_ROOT} ..."
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  bash /tmp/dotnet-install.sh --channel 8.0 --install-dir "${DOTNET_ROOT}"
fi

export DOTNET_ROOT
export PATH="${DOTNET_ROOT}:${HOME}/.dotnet/tools:${PATH}"

echo "[setup] dotnet version: $(dotnet --version)"

if ! dotnet --list-runtimes | grep -q '^Microsoft.NETCore.App 6\.0\.'; then
  echo "[setup] Installing .NET runtime 6.0 into ${DOTNET_ROOT} ..."
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  bash /tmp/dotnet-install.sh --runtime dotnet --channel 6.0 --install-dir "${DOTNET_ROOT}"
fi

if [ -f .config/dotnet-tools.json ]; then
  echo "[setup] Restoring local dotnet tools from .config/dotnet-tools.json ..."
  dotnet tool restore

  mkdir -p .codex/bin
  cat > .codex/bin/udonsharp-lint <<'EOF'
#!/usr/bin/env bash
set -euo pipefail
dotnet tool run udonsharp-lint -- "$@"
EOF
  chmod +x .codex/bin/udonsharp-lint
  export PATH="$(pwd)/.codex/bin:${PATH}"
fi

echo "DOTNET_ROOT=$DOTNET_ROOT"
dotnet --info >/dev/null

if [ -x .codex/bin/udonsharp-lint ]; then
  test -x .codex/bin/udonsharp-lint
fi
