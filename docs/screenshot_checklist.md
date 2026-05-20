# Screenshot Checklist

Capture after `dotnet run --project src/Workbench.App`. Save under `docs/assets/screenshots/`.

Recommended size: **1920×1080** or **1600×900**. Use the dark theme; crop browser chrome if needed.

## Required for README

| # | File | Page / subject | Check |
| --- | --- | --- | --- |
| 1 | `home.png` | `/` — hero, workflow strip, MVP panel | [x] |
| 2 | `generator-library.png` | `/generators` — all five cards | [x] |
| 3 | `generator-detail.png` | `/generators/primitive-test` — parameters + buttons | [x] |
| 4 | `recent-runs.png` | `/recent-runs` — table with status badges | [x] |
| 5 | `exports.png` | `/exports` — root path + folder list | [x] |

## Optional (strong credibility)

| # | File | Subject | Check |
| --- | --- | --- | --- |
| 6 | `run-output.png` | Run output page with log + file list | [ ] |
| 7 | `terminal-cli.png` | Terminal: `--list` and `primitive-test` success | [ ] |
| 8 | `slicer-stl.png` | External slicer with generated STL loaded | [ ] |
| 9 | `printed-part.jpg` | Physical part (only after real print) | [ ] |

## After capture

1. Copy files into `docs/assets/screenshots/`
2. In root `README.md`, uncomment the `![Home](...)` embed block
3. Commit: `docs: add UI screenshots for README`

## Redaction

- Blur or crop personal paths in exports list if publishing widely
- OK to show `exports/primitive-test/...` relative paths

## LinkedIn suggestion

Best single image: **home.png** or **generator-detail.png**  
Best pair: **generator-detail.png** + **printed-part.jpg** (when available)
