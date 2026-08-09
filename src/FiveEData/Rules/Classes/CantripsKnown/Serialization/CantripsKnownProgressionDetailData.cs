using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.CantripsKnown.Serialization;

internal sealed class CantripsKnownProgressionDetailData
{
    [JsonRequired]
    public CantripsKnownGrantData[]? CantripsKnownByLevel
    {
        get;
        init;
    }
}

internal sealed class CantripsKnownGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int CantripsKnown { get; init; }
}
