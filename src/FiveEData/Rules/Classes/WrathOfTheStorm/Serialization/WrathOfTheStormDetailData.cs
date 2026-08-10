using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.WrathOfTheStorm.Serialization;

internal sealed class WrathOfTheStormDetailData
{
    [JsonRequired]
    public int TriggerRangeFeet { get; init; }

    [JsonRequired]
    public DiceExpressionData? Damage { get; init; }

    [JsonRequired]
    public string[]? ChoosableDamageTypeIds { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public bool HalfDamageOnSuccessfulSave { get; init; }

    [JsonRequired]
    public AbilityModifierUsesGrantData? UsesPerRest { get; init; }
}
