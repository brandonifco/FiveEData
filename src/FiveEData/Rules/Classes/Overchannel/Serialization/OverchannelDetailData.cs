using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.Overchannel.Serialization;

internal sealed class OverchannelDetailData
{
    [JsonRequired]
    public int MaximumSpellLevel { get; init; }

    [JsonRequired]
    public bool DealsMaximumDamage { get; init; }

    [JsonRequired]
    public bool FirstUseHasNoAdverseEffect { get; init; }

    [JsonRequired]
    public DiceExpressionData? SelfDamagePerSpellLevel { get; init; }

    [JsonRequired]
    public string? SelfDamageTypeId { get; init; }

    [JsonRequired]
    public DiceExpressionData? SelfDamageIncreasePerSubsequentUse
    {
        get;
        init;
    }

    [JsonRequired]
    public bool IgnoresResistanceAndImmunity { get; init; }

    [JsonRequired]
    public bool RecoversOnLongRest { get; init; }
}
