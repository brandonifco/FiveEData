using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Spells.Concentration.Serialization;

internal sealed class ConcentrationRulesData
{
    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public int MinimumSavingThrowDC { get; init; }

    [JsonRequired]
    public int DamageDivisorForSavingThrowDC { get; init; }

    [JsonRequired]
    public bool SeparateSavingThrowPerDamageSource { get; init; }

    [JsonRequired]
    public bool EndsWhenCastingAnotherConcentrationSpell { get; init; }

    [JsonRequired]
    public bool EndsOnDeath { get; init; }

    [JsonRequired]
    public bool CanBeEndedVoluntarilyWithoutAnAction { get; init; }

    [JsonRequired]
    public string[]? EndedByConditionIds { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
