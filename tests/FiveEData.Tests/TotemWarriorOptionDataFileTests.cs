using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.TotemWarriorOptions;
using FiveEData.Rules.Classes.TotemWarriorOptions.Serialization;

namespace FiveEData.Tests;

public sealed class TotemWarriorOptionDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactOptionClosure()
    {
        IReadOnlyList<TotemWarriorOptionDefinition> definitions =
            LoadCanonical();

        Assert.Equal(
            [
                "dnd5e2014.totem-warrior-option.aspect-of-the-beast-bear",
                "dnd5e2014.totem-warrior-option.aspect-of-the-beast-eagle",
                "dnd5e2014.totem-warrior-option.aspect-of-the-beast-wolf",
                "dnd5e2014.totem-warrior-option.totem-spirit-bear",
                "dnd5e2014.totem-warrior-option.totem-spirit-eagle",
                "dnd5e2014.totem-warrior-option.totem-spirit-wolf",
                "dnd5e2014.totem-warrior-option.totemic-attunement-bear",
                "dnd5e2014.totem-warrior-option.totemic-attunement-eagle",
                "dnd5e2014.totem-warrior-option.totemic-attunement-wolf"
            ],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Theory]
    [InlineData("dnd5e2014.totem-warrior-option.totem-spirit-bear", 3)]
    [InlineData("dnd5e2014.totem-warrior-option.totem-spirit-eagle", 3)]
    [InlineData("dnd5e2014.totem-warrior-option.totem-spirit-wolf", 3)]
    [InlineData(
        "dnd5e2014.totem-warrior-option.aspect-of-the-beast-bear",
        6)]
    [InlineData(
        "dnd5e2014.totem-warrior-option.aspect-of-the-beast-eagle",
        6)]
    [InlineData(
        "dnd5e2014.totem-warrior-option.aspect-of-the-beast-wolf",
        6)]
    [InlineData("dnd5e2014.totem-warrior-option.totemic-attunement-bear", 14)]
    [InlineData("dnd5e2014.totem-warrior-option.totemic-attunement-eagle", 14)]
    [InlineData("dnd5e2014.totem-warrior-option.totemic-attunement-wolf", 14)]
    public void Option_HasExpectedRequiredLevel(string id, int expectedLevel)
    {
        Assert.Equal(expectedLevel, Get(id).RequiredLevel);
    }

    [Fact]
    public void TotemSpiritBear_ResistsEveryDamageTypeExceptPsychic()
    {
        TotemWarriorOptionDefinition definition =
            Get("dnd5e2014.totem-warrior-option.totem-spirit-bear");

        Assert.True(definition.RequiresRaging);
        Assert.Equal(
            "dnd5e2014.damage-type.psychic",
            definition.ResistsAllDamageExceptTypeId?.Value);
    }

    [Fact]
    public void TotemSpiritEagle_IsTheOnlyOptionGatedOnHeavyArmor()
    {
        TotemWarriorOptionDefinition definition =
            Get("dnd5e2014.totem-warrior-option.totem-spirit-eagle");

        Assert.True(definition.RequiresNotWearingHeavyArmor);
        Assert.True(
            definition.ImposesDisadvantageOnOpportunityAttacksAgainstYou);
        Assert.True(definition.GrantsDashAsBonusAction);

        Assert.All(
            LoadCanonical()
                .Where(other => other.Id != definition.Id),
            other => Assert.False(other.RequiresNotWearingHeavyArmor));
    }

    [Fact]
    public void TotemSpiritWolf_GrantsAlliesAdvantageWithinFiveFeet()
    {
        TotemWarriorOptionDefinition definition =
            Get("dnd5e2014.totem-warrior-option.totem-spirit-wolf");

        Assert.Equal(
            5,
            definition.GrantsAlliesAdvantageOnMeleeAttacksWithinFeet);
    }

    [Fact]
    public void AspectOfTheBeastBear_DoublesCarryingCapacity()
    {
        TotemWarriorOptionDefinition definition =
            Get("dnd5e2014.totem-warrior-option.aspect-of-the-beast-bear");

        Assert.True(definition.DoublesCarryingCapacity);
        Assert.True(definition.GrantsAdvantageOnStrengthChecksToMoveObjects);
    }

    [Fact]
    public void AspectOfTheBeastEagle_StoresOneMileCanonicalizedToFeet()
    {
        TotemWarriorOptionDefinition definition =
            Get("dnd5e2014.totem-warrior-option.aspect-of-the-beast-eagle");

        Assert.Equal(5280, definition.ClearSightRangeFeet);
        Assert.Equal(100, definition.ClearSightDetailEquivalentRangeFeet);
        Assert.True(definition.IgnoresDimLightPerceptionDisadvantage);
    }

    [Fact]
    public void AspectOfTheBeastWolf_ReferencesTwoDifferentTravelPaces()
    {
        TotemWarriorOptionDefinition definition =
            Get("dnd5e2014.totem-warrior-option.aspect-of-the-beast-wolf");

        Assert.Equal(
            "dnd5e2014.travel-pace.fast",
            definition.TracksAtTravelPaceId?.Value);
        Assert.Equal(
            "dnd5e2014.travel-pace.normal",
            definition.MovesStealthilyAtTravelPaceId?.Value);
    }

    [Fact]
    public void TotemicAttunementBear_ImposesDisadvantageWithinFiveFeet()
    {
        TotemWarriorOptionDefinition definition =
            Get("dnd5e2014.totem-warrior-option.totemic-attunement-bear");

        Assert.Equal(
            5,
            definition.ImposesDisadvantageOnAttacksAgainstOthersWithinFeet);
    }

    [Fact]
    public void TotemicAttunementEagle_GrantsFlightMatchingWalkingSpeed()
    {
        TotemWarriorOptionDefinition definition =
            Get("dnd5e2014.totem-warrior-option.totemic-attunement-eagle");

        Assert.True(definition.GrantsFlyingSpeedEqualToWalkingSpeed);
    }

    [Fact]
    public void TotemicAttunementWolf_KnocksLargeOrSmallerTargetsProne()
    {
        TotemWarriorOptionDefinition definition =
            Get("dnd5e2014.totem-warrior-option.totemic-attunement-wolf");

        Assert.Equal(
            "dnd5e2014.condition.prone",
            definition.ImposedConditionId?.Value);
        Assert.Equal(
            "dnd5e2014.creature-size.large",
            definition.MaximumTargetSizeId?.Value);
        Assert.True(definition.ImposedConditionRequiresBonusAction);
    }

    [Fact]
    public void AspectOfTheBeastOptions_AreTheOnlyOptionsNotRequiringRage()
    {
        Assert.All(
            LoadCanonical(),
            definition => Assert.Equal(
                definition.RequiredLevel != 6,
                definition.RequiresRaging));
    }

    [Fact]
    public void AllOptions_CitePhbFirstPrintingPageFifty()
    {
        foreach (TotemWarriorOptionDefinition definition in LoadCanonical())
        {
            var source = Assert.Single(definition.Sources);
            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(50, source.Page);
            Assert.Equal("Chapter 3: Classes", source.Section);
        }
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedResourceMatchingTheDataFile()
    {
        TotemWarriorOptionCatalog catalog =
            Dnd5e2014Ruleset.Instance.TotemWarriorOptions;

        Assert.Equal(
            LoadCanonical()
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal),
            catalog.All.Select(definition => definition.Id.Value));
    }

    private static TotemWarriorOptionDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<TotemWarriorOptionDefinition> LoadCanonical()
    {
        return TotemWarriorOptionDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "totem-warrior-options.json"));
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
