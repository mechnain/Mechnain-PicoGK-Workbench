using System.Text.Json;
using System.Text.Json.Serialization;

namespace Workbench.Core;

[JsonConverter(typeof(JsonStringEnumConverter<ParameterType>))]
public enum ParameterType
{
    Number,
    Integer,
    Boolean,
    String,
    Select
}

[JsonConverter(typeof(JsonStringEnumConverter<ExportType>))]
public enum ExportType
{
    Stl,
    Log,
    Json,
    Notes,
    Screenshot
}

public sealed record GeneratorManifest
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public string Category { get; init; } = "";
    public string Difficulty { get; init; } = "";
    public List<string> RequiredMeasurements { get; init; } = [];
    public List<GeneratorParameter> Parameters { get; init; } = [];
    public List<GeneratorPreset> Presets { get; init; } = [];
    public List<ExportType> OutputTypes { get; init; } = [];
}

public sealed record GeneratorParameter
{
    public string Name { get; init; } = "";
    public string Label { get; init; } = "";
    public ParameterType Type { get; init; }
    public string Unit { get; init; } = "";
    public object? DefaultValue { get; init; }
    public double? Min { get; init; }
    public double? Max { get; init; }
    public double? Step { get; init; }
    public string Description { get; init; } = "";
    public bool Required { get; init; }
    public List<string> Options { get; init; } = [];
}

public sealed record GeneratorPreset
{
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public Dictionary<string, object?> ParameterValues { get; init; } = [];
}

public sealed record GenerationRequest
{
    public string GeneratorId { get; init; } = "";
    public string ProjectName { get; init; } = "";
    public string VariantName { get; init; } = "default";
    public Dictionary<string, object?> ParameterValues { get; init; } = [];
    public string OutputDirectory { get; init; } = "";
    public string QualityMode { get; init; } = "preview";
}

public sealed record GenerationResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = "";
    public string OutputDirectory { get; init; } = "";
    public List<string> GeneratedFiles { get; init; } = [];
    public string LogText { get; init; } = "";
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;
}

public sealed record RunRecord
{
    public string GeneratorId { get; init; } = "";
    public string GeneratorName { get; init; } = "";
    public string ProjectName { get; init; } = "";
    public string VariantName { get; init; } = "";
    public bool Success { get; init; }
    public string OutputDirectory { get; init; } = "";
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;
}

public static class WorkbenchJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
}
