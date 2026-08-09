using System.Collections.Frozen;
using FiveEData.Rules.Adventuring.DowntimeActivities;

namespace FiveEData.Rules.Catalog;

public sealed class DowntimeActivityCatalog
{
    private readonly FrozenDictionary<
        DowntimeActivityId,
        DowntimeActivityDefinition> _byId;

    internal DowntimeActivityCatalog(
        IEnumerable<DowntimeActivityDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        DowntimeActivityDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (DowntimeActivityDefinition definition in ordered)
        {
            DowntimeActivityDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<DowntimeActivityDefinition> All { get; }
    public int Count => All.Count;

    public DowntimeActivityDefinition Get(DowntimeActivityId id)
    {
        if (_byId.TryGetValue(
                id,
                out DowntimeActivityDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Downtime activity '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        DowntimeActivityId id,
        out DowntimeActivityDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<DowntimeActivityDefinition> definitions)
    {
        var ids = new HashSet<DowntimeActivityId>();

        foreach (DowntimeActivityDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate downtime activity ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
