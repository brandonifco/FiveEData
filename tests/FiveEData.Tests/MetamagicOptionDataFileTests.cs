using FiveEData.Rules.Classes.Metamagic;
using FiveEData.Rules.Classes.Metamagic.Serialization;

namespace FiveEData.Tests;

public sealed class MetamagicOptionDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactMetamagicOptionClosure()
    {
        IReadOnlyList<MetamagicOptionDefinition> definitions =
            LoadCanonical();

        Assert.Equal(
            [
                "dnd5e2014.metamagic-option.careful-spell",
                "dnd5e2014.metamagic-option.distant-spell",
                "dnd5e2014.metamagic-option.empowered-spell",
                "dnd5e2014.metamagic-option.extended-spell",
                "dnd5e2014.metamagic-option.heightened-spell",
                "dnd5e2014.metamagic-option.quickened-spell",
                "dnd5e2014.metamagic-option.subtle-spell",
                "dnd5e2014.metamagic-option.twinned-spell"
            ],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Theory]
    [InlineData("dnd5e2014.metamagic-option.careful-spell", "Careful Spell", 1)]
    [InlineData("dnd5e2014.metamagic-option.distant-spell", "Distant Spell", 1)]
    [InlineData(
        "dnd5e2014.metamagic-option.empowered-spell",
        "Empowered Spell",
        1)]
    [InlineData(
        "dnd5e2014.metamagic-option.extended-spell",
        "Extended Spell",
        1)]
    [InlineData(
        "dnd5e2014.metamagic-option.heightened-spell",
        "Heightened Spell",
        3)]
    [InlineData(
        "dnd5e2014.metamagic-option.quickened-spell",
        "Quickened Spell",
        2)]
    [InlineData("dnd5e2014.metamagic-option.subtle-spell", "Subtle Spell", 1)]
    public void FixedCostOption_HasExpectedNameAndCost(
        string id,
        string expectedName,
        int expectedCost)
    {
        MetamagicOptionDefinition definition = Get(id);

        Assert.Equal(expectedName, definition.Name);
        Assert.Equal(expectedCost, definition.FixedSorceryPointCost);
        Assert.False(definition.CostEqualsSpellLevelWithCantripMinimum);
    }

    [Fact]
    public void TwinnedSpell_CostsAnAmountEqualToTheSpellsLevel()
    {
        MetamagicOptionDefinition definition =
            Get("dnd5e2014.metamagic-option.twinned-spell");

        Assert.Equal("Twinned Spell", definition.Name);
        Assert.Null(definition.FixedSorceryPointCost);
        Assert.True(definition.CostEqualsSpellLevelWithCantripMinimum);
    }

    [Fact]
    public void AllOptions_CitePhbFirstPrintingPageOneOhTwo()
    {
        foreach (MetamagicOptionDefinition definition in LoadCanonical())
        {
            var source = Assert.Single(definition.Sources);
            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(102, source.Page);
        }
    }

    private static MetamagicOptionDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<MetamagicOptionDefinition> LoadCanonical()
    {
        return MetamagicOptionDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "metamagic-options.json"));
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
}
