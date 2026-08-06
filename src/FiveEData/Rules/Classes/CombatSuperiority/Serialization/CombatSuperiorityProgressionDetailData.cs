using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.CombatSuperiority.Serialization;

internal sealed class CombatSuperiorityProgressionDetailData
{
    [JsonRequired]
    public CombatSuperiorityManeuversKnownGrantData[]? ManeuversKnownByLevel
    {
        get;
        init;
    }

    [JsonRequired]
    public CombatSuperiorityDiceCountGrantData[]? DiceCountByLevel
    {
        get;
        init;
    }

    [JsonRequired]
    public CombatSuperiorityDieSizeGrantData[]? DieSizeByLevel { get; init; }
}

internal sealed class CombatSuperiorityManeuversKnownGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int ManeuversKnown { get; init; }
}

internal sealed class CombatSuperiorityDiceCountGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int DiceCount { get; init; }
}

internal sealed class CombatSuperiorityDieSizeGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public DiceExpressionData? Die { get; init; }
}
