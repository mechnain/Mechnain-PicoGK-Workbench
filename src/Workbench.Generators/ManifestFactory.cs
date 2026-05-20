using Workbench.Core;

namespace Workbench.Generators;

static class ManifestFactory
{
    public static GeneratorParameter Number(
        string name,
        string label,
        string unit,
        double defaultValue,
        double min,
        double max,
        double step,
        string description,
        bool required = true)
    {
        return new GeneratorParameter
        {
            Name = name,
            Label = label,
            Type = ParameterType.Number,
            Unit = unit,
            DefaultValue = defaultValue,
            Min = min,
            Max = max,
            Step = step,
            Description = description,
            Required = required
        };
    }

    public static GeneratorParameter Integer(
        string name,
        string label,
        int defaultValue,
        int min,
        int max,
        int step,
        string description,
        bool required = true)
    {
        return new GeneratorParameter
        {
            Name = name,
            Label = label,
            Type = ParameterType.Integer,
            Unit = "",
            DefaultValue = defaultValue,
            Min = min,
            Max = max,
            Step = step,
            Description = description,
            Required = required
        };
    }

    public static GeneratorParameter Boolean(
        string name,
        string label,
        bool defaultValue,
        string description)
    {
        return new GeneratorParameter
        {
            Name = name,
            Label = label,
            Type = ParameterType.Boolean,
            DefaultValue = defaultValue,
            Description = description,
            Required = false
        };
    }

    public static GeneratorParameter Select(
        string name,
        string label,
        string defaultValue,
        IReadOnlyList<string> options,
        string description)
    {
        return new GeneratorParameter
        {
            Name = name,
            Label = label,
            Type = ParameterType.Select,
            DefaultValue = defaultValue,
            Options = options.ToList(),
            Description = description,
            Required = true
        };
    }

    public static List<ExportType> StlOutputs()
    {
        return [ExportType.Stl, ExportType.Log, ExportType.Json, ExportType.Notes];
    }
}
