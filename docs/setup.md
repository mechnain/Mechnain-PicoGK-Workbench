# Setup

Get Mechnain PicoGK Workbench running from a clean clone.

## Prerequisites

| Requirement | Notes |
| --- | --- |
| [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) | `dotnet --version` should show 9.x |
| Git | To clone the repository |
| Disk space | NuGet restore + PicoGK native runtimes |
| STL viewer/slicer | Bambu Studio, PrusaSlicer, MeshLab, etc. (optional but recommended) |

**Platforms:** Primary development on Windows x64. Other OS may work if .NET 9 and PicoGK native libraries are available for that RID—verify with a `primitive-test` run.

## Clone and build

```bash
git clone https://github.com/mechnain/Mechnain-PicoGK-Workbench.git
cd Mechnain-PicoGK-Workbench
dotnet restore
dotnet build
```

```powershell
# Windows PowerShell — same commands
git clone https://github.com/mechnain/Mechnain-PicoGK-Workbench.git
Set-Location Mechnain-PicoGK-Workbench
dotnet restore
dotnet build
```

## Run the app

```bash
dotnet run --project src/Workbench.App
```

Open the URL printed in the console (typically `http://localhost:5xxx` or `https://localhost:7xxx`).

HTTPS dev certificate (first time on Windows):

```powershell
dotnet dev-certs https --trust
```

## Run the CLI

```bash
dotnet run --project src/Workbench.Runner -- --list
dotnet run --project src/Workbench.Runner -- --generator primitive-test
```

Preview quality (faster):

```bash
dotnet run --project src/Workbench.Runner -- --generator rover-wheel --params generators/rover-wheel/default_params.json --quality preview
```

## Run tests

```bash
dotnet test
```

Tests cover registry and runner contracts—they do **not** execute PicoGK geometry (keeps CI reliable).

## Expected output

After a successful `primitive-test` run:

```text
exports/primitive-test/{timestamp}_{variant}/
  used_params.json
  result.json
  run_log.txt
  notes.md
  print_settings.md
  primitive_test.stl    # when PicoGK export succeeds
```

See [exports/examples/](../exports/examples/) for sample metadata layout (regenerate STL locally).

## Troubleshooting

### `dotnet` not found

- Install .NET 9 SDK from https://dotnet.microsoft.com/download
- Restart the terminal
- On Windows, if SDK is installed but not on PATH:

```powershell
$env:DOTNET_ROOT = "$env:ProgramFiles\dotnet"   # adjust if your install differs
$env:PATH = "$env:DOTNET_ROOT;$env:PATH"
dotnet --version
```

### Port already in use

- Change URLs in `src/Workbench.App/Properties/launchSettings.json`, or:

```bash
dotnet run --project src/Workbench.App --urls http://127.0.0.1:5099
```

### PicoGK / native dependency errors

- Use **x64** .NET 9
- Run `dotnet restore` from repo root
- Confirm `PicoGK` package restored under `src/Workbench.Generators`
- On Windows, PicoGK ships `win-x64` native binaries via NuGet
- If load still fails, run `primitive-test` from CLI and read `run_log.txt`

### No STL generated

- Open `result.json` in the run folder — `success` may be `false`
- Read `run_log.txt` for PicoGK errors
- Try `primitive-test` first (smallest smoke generator)
- Preview mode is more forgiving than final for quick checks

### Path / permission errors

- Run terminal from a writable clone directory
- Avoid generating into protected system folders
- Export paths are relative to repository root (`exports/...`)

### Empty Exports page in UI

- Run any generator once so a folder contains `result.json`
- `exports/runs_index.json` is optional local index data

## Windows notes

- PowerShell and Command Prompt both work; use forward slashes or escaped backslashes in JSON paths
- “Open folder” buttons use the OS shell—Windows opens Explorer
- Long final-quality runs may appear to freeze the UI until PicoGK completes; use CLI for long jobs

## Next steps

- [workflow.md](workflow.md) — engineering loop
- [generator_authoring.md](generator_authoring.md) — add a generator
- [screenshot_checklist.md](screenshot_checklist.md) — capture UI for README
