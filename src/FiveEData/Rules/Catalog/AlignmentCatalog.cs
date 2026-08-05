using System.Collections.Frozen;
using FiveEData.Rules.Creatures.Alignments;

namespace FiveEData.Rules.Catalog;

public sealed class AlignmentCatalog
{
    private readonly FrozenDictionary<
        AlignmentId,
        AlignmentDefinition> _byId;

    internal AlignmentCatalog(
        IEnumerable<AlignmentDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        AlignmentDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (AlignmentDefinition definition in ordered)
        {
            AlignmentDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<AlignmentDefinition> All { get; }
    public int Count => All.Count;

    public AlignmentDefinition Get(AlignmentId id)
    {
        if (_byId.TryGetValue(
                id,
                out AlignmentDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Alignment '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        AlignmentId id,
        out AlignmentDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<AlignmentDefinition> definitions)
    {
        var ids = new HashSet<AlignmentId>();

        foreach (AlignmentDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate alignment ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
