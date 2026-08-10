using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.BattleMasterManeuvers.Serialization;

internal sealed class BattleMasterManeuverDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public BattleMasterManeuverEffectTarget EffectTarget { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public string? ImposedConditionId { get; init; }

    [JsonRequired]
    public string? MaximumTargetSizeId { get; init; }

    [JsonRequired]
    public int? PushDistanceFeet { get; init; }

    [JsonRequired]
    public int? ReachIncreaseFeet { get; init; }

    [JsonRequired]
    public int? SecondaryTargetRangeFeet { get; init; }

    [JsonRequired]
    public bool ForcesDroppedItem { get; init; }

    [JsonRequired]
    public bool GrantsAdvantageOnNextAttackRoll { get; init; }

    [JsonRequired]
    public bool GrantsAdvantageToNextAttackAgainstTarget { get; init; }

    [JsonRequired]
    public bool ImposesDisadvantageOnAttacksAgainstOthers { get; init; }

    [JsonRequired]
    public bool AllowsAllyReactionMovement { get; init; }

    [JsonRequired]
    public BattleMasterManeuverDurationTrigger?
        SecondaryEffectDurationTrigger
    { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
