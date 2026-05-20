using System.Globalization;
using System.Text.Json;

namespace Workbench.Core;

public sealed class ParameterValues
{
    readonly IReadOnlyDictionary<string, object?> _values;
    readonly IReadOnlyDictionary<string, GeneratorParameter> _parameters;

    public ParameterValues(GenerationRequest request, GeneratorManifest manifest)
    {
        _values = request.ParameterValues;
        _parameters = manifest.Parameters.ToDictionary(parameter => parameter.Name, StringComparer.OrdinalIgnoreCase);
    }

    public double GetDouble(string name)
    {
        object? value = ValueOrDefault(name);
        return ToDouble(value, name);
    }

    public int GetInt(string name)
    {
        object? value = ValueOrDefault(name);
        return (int)Math.Round(ToDouble(value, name));
    }

    public bool GetBool(string name)
    {
        object? value = ValueOrDefault(name);

        if (value is bool b)
            return b;

        if (value is JsonElement element)
        {
            if (element.ValueKind is JsonValueKind.True or JsonValueKind.False)
                return element.GetBoolean();

            if (element.ValueKind == JsonValueKind.String && bool.TryParse(element.GetString(), out bool parsed))
                return parsed;
        }

        if (value is string s && bool.TryParse(s, out bool parsedString))
            return parsedString;

        throw new InvalidOperationException($"Parameter '{name}' cannot be converted to a boolean value.");
    }

    public string GetString(string name)
    {
        object? value = ValueOrDefault(name);

        if (value is JsonElement element)
            return element.ValueKind == JsonValueKind.String ? element.GetString() ?? "" : element.ToString();

        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
    }

    public Dictionary<string, object?> WithDefaults()
    {
        Dictionary<string, object?> merged = new(StringComparer.OrdinalIgnoreCase);

        foreach (GeneratorParameter parameter in _parameters.Values)
            merged[parameter.Name] = parameter.DefaultValue;

        foreach ((string key, object? value) in _values)
            merged[key] = value;

        return merged;
    }

    object? ValueOrDefault(string name)
    {
        if (_values.TryGetValue(name, out object? value))
            return value;

        if (_parameters.TryGetValue(name, out GeneratorParameter? parameter))
            return parameter.DefaultValue;

        throw new KeyNotFoundException($"Unknown parameter '{name}'.");
    }

    static double ToDouble(object? value, string name)
    {
        if (value is null)
            throw new InvalidOperationException($"Parameter '{name}' has no value.");

        if (value is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetDouble(out double number))
                return number;

            if (element.ValueKind == JsonValueKind.String &&
                double.TryParse(element.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double stringNumber))
                return stringNumber;
        }

        if (value is IConvertible)
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);

        throw new InvalidOperationException($"Parameter '{name}' cannot be converted to a numeric value.");
    }
}
