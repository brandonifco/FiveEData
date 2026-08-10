using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.Metamagic.Serialization;

internal sealed class MetamagicOptionDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int? FixedSorceryPointCost { get; init; }

    [JsonRequired]
    public bool CostEqualsSpellLevelWithCantripMinimum { get; init; }

    [JsonRequired]
    public bool ProtectsCreatureCountUpToSpellcastingModifier { get; init; }

    [JsonRequired]
    public bool DoublesRange { get; init; }

    [JsonRequired]
    public int? TouchRangeBecomesFeet { get; init; }

    [JsonRequired]
    public bool RerollsDiceCountUpToSpellcastingModifier { get; init; }

    [JsonRequired]
    public int? DoublesDurationMaxHours { get; init; }

    [JsonRequired]
    public bool GrantsDisadvantageOnFirstSavingThrow { get; init; }

    [JsonRequired]
    public bool ChangesCastingTimeToBonusAction { get; init; }

    [JsonRequired]
    public bool RemovesVerbalAndSomaticComponents { get; init; }

    [JsonRequired]
    public bool TargetsSecondCreatureInRange { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
