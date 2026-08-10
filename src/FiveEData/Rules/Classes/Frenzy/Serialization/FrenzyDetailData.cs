using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.Frenzy.Serialization;

internal sealed class FrenzyDetailData
{
    [JsonRequired]
    public bool GrantsBonusActionMeleeAttack { get; init; }

    [JsonRequired]
    public int ExhaustionLevelsWhenRageEnds { get; init; }
}
