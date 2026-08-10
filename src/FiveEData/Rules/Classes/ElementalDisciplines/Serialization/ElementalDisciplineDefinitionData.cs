using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.ElementalDisciplines.Serialization;

internal sealed class ElementalDisciplineDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int? KiPointCost { get; init; }

    [JsonRequired]
    public int? RequiredMinimumLevel { get; init; }

    [JsonRequired]
    public string? GrantedSpellId { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public DiceExpressionData? BaseDamage { get; init; }

    [JsonRequired]
    public string? BaseDamageTypeId { get; init; }

    [JsonRequired]
    public bool HalfDamageOnSuccessfulSave { get; init; }

    [JsonRequired]
    public int? RangeFeet { get; init; }

    [JsonRequired]
    public int? PushDistanceFeet { get; init; }

    [JsonRequired]
    public string? ImposedConditionId { get; init; }

    [JsonRequired]
    public int? ReachIncreaseFeet { get; init; }

    [JsonRequired]
    public string? ChangesUnarmedDamageTypeId { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
