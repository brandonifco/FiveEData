using FiveEData.Rules.Equipment.Mounts;
using FiveEData.Rules.Equipment.Mounts.Serialization;

namespace FiveEData.Tests;

public sealed class MountDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactlyEightPurchasableMountListings()
    {
        IReadOnlyList<MountDefinition> definitions = LoadCanonical();

        Assert.Equal(8, definitions.Count);
        Assert.Equal(8, definitions.Select(item => item.Id).Distinct().Count());
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingMountsTable()
    {
        IReadOnlyDictionary<MountId, MountDefinition> actual =
            LoadCanonical().ToDictionary(definition => definition.Id);

        foreach (ExpectedMountRow expected in Expected)
        {
            MountDefinition definition = actual[new MountId(expected.Id)];

            Assert.Equal(expected.Name, definition.Name);
            Assert.Equal(expected.CopperPieces, definition.Cost.CopperPieces);
            Assert.Equal(expected.SpeedFeet, definition.Speed.Feet);
            Assert.Equal(
                expected.BaseCarryingCapacityPounds,
                definition.BaseCarryingCapacity.Pounds);
            Assert.Empty(definition.SpecialRuleIds);

            var source = Assert.Single(definition.Sources);
            Assert.Equal(155, source.Page);
            Assert.Equal(
                "Chapter 5: Equipment — Mounts and Vehicles",
                source.Section);
        }
    }

    private static IReadOnlyList<MountDefinition> LoadCanonical()
    {
        return MountDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "mounts.json"));
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

    private sealed record ExpectedMountRow(
        string Id,
        string Name,
        long CopperPieces,
        int SpeedFeet,
        decimal BaseCarryingCapacityPounds);

    private static readonly ExpectedMountRow[] Expected =
    [
        new("dnd5e2014.mount.camel", "Camel", 5000L, 50, 480m),
        new("dnd5e2014.mount.donkey-or-mule", "Donkey or mule", 800L, 40, 420m),
        new("dnd5e2014.mount.elephant", "Elephant", 20000L, 40, 1320m),
        new("dnd5e2014.mount.horse-draft", "Horse, draft", 5000L, 40, 540m),
        new("dnd5e2014.mount.horse-riding", "Horse, riding", 7500L, 60, 480m),
        new("dnd5e2014.mount.mastiff", "Mastiff", 2500L, 40, 195m),
        new("dnd5e2014.mount.pony", "Pony", 3000L, 40, 225m),
        new("dnd5e2014.mount.warhorse", "Warhorse", 40000L, 60, 540m)
    ];
}
