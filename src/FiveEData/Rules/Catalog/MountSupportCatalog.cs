using System.Collections.Frozen;
using FiveEData.Rules.Equipment.MountSupport;

namespace FiveEData.Rules.Catalog;

public sealed class MountSupportCatalog
{
    private readonly FrozenDictionary<
        MountSupportId,
        MountSupportDefinition> _byId;

    internal MountSupportCatalog(
        IEnumerable<MountSupportDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        MountSupportDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (MountSupportDefinition definition in ordered)
        {
            MountSupportDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<MountSupportDefinition> All { get; }
    public int Count => All.Count;

    public MountSupportDefinition Get(MountSupportId id)
    {
        if (_byId.TryGetValue(id, out MountSupportDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Mount support '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        MountSupportId id,
        out MountSupportDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<MountSupportDefinition> definitions)
    {
        var ids = new HashSet<MountSupportId>();

        foreach (MountSupportDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate mount support ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
