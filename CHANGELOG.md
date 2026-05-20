# Changelog

All notable changes to this repository are documented here.

Format based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### Added

- Repository documentation cleanup (README, DESIGN_LOG, ROADMAP, docs/)
- `CONTRIBUTING.md`, `CHANGELOG.md`, GitHub issue templates
- GitHub Actions workflow: restore, build, test (no PicoGK in CI)
- `docs/assets/diagrams/architecture.md` (Mermaid)
- `docs/screenshot_checklist.md`
- `exports/examples/` sample run metadata

### Changed

- README restructured with status tables and portable quick start
- `.gitignore` — track `exports/examples/` only; ignore local runs and `runs_index.json`
- Removed machine-specific paths from public docs

## [0.1.0] — 2026-05-20

### Added

- Local Blazor Server app (`Workbench.App`)
- CLI runner (`Workbench.Runner`)
- Generator registry and five built-in generators
- Primitive Test PicoGK STL output (verified locally)
- Manifest-driven parameters and export folder layout
- MIT license for this workbench (PicoGK licensed separately)

[Unreleased]: https://github.com/mechnain/Mechnain-PicoGK-Workbench/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/mechnain/Mechnain-PicoGK-Workbench/releases/tag/v0.1.0
