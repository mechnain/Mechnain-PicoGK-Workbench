using Workbench.Core;
using Workbench.Generators;

namespace Workbench.Tests;

[TestClass]
public sealed class RunnerContractTests
{
    [TestMethod]
    public void PrimitiveGeneratorCanCreateRealOutputResult()
    {
        string outputDir = Path.Combine(Path.GetTempPath(), "mechnain-workbench-tests", Guid.NewGuid().ToString("N"));

        GenerationRequest request = new()
        {
            GeneratorId = "primitive-test",
            ProjectName = "Tests",
            VariantName = "primitive",
            OutputDirectory = outputDir,
            QualityMode = "preview"
        };

        GeneratorRegistry registry = new();
        GenerationResult result = registry.GetGenerator("primitive-test").Generate(request);

        Assert.IsTrue(result.Success, result.Message);
        Assert.IsTrue(result.GeneratedFiles.Any(file => file.EndsWith(".stl", StringComparison.OrdinalIgnoreCase)));
        Assert.IsTrue(File.Exists(result.GeneratedFiles.First(file => file.EndsWith(".stl", StringComparison.OrdinalIgnoreCase))));
    }
}
