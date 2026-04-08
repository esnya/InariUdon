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
export PATH="${DOTNET_ROOT}:${PATH}"

echo "[setup] dotnet version: $(dotnet --version)"

if [ -f .config/dotnet-tools.json ]; then
  echo "[setup] Restoring local dotnet tools from .config/dotnet-tools.json ..."
  dotnet tool restore
fi
