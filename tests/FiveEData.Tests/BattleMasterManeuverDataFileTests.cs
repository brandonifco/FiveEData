using FiveEData.Rules.Classes.BattleMasterManeuvers;
using FiveEData.Rules.Classes.BattleMasterManeuvers.Serialization;
using FiveEData.Rules.Common;

namespace FiveEData.Tests;

public sealed class BattleMasterManeuverDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactManeuverClosure()
    {
        IReadOnlyList<BattleMasterManeuverDefinition> definitions =
            LoadCanonical();

        Assert.Equal(
            [
                "dnd5e2014.battle-master-maneuver.commanders-strike",
                "dnd5e2014.battle-master-maneuver.disarming-attack",
                "dnd5e2014.battle-master-maneuver.distracting-strike",
                "dnd5e2014.battle-master-maneuver.evasive-footwork",
                "dnd5e2014.battle-master-maneuver.feinting-attack",
                "dnd5e2014.battle-master-maneuver.goading-attack",
                "dnd5e2014.battle-master-maneuver.lunging-attack",
                "dnd5e2014.battle-master-maneuver.maneuvering-attack",
                "dnd5e2014.battle-master-maneuver.menacing-attack",
                "dnd5e2014.battle-master-maneuver.parry",
                "dnd5e2014.battle-master-maneuver.precision-attack",
                "dnd5e2014.battle-master-maneuver.pushing-attack",
                "dnd5e2014.battle-master-maneuver.rally",
                "dnd5e2014.battle-master-maneuver.riposte",
                "dnd5e2014.battle-master-maneuver.sweeping-attack",
                "dnd5e2014.battle-master-maneuver.trip-attack"
            ],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Theory]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.commanders-strike",
        "Commander's Strike",
        BattleMasterManeuverEffectTarget.DamageRoll,
        null)]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.disarming-attack",
        "Disarming Attack",
        BattleMasterManeuverEffectTarget.DamageRoll,
        "dnd5e2014.ability.strength")]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.distracting-strike",
        "Distracting Strike",
        BattleMasterManeuverEffectTarget.DamageRoll,
        null)]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.evasive-footwork",
        "Evasive Footwork",
        BattleMasterManeuverEffectTarget.ArmorClass,
        null)]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.feinting-attack",
        "Feinting Attack",
        BattleMasterManeuverEffectTarget.DamageRoll,
        null)]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.goading-attack",
        "Goading Attack",
        BattleMasterManeuverEffectTarget.DamageRoll,
        "dnd5e2014.ability.wisdom")]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.lunging-attack",
        "Lunging Attack",
        BattleMasterManeuverEffectTarget.DamageRoll,
        null)]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.maneuvering-attack",
        "Maneuvering Attack",
        BattleMasterManeuverEffectTarget.DamageRoll,
        null)]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.menacing-attack",
        "Menacing Attack",
        BattleMasterManeuverEffectTarget.DamageRoll,
        "dnd5e2014.ability.wisdom")]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.parry",
        "Parry",
        BattleMasterManeuverEffectTarget.DamageReduction,
        null)]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.precision-attack",
        "Precision Attack",
        BattleMasterManeuverEffectTarget.AttackRoll,
        null)]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.pushing-attack",
        "Pushing Attack",
        BattleMasterManeuverEffectTarget.DamageRoll,
        "dnd5e2014.ability.strength")]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.rally",
        "Rally",
        BattleMasterManeuverEffectTarget.TemporaryHitPoints,
        null)]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.riposte",
        "Riposte",
        BattleMasterManeuverEffectTarget.DamageRoll,
        null)]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.sweeping-attack",
        "Sweeping Attack",
        BattleMasterManeuverEffectTarget.SecondaryTargetDamage,
        null)]
    [InlineData(
        "dnd5e2014.battle-master-maneuver.trip-attack",
        "Trip Attack",
        BattleMasterManeuverEffectTarget.DamageRoll,
        "dnd5e2014.ability.strength")]
    public void Maneuver_HasExpectedNameEffectTargetAndSavingThrow(
        string id,
        string expectedName,
        BattleMasterManeuverEffectTarget expectedEffectTarget,
        string? expectedSavingThrowAbilityId)
    {
        BattleMasterManeuverDefinition definition = Get(id);

        Assert.Equal(expectedName, definition.Name);
        Assert.Equal(expectedEffectTarget, definition.EffectTarget);
        Assert.Equal(
            expectedSavingThrowAbilityId,
            definition.SavingThrowAbilityId?.Value);
    }

    [Fact]
    public void DisarmingAttack_ForcesDroppedItem()
    {
        BattleMasterManeuverDefinition definition =
            Get("dnd5e2014.battle-master-maneuver.disarming-attack");

        Assert.True(definition.ForcesDroppedItem);
    }

    [Fact]
    public void
        DistractingStrike_GrantsAdvantageToNextAttackAgainstTargetUntilStartOfNextTurn()
    {
        BattleMasterManeuverDefinition definition =
            Get("dnd5e2014.battle-master-maneuver.distracting-strike");

        Assert.True(definition.GrantsAdvantageToNextAttackAgainstTarget);
        Assert.Equal(
            NextTurnDurationTrigger.StartOfYourNextTurn,
            definition.SecondaryEffectDurationTrigger);
    }

    [Fact]
    public void FeintingAttack_GrantsAdvantageOnNextAttackRoll()
    {
        BattleMasterManeuverDefinition definition =
            Get("dnd5e2014.battle-master-maneuver.feinting-attack");

        Assert.True(definition.GrantsAdvantageOnNextAttackRoll);
    }

    [Fact]
    public void
        GoadingAttack_ImposesDisadvantageOnAttacksAgainstOthersUntilEndOfNextTurn()
    {
        BattleMasterManeuverDefinition definition =
            Get("dnd5e2014.battle-master-maneuver.goading-attack");

        Assert.True(definition.ImposesDisadvantageOnAttacksAgainstOthers);
        Assert.Equal(
            NextTurnDurationTrigger.EndOfYourNextTurn,
            definition.SecondaryEffectDurationTrigger);
    }

    [Fact]
    public void LungingAttack_IncreasesReachByFiveFeet()
    {
        BattleMasterManeuverDefinition definition =
            Get("dnd5e2014.battle-master-maneuver.lunging-attack");

        Assert.Equal(5, definition.ReachIncreaseFeet);
    }

    [Fact]
    public void ManeuveringAttack_AllowsAllyReactionMovement()
    {
        BattleMasterManeuverDefinition definition =
            Get("dnd5e2014.battle-master-maneuver.maneuvering-attack");

        Assert.True(definition.AllowsAllyReactionMovement);
    }

    [Fact]
    public void
        MenacingAttack_ImposesFrightenedUntilEndOfNextTurnOnFailedSave()
    {
        BattleMasterManeuverDefinition definition =
            Get("dnd5e2014.battle-master-maneuver.menacing-attack");

        Assert.Equal(
            "dnd5e2014.condition.frightened",
            definition.ImposedConditionId?.Value);
        Assert.Equal(
            NextTurnDurationTrigger.EndOfYourNextTurn,
            definition.SecondaryEffectDurationTrigger);
    }

    [Fact]
    public void
        PushingAttack_PushesFifteenFeetIfTargetIsLargeOrSmallerOnFailedSave()
    {
        BattleMasterManeuverDefinition definition =
            Get("dnd5e2014.battle-master-maneuver.pushing-attack");

        Assert.Equal(15, definition.PushDistanceFeet);
        Assert.Equal(
            "dnd5e2014.creature-size.large",
            definition.MaximumTargetSizeId?.Value);
    }

    [Fact]
    public void SweepingAttack_TargetsASecondCreatureWithinFiveFeet()
    {
        BattleMasterManeuverDefinition definition =
            Get("dnd5e2014.battle-master-maneuver.sweeping-attack");

        Assert.Equal(5, definition.SecondaryTargetRangeFeet);
    }

    [Fact]
    public void
        TripAttack_ImposesProneIfTargetIsLargeOrSmallerOnFailedSave()
    {
        BattleMasterManeuverDefinition definition =
            Get("dnd5e2014.battle-master-maneuver.trip-attack");

        Assert.Equal(
            "dnd5e2014.condition.prone",
            definition.ImposedConditionId?.Value);
        Assert.Equal(
            "dnd5e2014.creature-size.large",
            definition.MaximumTargetSizeId?.Value);
        Assert.Null(definition.SecondaryEffectDurationTrigger);
    }

    [Theory]
    [InlineData("dnd5e2014.battle-master-maneuver.commanders-strike")]
    [InlineData("dnd5e2014.battle-master-maneuver.evasive-footwork")]
    [InlineData("dnd5e2014.battle-master-maneuver.parry")]
    [InlineData("dnd5e2014.battle-master-maneuver.precision-attack")]
    [InlineData("dnd5e2014.battle-master-maneuver.rally")]
    [InlineData("dnd5e2014.battle-master-maneuver.riposte")]
    public void ActionEconomyOrAbilityModifierOnlyManeuvers_HaveNoNewMechanismFields(
        string id)
    {
        BattleMasterManeuverDefinition definition = Get(id);

        Assert.Null(definition.ImposedConditionId);
        Assert.Null(definition.MaximumTargetSizeId);
        Assert.Null(definition.PushDistanceFeet);
        Assert.Null(definition.ReachIncreaseFeet);
        Assert.Null(definition.SecondaryTargetRangeFeet);
        Assert.False(definition.ForcesDroppedItem);
        Assert.False(definition.GrantsAdvantageOnNextAttackRoll);
        Assert.False(definition.GrantsAdvantageToNextAttackAgainstTarget);
        Assert.False(definition.ImposesDisadvantageOnAttacksAgainstOthers);
        Assert.False(definition.AllowsAllyReactionMovement);
        Assert.Null(definition.SecondaryEffectDurationTrigger);
    }

    [Fact]
    public void AllManeuvers_CitePhbFirstPrintingPageSeventyFour()
    {
        foreach (BattleMasterManeuverDefinition definition in LoadCanonical())
        {
            var source = Assert.Single(definition.Sources);
            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(74, source.Page);
        }
    }

    private static BattleMasterManeuverDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<BattleMasterManeuverDefinition>
        LoadCanonical()
    {
        return BattleMasterManeuverDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "battle-master-maneuvers.json"));
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
