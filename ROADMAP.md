# Roadmap

Phased goals for Mechnain PicoGK Workbench. Each phase builds on the previous without rewriting the core runner.

## V0.1 — Working MVP

- [x] Local Blazor app
- [x] Generator library and parameter editor
- [x] CLI runner (`--list`, `--generator`)
- [x] Organized `exports/` run folders
- [x] Real PicoGK STL generation (verified on `primitive-test`)
- [x] Recent runs index and exports browser
- [x] Built-in starter generators

## V0.2 — Polished engineering UI

- [x] Mechnain dark theme and copper accents
- [x] Home dashboard, About page, improved docs cards
- [x] GitHub-ready README, LICENSE, `.gitignore`
- [ ] README screenshots captured locally
- [ ] Optional `mechnain-logo.png` raster for favicon

## V0.3 — STL preview in browser

- [ ] Lightweight STL preview on run output page (no fake viewer claims until shipped)
- [ ] Thumbnail or mesh summary where practical

## V0.4 — Useful mechanical generators

- [ ] Rover wheel: print-tested variants documented in run folders
- [ ] Servo bracket: fit clearance validated on hardware
- [ ] Electronics enclosure: board/measured envelope workflow
- [ ] Lattice coupon: comparative print/test notes

Generators remain parameterized templates until print feedback says otherwise.

## V0.5 — Print / test workflow templates

- [ ] Structured `notes.md` / test table templates in UI
- [ ] Run detail page linking parameters → STL → print result
- [ ] Portfolio export helpers from [docs/portfolio_export.md](docs/portfolio_export.md)

## V1.0 — Parameter-to-print workflow

- [ ] End-to-end documented variants with real printed parts and photos
- [ ] Clear which generators are validated vs experimental
- [ ] Optional background jobs for long final-quality runs

## Not planned for this repo

- Cloud hosting, authentication, or multi-tenant SaaS positioning
- Topology optimization or AI “generative design” marketing
- LEAP 71 endorsement or PicoGK source forks
