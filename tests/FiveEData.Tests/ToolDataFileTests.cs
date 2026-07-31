using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Equipment.Tools.Serialization;

namespace FiveEData.Tests;

public sealed class ToolDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactlyThirtySevenPurchasableLeafTools()
    {
        IReadOnlyList<ToolDefinition> definitions = LoadCanonical();

        Assert.Equal(37, definitions.Count);
        Assert.Equal(37, definitions.Select(item => item.Id).Distinct().Count());
        Assert.DoesNotContain(
            definitions,
            definition => definition.Name.Contains(
                "Vehicles",
                StringComparison.Ordinal));
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingToolsTable()
    {
        IReadOnlyDictionary<ToolId, ToolDefinition> actual =
            LoadCanonical().ToDictionary(definition => definition.Id);

        foreach (ExpectedToolRow expected in Expected)
        {
            ToolDefinition definition = actual[new ToolId(expected.Id)];

            Assert.Equal(expected.Name, definition.Name);
            Assert.Equal(expected.CopperPieces, definition.Cost.CopperPieces);
            Assert.Equal(expected.Pounds, definition.Weight?.Pounds);
            Assert.Equal(expected.FamilyId, definition.FamilyId?.Value);

            var source = Assert.Single(definition.Sources);
            Assert.Equal(154, source.Page);
            Assert.Equal("Chapter 5: Equipment — Tools", source.Section);
        }
    }

    [Fact]
    public void CanonicalFile_AssociatesToolUseAndStandaloneDescriptionRules()
    {
        IReadOnlyList<ToolDefinition> definitions = LoadCanonical();

        var standaloneRules = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["dnd5e2014.tool.disguise-kit"] = "dnd5e2014.tool-rule.disguise-kit",
            ["dnd5e2014.tool.forgery-kit"] = "dnd5e2014.tool-rule.forgery-kit",
            ["dnd5e2014.tool.herbalism-kit"] = "dnd5e2014.tool-rule.herbalism-kit",
            ["dnd5e2014.tool.navigators-tools"] = "dnd5e2014.tool-rule.navigators-tools",
            ["dnd5e2014.tool.poisoners-kit"] = "dnd5e2014.tool-rule.poisoners-kit",
            ["dnd5e2014.tool.thieves-tools"] = "dnd5e2014.tool-rule.thieves-tools"
        };

        foreach (ToolDefinition definition in definitions)
        {
            Assert.Equal(
                "dnd5e2014.tool-rule.proficiency",
                definition.SpecialRuleIds[0].Value);

            if (standaloneRules.TryGetValue(definition.Id.Value, out string? specificRuleId))
            {
                Assert.Equal(2, definition.SpecialRuleIds.Count);
                Assert.Equal(specificRuleId, definition.SpecialRuleIds[1].Value);
            }
            else
            {
                Assert.Single(definition.SpecialRuleIds);
            }
        }
    }

    [Fact]
    public void CanonicalFile_PreservesSourceFamilyMembershipCounts()
    {
        IReadOnlyList<ToolDefinition> definitions = LoadCanonical();

        Assert.Equal(
            17,
            definitions.Count(definition =>
                definition.FamilyId?.Value ==
                "dnd5e2014.tool-family.artisans-tools"));
        Assert.Equal(
            4,
            definitions.Count(definition =>
                definition.FamilyId?.Value ==
                "dnd5e2014.tool-family.gaming-set"));
        Assert.Equal(
            10,
            definitions.Count(definition =>
                definition.FamilyId?.Value ==
                "dnd5e2014.tool-family.musical-instrument"));
        Assert.Equal(6, definitions.Count(definition => definition.FamilyId is null));
    }

    private static IReadOnlyList<ToolDefinition> LoadCanonical()
    {
        return ToolDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "tools.json"));
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }

    private sealed record ExpectedToolRow(
        string Id,
        string Name,
        long CopperPieces,
        decimal? Pounds,
        string? FamilyId);

    private static readonly ExpectedToolRow[] Expected =
    [
        new ExpectedToolRow("dnd5e2014.tool.alchemists-supplies", "Alchemist's supplies", 5000L, 8m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.brewers-supplies", "Brewer's supplies", 2000L, 9m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.calligraphers-supplies", "Calligrapher's supplies", 1000L, 5m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.carpenters-tools", "Carpenter's tools", 800L, 6m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.cartographers-tools", "Cartographer's tools", 1500L, 6m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.cobblers-tools", "Cobbler's tools", 500L, 5m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.cooks-utensils", "Cook's utensils", 100L, 8m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.glassblowers-tools", "Glassblower's tools", 3000L, 5m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.jewelers-tools", "Jeweler's tools", 2500L, 2m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.leatherworkers-tools", "Leatherworker's tools", 500L, 5m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.masons-tools", "Mason's tools", 1000L, 8m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.painters-supplies", "Painter's supplies", 1000L, 5m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.potters-tools", "Potter's tools", 1000L, 3m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.smiths-tools", "Smith's tools", 2000L, 8m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.tinkers-tools", "Tinker's tools", 5000L, 10m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.weavers-tools", "Weaver's tools", 100L, 5m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.woodcarvers-tools", "Woodcarver's tools", 100L, 5m, "dnd5e2014.tool-family.artisans-tools"),
        new ExpectedToolRow("dnd5e2014.tool.disguise-kit", "Disguise kit", 2500L, 3m, null),
        new ExpectedToolRow("dnd5e2014.tool.forgery-kit", "Forgery kit", 1500L, 5m, null),
        new ExpectedToolRow("dnd5e2014.tool.dice-set", "Dice set", 10L, null, "dnd5e2014.tool-family.gaming-set"),
        new ExpectedToolRow("dnd5e2014.tool.dragonchess-set", "Dragonchess set", 100L, 0.5m, "dnd5e2014.tool-family.gaming-set"),
        new ExpectedToolRow("dnd5e2014.tool.playing-card-set", "Playing card set", 50L, null, "dnd5e2014.tool-family.gaming-set"),
        new ExpectedToolRow("dnd5e2014.tool.three-dragon-ante-set", "Three-Dragon Ante set", 100L, null, "dnd5e2014.tool-family.gaming-set"),
        new ExpectedToolRow("dnd5e2014.tool.herbalism-kit", "Herbalism kit", 500L, 3m, null),
        new ExpectedToolRow("dnd5e2014.tool.bagpipes", "Bagpipes", 3000L, 6m, "dnd5e2014.tool-family.musical-instrument"),
        new ExpectedToolRow("dnd5e2014.tool.drum", "Drum", 600L, 3m, "dnd5e2014.tool-family.musical-instrument"),
        new ExpectedToolRow("dnd5e2014.tool.dulcimer", "Dulcimer", 2500L, 10m, "dnd5e2014.tool-family.musical-instrument"),
        new ExpectedToolRow("dnd5e2014.tool.flute", "Flute", 200L, 1m, "dnd5e2014.tool-family.musical-instrument"),
        new ExpectedToolRow("dnd5e2014.tool.lute", "Lute", 3500L, 2m, "dnd5e2014.tool-family.musical-instrument"),
        new ExpectedToolRow("dnd5e2014.tool.lyre", "Lyre", 3000L, 2m, "dnd5e2014.tool-family.musical-instrument"),
        new ExpectedToolRow("dnd5e2014.tool.horn", "Horn", 300L, 2m, "dnd5e2014.tool-family.musical-instrument"),
        new ExpectedToolRow("dnd5e2014.tool.pan-flute", "Pan flute", 1200L, 2m, "dnd5e2014.tool-family.musical-instrument"),
        new ExpectedToolRow("dnd5e2014.tool.shawm", "Shawm", 200L, 1m, "dnd5e2014.tool-family.musical-instrument"),
        new ExpectedToolRow("dnd5e2014.tool.viol", "Viol", 3000L, 1m, "dnd5e2014.tool-family.musical-instrument"),
        new ExpectedToolRow("dnd5e2014.tool.navigators-tools", "Navigator's tools", 2500L, 2m, null),
        new ExpectedToolRow("dnd5e2014.tool.poisoners-kit", "Poisoner's kit", 5000L, 2m, null),
        new ExpectedToolRow("dnd5e2014.tool.thieves-tools", "Thieves' tools", 2500L, 1m, null),
    ];
}
