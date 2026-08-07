using System.Collections.Frozen;
using FiveEData.Rules.Spells;

namespace FiveEData.Rules.Catalog;

public sealed class SpellCatalog
{
    private readonly FrozenDictionary<SpellId, SpellDefinition> _byId;

    internal SpellCatalog(IEnumerable<SpellDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        SpellDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (SpellDefinition definition in ordered)
        {
            SpellDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<SpellDefinition> All { get; }
    public int Count => All.Count;

    public SpellDefinition Get(SpellId id)
    {
        if (_byId.TryGetValue(id, out SpellDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Spell '{id}' does not exist in this catalog.");
    }

    public bool TryGet(SpellId id, out SpellDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<SpellDefinition> definitions)
    {
        var ids = new HashSet<SpellId>();

        foreach (SpellDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate spell ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
