using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.Auras.Serialization;

internal sealed class AuraOfCourageDetailData
{
    [JsonRequired]
    public AuraRangeData? Range { get; init; }

    [JsonRequired]
    public bool RequiresConsciousness { get; init; }
}
