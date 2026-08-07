using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.UnarmoredMovement.Serialization;

internal sealed class UnarmoredMovementProgressionDetailData
{
    [JsonRequired]
    public UnarmoredMovementSpeedBonusGrantData[]? SpeedBonusByLevel
    {
        get;
        init;
    }

    [JsonRequired]
    public bool RequiresNotWearingArmor { get; init; }

    [JsonRequired]
    public bool RequiresNotWieldingShield { get; init; }
}

internal sealed class UnarmoredMovementSpeedBonusGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int SpeedBonusFeet { get; init; }
}
