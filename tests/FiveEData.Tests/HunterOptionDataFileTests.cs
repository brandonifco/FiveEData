using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.HunterOptions;
using FiveEData.Rules.Classes.HunterOptions.Serialization;
using FiveEData.Rules.Common;

namespace FiveEData.Tests;

public sealed class HunterOptionDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactOptionClosure()
    {
        Assert.Equal(
            [
                "dnd5e2014.hunter-option.colossus-slayer",
                "dnd5e2014.hunter-option.escape-the-horde",
                "dnd5e2014.hunter-option.evasion",
                "dnd5e2014.hunter-option.giant-killer",
                "dnd5e2014.hunter-option.horde-breaker",
                "dnd5e2014.hunter-option.multiattack-defense",
                "dnd5e2014.hunter-option.stand-against-the-tide",
                "dnd5e2014.hunter-option.steel-will",
                "dnd5e2014.hunter-option.uncanny-dodge",
                "dnd5e2014.hunter-option.volley",
                "dnd5e2014.hunter-option.whirlwind-attack"
            ],
            LoadCanonical()
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Theory]
    [InlineData("dnd5e2014.hunter-option.colossus-slayer", 3)]
    [InlineData("dnd5e2014.hunter-option.giant-killer", 3)]
    [InlineData("dnd5e2014.hunter-option.horde-breaker", 3)]
    [InlineData("dnd5e2014.hunter-option.escape-the-horde", 7)]
    [InlineData("dnd5e2014.hunter-option.multiattack-defense", 7)]
    [InlineData("dnd5e2014.hunter-option.steel-will", 7)]
    [InlineData("dnd5e2014.hunter-option.volley", 11)]
    [InlineData("dnd5e2014.hunter-option.whirlwind-attack", 11)]
    [InlineData("dnd5e2014.hunter-option.evasion", 15)]
    [InlineData("dnd5e2014.hunter-option.stand-against-the-tide", 15)]
    [InlineData("dnd5e2014.hunter-option.uncanny-dodge", 15)]
    public void Option_HasExpectedRequiredLevel(string id, int expectedLevel)
    {
        Assert.Equal(expectedLevel, Get(id).RequiredLevel);
    }

    [Fact]
    public void EachChoicePoint_OffersItsExpectedNumberOfOptions()
    {
        Dictionary<int, int> countsByLevel = LoadCanonical()
            .GroupBy(definition => definition.RequiredLevel)
            .ToDictionary(group => group.Key, group => group.Count());

        Assert.Equal(
            new Dictionary<int, int>
            {
                [3] = 3,
                [7] = 3,
                [11] = 2,
                [15] = 3
            },
            countsByLevel);
    }

    [Fact]
    public void ColossusSlayer_DealsOncePerTurnDamageToWoundedTargets()
    {
        HunterOptionDefinition definition =
            Get("dnd5e2014.hunter-option.colossus-slayer");

        Assert.Equal(new DiceExpression(1, 8), definition.ExtraDamage);
        Assert.True(definition.OncePerTurn);
        Assert.True(definition.RequiresTargetBelowHitPointMaximum);
    }

    [Fact]
    public void GiantKiller_GatesOnAMinimumTargetSize()
    {
        HunterOptionDefinition definition =
            Get("dnd5e2014.hunter-option.giant-killer");

        Assert.Equal(
            "dnd5e2014.creature-size.large",
            definition.MinimumTargetSizeId?.Value);
    }

    [Fact]
    public void HordeBreaker_AttacksASecondTargetWithinFiveFeet()
    {
        HunterOptionDefinition definition =
            Get("dnd5e2014.hunter-option.horde-breaker");

        Assert.True(definition.GrantsExtraAttackAgainstDifferentTarget);
        Assert.Equal(5, definition.SecondaryTargetRangeFeet);
        Assert.True(definition.OncePerTurn);
    }

    [Fact]
    public void MultiattackDefense_GrantsAFourPointArmorClassBonus()
    {
        Assert.Equal(
            4,
            Get("dnd5e2014.hunter-option.multiattack-defense")
                .ArmorClassBonusAgainstSubsequentAttacks);
    }

    [Fact]
    public void SteelWill_ReferencesTheFrightenedCondition()
    {
        Assert.Equal(
            "dnd5e2014.condition.frightened",
            Get("dnd5e2014.hunter-option.steel-will")
                .GrantsAdvantageOnSavingThrowsAgainstConditionId?.Value);
    }

    [Theory]
    [InlineData(
        "dnd5e2014.hunter-option.volley",
        10,
        HunterMultiattackKind.Ranged)]
    [InlineData(
        "dnd5e2014.hunter-option.whirlwind-attack",
        5,
        HunterMultiattackKind.Melee)]
    public void MultiattackOption_HasExpectedRangeAndKind(
        string id,
        int expectedRangeFeet,
        HunterMultiattackKind expectedKind)
    {
        HunterOptionDefinition definition = Get(id);

        Assert.Equal(
            expectedRangeFeet,
            definition.AttacksAnyNumberOfCreaturesWithinFeet);
        Assert.Equal(expectedKind, definition.MultiattackKind);
    }

    [Fact]
    public void Evasion_NegatesOnSuccessAndHalvesOnFailure()
    {
        HunterOptionDefinition definition =
            Get("dnd5e2014.hunter-option.evasion");

        Assert.Equal(
            "dnd5e2014.ability.dexterity",
            definition.SavingThrowAbilityId?.Value);
        Assert.True(definition.NegatesDamageOnSuccessfulSave);
        Assert.True(definition.HalfDamageOnFailedSave);
    }

    [Fact]
    public void UncannyDodge_HalvesIncomingDamageAsAReaction()
    {
        Assert.True(
            Get("dnd5e2014.hunter-option.uncanny-dodge")
                .HalvesAttackDamageAsReaction);
    }

    [Fact]
    public void StandAgainstTheTide_IsEnumeratedWithNoMechanismFields()
    {
        HunterOptionDefinition definition =
            Get("dnd5e2014.hunter-option.stand-against-the-tide");

        Assert.Null(definition.ExtraDamage);
        Assert.False(definition.OncePerTurn);
        Assert.False(definition.RequiresTargetBelowHitPointMaximum);
        Assert.Null(definition.MinimumTargetSizeId);
        Assert.False(definition.GrantsExtraAttackAgainstDifferentTarget);
        Assert.Null(definition.SecondaryTargetRangeFeet);
        Assert.False(
            definition.ImposesDisadvantageOnOpportunityAttacksAgainstYou);
        Assert.Null(definition.ArmorClassBonusAgainstSubsequentAttacks);
        Assert.Null(definition.GrantsAdvantageOnSavingThrowsAgainstConditionId);
        Assert.Null(definition.AttacksAnyNumberOfCreaturesWithinFeet);
        Assert.Null(definition.MultiattackKind);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.False(definition.NegatesDamageOnSuccessfulSave);
        Assert.False(definition.HalfDamageOnFailedSave);
        Assert.False(definition.HalvesAttackDamageAsReaction);
    }

    [Fact]
    public void AllOptions_CitePhbFirstPrintingPageNinetyThree()
    {
        foreach (HunterOptionDefinition definition in LoadCanonical())
        {
            var source = Assert.Single(definition.Sources);
            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(93, source.Page);
            Assert.Equal("Chapter 3: Classes", source.Section);
        }
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedResourceMatchingTheDataFile()
    {
        HunterOptionCatalog catalog = Dnd5e2014Ruleset.Instance.HunterOptions;

        Assert.Equal(
            LoadCanonical()
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal),
            catalog.All.Select(definition => definition.Id.Value));
    }

    private static HunterOptionDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<HunterOptionDefinition> LoadCanonical()
    {
        return HunterOptionDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "hunter-options.json"));
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
