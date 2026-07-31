using System.Collections.Frozen;
using FiveEData.Rules.Equipment.Ammunition;

namespace FiveEData.Rules.Catalog;

public sealed class AmmunitionCatalog
{
    private readonly FrozenDictionary<AmmunitionTypeId, AmmunitionDefinition>
        _byId;

    internal AmmunitionCatalog(
        IEnumerable<AmmunitionDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        AmmunitionDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (AmmunitionDefinition definition in ordered)
        {
            AmmunitionDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);

        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<AmmunitionDefinition> All { get; }
    public int Count => All.Count;

    public AmmunitionDefinition Get(AmmunitionTypeId id)
    {
        if (_byId.TryGetValue(id, out AmmunitionDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Ammunition type '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        AmmunitionTypeId id,
        out AmmunitionDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<AmmunitionDefinition> definitions)
    {
        var ids = new HashSet<AmmunitionTypeId>();

        foreach (AmmunitionDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate ammunition ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
