using System.Collections.Frozen;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Rules.Catalog;

public sealed class CreatureSizeCatalog
{
    private readonly FrozenDictionary<
        CreatureSizeId,
        CreatureSizeDefinition> _byId;

    internal CreatureSizeCatalog(
        IEnumerable<CreatureSizeDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        CreatureSizeDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (CreatureSizeDefinition definition in ordered)
        {
            CreatureSizeDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<CreatureSizeDefinition> All { get; }
    public int Count => All.Count;

    public CreatureSizeDefinition Get(CreatureSizeId id)
    {
        if (_byId.TryGetValue(
                id,
                out CreatureSizeDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Creature size '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        CreatureSizeId id,
        out CreatureSizeDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<CreatureSizeDefinition> definitions)
    {
        var ids = new HashSet<CreatureSizeId>();

        foreach (CreatureSizeDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate creature-size ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
