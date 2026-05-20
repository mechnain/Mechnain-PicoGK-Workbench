# Export examples

This folder shows the **shape** of a PicoGK run folder without committing large generated artifacts.

## Sample run

`primitive-test/sample_run/` — metadata from a successful smoke test. Paths are repository-relative.

Regenerate the STL locally:

```bash
dotnet run --project src/Workbench.Runner -- --generator primitive-test --output exports/primitive-test/my_run --quality preview
```

## What is not in git

Timestamped runs under `exports/{generatorId}/` are **local** and gitignored. Your machine will create:

- `exports/runs_index.json` — recent runs index for the UI
- Full run folders with STL files

Do not commit bulk `exports/` trees or absolute paths from your PC.
