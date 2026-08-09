using System.Collections.Frozen;
using FiveEData.Rules.Adventuring.TravelPace;

namespace FiveEData.Rules.Catalog;

public sealed class TravelPaceCatalog
{
    private readonly FrozenDictionary<TravelPaceId, TravelPaceDefinition>
        _byId;

    internal TravelPaceCatalog(IEnumerable<TravelPaceDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        TravelPaceDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (TravelPaceDefinition definition in ordered)
        {
            TravelPaceDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<TravelPaceDefinition> All { get; }
    public int Count => All.Count;

    public TravelPaceDefinition Get(TravelPaceId id)
    {
        if (_byId.TryGetValue(id, out TravelPaceDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Travel pace '{id}' does not exist in this catalog.");
    }

    public bool TryGet(TravelPaceId id, out TravelPaceDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<TravelPaceDefinition> definitions)
    {
        var ids = new HashSet<TravelPaceId>();

        foreach (TravelPaceDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate travel pace ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
