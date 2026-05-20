using System.Diagnostics;
using System.Text.Json;
using Workbench.Core;
using Workbench.Generators;

namespace Workbench.App;

public sealed class WorkbenchAppService
{
    readonly GeneratorRegistry _registry = new();

    public string RepositoryRoot => WorkbenchPaths.FindRepositoryRoot();
    public string ExportsDirectory => Path.Combine(RepositoryRoot, "exports");
    public string GeneratorsDirectory => Path.Combine(RepositoryRoot, "generators");
    public string DocsDirectory => Path.Combine(RepositoryRoot, "docs");

    public IReadOnlyList<GeneratorManifest> ListGenerators()
    {
        return _registry.ListManifests();
    }

    public GeneratorManifest GetManifest(string id)
    {
        return _registry.GetManifest(id);
    }

    public GenerationResult Generate(string generatorId, string projectName, string variantName, string qualityMode, Dictionary<string, object?> values)
    {
        GeneratorManifest manifest = _registry.GetManifest(generatorId);
        string outputDir = Path.Combine(
            RepositoryRoot,
            "exports",
            manifest.Id,
            $"{WorkbenchPaths.Timestamp()}_{WorkbenchPaths.SanitizeSegment(variantName)}");

        GenerationRequest request = new()
        {
            GeneratorId = manifest.Id,
            ProjectName = string.IsNullOrWhiteSpace(projectName) ? manifest.Name : projectName,
            VariantName = string.IsNullOrWhiteSpace(variantName) ? "default" : variantName,
            ParameterValues = values,
            OutputDirectory = outputDir,
            QualityMode = qualityMode
        };

        GenerationResult result = _registry.GetGenerator(generatorId).Generate(request);
        UpdateRunsIndex(manifest, request, result);
        return result;
    }

    public GenerationResult? ReadResult(string outputDirectory)
    {
        string path = Path.Combine(outputDirectory, "result.json");
        if (!File.Exists(path))
            return null;

        return JsonSerializer.Deserialize<GenerationResult>(File.ReadAllText(path), WorkbenchJson.Options);
    }

    public IReadOnlyList<RunRecord> RecentRuns()
    {
        string path = Path.Combine(ExportsDirectory, "runs_index.json");
        if (!File.Exists(path))
            return [];

        return JsonSerializer.Deserialize<List<RunRecord>>(File.ReadAllText(path), WorkbenchJson.Options) ?? [];
    }

    public void CreateNotes(string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);
        string path = Path.Combine(outputDirectory, "notes.md");
        if (!File.Exists(path))
        {
            File.WriteAllText(path, """
# Run Notes

## Intent

## Measurements

## Print Result

## Next Revision
""");
        }
    }

    public void OpenFolder(string outputDirectory)
    {
        if (!Directory.Exists(outputDirectory))
            return;

        ProcessStartInfo info = new()
        {
            FileName = outputDirectory,
            UseShellExecute = true
        };
        Process.Start(info);
    }

    public IReadOnlyList<string> ExportRunFolders()
    {
        if (!Directory.Exists(ExportsDirectory))
            return [];

        return Directory
            .EnumerateDirectories(ExportsDirectory, "*", SearchOption.AllDirectories)
            .Where(directory => File.Exists(Path.Combine(directory, "result.json")))
            .OrderByDescending(Directory.GetLastWriteTimeUtc)
            .Take(50)
            .ToList();
    }

    public IReadOnlyList<string> DocumentationFiles()
    {
        if (!Directory.Exists(DocsDirectory))
            return [];

        return Directory
            .EnumerateFiles(DocsDirectory, "*.md", SearchOption.TopDirectoryOnly)
            .OrderBy(Path.GetFileName)
            .ToList();
    }

    static void UpdateRunsIndex(GeneratorManifest manifest, GenerationRequest request, GenerationResult result)
    {
        string exportsDir = Path.Combine(WorkbenchPaths.FindRepositoryRoot(), "exports");
        Directory.CreateDirectory(exportsDir);
        string path = Path.Combine(exportsDir, "runs_index.json");

        List<RunRecord> records = [];
        if (File.Exists(path))
            records = JsonSerializer.Deserialize<List<RunRecord>>(File.ReadAllText(path), WorkbenchJson.Options) ?? [];

        records.Insert(0, new RunRecord
        {
            GeneratorId = manifest.Id,
            GeneratorName = manifest.Name,
            ProjectName = request.ProjectName,
            VariantName = request.VariantName,
            Success = result.Success,
            OutputDirectory = result.OutputDirectory,
            Timestamp = result.Timestamp
        });

        File.WriteAllText(path, JsonSerializer.Serialize(records.Take(100).ToList(), WorkbenchJson.Options));
    }
}
