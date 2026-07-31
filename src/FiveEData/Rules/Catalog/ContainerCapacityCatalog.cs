using System.Collections.Frozen;
using FiveEData.Rules.Equipment.AdventuringGear;

namespace FiveEData.Rules.Catalog;

public sealed class ContainerCapacityCatalog
{
    private readonly FrozenDictionary<
        AdventuringGearId,
        ContainerCapacityDefinition> _byGearId;

    internal ContainerCapacityCatalog(
        IEnumerable<ContainerCapacityDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        ContainerCapacityDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.AdventuringGearId.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueGearIds(ordered);

        foreach (ContainerCapacityDefinition definition in ordered)
        {
            ContainerCapacityDefinitionValidator.EnsureValid(definition);
        }

        _byGearId = ordered.ToFrozenDictionary(
            definition => definition.AdventuringGearId);

        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<ContainerCapacityDefinition> All { get; }
    public int Count => All.Count;

    public ContainerCapacityDefinition Get(AdventuringGearId gearId)
    {
        if (_byGearId.TryGetValue(
            gearId,
            out ContainerCapacityDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Container capacity for adventuring gear '{gearId}' does not exist in this catalog.");
    }

    public bool TryGet(
        AdventuringGearId gearId,
        out ContainerCapacityDefinition? definition)
    {
        return _byGearId.TryGetValue(gearId, out definition);
    }

    private static void EnsureUniqueGearIds(
        IEnumerable<ContainerCapacityDefinition> definitions)
    {
        var gearIds = new HashSet<AdventuringGearId>();

        foreach (ContainerCapacityDefinition definition in definitions)
        {
            if (!gearIds.Add(definition.AdventuringGearId))
            {
                throw new ArgumentException(
                    $"Duplicate container-capacity adventuring gear ID '{definition.AdventuringGearId}'.",
                    nameof(definitions));
            }
        }
    }
}
