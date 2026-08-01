using System.Text.Json.Serialization;

namespace FiveEData.Rules.Common.Serialization;

internal sealed class ListedCostData
{
    [JsonRequired]
    public MoneyData? Amount { get; init; }

    [JsonRequired]
    public ListedCostKind Kind { get; init; }
}
