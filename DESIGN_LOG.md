# Design Log

Engineering notes for Mechnain PicoGK Workbench—what works, what was checked, and what is still open.

## Purpose

Provide a local-first loop for parameterized PicoGK geometry:

**generator → parameters → PicoGK → STL/artifacts → logs → documentation → print/test/iterate**

without rewriting `Program.cs` for every variant or losing track of outputs between prints.

## Build status

| Item | Status | Evidence |
| --- | --- | --- |
| Local app | Working | `dotnet run --project src/Workbench.App` |
| CLI runner | Working | `--list`, `--generator` |
| Primitive test STL | Verified | CLI run; `result.json` + STL path in log |
| Other generators | Starter | Runs logged locally; not mechanically validated |
| UI polish (dark theme) | Done | Blazor pages + CSS variables |
| README screenshots | Done | Five PNGs in `docs/assets/screenshots/` |
| STL viewer in browser | Not implemented | External viewer only |
| GitHub Actions CI | Added | `restore` / `build` / `test` only (no PicoGK in CI) |

## What was verified

- Solution builds on .NET 9 (`dotnet restore`, `dotnet build`)
- Unit tests pass (`dotnet test` — registry/runner contracts, no PicoGK in tests)
- CLI lists five built-in generators
- `primitive-test` produces a real STL via PicoGK (`SaveStl` / mesh export)
- Blazor routes respond: Home, Generator Library, generator detail, Recent Runs, Exports, Documentation, About
- Export folder layout: `used_params.json`, `result.json`, `run_log.txt`, note templates

## What is not verified

- Mechanical strength or load capacity of any part
- Printability of every generator without per-material tuning
- Fit on real hardware for rover wheel, bracket, enclosure, lattice coupon
- Topology optimization (not in scope)
- FEA or simulation coupling
- In-browser STL preview
- Cloud deployment or multi-user operation
- AI-assisted parameter drafting (roadmap only)

## Design decisions

| Decision | Rationale |
| --- | --- |
| Local-first | PicoGK native runtime and engineering data stay on the workstation |
| Blazor UI + CLI runner | Same `GeneratorRegistry` and export layout from UI or terminal |
| JSON manifests + `default_params.json` | Portable parameter contracts per generator |
| `exports/` run folders | Traceability: parameters, logs, and outputs stay together |
| External STL viewer for MVP | Avoid fake embedded 3D claims; ship preview in V0.3 |
| Built-in C# generators + disk manifests | Registry discovers classes; JSON mirrors manifests for tooling |
| Ignore bulk `exports/` in git | Generated runs are local artifacts; `exports/examples/` for shape only |

## Generator maturity (honest)

| Generator | STL in dev runs | Mechanical validation |
| --- | --- | --- |
| `primitive-test` | Yes | Smoke test only |
| `rover-wheel` | Logged locally | Not print-certified |
| `electronics-enclosure` | Logged locally | Not print-certified |
| `servo-bracket` | Not fully logged here | Starter template |
| `lattice-coupon` | Not fully logged here | Starter template |

## Known limitations

- Long final-quality PicoGK jobs may block the Blazor request until complete
- `exports/runs_index.json` stores local paths; regenerated per machine
- Screenshot output type in manifests is not implemented
- `bracketAngleDeg` reserved for future angled bracket variants
- CI does not run PicoGK generation (native/runtime constraints on GitHub-hosted runners)

## Next experiments

- Capture README screenshots ([docs/screenshot_checklist.md](docs/screenshot_checklist.md))
- Rover wheel print + notes in run folder
- Enclosure fit check on measured board envelope
- Lattice coupon compression comparison
- Browser STL preview spike (V0.3)
- AI-assisted layer only with explicit user approval before generation (V0.6)

## Honesty

- Not affiliated with or endorsed by LEAP 71
- Not generative AI design or topology optimization marketing
- No fake STL, viewer, or screenshot artifacts in the repo
