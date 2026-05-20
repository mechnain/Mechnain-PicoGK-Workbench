# Contributing

Thanks for looking at Mechnain PicoGK Workbench. This is an engineering MVP around PicoGK—not a commercial product.

## Project scope

- Local-first parameterized geometry generation
- Blazor UI + CLI runner sharing one registry
- Honest generator maturity labels (Starter / Verified)
- No fake STL, viewer, or AI claims

Out of scope for drive-by PRs: cloud hosting, auth, topology optimization marketing, LEAP 71 branding.

## Getting started

```bash
git clone https://github.com/mechnain/Mechnain-PicoGK-Workbench.git
cd Mechnain-PicoGK-Workbench
dotnet restore
dotnet build
dotnet test
```

Run PicoGK locally:

```bash
dotnet run --project src/Workbench.Runner -- --generator primitive-test
```

## Adding a generator

Follow [docs/generator_authoring.md](docs/generator_authoring.md) and the checklist there.

Mark maturity in PR description:

- **Verified** — STL smoke or print evidence described
- **Starter** — template only
- **Experimental** — incomplete or untested

## Coding style

- Match existing C# patterns in `src/Workbench.Generators`
- Small, focused diffs
- No unrelated refactors
- Comments only for non-obvious geometry or PicoGK behavior

## Reporting issues

Use GitHub Issues:

- [Bug report](.github/ISSUE_TEMPLATE/bug_report.md)
- [Generator request](.github/ISSUE_TEMPLATE/generator_request.md)

Include commands run, `run_log.txt` excerpts, and .NET OS info.

## Pull requests

1. Branch from `main`
2. `dotnet build` and `dotnet test` pass
3. Update docs if behavior or generator list changes
4. Do not commit large `exports/` run trees or personal absolute paths
5. Screenshots only if you add real files under `docs/assets/screenshots/`

## Attribution

Contributors should preserve the PicoGK attribution and independent-project disclaimer in user-facing docs.
