using System.Text.Json.Serialization;

namespace FiveEData.Rules.Common.Serialization;

internal sealed class ListedWeightData
{
    [JsonRequired]
    public decimal Pounds { get; init; }

    [JsonRequired]
    public string? Qualifier { get; init; }
}
