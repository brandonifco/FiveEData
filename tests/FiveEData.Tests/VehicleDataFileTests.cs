using FiveEData.Rules.Common;
using FiveEData.Rules.Equipment.Vehicles;
using FiveEData.Rules.Equipment.Vehicles.Serialization;

namespace FiveEData.Tests;

public sealed class VehicleDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactlyElevenVehicleListings()
    {
        IReadOnlyList<VehicleDefinition> definitions = LoadCanonical();

        Assert.Equal(11, definitions.Count);
        Assert.Equal(11, definitions.Select(item => item.Id).Distinct().Count());
        Assert.Equal(
            5,
            definitions.Count(item => item.Kind == VehicleKind.Land));
        Assert.Equal(
            6,
            definitions.Count(item => item.Kind == VehicleKind.Water));
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingVehicleTables()
    {
        IReadOnlyDictionary<VehicleId, VehicleDefinition> actual =
            LoadCanonical().ToDictionary(definition => definition.Id);

        foreach (ExpectedVehicleRow expected in Expected)
        {
            VehicleDefinition definition =
                actual[new VehicleId(expected.Id)];

            Assert.Equal(expected.Name, definition.Name);
            Assert.Equal(expected.Kind, definition.Kind);
            Assert.Equal(expected.CopperPieces, definition.Cost.CopperPieces);
            Assert.Equal(
                expected.ListedWeightPounds,
                definition.ListedWeight?.Pounds);
            Assert.Equal(
                expected.ListedSpeedMilesPerHour,
                definition.ListedSpeed?.MilesPerHour);
            var expectedRuleIds = new List<RuleId>();

            if (definition.Kind == VehicleKind.Land)
            {
                expectedRuleIds.Add(
                    new RuleId(
                        "dnd5e2014.mount-vehicle-rule.drawn-vehicle-pulling-capacity"));
            }

            expectedRuleIds.Add(
                new RuleId(
                    "dnd5e2014.mount-vehicle-rule.vehicle-proficiency"));

            if (definition.Id.Value is
                "dnd5e2014.vehicle.keelboat" or
                "dnd5e2014.vehicle.rowboat")
            {
                expectedRuleIds.Add(
                    new RuleId(
                        "dnd5e2014.mount-vehicle-rule.rowed-vessels"));
            }

            Assert.Equal(
                expectedRuleIds,
                definition.SpecialRuleIds);

            var source = Assert.Single(definition.Sources);
            Assert.Equal(157, source.Page);
            Assert.Equal(expected.Section, source.Section);
        }
    }

    [Fact]
    public void CanonicalFile_PreservesSourceSpecificTableColumns()
    {
        IReadOnlyList<VehicleDefinition> definitions = LoadCanonical();

        Assert.All(
            definitions.Where(item => item.Kind == VehicleKind.Land),
            item =>
            {
                Assert.NotNull(item.ListedWeight);
                Assert.Null(item.ListedSpeed);
            });

        Assert.All(
            definitions.Where(item => item.Kind == VehicleKind.Water),
            item =>
            {
                Assert.Null(item.ListedWeight);
                Assert.NotNull(item.ListedSpeed);
            });
    }

    private static IReadOnlyList<VehicleDefinition> LoadCanonical()
    {
        return VehicleDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "vehicles.json"));
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(directory.FullName, "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }

    private sealed record ExpectedVehicleRow(
        string Id,
        string Name,
        VehicleKind Kind,
        long CopperPieces,
        decimal? ListedWeightPounds,
        decimal? ListedSpeedMilesPerHour,
        string Section);

    private static readonly ExpectedVehicleRow[] Expected =
    [
        new(
            "dnd5e2014.vehicle.carriage",
            "Carriage",
            VehicleKind.Land,
            10000L,
            600m,
            null,
            "Tack, Harness, and Drawn Vehicles"),
        new(
            "dnd5e2014.vehicle.cart",
            "Cart",
            VehicleKind.Land,
            1500L,
            200m,
            null,
            "Tack, Harness, and Drawn Vehicles"),
        new(
            "dnd5e2014.vehicle.chariot",
            "Chariot",
            VehicleKind.Land,
            25000L,
            100m,
            null,
            "Tack, Harness, and Drawn Vehicles"),
        new(
            "dnd5e2014.vehicle.sled",
            "Sled",
            VehicleKind.Land,
            2000L,
            300m,
            null,
            "Tack, Harness, and Drawn Vehicles"),
        new(
            "dnd5e2014.vehicle.wagon",
            "Wagon",
            VehicleKind.Land,
            3500L,
            400m,
            null,
            "Tack, Harness, and Drawn Vehicles"),
        new(
            "dnd5e2014.vehicle.galley",
            "Galley",
            VehicleKind.Water,
            3000000L,
            null,
            4m,
            "Waterborne Vehicles"),
        new(
            "dnd5e2014.vehicle.keelboat",
            "Keelboat",
            VehicleKind.Water,
            300000L,
            null,
            1m,
            "Waterborne Vehicles"),
        new(
            "dnd5e2014.vehicle.longship",
            "Longship",
            VehicleKind.Water,
            1000000L,
            null,
            3m,
            "Waterborne Vehicles"),
        new(
            "dnd5e2014.vehicle.rowboat",
            "Rowboat",
            VehicleKind.Water,
            5000L,
            null,
            1.5m,
            "Waterborne Vehicles"),
        new(
            "dnd5e2014.vehicle.sailing-ship",
            "Sailing ship",
            VehicleKind.Water,
            1000000L,
            null,
            2m,
            "Waterborne Vehicles"),
        new(
            "dnd5e2014.vehicle.warship",
            "Warship",
            VehicleKind.Water,
            2500000L,
            null,
            2.5m,
            "Waterborne Vehicles")
    ];
}
