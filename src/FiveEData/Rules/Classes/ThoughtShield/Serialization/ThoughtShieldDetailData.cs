using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.ThoughtShield.Serialization;

internal sealed class ThoughtShieldDetailData
{
    [JsonRequired]
    public bool BlocksTelepathicReading { get; init; }

    [JsonRequired]
    public string? ResistedDamageTypeId { get; init; }

    [JsonRequired]
    public bool ReflectsDamageToAttacker { get; init; }
}
