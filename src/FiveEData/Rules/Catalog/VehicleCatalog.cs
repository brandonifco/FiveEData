using System.Collections.Frozen;
using FiveEData.Rules.Equipment.Vehicles;

namespace FiveEData.Rules.Catalog;

public sealed class VehicleCatalog
{
    private readonly FrozenDictionary<VehicleId, VehicleDefinition> _byId;

    internal VehicleCatalog(IEnumerable<VehicleDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        VehicleDefinition[] ordered = definitions
            .OrderBy(definition => definition.Id.Value, StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (VehicleDefinition definition in ordered)
        {
            VehicleDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<VehicleDefinition> All { get; }
    public int Count => All.Count;

    public VehicleDefinition Get(VehicleId id)
    {
        if (_byId.TryGetValue(id, out VehicleDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Vehicle '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        VehicleId id,
        out VehicleDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<VehicleDefinition> definitions)
    {
        var ids = new HashSet<VehicleId>();

        foreach (VehicleDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate vehicle ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
