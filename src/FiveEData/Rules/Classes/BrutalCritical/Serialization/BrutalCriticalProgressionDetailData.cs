using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.BrutalCritical.Serialization;

internal sealed class BrutalCriticalProgressionDetailData
{
    [JsonRequired]
    public BrutalCriticalDiceGrantData[]? AdditionalDiceByLevel { get; init; }

    [JsonRequired]
    public bool RequiresMeleeAttack { get; init; }
}

internal sealed class BrutalCriticalDiceGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int AdditionalDice { get; init; }
}
