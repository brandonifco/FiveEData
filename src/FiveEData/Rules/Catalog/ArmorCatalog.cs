using System.Collections.Frozen;
using FiveEData.Rules.Equipment.Armor;

namespace FiveEData.Rules.Catalog;

public sealed class ArmorCatalog
{
    private readonly FrozenDictionary<ArmorId, ArmorDefinition> _byId;

    internal ArmorCatalog(IEnumerable<ArmorDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        ArmorDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (ArmorDefinition definition in ordered)
        {
            ArmorDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);

        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<ArmorDefinition> All { get; }
    public int Count => All.Count;

    public ArmorDefinition Get(ArmorId id)
    {
        if (_byId.TryGetValue(id, out ArmorDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Armor '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        ArmorId id,
        out ArmorDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<ArmorDefinition> definitions)
    {
        var ids = new HashSet<ArmorId>();

        foreach (ArmorDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate armor ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
