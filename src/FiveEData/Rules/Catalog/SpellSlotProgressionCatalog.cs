using System.Collections.Frozen;
using FiveEData.Rules.Classes.Spellcasting;

namespace FiveEData.Rules.Catalog;

public sealed class SpellSlotProgressionCatalog
{
    private readonly FrozenDictionary<
        SpellSlotProgressionId,
        SpellSlotProgressionDefinition> _byId;

    internal SpellSlotProgressionCatalog(
        IEnumerable<SpellSlotProgressionDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        SpellSlotProgressionDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (SpellSlotProgressionDefinition definition in ordered)
        {
            SpellSlotProgressionDefinitionValidator.EnsureValid(
                definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<SpellSlotProgressionDefinition> All { get; }
    public int Count => All.Count;

    public SpellSlotProgressionDefinition Get(
        SpellSlotProgressionId id)
    {
        if (_byId.TryGetValue(
                id,
                out SpellSlotProgressionDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Spell slot progression '{id}' does not exist in " +
            "this catalog.");
    }

    public bool TryGet(
        SpellSlotProgressionId id,
        out SpellSlotProgressionDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<SpellSlotProgressionDefinition> definitions)
    {
        var ids = new HashSet<SpellSlotProgressionId>();

        foreach (SpellSlotProgressionDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    "Duplicate spell slot progression ID " +
                    $"'{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
