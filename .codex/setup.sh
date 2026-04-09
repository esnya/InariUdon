#!/usr/bin/env bash
set -euo pipefail

DOTNET_DIR="${DOTNET_DIR:-$HOME/.dotnet}"

if [ ! -x "$DOTNET_DIR/dotnet" ]; then
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  bash /tmp/dotnet-install.sh --channel 8.0 --install-dir "$DOTNET_DIR"
fi

# UdonSharpLinter requires Microsoft.NETCore.App 6.0 runtime.
if ! compgen -G "$DOTNET_DIR/shared/Microsoft.NETCore.App/6.0.*" > /dev/null; then
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  bash /tmp/dotnet-install.sh --channel 6.0 --runtime dotnet --install-dir "$DOTNET_DIR"
fi

export DOTNET_ROOT="$DOTNET_DIR"
export PATH="$DOTNET_ROOT:$HOME/.dotnet/tools:$PATH"

echo "DOTNET_ROOT=$DOTNET_ROOT"
dotnet --info >/dev/null
dotnet tool restore

mkdir -p .codex/bin
cat > .codex/bin/udonsharp-lint <<'EOF'
#!/usr/bin/env bash
set -euo pipefail
dotnet tool run udonsharp-lint -- "$@"
EOF
chmod +x .codex/bin/udonsharp-lint

export PATH="$(pwd)/.codex/bin:$PATH"
test -x .codex/bin/udonsharp-lint
