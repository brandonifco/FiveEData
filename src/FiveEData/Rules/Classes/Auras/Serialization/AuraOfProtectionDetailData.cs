using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.Auras.Serialization;

internal sealed class AuraOfProtectionDetailData
{
    [JsonRequired]
    public AuraRangeData? Range { get; init; }

    [JsonRequired]
    public bool RequiresConsciousness { get; init; }

    [JsonRequired]
    public int SavingThrowBonusMinimum { get; init; }
}
