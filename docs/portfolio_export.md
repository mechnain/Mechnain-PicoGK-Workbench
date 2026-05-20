# Portfolio Export

Turn **real** runs into credible engineering evidence—GitHub, LinkedIn, resume, or lab notebooks.

Only document what you executed. If you claim a print result, you should have printed (or clearly label “STL only, not printed”).

## What proof to capture

| Proof type | When to use |
| --- | --- |
| UI screenshot | Shows workbench workflow |
| `result.json` + `run_log.txt` | Shows generation succeeded |
| STL in slicer screenshot | Shows manufacturability review |
| Photo of printed part | Shows physical validation |
| `notes.md` excerpt | Shows honest test outcome |

Suggested screenshots: [screenshot_checklist.md](screenshot_checklist.md)

## GitHub README section template

```markdown
## Variant: {part name}

| Field | Value |
| --- | --- |
| Generator | `{generator-id}` |
| Variant | `{variant}` |
| Intent | {one sentence} |
| Key parameters | {dims with units} |
| Output | STL + logs in `exports/...` |
| Print / test | {honest result or "STL only"} |
| Next change | {planned tweak} |
```

Embed `docs/assets/screenshots/generator-detail.png` only after the file exists.

## LinkedIn outline (factual)

1. One line: local C# workbench around PicoGK for parameterized mechanical parts  
2. One variant: generator id + key dimensions  
3. What the run folder contains (STL, JSON, logs)  
4. Stack: .NET, PicoGK, slicer of choice  
5. Disclaimer: independent project, not affiliated with LEAP 71  

Avoid “revolutionizing manufacturing” language.

## Resume bullets (examples)

Pick one or two that match what you actually did:

- Built a local C#/.NET workbench for running parameterized PicoGK geometry generators and organizing STL, JSON, and log outputs for computational mechanical design iterations.
- Implemented a generator registry, manifest-driven parameter inputs, CLI runner, and structured export workflow for reproducible geometry generation.
- Developed starter generators for robotics-oriented parts including rover wheels, servo brackets, electronics enclosures, and lattice coupons.

Adjust tense and outcomes to match verification level (STL verified vs print validated).

## Test table template

| Variant | Generator | Key parameters | Print | Fit / test | Next change |
| --- | --- | --- | --- | --- | --- |
| v001 | primitive-test | defaults | — | STL OK in viewer | — |
| v002 | rover-wheel | OD 80 mm, W 28 mm | PLA 0.2 mm | Axle tight | +0.2 mm bore |

## How to avoid overclaiming

| Do not say | Say instead |
| --- | --- |
| Production-ready bracket | Starter bracket template; fit not validated |
| Topology optimized | Parametric PicoGK geometry |
| LEAP 71 endorsed tool | Independent workbench using PicoGK kernel |
| AI-designed part | Parameter-driven generator (or future assisted drafting) |
| Every generator validated | Only list generators with evidence |

## What not to publish yet

- Generators without `success: true` in `result.json`
- Broken README image links
- Absolute paths from your PC in public posts
- Implied LEAP 71 partnership
