# Design Log

## 2026-05-20 — V0.1 baseline

Initial scope: general-purpose local PicoGK workbench (not a single-purpose gauntlet tool).

### Implemented

- Blazor app (`src/Workbench.App`)
- CLI runner (`src/Workbench.Runner`)
- Shared models (`src/Workbench.Core`)
- Built-in generators (`src/Workbench.Generators`)
- Tests (`src/Workbench.Tests`)

PicoGK API usage (from package behavior and examples):

- `new Library(voxelSize)` for headless generation
- Implicit bodies, voxels, boolean ops, mesh export via `SaveToStlFile`

### Verified (2026-05-20 UI polish pass)

- `dotnet restore` / `dotnet build` — 0 warnings
- `dotnet test` — 3 passed
- CLI `--list` — 5 generators
- CLI `primitive-test` → `exports/primitive-test/ui_polish_validation/primitive_test.stl`
- App HTTP 200: `/`, `/generators`, `/recent-runs`, `/exports`, `/documentation`, `/about`, `/assets/mechnain-logo.svg`

### Verified (earlier)

- `dotnet restore` / `dotnet build` succeed
- CLI `--list` lists built-in generators
- `primitive-test` produces a real STL under `exports/primitive-test/`
- Rover wheel and electronics enclosure runs logged in `exports/runs_index.json` with STL paths in `result.json` where generation succeeded
- Local app pages: Home, Generator Library, generator detail, Recent Runs, Exports, Documentation, About

### UI (V0.2 polish)

- Dark Mechnain theme via CSS variables (charcoal base, copper accents)
- Logo at `src/Workbench.App/wwwroot/assets/mechnain-logo.png` (from `Mechnain Labs Logo.png` in the workbench parent folder; SVG copy also kept as fallback)
- No in-browser 3D viewer — external slicer/viewer only

### Generator status

| Generator | STL export | Notes |
| --- | --- | --- |
| `primitive-test` | Yes (verified) | Smoke / regression |
| `rover-wheel` | Logged success | Starter wheel template |
| `electronics-enclosure` | Logged success (base + lid) | Starter enclosure |
| `servo-bracket` | Not fully validated in this log | Starter bracket |
| `lattice-coupon` | Not fully validated in this log | Comparison coupon |

Do not claim all generators are production-ready.

### Output / export status

- Run folders: `used_params.json`, `result.json`, `run_log.txt`, `notes.md`, `print_settings.md`, STL when `SaveStl` succeeds
- Screenshot output type in manifests is **not** implemented — no fake screenshot files
- `exports/runs_index.json` tracks recent runs for the UI

### Known limitations

- Built-in generators are starter mechanical templates, not final production designs
- `bracketAngleDeg` documented as reserved for future angled bracket variants
- Manifest JSON under `generators/` mirrors C# manifests; registry uses built-in generator classes
- Long final-quality runs may block the Blazor request until PicoGK completes
- UI does not embed PicoGK viewer or STL preview (roadmap V0.3)

### Honesty

- Not topology optimization or generative AI design
- Not affiliated with or endorsed by LEAP 71
- No fake STL or fake 3D viewer claims
