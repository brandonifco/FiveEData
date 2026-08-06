using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.SneakAttack.Serialization;

internal sealed class SneakAttackProgressionDetailData
{
    [JsonRequired]
    public SneakAttackDiceGrantData[]? DiceByLevel { get; init; }

    [JsonRequired]
    public bool OncePerTurn { get; init; }

    [JsonRequired]
    public bool RequiresFinesseOrRangedWeapon { get; init; }
}

internal sealed class SneakAttackDiceGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public DiceExpressionData? Damage { get; init; }
}
