using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.DiscipleOfTheElements.Serialization;

internal sealed class DiscipleOfTheElementsProgressionDetailData
{
    [JsonRequired]
    public DiscipleOfTheElementsDisciplinesKnownGrantData[]?
        DisciplinesKnownByLevel
    {
        get;
        init;
    }

    [JsonRequired]
    public DiscipleOfTheElementsMaxKiPointsGrantData[]?
        MaxKiPointsPerSpellByLevel
    {
        get;
        init;
    }
}

internal sealed class DiscipleOfTheElementsDisciplinesKnownGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int DisciplinesKnown { get; init; }
}

internal sealed class DiscipleOfTheElementsMaxKiPointsGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int MaxKiPoints { get; init; }
}
