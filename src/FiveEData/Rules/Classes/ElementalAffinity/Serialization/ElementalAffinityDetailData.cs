using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.ElementalAffinity.Serialization;

internal sealed class ElementalAffinityDetailData
{
    [JsonRequired]
    public bool AddsSpellcastingModifierToDamage { get; init; }

    [JsonRequired]
    public int ResistanceSorceryPointCost { get; init; }

    [JsonRequired]
    public int ResistanceDurationHours { get; init; }
}
