using System.Collections.Frozen;
using FiveEData.Rules.Creatures.Races;

namespace FiveEData.Rules.Catalog;

public sealed class SubraceCatalog
{
    private readonly FrozenDictionary<SubraceId, SubraceDefinition> _byId;

    internal SubraceCatalog(IEnumerable<SubraceDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        SubraceDefinition[] ordered = definitions
            .OrderBy(definition => definition.Id.Value, StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (SubraceDefinition definition in ordered)
        {
            SubraceDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<SubraceDefinition> All { get; }
    public int Count => All.Count;

    public SubraceDefinition Get(SubraceId id)
    {
        if (_byId.TryGetValue(id, out SubraceDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Subrace '{id}' does not exist in this catalog.");
    }

    public bool TryGet(SubraceId id, out SubraceDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<SubraceDefinition> definitions)
    {
        var ids = new HashSet<SubraceId>();

        foreach (SubraceDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate subrace ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
