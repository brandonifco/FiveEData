using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.ChannelDivinity.Serialization;

internal sealed class ChannelDivinityProgressionDetailData
{
    [JsonRequired]
    public ChannelDivinityUseGrantData[]? UsesByLevel { get; init; }

    [JsonRequired]
    public bool RecoversOnShortRest { get; init; }
}

internal sealed class ChannelDivinityUseGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int UsesPerRest { get; init; }
}
