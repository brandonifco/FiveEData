using System.Text.Json.Serialization;

namespace FiveEData.Rules.Common.Serialization;

internal sealed class MoneyData
{
    [JsonRequired]
    public long CopperPieces { get; init; }
}
