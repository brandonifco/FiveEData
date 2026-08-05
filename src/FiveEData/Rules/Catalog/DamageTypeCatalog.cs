using System.Collections.Frozen;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Catalog;

public sealed class DamageTypeCatalog
{
    private readonly FrozenDictionary<
        DamageTypeId,
        DamageTypeDefinition> _byId;

    internal DamageTypeCatalog(
        IEnumerable<DamageTypeDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        DamageTypeDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (DamageTypeDefinition definition in ordered)
        {
            DamageTypeDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<DamageTypeDefinition> All { get; }
    public int Count => All.Count;

    public DamageTypeDefinition Get(DamageTypeId id)
    {
        if (_byId.TryGetValue(
                id,
                out DamageTypeDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Damage type '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        DamageTypeId id,
        out DamageTypeDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<DamageTypeDefinition> definitions)
    {
        var ids = new HashSet<DamageTypeId>();

        foreach (DamageTypeDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate damage type ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
