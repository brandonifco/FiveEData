using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.Indomitable.Serialization;

internal sealed class IndomitableProgressionDetailData
{
    [JsonRequired]
    public IndomitableUseGrantData[]? UsesByLevel { get; init; }

    [JsonRequired]
    public bool RecoversOnShortRest { get; init; }
}

internal sealed class IndomitableUseGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int UsesPerRest { get; init; }
}
