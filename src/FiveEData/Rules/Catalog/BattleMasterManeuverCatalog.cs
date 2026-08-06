using System.Collections.Frozen;
using FiveEData.Rules.Classes.BattleMasterManeuvers;

namespace FiveEData.Rules.Catalog;

public sealed class BattleMasterManeuverCatalog
{
    private readonly FrozenDictionary<
        BattleMasterManeuverId,
        BattleMasterManeuverDefinition> _byId;

    internal BattleMasterManeuverCatalog(
        IEnumerable<BattleMasterManeuverDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        BattleMasterManeuverDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (BattleMasterManeuverDefinition definition in ordered)
        {
            BattleMasterManeuverDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<BattleMasterManeuverDefinition> All { get; }
    public int Count => All.Count;

    public BattleMasterManeuverDefinition Get(BattleMasterManeuverId id)
    {
        if (_byId.TryGetValue(
                id,
                out BattleMasterManeuverDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Battle Master maneuver '{id}' does not exist in this " +
            "catalog.");
    }

    public bool TryGet(
        BattleMasterManeuverId id,
        out BattleMasterManeuverDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<BattleMasterManeuverDefinition> definitions)
    {
        var ids = new HashSet<BattleMasterManeuverId>();

        foreach (BattleMasterManeuverDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    "Duplicate Battle Master maneuver ID " +
                    $"'{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
