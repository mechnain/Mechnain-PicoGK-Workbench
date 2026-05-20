# Setup

## Prerequisites

- Windows PC (primary development target) or another OS supported by .NET 9 and PicoGK native libraries
- .NET 9 SDK
- Internet access for first NuGet restore
- Bambu Studio or another STL viewer/slicer

## Project path

This repository is expected at:

```text
D:\Mechnain Projects\Mechnain PicoGK Workbench V1\Mechnain-PicoGK-Workbench
```

## Restore and build

```powershell
cd "D:\Mechnain Projects\Mechnain PicoGK Workbench V1\Mechnain-PicoGK-Workbench"
dotnet restore
dotnet build
```

### If `dotnet` is not on PATH

Set the SDK root (adjust if your install differs):

```powershell
$env:DOTNET_ROOT = "D:\Leap71\.dotnet"
$env:PATH = "$env:DOTNET_ROOT;$env:PATH"
```

Or invoke the executable directly:

```powershell
& "D:\Leap71\.dotnet\dotnet.exe" restore
& "D:\Leap71\.dotnet\dotnet.exe" build
```

## Run the app

```powershell
cd "D:\Mechnain Projects\Mechnain PicoGK Workbench V1\Mechnain-PicoGK-Workbench"
dotnet run --project src\Workbench.App
```

Open the `http://localhost:...` URL from the console.

Development HTTPS certificate (if prompted):

```powershell
dotnet dev-certs https --trust
```

## Run the CLI

```powershell
cd "D:\Mechnain Projects\Mechnain PicoGK Workbench V1\Mechnain-PicoGK-Workbench"
dotnet run --project src\Workbench.Runner -- --list
dotnet run --project src\Workbench.Runner -- --generator primitive-test
dotnet run --project src\Workbench.Runner -- --generator rover-wheel --params generators\rover-wheel\default_params.json --quality preview
```

## Run tests

```powershell
dotnet test
```

## Output folders

Default run layout:

```text
exports/{generatorId}/{timestamp}_{variantName}/
  used_params.json
  result.json
  run_log.txt
  notes.md
  print_settings.md
  *.stl                 (when PicoGK export succeeds)
```

Recent runs are also listed in `exports/runs_index.json`.

## Logo asset

Place a raster logo at:

```text
src\Workbench.App\wwwroot\assets\mechnain-logo.png
```

The UI uses `src/Workbench.App/wwwroot/assets/mechnain-logo.png`. An SVG copy may remain in the same folder as a fallback asset.

## Troubleshooting

- **PicoGK native load errors** — confirm .NET 9 x64 and that NuGet restored `PicoGK` runtimes for `win-x64`.
- **Empty exports list** — run any generator once; folders must contain `result.json` to appear in the Exports page.
- **Blocked UI on final run** — expected until PicoGK finishes; use CLI for long jobs if needed.
