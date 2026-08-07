using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.HurlThroughHell.Serialization;

internal sealed class HurlThroughHellDetailData
{
    [JsonRequired]
    public DiceExpressionData? Damage { get; init; }

    [JsonRequired]
    public string? DamageTypeId { get; init; }

    [JsonRequired]
    public bool ExemptsFiends { get; init; }

    [JsonRequired]
    public bool RecoversOnLongRest { get; init; }
}
