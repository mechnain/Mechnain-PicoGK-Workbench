using PicoGK;
using System.Text;
using System.Text.Json;
using Workbench.Core;

namespace Workbench.Generators.Pico;

public sealed class PicoGeneration : IDisposable
{
    readonly Library _library;
    readonly StringBuilder _log = new();
    readonly List<string> _files = [];

    public PicoGeneration(GenerationRequest request, GeneratorManifest manifest)
    {
        Request = request;
        Manifest = manifest;
        Values = new ParameterValues(request, manifest);
        Directory.CreateDirectory(request.OutputDirectory);

        float voxelSize = request.QualityMode.Equals("final", StringComparison.OrdinalIgnoreCase) ? 0.45f : 0.8f;
        _library = new Library(voxelSize);
        Log($"PicoGK library created with voxel size {voxelSize:0.###} mm.");
    }

    public GenerationRequest Request { get; }
    public GeneratorManifest Manifest { get; }
    public ParameterValues Values { get; }
    public Library Library => _library;
    public IReadOnlyList<string> GeneratedFiles => _files;
    public string LogText => _log.ToString();

    public Voxels Render(params ISdfBody[] bodies)
    {
        return Render((IReadOnlyList<ISdfBody>)bodies);
    }

    public Voxels Render(IReadOnlyList<ISdfBody> bodies)
    {
        return new Voxels(_library, new UnionImplicit(bodies));
    }

    public Voxels RenderDifference(ISdfBody solid, IReadOnlyList<ISdfBody> cuts)
    {
        return new Voxels(_library, new DifferenceImplicit(solid, cuts));
    }

    public string SaveStl(Voxels voxels, string fileName)
    {
        string path = Path.Combine(Request.OutputDirectory, fileName);
        using Mesh mesh = new(voxels);
        mesh.SaveToStlFile(path);
        _files.Add(path);
        Log($"Saved STL: {path}");
        return path;
    }

    public string SaveStl(Mesh mesh, string fileName)
    {
        string path = Path.Combine(Request.OutputDirectory, fileName);
        mesh.SaveToStlFile(path);
        _files.Add(path);
        Log($"Saved STL: {path}");
        return path;
    }

    public Dictionary<string, object?> ParameterSnapshot()
    {
        return Values.WithDefaults();
    }

    public GenerationResult Success(string message)
    {
        return new GenerationResult
        {
            Success = true,
            Message = message,
            OutputDirectory = Request.OutputDirectory,
            GeneratedFiles = _files.ToList(),
            LogText = _log.ToString(),
            Timestamp = DateTimeOffset.Now
        };
    }

    public GenerationResult Failure(string message, Exception exception)
    {
        Log(exception.ToString());

        return new GenerationResult
        {
            Success = false,
            Message = message,
            OutputDirectory = Request.OutputDirectory,
            GeneratedFiles = _files.ToList(),
            LogText = _log.ToString(),
            Timestamp = DateTimeOffset.Now
        };
    }

    public void SaveRunFiles(GenerationResult result)
    {
        File.WriteAllText(Path.Combine(Request.OutputDirectory, "run_log.txt"), result.LogText);
        File.WriteAllText(
            Path.Combine(Request.OutputDirectory, "used_params.json"),
            JsonSerializer.Serialize(ParameterSnapshot(), WorkbenchJson.Options));
        File.WriteAllText(
            Path.Combine(Request.OutputDirectory, "result.json"),
            JsonSerializer.Serialize(result, WorkbenchJson.Options));

        string notesPath = Path.Combine(Request.OutputDirectory, "notes.md");
        if (!File.Exists(notesPath))
            File.WriteAllText(notesPath, NotesTemplate());

        string printSettingsPath = Path.Combine(Request.OutputDirectory, "print_settings.md");
        if (!File.Exists(printSettingsPath))
            File.WriteAllText(printSettingsPath, PrintSettingsTemplate());
    }

    public void Log(string message)
    {
        _log.AppendLine($"[{DateTimeOffset.Now:HH:mm:ss}] {message}");
    }

    public void Dispose()
    {
        _library.Dispose();
    }

    string NotesTemplate()
    {
        return $"""
# {Request.ProjectName} / {Request.VariantName}

- Generator: {Manifest.Name}
- Quality: {Request.QualityMode}
- Date: {DateTimeOffset.Now:yyyy-MM-dd HH:mm}

## Intent

## Measurements

## Slicer Notes

## Test Result

## Next Variant
""";
    }

    static string PrintSettingsTemplate()
    {
        return """
# Print Settings

- Printer: Bambu Lab A1 mini
- Material: PLA
- Nozzle: 0.4 mm
- Layer height: 0.20 mm preview, 0.16-0.20 mm final
- Walls: 3
- Infill: 20-30%
- Supports: use only where geometry requires it
- Brim: recommended for thin or tall parts
""";
    }
}
