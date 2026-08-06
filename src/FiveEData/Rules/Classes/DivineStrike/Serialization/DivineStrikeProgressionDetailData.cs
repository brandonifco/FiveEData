using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.DivineStrike.Serialization;

internal sealed class DivineStrikeProgressionDetailData
{
    [JsonRequired]
    public DivineStrikeDamageGrantData[]? DamageByLevel { get; init; }

    [JsonRequired]
    public string? FixedDamageTypeId { get; init; }

    [JsonRequired]
    public string[]? ChoosableDamageTypeIds { get; init; }

    [JsonRequired]
    public bool MatchesWeaponDamageType { get; init; }
}

internal sealed class DivineStrikeDamageGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public DiceExpressionData? Damage { get; init; }
}
