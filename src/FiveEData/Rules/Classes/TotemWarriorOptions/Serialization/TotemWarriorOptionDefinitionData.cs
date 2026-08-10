using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.TotemWarriorOptions.Serialization;

internal sealed class TotemWarriorOptionDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int RequiredLevel { get; init; }

    [JsonRequired]
    public bool RequiresRaging { get; init; }

    [JsonRequired]
    public bool RequiresNotWearingHeavyArmor { get; init; }

    [JsonRequired]
    public string? ResistsAllDamageExceptTypeId { get; init; }

    [JsonRequired]
    public bool ImposesDisadvantageOnOpportunityAttacksAgainstYou
    {
        get;
        init;
    }

    [JsonRequired]
    public bool GrantsDashAsBonusAction { get; init; }

    [JsonRequired]
    public int? GrantsAlliesAdvantageOnMeleeAttacksWithinFeet { get; init; }

    [JsonRequired]
    public bool DoublesCarryingCapacity { get; init; }

    [JsonRequired]
    public bool GrantsAdvantageOnStrengthChecksToMoveObjects { get; init; }

    [JsonRequired]
    public int? ClearSightRangeFeet { get; init; }

    [JsonRequired]
    public int? ClearSightDetailEquivalentRangeFeet { get; init; }

    [JsonRequired]
    public bool IgnoresDimLightPerceptionDisadvantage { get; init; }

    [JsonRequired]
    public string? TracksAtTravelPaceId { get; init; }

    [JsonRequired]
    public string? MovesStealthilyAtTravelPaceId { get; init; }

    [JsonRequired]
    public int? ImposesDisadvantageOnAttacksAgainstOthersWithinFeet
    {
        get;
        init;
    }

    [JsonRequired]
    public bool GrantsFlyingSpeedEqualToWalkingSpeed { get; init; }

    [JsonRequired]
    public string? ImposedConditionId { get; init; }

    [JsonRequired]
    public string? MaximumTargetSizeId { get; init; }

    [JsonRequired]
    public bool ImposedConditionRequiresBonusAction { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
