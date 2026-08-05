using FiveEData.Rules.Creatures.Races;

namespace FiveEData.Rules.Catalog;

internal sealed class RaceDefinitionSet
{
    public RaceDefinitionSet(
        IReadOnlyList<RaceDefinition> races,
        IReadOnlyList<SubraceDefinition> subraces)
    {
        ArgumentNullException.ThrowIfNull(races);
        ArgumentNullException.ThrowIfNull(subraces);

        Races = races;
        Subraces = subraces;
    }

    public IReadOnlyList<RaceDefinition> Races { get; }
    public IReadOnlyList<SubraceDefinition> Subraces { get; }
}
