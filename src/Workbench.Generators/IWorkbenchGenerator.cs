using Workbench.Core;

namespace Workbench.Generators;

public interface IWorkbenchGenerator
{
    string Id { get; }
    GeneratorManifest Manifest { get; }
    GenerationResult Generate(GenerationRequest request);
}
