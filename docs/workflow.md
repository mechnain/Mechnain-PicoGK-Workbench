# Workflow

End-to-end loop for parameterized mechanical design with Mechnain PicoGK Workbench.

## Steps

1. **Choose a generator** — Generator Library in the app, or `dotnet run --project src\Workbench.Runner -- --list`.
2. **Enter measured dimensions** — Use the parameter page or a JSON params file aligned with the manifest.
3. **Generate preview** — Coarser voxel size for fast iteration (`preview` quality).
4. **Inspect output** — Open the run folder or Recent Runs → **Open run**. View STL in Bambu Studio or another viewer (no in-app 3D viewer in V0.2).
5. **Generate final** — Finer voxels when the form matches intent (`final` quality); may take longer.
6. **Slice** — Import STL into Bambu Studio or your slicer; apply material-specific settings.
7. **Print** — Fabricate the part; record machine, material, and issues in `notes.md`.
8. **Test** — Check fit, clearance, strength, or assembly against measured requirements.
9. **Document results** — Update `notes.md`, `print_settings.md`, and optionally portfolio snippets per [portfolio_export.md](portfolio_export.md).
10. **Iterate** — Adjust parameters, save a new variant name, and run again.

## Preview vs final

| Mode | Voxel size | Use |
| --- | --- | --- |
| Preview | Coarser | Quick shape and proportion checks |
| Final | Finer | Export for slicing when parameters are stable |

## Where files live

```text
exports/{generatorId}/{timestamp}_{variant}/
```

The UI writes the same layout as the CLI. Keep variant names descriptive (`axle_clearance_v2`, not `test3`).

## CLI example

```powershell
cd "D:\Mechnain Projects\Mechnain PicoGK Workbench V1\Mechnain-PicoGK-Workbench"
dotnet run --project src\Workbench.Runner -- --generator primitive-test --quality preview
```

## Honesty

- Generators are starter templates until you validate prints.
- Failed runs still create folders with logs — check `result.json` and `run_log.txt`.
- This workflow is not topology optimization or automated design-of-experiments (future roadmap items may add batch runs).
