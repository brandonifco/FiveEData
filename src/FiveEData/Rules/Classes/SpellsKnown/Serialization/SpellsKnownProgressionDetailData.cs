using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.SpellsKnown.Serialization;

internal sealed class SpellsKnownProgressionDetailData
{
    [JsonRequired]
    public SpellsKnownGrantData[]? SpellsKnownByLevel
    {
        get;
        init;
    }
}

internal sealed class SpellsKnownGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int SpellsKnown { get; init; }
}
