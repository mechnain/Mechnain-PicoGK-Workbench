using Workbench.Core;
using Workbench.Generators.Pico;

namespace Workbench.Generators;

public abstract class WorkbenchGeneratorBase : IWorkbenchGenerator
{
    public string Id => Manifest.Id;
    public abstract GeneratorManifest Manifest { get; }

    public GenerationResult Generate(GenerationRequest request)
    {
        GenerationRequest normalized = request with
        {
            GeneratorId = Manifest.Id,
            OutputDirectory = string.IsNullOrWhiteSpace(request.OutputDirectory)
                ? Path.Combine(Directory.GetCurrentDirectory(), "exports", Manifest.Id, $"{WorkbenchPaths.Timestamp()}_{WorkbenchPaths.SanitizeSegment(request.VariantName)}")
                : request.OutputDirectory
        };

        Directory.CreateDirectory(normalized.OutputDirectory);

        using PicoGeneration generation = new(normalized, Manifest);

        try
        {
            GenerateGeometry(generation);
            GenerationResult result = generation.Success($"{Manifest.Name} generated successfully.");
            generation.SaveRunFiles(result);
            return result;
        }
        catch (Exception exception)
        {
            GenerationResult result = generation.Failure($"{Manifest.Name} generation failed: {exception.Message}", exception);
            generation.SaveRunFiles(result);
            return result;
        }
    }

    protected abstract void GenerateGeometry(PicoGeneration generation);
}
