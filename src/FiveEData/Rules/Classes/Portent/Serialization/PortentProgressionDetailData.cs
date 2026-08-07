using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.Portent.Serialization;

internal sealed class PortentProgressionDetailData
{
    [JsonRequired]
    public PortentRollGrantData[]? ForetellingRollsByLevel { get; init; }

    [JsonRequired]
    public bool OncePerTurn { get; init; }

    [JsonRequired]
    public bool RecoversOnLongRest { get; init; }
}

internal sealed class PortentRollGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int ForetellingRolls { get; init; }
}
