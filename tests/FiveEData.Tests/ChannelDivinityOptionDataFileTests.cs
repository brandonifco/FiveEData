using FiveEData.Rules.Classes.ChannelDivinityOptions;
using FiveEData.Rules.Classes.ChannelDivinityOptions.Serialization;
using FiveEData.Rules.Common;

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
                "dnd5e2014.channel-divinity-option.abjure-enemy",
                "dnd5e2014.channel-divinity-option.charm-animals-and-plants",
                "dnd5e2014.channel-divinity-option.cloak-of-shadows",
                "dnd5e2014.channel-divinity-option.destructive-wrath",
                "dnd5e2014.channel-divinity-option.guided-strike",
                "dnd5e2014.channel-divinity-option.invoke-duplicity",
                "dnd5e2014.channel-divinity-option.knowledge-of-the-ages",
                "dnd5e2014.channel-divinity-option.natures-wrath",
                "dnd5e2014.channel-divinity-option.preserve-life",
                "dnd5e2014.channel-divinity-option.radiance-of-the-dawn",
                "dnd5e2014.channel-divinity-option.read-thoughts",
                "dnd5e2014.channel-divinity-option.sacred-weapon",
                "dnd5e2014.channel-divinity-option.turn-the-faithless",
                "dnd5e2014.channel-divinity-option.turn-the-unholy",
                "dnd5e2014.channel-divinity-option.vow-of-enmity",
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

    [Fact]
    public void DestructiveWrath_MaximizesDamageRollAndHasNoOtherFact()
    {
        ChannelDivinityOptionDefinition definition =
            Get("dnd5e2014.channel-divinity-option.destructive-wrath");

        Assert.Null(definition.RangeFeet);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Null(definition.DurationMinutes);
        Assert.Null(definition.RollBonus);
        Assert.True(definition.MaximizesDamageRoll);
    }

    [Fact]
    public void
        CloakOfShadows_GrantsInvisibilityUntilEndOfYourNextTurn()
    {
        ChannelDivinityOptionDefinition definition =
            Get("dnd5e2014.channel-divinity-option.cloak-of-shadows");

        Assert.Null(definition.RangeFeet);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Null(definition.DurationMinutes);
        Assert.Null(definition.RollBonus);
        Assert.Equal(
            "dnd5e2014.condition.invisible",
            definition.ImposedConditionId?.Value);
        Assert.Equal(
            NextTurnDurationTrigger.EndOfYourNextTurn,
            definition.ConditionDurationTrigger);
    }

    [Fact]
    public void CharmAnimalsAndPlants_ImposesCharmedOnFailedSave()
    {
        ChannelDivinityOptionDefinition definition = Get(
            "dnd5e2014.channel-divinity-option.charm-animals-and-plants");

        Assert.Equal(
            "dnd5e2014.condition.charmed",
            definition.ImposedConditionId?.Value);
        Assert.Null(definition.ConditionDurationTrigger);
    }

    [Fact]
    public void
        ReadThoughts_GrantsSuggestionThatAutomaticallyFailsItsSave()
    {
        ChannelDivinityOptionDefinition definition =
            Get("dnd5e2014.channel-divinity-option.read-thoughts");

        Assert.Equal(
            "dnd5e2014.spell.suggestion",
            definition.GrantedSpellId?.Value);
        Assert.True(definition.AutomaticallyFailsGrantedSpellSave);
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
    public void SacredWeapon_AddsSpellcastingModifierToAttackRollsAndEmitsLight()
    {
        ChannelDivinityOptionDefinition definition =
            Get("dnd5e2014.channel-divinity-option.sacred-weapon");

        Assert.Equal(1, definition.DurationMinutes);
        Assert.True(definition.AddsSpellcastingModifierToAttackRolls);
        Assert.Equal(20, definition.BrightLightRadiusFeet);
        Assert.Equal(20, definition.DimLightRadiusFeet);
    }

    [Theory]
    [InlineData("dnd5e2014.channel-divinity-option.turn-the-unholy")]
    [InlineData("dnd5e2014.channel-divinity-option.turn-the-faithless")]
    public void TurnOption_HasThirtyFootRangeWisdomSaveAndOneMinuteDuration(
        string id)
    {
        ChannelDivinityOptionDefinition definition = Get(id);

        Assert.Equal(30, definition.RangeFeet);
        Assert.Equal(
            "dnd5e2014.ability.wisdom",
            definition.SavingThrowAbilityId?.Value);
        Assert.Equal(1, definition.DurationMinutes);
    }

    [Fact]
    public void NaturesWrath_RestrainsWithAChoosableStrengthOrDexteritySave()
    {
        ChannelDivinityOptionDefinition definition =
            Get("dnd5e2014.channel-divinity-option.natures-wrath");

        Assert.Equal(10, definition.RangeFeet);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Equal(
            [
                "dnd5e2014.ability.strength",
                "dnd5e2014.ability.dexterity"
            ],
            definition.ChoosableSavingThrowAbilityIds
                .Select(abilityId => abilityId.Value)
                .ToArray());
        Assert.Equal(
            "dnd5e2014.condition.restrained",
            definition.ImposedConditionId?.Value);
    }

    [Fact]
    public void AbjureEnemy_FrightensOnFailedWisdomSaveWithinSixtyFeet()
    {
        ChannelDivinityOptionDefinition definition =
            Get("dnd5e2014.channel-divinity-option.abjure-enemy");

        Assert.Equal(60, definition.RangeFeet);
        Assert.Equal(
            "dnd5e2014.ability.wisdom",
            definition.SavingThrowAbilityId?.Value);
        Assert.Equal(
            "dnd5e2014.condition.frightened",
            definition.ImposedConditionId?.Value);
        Assert.Equal(1, definition.DurationMinutes);
    }

    [Fact]
    public void VowOfEnmity_GrantsAdvantageWithinTenFeetForOneMinute()
    {
        ChannelDivinityOptionDefinition definition =
            Get("dnd5e2014.channel-divinity-option.vow-of-enmity");

        Assert.Equal(10, definition.RangeFeet);
        Assert.Equal(1, definition.DurationMinutes);
        Assert.True(definition.GrantsAdvantageOnAttackRollsAgainstTarget);
    }

    [Fact]
    public void AllOptions_CitePhbFirstPrintingPageFiftyNineThroughEightyEight()
    {
        foreach (
            ChannelDivinityOptionDefinition definition in LoadCanonical())
        {
            var source = Assert.Single(definition.Sources);
            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.True(
                source.Page is (>= 59 and <= 63) or (>= 86 and <= 88),
                $"{definition.Id} cited page {source.Page}, expected " +
                "59 through 63 or 86 through 88.");
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
