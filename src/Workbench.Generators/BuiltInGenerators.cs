using PicoGK;
using System.Numerics;
using Workbench.Core;
using Workbench.Generators.Pico;

namespace Workbench.Generators;

public sealed class PrimitiveTestGenerator : WorkbenchGeneratorBase
{
    public override GeneratorManifest Manifest { get; } = new()
    {
        Id = "primitive-test",
        Name = "Primitive Test",
        Description = "Small PicoGK smoke test: box, sphere, lattice beam, and boolean hole.",
        Category = "Diagnostics",
        Difficulty = "Beginner",
        RequiredMeasurements = [],
        OutputTypes = ManifestFactory.StlOutputs(),
        Parameters =
        [
            ManifestFactory.Number("boxSizeMm", "Box size", "mm", 32, 10, 80, 1, "Overall diagnostic box size."),
            ManifestFactory.Number("sphereRadiusMm", "Sphere radius", "mm", 14, 5, 40, 1, "Boolean-add sphere radius."),
            ManifestFactory.Number("holeDiameterMm", "Hole diameter", "mm", 10, 2, 30, 0.5, "Through-hole diameter."),
            ManifestFactory.Number("beamRadiusMm", "Beam radius", "mm", 3, 1, 10, 0.5, "Lattice beam radius.")
        ],
        Presets =
        [
            new GeneratorPreset { Name = "Default smoke test", ParameterValues = [] }
        ]
    };

    protected override void GenerateGeometry(PicoGeneration generation)
    {
        double boxSize = generation.Values.GetDouble("boxSizeMm");
        double sphereRadius = generation.Values.GetDouble("sphereRadiusMm");
        double holeDiameter = generation.Values.GetDouble("holeDiameterMm");
        double beamRadius = generation.Values.GetDouble("beamRadiusMm");

        Mesh boxMesh = Utils.mshCreateCube(
            generation.Library,
            new Vector3((float)boxSize, (float)boxSize, 12),
            new Vector3(-18, 0, 0));

        using Voxels box = new(boxMesh);
        using Voxels hole = generation.Render(new CylinderBody(new Vector3(-18, 0, 0), Vector3.UnitZ, (float)holeDiameter * 0.5f, 28));
        using Voxels boxWithHole = box - hole;
        using Voxels sphere = Voxels.voxSphere(generation.Library, new Vector3(20, 0, 0), (float)sphereRadius);
        using Voxels beam = Voxels.voxLatticeBeam(generation.Library, new Vector3(-45, -18, 10), (float)beamRadius, new Vector3(45, 18, 10), (float)beamRadius);
        using Voxels all = boxWithHole + sphere + beam;

        generation.SaveStl(all, "primitive_test.stl");
    }
}

public sealed class RoverWheelGenerator : WorkbenchGeneratorBase
{
    public override GeneratorManifest Manifest { get; } = new()
    {
        Id = "rover-wheel",
        Name = "Rover Wheel",
        Description = "Parametric wheel with hub, axle hole, spokes, and repeated tread blocks.",
        Category = "Mobility",
        Difficulty = "Intermediate",
        RequiredMeasurements = ["Axle shaft diameter", "Target tire outside diameter", "Available chassis clearance"],
        OutputTypes = ManifestFactory.StlOutputs(),
        Parameters =
        [
            ManifestFactory.Number("outerDiameterMm", "Outer diameter", "mm", 80, 30, 160, 1, "Overall wheel diameter."),
            ManifestFactory.Number("wheelWidthMm", "Wheel width", "mm", 28, 8, 80, 1, "Wheel width along the axle."),
            ManifestFactory.Number("hubDiameterMm", "Hub diameter", "mm", 28, 8, 80, 1, "Center hub diameter."),
            ManifestFactory.Number("axleHoleDiameterMm", "Axle hole diameter", "mm", 6, 2, 30, 0.5, "Center axle clearance hole."),
            ManifestFactory.Integer("spokeCount", "Spoke count", 8, 3, 24, 1, "Number of radial spokes."),
            ManifestFactory.Number("spokeThicknessMm", "Spoke thickness", "mm", 5, 2, 15, 0.5, "Spoke capsule diameter."),
            ManifestFactory.Number("rimThicknessMm", "Rim thickness", "mm", 8, 3, 24, 0.5, "Radial rim thickness."),
            ManifestFactory.Integer("treadCount", "Tread count", 24, 6, 64, 1, "Number of repeated tread blocks."),
            ManifestFactory.Number("treadDepthMm", "Tread depth", "mm", 4, 0, 12, 0.5, "Radial tread block height."),
            ManifestFactory.Number("treadWidthMm", "Tread width", "mm", 8, 2, 24, 0.5, "Tread block tangential width.")
        ],
        Presets = [new GeneratorPreset { Name = "80 mm test wheel", ParameterValues = [] }]
    };

    protected override void GenerateGeometry(PicoGeneration generation)
    {
        float outerRadius = (float)generation.Values.GetDouble("outerDiameterMm") * 0.5f;
        float width = (float)generation.Values.GetDouble("wheelWidthMm");
        float hubRadius = (float)generation.Values.GetDouble("hubDiameterMm") * 0.5f;
        float axleRadius = (float)generation.Values.GetDouble("axleHoleDiameterMm") * 0.5f;
        int spokeCount = generation.Values.GetInt("spokeCount");
        float spokeRadius = (float)generation.Values.GetDouble("spokeThicknessMm") * 0.5f;
        float rimThickness = (float)generation.Values.GetDouble("rimThicknessMm");
        int treadCount = generation.Values.GetInt("treadCount");
        float treadDepth = (float)generation.Values.GetDouble("treadDepthMm");
        float treadWidth = (float)generation.Values.GetDouble("treadWidthMm");

        List<ISdfBody> solids =
        [
            new CylinderBody(Vector3.Zero, Vector3.UnitY, outerRadius, width),
            new CylinderBody(Vector3.Zero, Vector3.UnitY, hubRadius, width + 6)
        ];

        for (int i = 0; i < spokeCount; i++)
        {
            float angle = i * MathF.Tau / spokeCount;
            Vector3 radial = new(MathF.Cos(angle), 0, MathF.Sin(angle));
            solids.Add(new CapsuleBody(radial * (hubRadius * 0.8f), radial * (outerRadius - rimThickness * 0.55f), spokeRadius));
        }

        for (int i = 0; i < treadCount; i++)
        {
            float angle = i * MathF.Tau / treadCount;
            Vector3 radial = new(MathF.Cos(angle), 0, MathF.Sin(angle));
            Vector3 tangent = new(-MathF.Sin(angle), 0, MathF.Cos(angle));
            solids.Add(new OrientedBoxBody(
                radial * (outerRadius + treadDepth * 0.35f),
                tangent,
                Vector3.UnitY,
                radial,
                new Vector3(treadWidth, width + 3, treadDepth),
                1.2f));
        }

        List<ISdfBody> cuts =
        [
            new CylinderBody(Vector3.Zero, Vector3.UnitY, outerRadius - rimThickness, width + 8),
            new CylinderBody(Vector3.Zero, Vector3.UnitY, axleRadius, width + 14)
        ];

        using Voxels solid = generation.Render(solids);
        using Voxels cut = generation.Render(cuts);
        using Voxels wheel = solid - cut;

        generation.SaveStl(wheel, "rover_wheel.stl");
    }
}

public sealed class ServoBracketGenerator : WorkbenchGeneratorBase
{
    public override GeneratorManifest Manifest { get; } = new()
    {
        Id = "servo-bracket",
        Name = "Servo Bracket",
        Description = "U-bracket with servo pocket, base plate, mounting holes, and horn clearance.",
        Category = "Robotics",
        Difficulty = "Intermediate",
        RequiredMeasurements = ["Servo body width", "Servo body length", "Servo body height", "Screw diameter"],
        OutputTypes = ManifestFactory.StlOutputs(),
        Parameters =
        [
            ManifestFactory.Number("servoWidthMm", "Servo width", "mm", 20, 8, 60, 0.5, "Servo body width."),
            ManifestFactory.Number("servoLengthMm", "Servo length", "mm", 40, 12, 80, 0.5, "Servo body length."),
            ManifestFactory.Number("servoHeightMm", "Servo height", "mm", 38, 10, 80, 0.5, "Servo body height."),
            ManifestFactory.Number("wallThicknessMm", "Wall thickness", "mm", 3, 1.5, 8, 0.25, "Printed bracket wall thickness."),
            ManifestFactory.Number("baseLengthMm", "Base length", "mm", 60, 20, 120, 1, "Mounting base length."),
            ManifestFactory.Number("baseWidthMm", "Base width", "mm", 38, 16, 100, 1, "Mounting base width."),
            ManifestFactory.Number("screwHoleDiameterMm", "Screw hole diameter", "mm", 3.2, 1.5, 8, 0.1, "Mounting screw clearance."),
            ManifestFactory.Number("bracketAngleDeg", "Bracket angle", "deg", 90, 45, 135, 1, "Reserved for future angled side-wall variants.")
        ],
        Presets = [new GeneratorPreset { Name = "Standard micro/mini servo bracket", ParameterValues = [] }]
    };

    protected override void GenerateGeometry(PicoGeneration generation)
    {
        float servoW = (float)generation.Values.GetDouble("servoWidthMm");
        float servoL = (float)generation.Values.GetDouble("servoLengthMm");
        float servoH = (float)generation.Values.GetDouble("servoHeightMm");
        float wall = (float)generation.Values.GetDouble("wallThicknessMm");
        float baseL = (float)generation.Values.GetDouble("baseLengthMm");
        float baseW = (float)generation.Values.GetDouble("baseWidthMm");
        float screwRadius = (float)generation.Values.GetDouble("screwHoleDiameterMm") * 0.5f;

        List<ISdfBody> solids =
        [
            new BoxBody(new Vector3(0, 0, wall * 0.5f), new Vector3(baseL, baseW, wall), 1.5f),
            new BoxBody(new Vector3(0, -servoW * 0.5f - wall * 0.5f, servoH * 0.5f + wall), new Vector3(servoL + wall * 2, wall, servoH), 1.5f),
            new BoxBody(new Vector3(0, servoW * 0.5f + wall * 0.5f, servoH * 0.5f + wall), new Vector3(servoL + wall * 2, wall, servoH), 1.5f),
            new BoxBody(new Vector3(-servoL * 0.5f - wall * 0.5f, 0, servoH * 0.5f + wall), new Vector3(wall, servoW + wall * 2, servoH), 1.5f)
        ];

        List<ISdfBody> cuts =
        [
            new BoxBody(new Vector3(0, 0, servoH * 0.5f + wall + 1), new Vector3(servoL, servoW, servoH + 2), 1f),
            new CylinderBody(new Vector3(servoL * 0.5f + wall, 0, servoH + wall), Vector3.UnitX, MathF.Max(servoW * 0.33f, 6), wall * 4)
        ];

        foreach (float x in new[] { -baseL * 0.35f, baseL * 0.35f })
        {
            cuts.Add(new CylinderBody(new Vector3(x, -baseW * 0.32f, wall * 0.5f), Vector3.UnitZ, screwRadius, wall + 4));
            cuts.Add(new CylinderBody(new Vector3(x, baseW * 0.32f, wall * 0.5f), Vector3.UnitZ, screwRadius, wall + 4));
        }

        using Voxels solid = generation.Render(solids);
        using Voxels cut = generation.Render(cuts);
        using Voxels bracket = solid - cut;

        generation.SaveStl(bracket, "servo_bracket.stl");
    }
}

public sealed class ElectronicsEnclosureGenerator : WorkbenchGeneratorBase
{
    public override GeneratorManifest Manifest { get; } = new()
    {
        Id = "electronics-enclosure",
        Name = "Electronics Enclosure",
        Description = "Board tray with walls, standoffs, wire openings, and optional lid.",
        Category = "Electronics",
        Difficulty = "Intermediate",
        RequiredMeasurements = ["Board length", "Board width", "Board component height", "Mounting hole diameter"],
        OutputTypes = ManifestFactory.StlOutputs(),
        Parameters =
        [
            ManifestFactory.Number("boardLengthMm", "Board length", "mm", 70, 20, 160, 1, "Circuit board length."),
            ManifestFactory.Number("boardWidthMm", "Board width", "mm", 45, 15, 120, 1, "Circuit board width."),
            ManifestFactory.Number("boardHeightMm", "Board height", "mm", 14, 4, 60, 0.5, "Component clearance height."),
            ManifestFactory.Number("wallThicknessMm", "Wall thickness", "mm", 3, 1.5, 8, 0.25, "Enclosure wall thickness."),
            ManifestFactory.Number("clearanceMm", "Clearance", "mm", 2, 0, 8, 0.25, "Board side clearance."),
            ManifestFactory.Number("standoffHeightMm", "Standoff height", "mm", 5, 2, 20, 0.5, "Board standoff height."),
            ManifestFactory.Number("screwHoleDiameterMm", "Screw hole diameter", "mm", 3, 1.2, 8, 0.1, "Board screw hole clearance."),
            ManifestFactory.Boolean("lidEnabled", "Generate lid", true, "Generate a separate lid STL.")
        ],
        Presets = [new GeneratorPreset { Name = "Small controller enclosure", ParameterValues = [] }]
    };

    protected override void GenerateGeometry(PicoGeneration generation)
    {
        float boardL = (float)generation.Values.GetDouble("boardLengthMm");
        float boardW = (float)generation.Values.GetDouble("boardWidthMm");
        float boardH = (float)generation.Values.GetDouble("boardHeightMm");
        float wall = (float)generation.Values.GetDouble("wallThicknessMm");
        float clearance = (float)generation.Values.GetDouble("clearanceMm");
        float standoffH = (float)generation.Values.GetDouble("standoffHeightMm");
        float screwRadius = (float)generation.Values.GetDouble("screwHoleDiameterMm") * 0.5f;
        bool lidEnabled = generation.Values.GetBool("lidEnabled");

        float outerL = boardL + clearance * 2 + wall * 2;
        float outerW = boardW + clearance * 2 + wall * 2;
        float wallH = boardH + standoffH + wall;

        List<ISdfBody> solids =
        [
            new BoxBody(new Vector3(0, 0, wall * 0.5f), new Vector3(outerL, outerW, wall), 2),
            new BoxBody(new Vector3(0, -outerW * 0.5f + wall * 0.5f, wallH * 0.5f), new Vector3(outerL, wall, wallH), 2),
            new BoxBody(new Vector3(0, outerW * 0.5f - wall * 0.5f, wallH * 0.5f), new Vector3(outerL, wall, wallH), 2),
            new BoxBody(new Vector3(-outerL * 0.5f + wall * 0.5f, 0, wallH * 0.5f), new Vector3(wall, outerW, wallH), 2),
            new BoxBody(new Vector3(outerL * 0.5f - wall * 0.5f, 0, wallH * 0.5f), new Vector3(wall, outerW, wallH), 2)
        ];

        foreach (Vector3 pos in CornerPositions(boardL * 0.42f, boardW * 0.38f, wall + standoffH * 0.5f))
            solids.Add(new CylinderBody(pos, Vector3.UnitZ, 3.2f, standoffH));

        List<ISdfBody> cuts =
        [
            new BoxBody(new Vector3(outerL * 0.5f - wall * 0.5f, 0, wall + 8), new Vector3(wall + 2, 16, 8), 1),
            new BoxBody(new Vector3(-outerL * 0.5f + wall * 0.5f, 0, wall + 8), new Vector3(wall + 2, 16, 8), 1)
        ];

        foreach (Vector3 pos in CornerPositions(boardL * 0.42f, boardW * 0.38f, wall + standoffH * 0.5f))
            cuts.Add(new CylinderBody(pos, Vector3.UnitZ, screwRadius, standoffH + wall + 4));

        using Voxels solid = generation.Render(solids);
        using Voxels cut = generation.Render(cuts);
        using Voxels tray = solid - cut;
        generation.SaveStl(tray, "electronics_enclosure_base.stl");

        if (lidEnabled)
        {
            using Voxels lid = generation.RenderDifference(
                new BoxBody(new Vector3(0, 0, 2), new Vector3(outerL, outerW, 4), 2),
                [new BoxBody(new Vector3(0, 0, 1), new Vector3(outerL - wall * 2, outerW - wall * 2, 3), 1)]);
            generation.SaveStl(lid, "electronics_enclosure_lid.stl");
        }
    }

    static IEnumerable<Vector3> CornerPositions(float x, float y, float z)
    {
        yield return new Vector3(-x, -y, z);
        yield return new Vector3(x, -y, z);
        yield return new Vector3(-x, y, z);
        yield return new Vector3(x, y, z);
    }
}

public sealed class LatticeCouponGenerator : WorkbenchGeneratorBase
{
    public override GeneratorManifest Manifest { get; } = new()
    {
        Id = "lattice-coupon",
        Name = "Lattice Coupon",
        Description = "Compression coupon with internal lattice beams for comparative testing.",
        Category = "Test Coupons",
        Difficulty = "Beginner",
        RequiredMeasurements = ["Coupon envelope dimensions"],
        OutputTypes = ManifestFactory.StlOutputs(),
        Parameters =
        [
            ManifestFactory.Number("lengthMm", "Length", "mm", 60, 20, 160, 1, "Coupon length."),
            ManifestFactory.Number("widthMm", "Width", "mm", 30, 10, 120, 1, "Coupon width."),
            ManifestFactory.Number("heightMm", "Height", "mm", 20, 5, 100, 1, "Coupon height."),
            ManifestFactory.Number("ribRadiusMm", "Rib radius", "mm", 1.8, 0.5, 8, 0.1, "Lattice rib radius."),
            ManifestFactory.Integer("cellCountX", "Cells X", 4, 1, 12, 1, "Cell count along length."),
            ManifestFactory.Integer("cellCountY", "Cells Y", 3, 1, 12, 1, "Cell count along width."),
            ManifestFactory.Select("patternType", "Pattern", "cross", ["cross", "diagonal"], "Lattice beam pattern.")
        ],
        Presets = [new GeneratorPreset { Name = "Small compression coupon", ParameterValues = [] }]
    };

    protected override void GenerateGeometry(PicoGeneration generation)
    {
        float length = (float)generation.Values.GetDouble("lengthMm");
        float width = (float)generation.Values.GetDouble("widthMm");
        float height = (float)generation.Values.GetDouble("heightMm");
        float radius = (float)generation.Values.GetDouble("ribRadiusMm");
        int cellsX = generation.Values.GetInt("cellCountX");
        int cellsY = generation.Values.GetInt("cellCountY");
        string pattern = generation.Values.GetString("patternType");

        List<ISdfBody> solids =
        [
            new BoxBody(new Vector3(0, 0, radius), new Vector3(length, width, radius * 2), 1),
            new BoxBody(new Vector3(0, 0, height - radius), new Vector3(length, width, radius * 2), 1)
        ];

        float dx = length / cellsX;
        float dy = width / cellsY;

        for (int ix = 0; ix < cellsX; ix++)
        {
            for (int iy = 0; iy < cellsY; iy++)
            {
                float x0 = -length * 0.5f + ix * dx;
                float x1 = x0 + dx;
                float y0 = -width * 0.5f + iy * dy;
                float y1 = y0 + dy;

                solids.Add(new CapsuleBody(new Vector3(x0, y0, radius), new Vector3(x1, y1, height - radius), radius));

                if (pattern.Equals("cross", StringComparison.OrdinalIgnoreCase))
                    solids.Add(new CapsuleBody(new Vector3(x0, y1, radius), new Vector3(x1, y0, height - radius), radius));
            }
        }

        using Voxels coupon = generation.Render(solids);
        generation.SaveStl(coupon, "lattice_coupon.stl");
    }
}
