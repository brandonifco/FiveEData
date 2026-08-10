using System.Text.Json.Serialization;

namespace FiveEData.Rules.Common.Serialization;

internal sealed class AbilityModifierUsesGrantData
{
    [JsonRequired]
    public string? AbilityId { get; init; }

    [JsonRequired]
    public bool RecoversOnLongRest { get; init; }
}
