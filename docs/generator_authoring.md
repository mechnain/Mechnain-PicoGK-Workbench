# Generator Authoring

How to add a new PicoGK generator to the workbench without breaking the existing registry or export pipeline.

## 1. Create the generator class

Add a class in `src/Workbench.Generators`, inheriting `WorkbenchGeneratorBase`:

```csharp
public sealed class MyGenerator : WorkbenchGeneratorBase
{
    public override GeneratorManifest Manifest { get; } = new()
    {
        Id = "my-generator",
        Name = "My Generator",
        Category = "Mechanical",
        Difficulty = "Starter",
        Description = "Short honest description.",
        Parameters = [ /* GeneratorParameter entries */ ],
        OutputTypes = ["stl"]
    };

    protected override void GenerateGeometry(PicoGeneration generation)
    {
        using Voxels voxels = generation.Render(/* implicit body */);
        generation.SaveStl(voxels, "my_part.stl");
    }
}
```

Use real PicoGK operations only. Do not stub STL paths without calling `SaveStl`.

## 2. Create manifest files on disk

```text
generators/my-generator/manifest.json
generators/my-generator/default_params.json
```

Parameter names must match what `GenerateGeometry` reads. The UI and CLI load defaults from these files when present; built-in manifests come from the C# class via `GeneratorRegistry`.

## 3. Register the generator

In `GeneratorRegistry`:

```csharp
Register(new MyGenerator());
```

## 4. Test with CLI

```powershell
cd "D:\Mechnain Projects\Mechnain PicoGK Workbench V1\Mechnain-PicoGK-Workbench"
dotnet build
dotnet run --project src\Workbench.Runner -- --generator my-generator --quality preview
```

## 5. Verify the output folder

Confirm under `exports/my-generator/{timestamp}_{variant}/`:

- `used_params.json` — input parameters
- `result.json` — success flag and file list
- `run_log.txt` — PicoGK log lines
- `*.stl` — only if generation succeeded

Open the app → Generator Library → your generator → run preview from the UI to confirm the same pipeline.

## 6. Tests (when appropriate)

Add a test in `src/Workbench.Tests` if the generator touches shared utilities or you need a regression guard (e.g. manifest parsing, path sanitization).

## Checklist

- [ ] Unique `Id` (kebab-case)
- [ ] Honest difficulty and required measurements
- [ ] Preview and final both tested or documented if one is unsupported
- [ ] No fake screenshot or viewer outputs
- [ ] DESIGN_LOG / README updated if the generator is user-facing
