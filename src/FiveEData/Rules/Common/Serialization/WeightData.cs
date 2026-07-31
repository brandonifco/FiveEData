using System.Text.Json.Serialization;

namespace FiveEData.Rules.Common.Serialization;

internal sealed class WeightData
{
    [JsonRequired]
    public decimal Pounds { get; init; }
}
