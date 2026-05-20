using Workbench.Generators;

namespace Workbench.Tests;

[TestClass]
public sealed class RegistryTests
{
    [TestMethod]
    public void RegistryListsRequiredBuiltInGenerators()
    {
        GeneratorRegistry registry = new();

        string[] ids = registry.ListManifests().Select(manifest => manifest.Id).Order().ToArray();

        CollectionAssert.AreEqual(
            new[]
            {
                "electronics-enclosure",
                "lattice-coupon",
                "primitive-test",
                "rover-wheel",
                "servo-bracket"
            },
            ids);
    }

    [TestMethod]
    public void ManifestsExposeParametersAndOutputs()
    {
        GeneratorRegistry registry = new();
        var manifest = registry.GetManifest("rover-wheel");

        Assert.AreEqual("rover-wheel", manifest.Id);
        Assert.IsTrue(manifest.Parameters.Count >= 10);
        Assert.IsTrue(manifest.OutputTypes.Count > 0);
        Assert.IsTrue(manifest.Parameters.Any(parameter => parameter.Name == "outerDiameterMm"));
    }
}
