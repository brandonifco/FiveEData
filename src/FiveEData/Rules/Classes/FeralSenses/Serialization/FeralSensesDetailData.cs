using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.FeralSenses.Serialization;

internal sealed class FeralSensesDetailData
{
    [JsonRequired]
    public int RangeFeet { get; init; }

    [JsonRequired]
    public bool NegatesUnseenAttackDisadvantage { get; init; }
}
