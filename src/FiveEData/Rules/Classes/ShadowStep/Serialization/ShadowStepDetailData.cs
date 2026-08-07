using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.ShadowStep.Serialization;

internal sealed class ShadowStepDetailData
{
    [JsonRequired]
    public int TeleportRangeFeet { get; init; }

    [JsonRequired]
    public bool GrantsAdvantageOnNextMeleeAttack { get; init; }
}
