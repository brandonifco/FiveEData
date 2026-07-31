using System.Collections.Frozen;
using FiveEData.Rules.Equipment.AdventuringGear;

namespace FiveEData.Rules.Catalog;

public sealed class AdventuringGearCatalog
{
    private readonly FrozenDictionary<
        AdventuringGearId,
        AdventuringGearDefinition> _byId;

    internal AdventuringGearCatalog(
        IEnumerable<AdventuringGearDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        AdventuringGearDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (AdventuringGearDefinition definition in ordered)
        {
            AdventuringGearDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);

        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<AdventuringGearDefinition> All { get; }
    public int Count => All.Count;

    public AdventuringGearDefinition Get(AdventuringGearId id)
    {
        if (_byId.TryGetValue(id, out AdventuringGearDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Adventuring gear '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        AdventuringGearId id,
        out AdventuringGearDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<AdventuringGearDefinition> definitions)
    {
        var ids = new HashSet<AdventuringGearId>();

        foreach (AdventuringGearDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate adventuring gear ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
