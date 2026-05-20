# Portfolio Export

Turn a real PicoGK run folder into engineering evidence for GitHub, LinkedIn, or a resume. Only document runs you actually executed and, when claiming print results, parts you actually printed.

## GitHub README section

```markdown
## PicoGK variant: {part name}

- **Generator:** `{generator-id}`
- **Variant:** `{variant}`
- **Intent:** {one sentence design goal}
- **Key parameters:** {2–4 measured dimensions with units}
- **Output:** STL from Mechnain PicoGK Workbench (`exports/...`)
- **Print/test:** {fit, load, clearance — honest result}
- **Next:** {planned parameter change}
```

Optional screenshot: `docs/assets/screenshots/generator-detail.png` or a photo of the printed part (not a fake render).

## LinkedIn post (factual tone)

```text
Built a parameterized mechanical workflow with LEAP 71 PicoGK and a local C# workbench.

This variant: {part name} — generator `{id}`, key dims {summary}.
Exported STL + run logs to a timestamped folder for print/test notes.

Stack: .NET, PicoGK, Bambu Studio
Independent project — not affiliated with LEAP 71.
```

## Resume bullet

```text
Developed a local C#/.NET PicoGK workbench for parameterized mechanical design: manifest-driven generators, CLI and Blazor UI, and documented export folders with STL artifacts and print/test notes.
```

Adjust numbers and outcomes to match what you verified.

## Project test table

| Variant | Generator | Key parameters | Print | Fit / test | Next change |
| --- | --- | --- | --- | --- | --- |
| v001 | rover-wheel | OD 80 mm, width 28 mm | PLA, 0.2 mm | Axle tight | +0.2 mm hole |
| v002 | primitive-test | defaults | — | STL OK in viewer | — |

## Photo / video proof

- Printed part on scale or next to calipers
- Screenshot of slicer with STL loaded (path can be redacted)
- Short clip of fit check — optional

## What not to publish yet

- Generators without a successful STL in `result.json`
- “Production-ready” claims for starter templates
- Topology optimization or AI generative design language
- Implied LEAP 71 endorsement
