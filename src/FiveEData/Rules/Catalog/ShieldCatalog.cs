using System.Collections.Frozen;
using FiveEData.Rules.Equipment.Shields;

namespace FiveEData.Rules.Catalog;

public sealed class ShieldCatalog
{
    private readonly FrozenDictionary<ShieldId, ShieldDefinition> _byId;

    internal ShieldCatalog(IEnumerable<ShieldDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        ShieldDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (ShieldDefinition definition in ordered)
        {
            ShieldDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);

        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<ShieldDefinition> All { get; }
    public int Count => All.Count;

    public ShieldDefinition Get(ShieldId id)
    {
        if (_byId.TryGetValue(id, out ShieldDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Shield '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        ShieldId id,
        out ShieldDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<ShieldDefinition> definitions)
    {
        var ids = new HashSet<ShieldId>();

        foreach (ShieldDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate shield ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
