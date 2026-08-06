using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.Rage.Serialization;

internal sealed class RageProgressionDetailData
{
    [JsonRequired]
    public RageUseGrantData[]? UsesByLevel { get; init; }

    [JsonRequired]
    public RageDamageBonusGrantData[]? DamageBonusByLevel { get; init; }

    [JsonRequired]
    public int DurationMinutes { get; init; }

    [JsonRequired]
    public string[]? ResistedDamageTypeIds { get; init; }

    [JsonRequired]
    public bool RequiresNotWearingHeavyArmor { get; init; }
}

internal sealed class RageUseGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int? UsesPerLongRest { get; init; }
}

internal sealed class RageDamageBonusGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int Bonus { get; init; }
}
