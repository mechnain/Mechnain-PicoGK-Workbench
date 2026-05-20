# Roadmap

Phased plan for Mechnain PicoGK Workbench. Scope stays local-first and engineering-honest.

## V0.1 — Working MVP

- [x] Local Blazor app
- [x] CLI runner
- [x] Generator registry
- [x] Primitive PicoGK STL output verified
- [x] Structured run folders under `exports/`
- [x] Starter generator library (5 generators)

## V0.2 — Repo and UI polish

- [x] Dark Mechnain theme and logo
- [x] README, DESIGN_LOG, ROADMAP, docs cleanup
- [x] CONTRIBUTING, CHANGELOG, issue templates
- [x] GitHub Actions: restore / build / test
- [x] `exports/examples/` sample metadata
- [x] Screenshots in `docs/assets/screenshots/`
- [x] Public README embeds

## V0.3 — Output inspection

- [ ] Browser STL preview on run output page
- [ ] Run detail page improvements
- [ ] File size / artifact summary in UI
- [ ] Export browser filters and sorting

## V0.4 — Useful mechanical generators

- [ ] Rover wheel — print-tested variants documented
- [ ] Servo bracket — clearance validated on hardware
- [ ] Electronics enclosure — measured board workflow
- [ ] Lattice coupon — comparative test notes
- [ ] Sensor mount (new generator candidate)

Generators stay **starter** until print feedback says otherwise.

## V0.5 — Print / test workflow

- [ ] Print settings templates per material
- [ ] Test plan templates in UI
- [ ] `notes.md` prompts per generator type
- [ ] Result comparison table across variants

## V0.6 — AI-assisted layer

- [ ] Natural language → generator recommendation
- [ ] Parameter JSON drafting from measurements
- [ ] Constraint / sanity warnings before run
- [ ] Documentation draft from run folder
- [ ] **User approval required** before any generation

Not autonomous design—assisted drafting only.

## V1.0 — Validated local-first workbench

- [ ] Generators with documented print/test evidence
- [ ] Complete docs + screenshots + example outputs
- [ ] Stable install path for new clones
- [ ] Optional background jobs for long final-quality runs

## Explicitly not planned

- Cloud-hosted CAD or multi-tenant SaaS positioning
- Topology optimization or “AI generative design” hype
- LEAP 71 endorsement or PicoGK source forks
- FEA solver integration in this repo
