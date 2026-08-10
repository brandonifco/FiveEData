using System.Text.Json.Serialization;
using FiveEData.Rules.Common;

namespace FiveEData.Rules.Classes.EntropicWard.Serialization;

internal sealed class EntropicWardDetailData
{
    [JsonRequired]
    public bool ImposesDisadvantageOnTriggeringAttackRoll { get; init; }

    [JsonRequired]
    public bool GrantsAdvantageOnNextAttackRollIfMissed { get; init; }

    [JsonRequired]
    public NextTurnDurationTrigger AdvantageDurationTrigger { get; init; }

    [JsonRequired]
    public bool RecoversOnShortRest { get; init; }
}
