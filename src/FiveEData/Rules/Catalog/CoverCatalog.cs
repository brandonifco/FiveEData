using System.Collections.Frozen;
using FiveEData.Rules.Combat.Cover;

namespace FiveEData.Rules.Catalog;

public sealed class CoverCatalog
{
    private readonly FrozenDictionary<CoverId, CoverDefinition> _byId;

    internal CoverCatalog(IEnumerable<CoverDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        CoverDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (CoverDefinition definition in ordered)
        {
            CoverDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<CoverDefinition> All { get; }
    public int Count => All.Count;

    public CoverDefinition Get(CoverId id)
    {
        if (_byId.TryGetValue(id, out CoverDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Cover '{id}' does not exist in this catalog.");
    }

    public bool TryGet(CoverId id, out CoverDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<CoverDefinition> definitions)
    {
        var ids = new HashSet<CoverId>();

        foreach (CoverDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate cover ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
