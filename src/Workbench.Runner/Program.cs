using System.Text.Json;
using Workbench.Core;
using Workbench.Generators;

try
{
    CliOptions options = CliOptions.Parse(args);
    GeneratorRegistry registry = new();

    if (options.ShowHelp)
    {
        PrintHelp();
        return 0;
    }

    if (options.List)
    {
        foreach (GeneratorManifest listManifest in registry.ListManifests())
            Console.WriteLine($"{listManifest.Id,-24} {listManifest.Name} - {listManifest.Description}");

        return 0;
    }

    if (string.IsNullOrWhiteSpace(options.GeneratorId))
        throw new ArgumentException("Missing --generator. Use --list to see available generators.");

    IWorkbenchGenerator generator = registry.GetGenerator(options.GeneratorId);
    GeneratorManifest manifest = generator.Manifest;

    Dictionary<string, object?> parameterValues = LoadParameters(options.ParamsPath, manifest);
    string variant = options.VariantName ?? "run";
    string outputDirectory = options.OutputDirectory ??
        Path.Combine(
            WorkbenchPaths.FindRepositoryRoot(),
            "exports",
            manifest.Id,
            $"{WorkbenchPaths.Timestamp()}_{WorkbenchPaths.SanitizeSegment(variant)}");

    GenerationRequest request = new()
    {
        GeneratorId = manifest.Id,
        ProjectName = options.ProjectName ?? manifest.Name,
        VariantName = variant,
        ParameterValues = parameterValues,
        OutputDirectory = outputDirectory,
        QualityMode = options.QualityMode ?? "preview"
    };

    GenerationResult result = generator.Generate(request);
    UpdateRunsIndex(manifest, request, result);

    Console.WriteLine(result.Success ? "Generation succeeded." : "Generation failed.");
    Console.WriteLine(result.Message);
    Console.WriteLine($"Output: {result.OutputDirectory}");

    foreach (string file in result.GeneratedFiles)
        Console.WriteLine($"- {file}");

    return result.Success ? 0 : 1;
}
catch (Exception exception)
{
    Console.Error.WriteLine("Workbench runner failed:");
    Console.Error.WriteLine(exception.Message);
    return 1;
}

static Dictionary<string, object?> LoadParameters(string? paramsPath, GeneratorManifest manifest)
{
    Dictionary<string, object?> values = manifest.Parameters.ToDictionary(
        parameter => parameter.Name,
        parameter => parameter.DefaultValue,
        StringComparer.OrdinalIgnoreCase);

    if (!string.IsNullOrWhiteSpace(paramsPath))
    {
        string json = File.ReadAllText(paramsPath);
        Dictionary<string, object?>? fromFile = JsonSerializer.Deserialize<Dictionary<string, object?>>(json, WorkbenchJson.Options);

        if (fromFile is not null)
        {
            foreach ((string key, object? value) in fromFile)
                values[key] = value;
        }
    }

    return values;
}

static void UpdateRunsIndex(GeneratorManifest manifest, GenerationRequest request, GenerationResult result)
{
    string root = WorkbenchPaths.FindRepositoryRoot();
    string exportsDir = Path.Combine(root, "exports");
    Directory.CreateDirectory(exportsDir);
    string path = Path.Combine(exportsDir, "runs_index.json");

    List<RunRecord> records = [];
    if (File.Exists(path))
    {
        string json = File.ReadAllText(path);
        records = JsonSerializer.Deserialize<List<RunRecord>>(json, WorkbenchJson.Options) ?? [];
    }

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

static void PrintHelp()
{
    Console.WriteLine("""
Mechnain PicoGK Workbench Runner

Usage:
  dotnet run --project src/Workbench.Runner -- --list
  dotnet run --project src/Workbench.Runner -- --generator primitive-test
  dotnet run --project src/Workbench.Runner -- --generator rover-wheel --params generators/rover-wheel/default_params.json --output exports/rover-wheel/run_001 --quality preview

Options:
  --list                 List available generators.
  --generator <id>       Generator id to run.
  --params <path>        JSON parameter file.
  --output <path>        Output directory. Defaults to exports/{generator}/{timestamp}_{variant}.
  --quality <mode>       preview or final.
  --project <name>       Project name stored in run metadata.
  --variant <name>       Variant name stored in run metadata.
""");
}

sealed record CliOptions
{
    public bool List { get; init; }
    public bool ShowHelp { get; init; }
    public string? GeneratorId { get; init; }
    public string? ParamsPath { get; init; }
    public string? OutputDirectory { get; init; }
    public string? QualityMode { get; init; }
    public string? ProjectName { get; init; }
    public string? VariantName { get; init; }

    public static CliOptions Parse(string[] args)
    {
        bool list = false;
        bool help = false;
        string? generator = null;
        string? parameters = null;
        string? output = null;
        string? quality = null;
        string? project = null;
        string? variant = null;

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];
            switch (arg)
            {
                case "--list":
                    list = true;
                    break;
                case "--help":
                case "-h":
                    help = true;
                    break;
                case "--generator":
                    generator = RequireValue(args, ref i, arg);
                    break;
                case "--params":
                    parameters = RequireValue(args, ref i, arg);
                    break;
                case "--output":
                    output = RequireValue(args, ref i, arg);
                    break;
                case "--quality":
                    quality = RequireValue(args, ref i, arg);
                    break;
                case "--project":
                    project = RequireValue(args, ref i, arg);
                    break;
                case "--variant":
                    variant = RequireValue(args, ref i, arg);
                    break;
                default:
                    throw new ArgumentException($"Unknown argument '{arg}'.");
            }
        }

        return new CliOptions
        {
            List = list,
            ShowHelp = help,
            GeneratorId = generator,
            ParamsPath = parameters,
            OutputDirectory = output,
            QualityMode = quality,
            ProjectName = project,
            VariantName = variant
        };
    }

    static string RequireValue(string[] args, ref int index, string option)
    {
        if (index + 1 >= args.Length)
            throw new ArgumentException($"{option} requires a value.");

        index++;
        return args[index];
    }
}
