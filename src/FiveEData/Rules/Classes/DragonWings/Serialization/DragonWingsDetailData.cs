using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.DragonWings.Serialization;

internal sealed class DragonWingsDetailData
{
    [JsonRequired]
    public bool GrantsFlyingSpeedEqualToCurrentSpeed { get; init; }

    [JsonRequired]
    public bool RequiresBonusActionToCreateOrDismiss { get; init; }
}
