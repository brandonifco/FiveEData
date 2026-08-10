using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.PotentCantrip.Serialization;

internal sealed class PotentCantripDetailData
{
    [JsonRequired]
    public bool GrantsHalfDamageOnSuccessfulSave { get; init; }

    [JsonRequired]
    public bool NegatesAdditionalCantripEffectsOnSuccessfulSave { get; init; }
}
