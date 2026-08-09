using System.Collections.Frozen;
using FiveEData.Rules.Combat.CombatActions;

namespace FiveEData.Rules.Catalog;

public sealed class CombatActionCatalog
{
    private readonly FrozenDictionary<
        CombatActionId,
        CombatActionDefinition> _byId;

    internal CombatActionCatalog(
        IEnumerable<CombatActionDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        CombatActionDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (CombatActionDefinition definition in ordered)
        {
            CombatActionDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<CombatActionDefinition> All { get; }
    public int Count => All.Count;

    public CombatActionDefinition Get(CombatActionId id)
    {
        if (_byId.TryGetValue(
                id,
                out CombatActionDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Combat action '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        CombatActionId id,
        out CombatActionDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<CombatActionDefinition> definitions)
    {
        var ids = new HashSet<CombatActionId>();

        foreach (CombatActionDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate combat action ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
