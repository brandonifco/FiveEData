using System.Text.Json.Serialization;

namespace FiveEData.Rules.Common.Serialization;

internal sealed class DiceExpressionData
{
    [JsonRequired]
    public int Count { get; init; }

    [JsonRequired]
    public int Sides { get; init; }
}
