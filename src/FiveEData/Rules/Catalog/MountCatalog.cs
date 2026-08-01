using System.Collections.Frozen;
using FiveEData.Rules.Equipment.Mounts;

namespace FiveEData.Rules.Catalog;

public sealed class MountCatalog
{
    private readonly FrozenDictionary<MountId, MountDefinition> _byId;

    internal MountCatalog(IEnumerable<MountDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        MountDefinition[] ordered = definitions
            .OrderBy(definition => definition.Id.Value, StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (MountDefinition definition in ordered)
        {
            MountDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<MountDefinition> All { get; }
    public int Count => All.Count;

    public MountDefinition Get(MountId id)
    {
        if (_byId.TryGetValue(id, out MountDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Mount '{id}' does not exist in this catalog.");
    }

    public bool TryGet(MountId id, out MountDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(IEnumerable<MountDefinition> definitions)
    {
        var ids = new HashSet<MountId>();

        foreach (MountDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate mount ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
