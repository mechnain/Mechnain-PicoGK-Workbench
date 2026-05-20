using Workbench.Core;

namespace Workbench.Generators;

public sealed class GeneratorRegistry
{
    readonly Dictionary<string, IWorkbenchGenerator> _generators;

    public GeneratorRegistry()
    {
        IWorkbenchGenerator[] builtIns =
        [
            new PrimitiveTestGenerator(),
            new RoverWheelGenerator(),
            new ServoBracketGenerator(),
            new ElectronicsEnclosureGenerator(),
            new LatticeCouponGenerator()
        ];

        _generators = builtIns.ToDictionary(generator => generator.Id, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyList<GeneratorManifest> ListManifests()
    {
        return _generators.Values
            .Select(generator => generator.Manifest)
            .OrderBy(manifest => manifest.Name)
            .ToList();
    }

    public IWorkbenchGenerator GetGenerator(string id)
    {
        if (_generators.TryGetValue(id, out IWorkbenchGenerator? generator))
            return generator;

        throw new KeyNotFoundException($"No generator registered with id '{id}'.");
    }

    public GeneratorManifest GetManifest(string id)
    {
        return GetGenerator(id).Manifest;
    }
}
