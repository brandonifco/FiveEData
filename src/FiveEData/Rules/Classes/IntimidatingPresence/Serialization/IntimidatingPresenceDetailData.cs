using System.Text.Json.Serialization;
using FiveEData.Rules.Common;

namespace FiveEData.Rules.Classes.IntimidatingPresence.Serialization;

internal sealed class IntimidatingPresenceDetailData
{
    [JsonRequired]
    public int RangeFeet { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public string? ImposedConditionId { get; init; }

    [JsonRequired]
    public NextTurnDurationTrigger ConditionDurationTrigger { get; init; }
}
