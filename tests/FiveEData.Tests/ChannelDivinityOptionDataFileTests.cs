using FiveEData.Rules.Classes.ChannelDivinityOptions;
using FiveEData.Rules.Classes.ChannelDivinityOptions.Serialization;

namespace FiveEData.Tests;

public sealed class ChannelDivinityOptionDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactOptionClosure()
    {
        IReadOnlyList<ChannelDivinityOptionDefinition> definitions =
            LoadCanonical();

        Assert.Equal(
            [
                "dnd5e2014.channel-divinity-option.charm-animals-and-plants",
                "dnd5e2014.channel-divinity-option.cloak-of-shadows",
                "dnd5e2014.channel-divinity-option.destructive-wrath",
                "dnd5e2014.channel-divinity-option.guided-strike",
                "dnd5e2014.channel-divinity-option.invoke-duplicity",
                "dnd5e2014.channel-divinity-option.knowledge-of-the-ages",
                "dnd5e2014.channel-divinity-option.preserve-life",
                "dnd5e2014.channel-divinity-option.radiance-of-the-dawn",
                "dnd5e2014.channel-divinity-option.read-thoughts",
                "dnd5e2014.channel-divinity-option.war-gods-blessing"
            ],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void KnowledgeOfTheAges_HasOnlyADurationFact()
    {
        ChannelDivinityOptionDefinition definition =
            Get("dnd5e2014.channel-divinity-option.knowledge-of-the-ages");

        Assert.Null(definition.RangeFeet);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Equal(10, definition.DurationMinutes);
        Assert.Null(definition.RollBonus);
    }

    [Fact]
    public void ReadThoughts_HasRangeSaveAndDuration()
    {
        ChannelDivinityOptionDefinition definition =
            Get("dnd5e2014.channel-divinity-option.read-thoughts");

        Assert.Equal(60, definition.RangeFeet);
        Assert.Equal(
            "dnd5e2014.ability.wisdom",
            definition.SavingThrowAbilityId?.Value);
        Assert.Equal(1, definition.DurationMinutes);
        Assert.Null(definition.RollBonus);
    }

    [Fact]
    public void PreserveLife_HasOnlyARangeFact()
    {
        ChannelDivinityOptionDefinition definition =
            Get("dnd5e2014.channel-divinity-option.preserve-life");

        Assert.Equal(30, definition.RangeFeet);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Null(definition.DurationMinutes);
        Assert.Null(definition.RollBonus);
    }

    [Fact]
    public void RadianceOfTheDawn_HasRangeAndConstitutionSave()
    {
        ChannelDivinityOptionDefinition definition =
            Get("dnd5e2014.channel-divinity-option.radiance-of-the-dawn");

        Assert.Equal(30, definition.RangeFeet);
        Assert.Equal(
            "dnd5e2014.ability.constitution",
            definition.SavingThrowAbilityId?.Value);
        Assert.Null(definition.DurationMinutes);
        Assert.Null(definition.RollBonus);
    }

    [Fact]
    public void CharmAnimalsAndPlants_HasRangeSaveAndDuration()
    {
        ChannelDivinityOptionDefinition definition = Get(
            "dnd5e2014.channel-divinity-option.charm-animals-and-plants");

        Assert.Equal(30, definition.RangeFeet);
        Assert.Equal(
            "dnd5e2014.ability.wisdom",
            definition.SavingThrowAbilityId?.Value);
        Assert.Equal(1, definition.DurationMinutes);
        Assert.Null(definition.RollBonus);
    }

    [Theory]
    [InlineData("dnd5e2014.channel-divinity-option.destructive-wrath")]
    [InlineData("dnd5e2014.channel-divinity-option.cloak-of-shadows")]
    public void OptionWithNoQuantizableFact_HasAllFactsNull(string id)
    {
        ChannelDivinityOptionDefinition definition = Get(id);

        Assert.Null(definition.RangeFeet);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Null(definition.DurationMinutes);
        Assert.Null(definition.RollBonus);
    }

    [Fact]
    public void InvokeDuplicity_HasRangeAndDuration()
    {
        ChannelDivinityOptionDefinition definition =
            Get("dnd5e2014.channel-divinity-option.invoke-duplicity");

        Assert.Equal(30, definition.RangeFeet);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Equal(1, definition.DurationMinutes);
        Assert.Null(definition.RollBonus);
    }

    [Fact]
    public void GuidedStrike_HasOnlyARollBonus()
    {
        ChannelDivinityOptionDefinition definition =
            Get("dnd5e2014.channel-divinity-option.guided-strike");

        Assert.Null(definition.RangeFeet);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Null(definition.DurationMinutes);
        Assert.Equal(10, definition.RollBonus);
    }

    [Fact]
    public void WarGodsBlessing_HasRangeAndRollBonus()
    {
        ChannelDivinityOptionDefinition definition =
            Get("dnd5e2014.channel-divinity-option.war-gods-blessing");

        Assert.Equal(30, definition.RangeFeet);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Null(definition.DurationMinutes);
        Assert.Equal(10, definition.RollBonus);
    }

    [Fact]
    public void AllOptions_CitePhbFirstPrintingPageFiftyNineThroughSixtyThree()
    {
        foreach (
            ChannelDivinityOptionDefinition definition in LoadCanonical())
        {
            var source = Assert.Single(definition.Sources);
            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.True(
                source.Page is >= 59 and <= 63,
                $"{definition.Id} cited page {source.Page}, expected " +
                "59 through 63.");
        }
    }

    private static ChannelDivinityOptionDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<ChannelDivinityOptionDefinition>
        LoadCanonical()
    {
        return ChannelDivinityOptionDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "channel-divinity-options.json"));
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
