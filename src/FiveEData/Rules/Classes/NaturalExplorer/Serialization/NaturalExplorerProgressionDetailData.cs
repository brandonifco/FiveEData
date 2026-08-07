using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.NaturalExplorer.Serialization;

internal sealed class NaturalExplorerProgressionDetailData
{
    [JsonRequired]
    public NaturalExplorerChoiceGrantData[]? FavoredTerrainsKnownByLevel
    {
        get;
        init;
    }
}

internal sealed class NaturalExplorerChoiceGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int FavoredTerrainsKnown { get; init; }
}
