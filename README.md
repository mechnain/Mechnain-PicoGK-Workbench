# Mechnain PicoGK Workbench

Mechnain PicoGK Workbench is a local computational engineering interface for running parameterized PicoGK geometry generators, organizing outputs, and documenting design variants for 3D-printable mechanical and robotics components.

## Why it exists

PicoGK is a strong geometry kernel for computational mechanical parts, but ad-hoc scripts make it easy to lose parameters, logs, and STL paths between iterations. This workbench provides a repeatable local workflow: pick a generator, enter measured dimensions, run preview or final quality, and store every variant in a structured export folder.

## What it does

- **Generator library** — manifest-driven generators with parameters, units, and required measurements.
- **Parameter editor** — edit values in the Blazor UI or via JSON for the CLI.
- **PicoGK execution** — headless generation through the shared runner and generator registry.
- **Organized exports** — each run folder includes `used_params.json`, `result.json`, `run_log.txt`, notes, and STL files when export succeeds.
- **Run history** — indexed recent runs and an exports browser in the UI.

PicoGK remains the geometry engine. This repository does not modify PicoGK source code.

## Current MVP status

| Area | Status |
| --- | --- |
| Local Blazor app | Running |
| CLI runner | Running (`--list`, `--generator`) |
| Real STL generation | Verified (`primitive-test`; additional generators logged) |
| In-browser 3D viewer | Not implemented |
| Built-in generators | Starter templates — not production-certified parts |

## Screenshots

Add captures under `docs/assets/screenshots/` after running the UI locally:

| Screenshot | Path |
| --- | --- |
| Home | `docs/assets/screenshots/home.png` |
| Generator Library | `docs/assets/screenshots/generator-library.png` |
| Generator detail | `docs/assets/screenshots/generator-detail.png` |
| Recent Runs | `docs/assets/screenshots/recent-runs.png` |
| Exports | `docs/assets/screenshots/exports.png` |

```markdown
![Mechnain PicoGK Workbench — Home](docs/assets/screenshots/home.png)
```

## Architecture

```text
Mechnain-PicoGK-Workbench/
  src/
    Workbench.App/         Blazor Server UI
    Workbench.Runner/      CLI entry for generation
    Workbench.Core/        Manifests, requests, results, paths
    Workbench.Generators/  PicoGK generator implementations
    Workbench.Tests/       Smoke and regression tests
  generators/              Per-generator manifest + default_params JSON
  projects/                Saved parameter variants (local)
  exports/                 Timestamped run output folders
  docs/                    Setup, workflow, authoring, portfolio notes
```

Flow: **UI or CLI** → `GeneratorRegistry` → `IWorkbenchGenerator.Generate` → PicoGK voxels/mesh → STL + JSON artifacts under `exports/{generatorId}/{timestamp}_{variant}/`.

## Features

- Generator library with category, difficulty, and measurement hints
- Preview and final quality modes (voxel resolution differs)
- Save parameters JSON and named presets from the UI
- Recent runs table with success/failure and folder links
- Exports browser with open-folder actions
- Documentation page linking project docs
- About page with scope and attribution

## Toolchain

- .NET 9 SDK
- C# / Blazor Server
- [PicoGK](https://github.com/leap71/PicoGK) NuGet package (geometry kernel)
- Bambu Studio or another slicer/viewer for STL inspection

## How to run the app

```powershell
cd "D:\Mechnain Projects\Mechnain PicoGK Workbench V1\Mechnain-PicoGK-Workbench"
dotnet restore
dotnet build
dotnet run --project src\Workbench.App
```

Open the `http://localhost:...` URL printed in the console.

If `dotnet` is not on PATH, set `DOTNET_ROOT` or call your SDK directly, for example:

```powershell
$env:DOTNET_ROOT = "D:\Leap71\.dotnet"
& "$env:DOTNET_ROOT\dotnet.exe" run --project src\Workbench.App
```

See [docs/setup.md](docs/setup.md) for full setup notes.

## How to run the CLI

```powershell
dotnet run --project src\Workbench.Runner -- --list
dotnet run --project src\Workbench.Runner -- --generator primitive-test
dotnet run --project src\Workbench.Runner -- --generator rover-wheel --params generators/rover-wheel/default_params.json --quality preview
```

## How to add a generator

1. Implement `IWorkbenchGenerator` (typically `WorkbenchGeneratorBase`) in `src/Workbench.Generators`.
2. Add `generators/{id}/manifest.json` and `default_params.json`.
3. Register in `GeneratorRegistry`.
4. Run via CLI and confirm files under `exports/{id}/`.

Details: [docs/generator_authoring.md](docs/generator_authoring.md).

## Example workflow

1. Open **Generator Library** → **Primitive Test** (or a mechanical generator).
2. Enter dimensions and variant name.
3. **Generate Preview** → inspect STL in an external viewer.
4. **Generate Final** when satisfied.
5. Open **Recent Runs** or **Exports** for logs and paths.
6. Slice, print, test, and update `notes.md` in the run folder.

Full loop: [docs/workflow.md](docs/workflow.md).

## Built-in generators

| ID | Purpose | Maturity |
| --- | --- | --- |
| `primitive-test` | PicoGK smoke test (box, sphere, lattice, boolean) | Verified STL |
| `rover-wheel` | Wheel body, hub, spokes, tread | Starter / logged runs |
| `servo-bracket` | Bracket pocket and clearance | Starter |
| `electronics-enclosure` | Tray, walls, standoffs, optional lid | Starter / logged runs |
| `lattice-coupon` | Lattice comparison coupon | Starter |

These are engineering starting points, not certified production designs.

## Current limitations

- No embedded 3D viewer in the web UI
- Long final-quality runs may block the UI request until PicoGK finishes
- Manifest JSON on disk mirrors built-in C# manifests; registry discovers built-in classes
- Not topology optimization, AI generative design, or cloud CAD
- No authentication, database, or multi-user hosting

## Roadmap

See [ROADMAP.md](ROADMAP.md) for phased goals (UI polish, STL preview, print/test templates, validated parts).

## Attribution

This project uses LEAP 71’s PicoGK as the geometry kernel. PicoGK is an open-source computational geometry kernel. This workbench is an independent project and is not affiliated with or endorsed by LEAP 71.

## License

This workbench repository is licensed under the MIT License — see [LICENSE](LICENSE). PicoGK is licensed separately by its authors; this license does not apply to PicoGK itself.

## Related docs

- [DESIGN_LOG.md](DESIGN_LOG.md) — what was verified and what is still limited
- [docs/setup.md](docs/setup.md)
- [docs/workflow.md](docs/workflow.md)
- [docs/generator_authoring.md](docs/generator_authoring.md)
- [docs/portfolio_export.md](docs/portfolio_export.md)
