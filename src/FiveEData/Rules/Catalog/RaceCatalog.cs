using System.Collections.Frozen;
using FiveEData.Rules.Creatures.Races;

namespace FiveEData.Rules.Catalog;

public sealed class RaceCatalog
{
    private readonly FrozenDictionary<RaceId, RaceDefinition> _byId;

    internal RaceCatalog(IEnumerable<RaceDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        RaceDefinition[] ordered = definitions
            .OrderBy(definition => definition.Id.Value, StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (RaceDefinition definition in ordered)
        {
            RaceDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<RaceDefinition> All { get; }
    public int Count => All.Count;

    public RaceDefinition Get(RaceId id)
    {
        if (_byId.TryGetValue(id, out RaceDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Race '{id}' does not exist in this catalog.");
    }

    public bool TryGet(RaceId id, out RaceDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<RaceDefinition> definitions)
    {
        var ids = new HashSet<RaceId>();

        foreach (RaceDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate race ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
