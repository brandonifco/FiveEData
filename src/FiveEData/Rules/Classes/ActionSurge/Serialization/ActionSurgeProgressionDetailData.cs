using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.ActionSurge.Serialization;

internal sealed class ActionSurgeProgressionDetailData
{
    [JsonRequired]
    public ActionSurgeUseGrantData[]? UsesByLevel { get; init; }

    [JsonRequired]
    public bool RecoversOnShortRest { get; init; }

    [JsonRequired]
    public bool OncePerTurn { get; init; }
}

internal sealed class ActionSurgeUseGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int UsesPerRest { get; init; }
}
