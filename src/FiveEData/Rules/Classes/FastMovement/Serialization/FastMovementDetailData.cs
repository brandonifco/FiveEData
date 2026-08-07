using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.FastMovement.Serialization;

internal sealed class FastMovementDetailData
{
    [JsonRequired]
    public int SpeedBonusFeet { get; init; }

    [JsonRequired]
    public bool RequiresNotWearingHeavyArmor { get; init; }
}
