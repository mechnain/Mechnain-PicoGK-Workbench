# Generator Authoring

Developer guide for adding a PicoGK generator to the workbench without breaking the registry or export pipeline.

## Generator folder structure

```text
generators/{generator-id}/
  manifest.json          # Describes parameters, outputs, measurements (mirror of C# manifest)
  default_params.json    # Default parameter values

src/Workbench.Generators/
  MyGenerator.cs         # PicoGK geometry implementation

src/Workbench.Generators/GeneratorRegistry.cs
  Register(new MyGenerator());
```

## manifest.json (on disk)

Align with the C# `GeneratorManifest`:

- `id` — kebab-case, unique
- `name`, `description`, `category`, `difficulty`
- `requiredMeasurements` — strings the engineer must know
- `parameters` — name, type, label, unit, min/max, default
- `outputTypes` — e.g. `["stl"]` only; do not list screenshot until implemented

## default_params.json

JSON object keyed by parameter name. Values must match types the generator reads (`number`, `boolean`, `string`).

## Generator class

```csharp
public sealed class MyGenerator : WorkbenchGeneratorBase
{
    public override GeneratorManifest Manifest { get; } = new()
    {
        Id = "my-generator",
        Name = "My Generator",
        Category = "Mechanical",
        Difficulty = "Starter",
        Description = "Honest one-line purpose.",
        RequiredMeasurements = ["Overall width (mm)"],
        Parameters = [ /* GeneratorParameter entries */ ],
        OutputTypes = ["stl"]
    };

    protected override void GenerateGeometry(PicoGeneration generation)
    {
        // Real PicoGK: implicit bodies → voxels → mesh
        using Voxels voxels = generation.Render(/* body */);
        generation.SaveStl(voxels, "my_part.stl");
    }
}
```

**Rules:**

- Call `SaveStl` only with real voxel/mesh data
- Read parameters via `generation` / request values consistently
- Do not write fake STL paths without generating geometry

## Registration

In `GeneratorRegistry` constructor or registration method:

```csharp
Register(new MyGenerator());
```

## Parameter validation

- Use manifest `min` / `max` / `step` for UI hints
- Validate in `GenerateGeometry` or base class hooks when invalid combos would silently fail
- Return clear errors in logs when parameters are out of range

## Output artifacts

The base pipeline writes:

- `used_params.json`, `result.json`, `run_log.txt`
- Note templates: `notes.md`, `print_settings.md`
- STL files listed in `result.json` → `generatedFiles`

## CLI testing

```bash
dotnet build
dotnet run --project src/Workbench.Runner -- --generator my-generator --quality preview
```

Inspect:

```text
exports/my-generator/{timestamp}_{variant}/
```

Then run the same generator from the Blazor UI to confirm parity.

## New generator checklist

- [ ] Define purpose and maturity label (Starter / Experimental / Verified)
- [ ] Define required measurements
- [ ] Create `generators/{id}/manifest.json`
- [ ] Create `generators/{id}/default_params.json`
- [ ] Implement `WorkbenchGeneratorBase` subclass
- [ ] Register in `GeneratorRegistry`
- [ ] Run CLI preview and confirm `result.json` success
- [ ] Confirm `run_log.txt` shows PicoGK steps
- [ ] Inspect STL in external viewer
- [ ] Update README generator table if user-facing
- [ ] Add test if touching shared registry/path logic
- [ ] Mark maturity honestly in DESIGN_LOG

## Documentation checklist

- [ ] One-line description matches actual geometry
- [ ] No production-ready claim without print evidence
- [ ] Limitations noted (e.g. reserved parameters, unsupported angles)

## Tests

Add to `src/Workbench.Tests` when:

- Registry must list the new id
- Path sanitization or manifest parsing changes

Do not require PicoGK native runtime in unit tests unless you add an explicit integration job (not in default CI).
