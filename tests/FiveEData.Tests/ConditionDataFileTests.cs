using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.Conditions.Serialization;

namespace FiveEData.Tests;

public sealed class ConditionDataFileTests
{
    private const string ExpectedSection =
        "Appendix A: Conditions";

    private static readonly ExpectedCondition[] Expected =
    [
        Condition("blinded", "Blinded"),
        Condition("charmed", "Charmed"),
        Condition("deafened", "Deafened"),
        Condition("exhaustion", "Exhaustion"),
        Condition("frightened", "Frightened"),
        Condition("grappled", "Grappled"),
        Condition("incapacitated", "Incapacitated"),
        Condition("invisible", "Invisible"),
        Condition("paralyzed", "Paralyzed"),
        Condition("petrified", "Petrified"),
        Condition("poisoned", "Poisoned"),
        Condition("prone", "Prone"),
        Condition("restrained", "Restrained"),
        Condition("stunned", "Stunned"),
        Condition("unconscious", "Unconscious")
    ];

    [Fact]
    public void CanonicalFile_ContainsExactConditionClosure()
    {
        IReadOnlyList<ConditionDefinition> definitions =
            LoadCanonical();

        Assert.Equal(15, definitions.Count);
        Assert.Equal(
            15,
            definitions
                .Select(definition => definition.Id)
                .Distinct()
                .Count());

        Assert.Equal(
            Expected
                .Select(expected => expected.Id)
                .OrderBy(
                    id => id,
                    StringComparer.Ordinal),
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(
                    id => id,
                    StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingConditions()
    {
        IReadOnlyDictionary<
            ConditionId,
            ConditionDefinition> actual =
                LoadCanonical()
                    .ToDictionary(
                        definition => definition.Id);

        foreach (
            ExpectedCondition expected
            in Expected)
        {
            ConditionDefinition definition =
                actual[new ConditionId(expected.Id)];

            Assert.Equal(
                expected.Name,
                definition.Name);

            SourceReference source =
                Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(290, source.Page);
            Assert.Equal(
                ExpectedSection,
                source.Section);
        }
    }

    [Fact]
    public void Blinded_FailsSightChecksAndSwapsAttackRollAdvantage()
    {
        ConditionDefinition definition = Get("blinded");

        Assert.True(definition.AutomaticallyFailsAbilityChecksRequiringSight);
        Assert.Equal(
            RollModifier.Advantage,
            definition.AttackRollsAgainstTheCreature);
        Assert.Equal(
            RollModifier.Disadvantage,
            definition.TheCreaturesOwnAttackRolls);
    }

    [Fact]
    public void Charmed_RestrictsAttackingTheSourceAndAidsItsSocialChecks()
    {
        ConditionDefinition definition = Get("charmed");

        Assert.True(definition.CannotAttackOrTargetSourceWithHarmfulEffects);
        Assert.True(
            definition
                .SourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature);
    }

    [Fact]
    public void Deafened_FailsHearingChecks()
    {
        ConditionDefinition definition = Get("deafened");

        Assert.True(
            definition.AutomaticallyFailsAbilityChecksRequiringHearing);
    }

    [Fact]
    public void Exhaustion_HasSixCumulativeLevelsAndRecoversOnLongRest()
    {
        ConditionDefinition definition = Get("exhaustion");

        ExhaustionEffectDetail exhaustionEffect =
            definition.ExhaustionEffect
            ?? throw new InvalidOperationException(
                "Expected Exhaustion to have an exhaustion effect.");

        Assert.Equal(
            [
                ExhaustionLevelEffect.DisadvantageOnAbilityChecks,
                ExhaustionLevelEffect.SpeedHalved,
                ExhaustionLevelEffect
                    .DisadvantageOnAttackRollsAndSavingThrows,
                ExhaustionLevelEffect.HitPointMaximumHalved,
                ExhaustionLevelEffect.SpeedReducedToZero,
                ExhaustionLevelEffect.Death
            ],
            exhaustionEffect.LevelEffects);
        Assert.True(exhaustionEffect.RecoversOneLevelPerLongRest);
        Assert.True(exhaustionEffect.RecoveryRequiresFoodAndDrink);
    }

    [Fact]
    public void Frightened_AppliesDisadvantageOnlyWhileSourceIsInSight()
    {
        ConditionDefinition definition = Get("frightened");

        Assert.True(definition.OwnAbilityChecksHaveDisadvantage);
        Assert.Equal(
            RollModifier.Disadvantage,
            definition.TheCreaturesOwnAttackRolls);
        Assert.True(definition.RequiresSourceInLineOfSightForRollEffects);
        Assert.True(definition.CannotWillinglyMoveCloserToSource);
    }

    [Fact]
    public void Grappled_ZeroesSpeedAndEndsIfTheGrapplerIsIncapacitated()
    {
        ConditionDefinition definition = Get("grappled");

        Assert.True(definition.SpeedBecomesZero);
        Assert.True(definition.IgnoresBonusesToSpeed);
        Assert.True(definition.EndsIfSourceCreatureIsIncapacitated);
    }

    [Fact]
    public void Incapacitated_PreventsActionsAndReactions()
    {
        ConditionDefinition definition = Get("incapacitated");

        Assert.True(definition.PreventsActionsAndReactions);
    }

    [Fact]
    public void Invisible_SwapsAttackRollAdvantageAndHidesTheCreature()
    {
        ConditionDefinition definition = Get("invisible");

        Assert.Equal(
            RollModifier.Disadvantage,
            definition.AttackRollsAgainstTheCreature);
        Assert.Equal(
            RollModifier.Advantage,
            definition.TheCreaturesOwnAttackRolls);
        Assert.True(definition.HeavilyObscuredForHidingPurposes);
    }

    [Fact]
    public void Paralyzed_IncludesIncapacitatedAndAutoCritsWithinFiveFeet()
    {
        ConditionDefinition definition = Get("paralyzed");

        Assert.True(definition.PreventsActionsAndReactions);
        Assert.True(definition.PreventsMovement);
        Assert.Equal(SpeechRestriction.CannotSpeak, definition.SpeechRestriction);
        Assert.True(
            definition
                .AutomaticallyFailsStrengthAndDexteritySavingThrows);
        Assert.Equal(
            RollModifier.Advantage,
            definition.AttackRollsAgainstTheCreature);
        Assert.True(
            definition.AnyHitIsACriticalHitIfAttackerIsWithinFiveFeet);
    }

    [Fact]
    public void Petrified_ResistsAllDamageAndTensTheCreaturesWeight()
    {
        ConditionDefinition definition = Get("petrified");

        Assert.True(definition.PreventsActionsAndReactions);
        Assert.True(definition.PreventsMovement);
        Assert.Equal(SpeechRestriction.CannotSpeak, definition.SpeechRestriction);
        Assert.True(definition.UnawareOfSurroundings);
        Assert.Equal(
            RollModifier.Advantage,
            definition.AttackRollsAgainstTheCreature);
        Assert.True(
            definition
                .AutomaticallyFailsStrengthAndDexteritySavingThrows);
        Assert.True(definition.ResistantToAllDamage);
        Assert.True(definition.ImmuneToPoisonAndDisease);
        Assert.Equal(10, definition.WeightMultiplier);
    }

    [Fact]
    public void Poisoned_HasUnconditionalDisadvantageOnAttacksAndChecks()
    {
        ConditionDefinition definition = Get("poisoned");

        Assert.Equal(
            RollModifier.Disadvantage,
            definition.TheCreaturesOwnAttackRolls);
        Assert.True(definition.OwnAbilityChecksHaveDisadvantage);
        Assert.False(definition.RequiresSourceInLineOfSightForRollEffects);
    }

    [Fact]
    public void Prone_OnlyCrawlsAndHasFlatAttackDisadvantage()
    {
        ConditionDefinition definition = Get("prone");

        Assert.True(definition.OnlyMovementOptionIsToCrawl);
        Assert.Equal(
            RollModifier.Disadvantage,
            definition.TheCreaturesOwnAttackRolls);

        // Attackers' advantage/disadvantage against a prone target
        // depends on their distance (advantage within 5 feet, otherwise
        // disadvantage) — a compound, range-conditional rule this
        // project declines to flatten, same as every other DM-adjudicated
        // conditional. It stays in the citation.
        Assert.Equal(
            RollModifier.None,
            definition.AttackRollsAgainstTheCreature);
    }

    [Fact]
    public void Restrained_ZeroesSpeedAndSwapsAttackRollAdvantage()
    {
        ConditionDefinition definition = Get("restrained");

        Assert.True(definition.SpeedBecomesZero);
        Assert.True(definition.IgnoresBonusesToSpeed);
        Assert.Equal(
            RollModifier.Advantage,
            definition.AttackRollsAgainstTheCreature);
        Assert.Equal(
            RollModifier.Disadvantage,
            definition.TheCreaturesOwnAttackRolls);
        Assert.True(definition.DexteritySavingThrowsHaveDisadvantage);
    }

    [Fact]
    public void Stunned_IncludesIncapacitatedAndSpeaksOnlyFalteringly()
    {
        ConditionDefinition definition = Get("stunned");

        Assert.True(definition.PreventsActionsAndReactions);
        Assert.True(definition.PreventsMovement);
        Assert.Equal(
            SpeechRestriction.CanOnlySpeakFalteringly,
            definition.SpeechRestriction);
        Assert.True(
            definition
                .AutomaticallyFailsStrengthAndDexteritySavingThrows);
        Assert.Equal(
            RollModifier.Advantage,
            definition.AttackRollsAgainstTheCreature);
    }

    [Fact]
    public void Unconscious_DropsItemsFallsProneAndAutoCritsWithinFiveFeet()
    {
        ConditionDefinition definition = Get("unconscious");

        Assert.True(definition.PreventsActionsAndReactions);
        Assert.True(definition.PreventsMovement);
        Assert.Equal(SpeechRestriction.CannotSpeak, definition.SpeechRestriction);
        Assert.True(definition.UnawareOfSurroundings);
        Assert.True(definition.DropsHeldItemsAndFallsProne);
        Assert.True(
            definition
                .AutomaticallyFailsStrengthAndDexteritySavingThrows);
        Assert.Equal(
            RollModifier.Advantage,
            definition.AttackRollsAgainstTheCreature);
        Assert.True(
            definition.AnyHitIsACriticalHitIfAttackerIsWithinFiveFeet);
    }

    private static ConditionDefinition Get(string idSuffix)
    {
        return LoadCanonical()
            .Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.condition." + idSuffix);
    }

    private static IReadOnlyList<ConditionDefinition>
        LoadCanonical()
    {
        return ConditionDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "conditions.json"));
    }

    private static ExpectedCondition Condition(
        string suffix,
        string name)
    {
        return new ExpectedCondition(
            "dnd5e2014.condition." + suffix,
            name);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(
                        directory.FullName,
                        "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }

    private sealed record ExpectedCondition(
        string Id,
        string Name);
}
