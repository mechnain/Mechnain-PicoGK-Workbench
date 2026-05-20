# Workflow

Practical guide for parameterized mechanical design with Mechnain PicoGK Workbench.

> **Warning:** A generated STL is not automatically a validated mechanical part. Check dimensions, clearances, loads, print orientation, material, and fit before relying on any part physically.

## Engineering workflow

```text
Select generator → Enter measurements → Preview → Inspect STL → Final → Slice → Print → Test → Document → Iterate
```

## 1. Generator selection

- **UI:** Generator Library → pick by category and required measurements
- **CLI:** `dotnet run --project src/Workbench.Runner -- --list`

Choose based on part type (smoke test, wheel, bracket, enclosure, coupon)—not every generator is validated on hardware.

## 2. Parameter entry

- Use the generator detail page or `generators/{id}/default_params.json` as a starting point
- Match **required measurements** from the manifest (real caliper/board dimensions)
- Save variants with descriptive names (`axle_fit_v2`, not `test4`)
- Optional: **Save Parameters JSON** or preset from the UI into `projects/`

## 3. Preview vs final

| Mode | CLI flag / UI | Voxel size | Use |
| --- | --- | --- | --- |
| Preview | `preview` | Coarser | Fast shape and proportion checks |
| Final | `final` | Finer | STL for slicing when parameters are stable |

Preview first. Final can take noticeably longer on complex generators.

## 4. Exported artifacts

Each run creates:

```text
exports/{generatorId}/{timestamp}_{variant}/
```

| File | Purpose |
| --- | --- |
| `used_params.json` | Exact inputs |
| `result.json` | Success, paths, message |
| `run_log.txt` | PicoGK and runner log |
| `notes.md` | Your print/test notes |
| `print_settings.md` | Slicer settings |
| `*.stl` | Geometry when export succeeds |

Recent Runs and Exports pages link to these folders.

## 5. STL inspection

There is **no in-browser 3D viewer** in V0.2. Open STL in:

- Bambu Studio / PrusaSlicer — manufacturability
- MeshLab / Windows 3D Viewer — quick visual check

Confirm scale, wall thickness, holes, and clearances against design intent.

## 6. Slicing and printing

- Orient for strength and surface quality
- Record material, nozzle, layer height in `print_settings.md`
- Note supports, brim, or enclosure requirements

## 7. Testing

- Dimensional check (calipers vs parameters)
- Fit with mating parts (axle, board, servo, lid)
- Functional check (rotation, latch, airflow) as applicable
- Record failures honestly in `notes.md`

## 8. Document results

Update the run folder before starting the next variant:

- What changed vs last variant
- Print outcome (warp, stringing, tolerance)
- Measured fit result
- Next parameter change

Portfolio templates: [portfolio_export.md](portfolio_export.md)

## CLI quick reference

```bash
dotnet run --project src/Workbench.Runner -- --generator primitive-test --quality preview
```

## What this workflow is not

- Topology optimization
- Automated design-of-experiments (future consideration)
- FEA or certified structural sign-off
- Cloud rendering or team collaboration (local-first today)
