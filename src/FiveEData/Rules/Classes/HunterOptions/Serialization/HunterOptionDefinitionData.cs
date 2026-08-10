using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.HunterOptions.Serialization;

internal sealed class HunterOptionDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int RequiredLevel { get; init; }

    [JsonRequired]
    public DiceExpressionData? ExtraDamage { get; init; }

    [JsonRequired]
    public bool OncePerTurn { get; init; }

    [JsonRequired]
    public bool RequiresTargetBelowHitPointMaximum { get; init; }

    [JsonRequired]
    public string? MinimumTargetSizeId { get; init; }

    [JsonRequired]
    public bool GrantsExtraAttackAgainstDifferentTarget { get; init; }

    [JsonRequired]
    public int? SecondaryTargetRangeFeet { get; init; }

    [JsonRequired]
    public bool ImposesDisadvantageOnOpportunityAttacksAgainstYou
    {
        get;
        init;
    }

    [JsonRequired]
    public int? ArmorClassBonusAgainstSubsequentAttacks { get; init; }

    [JsonRequired]
    public string? GrantsAdvantageOnSavingThrowsAgainstConditionId
    {
        get;
        init;
    }

    [JsonRequired]
    public int? AttacksAnyNumberOfCreaturesWithinFeet { get; init; }

    [JsonRequired]
    public HunterMultiattackKind? MultiattackKind { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public bool NegatesDamageOnSuccessfulSave { get; init; }

    [JsonRequired]
    public bool HalfDamageOnFailedSave { get; init; }

    [JsonRequired]
    public bool HalvesAttackDamageAsReaction { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
