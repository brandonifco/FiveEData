using System.Text.Json.Serialization;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Creatures.Conditions.Serialization;

internal sealed class ConditionDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public bool PreventsActionsAndReactions { get; init; }

    [JsonRequired]
    public bool PreventsMovement { get; init; }

    [JsonRequired]
    public bool OnlyMovementOptionIsToCrawl { get; init; }

    [JsonRequired]
    public bool SpeedBecomesZero { get; init; }

    [JsonRequired]
    public bool IgnoresBonusesToSpeed { get; init; }

    [JsonRequired]
    public SpeechRestriction SpeechRestriction { get; init; }

    [JsonRequired]
    public bool UnawareOfSurroundings { get; init; }

    [JsonRequired]
    public bool AutomaticallyFailsStrengthAndDexteritySavingThrows
    {
        get;
        init;
    }

    [JsonRequired]
    public bool DexteritySavingThrowsHaveDisadvantage { get; init; }

    [JsonRequired]
    public bool AutomaticallyFailsAbilityChecksRequiringSight
    {
        get;
        init;
    }

    [JsonRequired]
    public bool AutomaticallyFailsAbilityChecksRequiringHearing
    {
        get;
        init;
    }

    [JsonRequired]
    public bool OwnAbilityChecksHaveDisadvantage { get; init; }

    [JsonRequired]
    public RollModifier AttackRollsAgainstTheCreature { get; init; }

    [JsonRequired]
    public RollModifier TheCreaturesOwnAttackRolls { get; init; }

    [JsonRequired]
    public bool AnyHitIsACriticalHitIfAttackerIsWithinFiveFeet
    {
        get;
        init;
    }

    [JsonRequired]
    public bool RequiresSourceInLineOfSightForRollEffects { get; init; }

    [JsonRequired]
    public bool CannotWillinglyMoveCloserToSource { get; init; }

    [JsonRequired]
    public bool CannotAttackOrTargetSourceWithHarmfulEffects
    {
        get;
        init;
    }

    [JsonRequired]
    public bool SourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature
    {
        get;
        init;
    }

    [JsonRequired]
    public bool EndsIfSourceCreatureIsIncapacitated { get; init; }

    [JsonRequired]
    public bool ResistantToAllDamage { get; init; }

    [JsonRequired]
    public bool ImmuneToPoisonAndDisease { get; init; }

    [JsonRequired]
    public int? WeightMultiplier { get; init; }

    [JsonRequired]
    public bool DropsHeldItemsAndFallsProne { get; init; }

    [JsonRequired]
    public bool HeavilyObscuredForHidingPurposes { get; init; }

    [JsonRequired]
    public ExhaustionEffectDetailData? ExhaustionEffect { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}

internal sealed class ExhaustionEffectDetailData
{
    [JsonRequired]
    public ExhaustionLevelEffect[]? LevelEffects { get; init; }

    [JsonRequired]
    public bool RecoversOneLevelPerLongRest { get; init; }

    [JsonRequired]
    public bool RecoveryRequiresFoodAndDrink { get; init; }
}
