using System.Collections.Frozen;
using FiveEData.Rules.Adventuring.Resting;

namespace FiveEData.Rules.Catalog;

public sealed class RestTypeCatalog
{
    private readonly FrozenDictionary<RestTypeId, RestTypeDefinition> _byId;

    internal RestTypeCatalog(IEnumerable<RestTypeDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        RestTypeDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (RestTypeDefinition definition in ordered)
        {
            RestTypeDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<RestTypeDefinition> All { get; }
    public int Count => All.Count;

    public RestTypeDefinition Get(RestTypeId id)
    {
        if (_byId.TryGetValue(id, out RestTypeDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Rest type '{id}' does not exist in this catalog.");
    }

    public bool TryGet(RestTypeId id, out RestTypeDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<RestTypeDefinition> definitions)
    {
        var ids = new HashSet<RestTypeId>();

        foreach (RestTypeDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate rest type ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
