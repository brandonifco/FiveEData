using System.Text.Json.Serialization;
using FiveEData.Rules.Common;

namespace FiveEData.Rules.Classes.FeyPresence.Serialization;

internal sealed class FeyPresenceDetailData
{
    [JsonRequired]
    public int AreaSizeFeet { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public string[]? ChoosableConditionIds { get; init; }

    [JsonRequired]
    public NextTurnDurationTrigger ConditionDurationTrigger { get; init; }

    [JsonRequired]
    public bool RecoversOnShortRest { get; init; }
}
