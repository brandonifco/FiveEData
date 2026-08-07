using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.MartialArts.Serialization;

internal sealed class MartialArtsProgressionDetailData
{
    [JsonRequired]
    public MartialArtsDieGrantData[]? DieByLevel { get; init; }

    [JsonRequired]
    public bool CanUseDexterityForAttackAndDamage { get; init; }

    [JsonRequired]
    public bool GrantsBonusActionUnarmedStrike { get; init; }

    [JsonRequired]
    public bool RequiresNotWearingArmor { get; init; }

    [JsonRequired]
    public bool RequiresNotWieldingShield { get; init; }
}

internal sealed class MartialArtsDieGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public DiceExpressionData? Die { get; init; }
}
