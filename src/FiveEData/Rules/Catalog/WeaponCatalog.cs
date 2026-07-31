using System.Collections.Frozen;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Catalog;

public sealed class WeaponCatalog
{
    private readonly FrozenDictionary<WeaponId, WeaponDefinition> _byId;

    internal WeaponCatalog(IEnumerable<WeaponDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        WeaponDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (WeaponDefinition definition in ordered)
        {
            WeaponDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);

        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<WeaponDefinition> All { get; }
    public int Count => All.Count;

    public WeaponDefinition Get(WeaponId id)
    {
        if (_byId.TryGetValue(id, out WeaponDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Weapon '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        WeaponId id,
        out WeaponDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<WeaponDefinition> definitions)
    {
        var ids = new HashSet<WeaponId>();

        foreach (WeaponDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate weapon ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
