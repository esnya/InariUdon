# InariUdon
Useful prefabs and Udon scripts for VRChat World SDK 3.10.2

**BREAKING_CHANGE: UdonSharp is upgraded to 1.x. You must create project via VRChat Creator Companion**

Do you looking for Udon Sun Controller ? This package does not contains them. It's [here](https://github.com/esnya/UdonSunController).

## Getting Started
1. Create World Unity Project with UdonSharp using VRChat Create Companion.
2. Open Package Manager via Menu / Window / Package Manager
3. Click + button on top left on the window and select `Add package from git URL`
4. Enter `https://github.com/esnya/InariUdon.git?path=/Packages/com.nekometer.esnya.inari-udon` (or `https://github.com/esnya/InariUdon.git?path=/Packages/com.nekometer.esnya.inari-udon#beta` to use beta release) into text box and click Add button.

![image](https://user-images.githubusercontent.com/2088693/180705211-f0f25559-d66f-460c-aede-445a230ae87a.png)
![image](https://user-images.githubusercontent.com/2088693/180705244-5dea9e3b-62a0-4ed5-b12d-89e612f49ecc.png)

## CI for UdonSharp (feasible on GitHub-hosted runners)
Because this repository is a UPM package (not a full Unity project), the practical CI method is **static check with UdonSharp linter**.

### What this repository runs
1. Install .NET 8 SDK and .NET 6 runtime (`actions/setup-dotnet`)
2. Install `tktco.UdonSharpLinter` as a global dotnet tool
3. Run `udonsharp-lint` against `Packages/com.nekometer.esnya.inari-udon`

### Local reproduction
```bash
# 1) Install .NET SDK 8.x and .NET Runtime 6.x first
# (UdonSharpLinter depends on Microsoft.NETCore.App 6.0)

# 2) Install linter
dotnet tool install --global tktco.UdonSharpLinter --version 0.3.1

# 3) If dotnet was installed into a custom location, export runtime path
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$HOME/.dotnet/tools:$PATH"

# 4) Run lint
udonsharp-lint Packages/com.nekometer.esnya.inari-udon
```

### About Test / Build
Running Unity Test Runner or full build requires a host Unity project and Unity license/secrets in CI, so they are intentionally not part of this workflow.


### Alternative install (network-enabled env)
If `dotnet` is not preinstalled, you can bootstrap it with the official installer script:

```bash
curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
bash /tmp/dotnet-install.sh --channel 8.0 --install-dir "$HOME/.dotnet"
bash /tmp/dotnet-install.sh --channel 6.0 --runtime dotnet --install-dir "$HOME/.dotnet"

export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$HOME/.dotnet/tools:$PATH"
```


### Codex Cloud development environment
For Codex Cloud tasks, this repository now includes environment files:
- `.codex/environment.yml`
- `.codex/setup.sh`

These scripts bootstrap dotnet (SDK 8 + runtime 6) and restore the repo-pinned `UdonSharpLinter` tool so cloud tasks can run lint immediately.

## Local Checks
Fast static verification without dotnet or Unity:

```bash
bash scripts/run-static-checks.sh
```

This covers:
- `git diff --check`
- YAML parsing for `.releaserc.yml` and GitHub Actions workflows
- JSON parsing for root/package manifests and asmdefs
- runtime asmdef sanity checks
- runtime scripts that accidentally import `UnityEditor` / `UdonSharpEditor` without editor guards

Run the fast local verification bundle with:

```bash
bash scripts/run-local-checks.sh
```

This runs the static checks above, then `udonsharp-lint` via the repo-local dotnet tool manifest.

Optional Unity batchmode compile is available with:

```bash
UNITY_EXE="/path/to/Unity.exe" \
UNITY_PROJECT_PATH="/path/to/vrchat-host-project" \
bash scripts/run-local-checks.sh --unity
```

`--unity` intentionally requires a separate VRChat Creator Companion host project that already includes this package, because package-only repositories do not compile reliably in batchmode without a pinned host project.
