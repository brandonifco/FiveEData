using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Creatures.Races.BreathWeapon.Serialization;

internal sealed class BreathWeaponProgressionDetailData
{
    [JsonRequired]
    public BreathWeaponDamageGrantData[]? DamageByLevel { get; init; }

    [JsonRequired]
    public bool RecoversOnShortRest { get; init; }
}

internal sealed class BreathWeaponDamageGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public DiceExpressionData? Damage { get; init; }
}
