using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.BendLuck.Serialization;

internal sealed class BendLuckDetailData
{
    [JsonRequired]
    public int SorceryPointCost { get; init; }

    [JsonRequired]
    public DiceExpressionData? Die { get; init; }
}
