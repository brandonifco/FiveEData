using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.SecondStoryWork.Serialization;

internal sealed class SecondStoryWorkDetailData
{
    [JsonRequired]
    public bool ClimbingCostsNoExtraMovement { get; init; }

    [JsonRequired]
    public bool AddsDexterityModifierToRunningJumpDistance { get; init; }
}
