using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.DeathStrike.Serialization;

internal sealed class DeathStrikeDetailData
{
    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public int DamageMultiplierOnFailedSave { get; init; }

    [JsonRequired]
    public bool RequiresSurprisedTarget { get; init; }
}
