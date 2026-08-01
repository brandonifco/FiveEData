using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.MountSupport;
using FiveEData.Rules.Equipment.MountSupport.Serialization;

namespace FiveEData.Tests;

public sealed class MountSupportDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactlyEightFixedMarketplaceEntries()
    {
        IReadOnlyList<MountSupportDefinition> definitions = LoadCanonical();

        Assert.Equal(8, definitions.Count);
        Assert.Equal(
            8,
            definitions.Select(definition => definition.Id).Distinct().Count());
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingFixedTableEntries()
    {
        IReadOnlyDictionary<MountSupportId, MountSupportDefinition> actual =
            LoadCanonical().ToDictionary(definition => definition.Id);

        foreach (ExpectedMountSupportRow expected in Expected)
        {
            MountSupportDefinition definition =
                actual[new MountSupportId(expected.Id)];

            Assert.Equal(expected.Name, definition.Name);
            Assert.Equal(expected.CopperPieces, definition.Cost.CopperPieces);
            Assert.Equal(
                expected.ListedWeightPounds,
                definition.ListedWeight?.Pounds);
            Assert.Empty(definition.SpecialRuleIds);

            SourceReference source = Assert.Single(definition.Sources);
            Assert.Equal(157, source.Page);
            Assert.Equal(
                "Tack, Harness, and Drawn Vehicles",
                source.Section);
        }
    }

    [Fact]
    public void CanonicalFile_ExcludesDerivedBardingAndDrawnVehicles()
    {
        string[] names = LoadCanonical()
            .Select(definition => definition.Name)
            .ToArray();

        Assert.DoesNotContain("Barding", names);
        Assert.DoesNotContain("Carriage", names);
        Assert.DoesNotContain("Cart", names);
        Assert.DoesNotContain("Chariot", names);
        Assert.DoesNotContain("Sled", names);
        Assert.DoesNotContain("Wagon", names);

        MountSupportDefinition stabling = LoadCanonical().Single(
            definition =>
                definition.Id.Value ==
                "dnd5e2014.mount-support.stabling-per-day");

        Assert.Null(stabling.ListedWeight);
    }

    private static IReadOnlyList<MountSupportDefinition> LoadCanonical()
    {
        return MountSupportDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "mount-support.json"));
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

    private sealed record ExpectedMountSupportRow(
        string Id,
        string Name,
        long CopperPieces,
        decimal? ListedWeightPounds);

    private static readonly ExpectedMountSupportRow[] Expected =
    [
        new(
            "dnd5e2014.mount-support.bit-and-bridle",
            "Bit and bridle",
            200L,
            1m),
        new(
            "dnd5e2014.mount-support.feed-per-day",
            "Feed (per day)",
            5L,
            10m),
        new(
            "dnd5e2014.mount-support.saddle-exotic",
            "Saddle, exotic",
            6000L,
            40m),
        new(
            "dnd5e2014.mount-support.saddle-military",
            "Saddle, military",
            2000L,
            30m),
        new(
            "dnd5e2014.mount-support.saddle-pack",
            "Saddle, pack",
            500L,
            15m),
        new(
            "dnd5e2014.mount-support.saddle-riding",
            "Saddle, riding",
            1000L,
            25m),
        new(
            "dnd5e2014.mount-support.saddlebags",
            "Saddlebags",
            400L,
            8m),
        new(
            "dnd5e2014.mount-support.stabling-per-day",
            "Stabling (per day)",
            50L,
            null)
    ];
}
