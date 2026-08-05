using System.Collections.Frozen;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Catalog;

public sealed class ConditionCatalog
{
    private readonly FrozenDictionary<
        ConditionId,
        ConditionDefinition> _byId;

    internal ConditionCatalog(
        IEnumerable<ConditionDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        ConditionDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (ConditionDefinition definition in ordered)
        {
            ConditionDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<ConditionDefinition> All { get; }
    public int Count => All.Count;

    public ConditionDefinition Get(ConditionId id)
    {
        if (_byId.TryGetValue(
                id,
                out ConditionDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Condition '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        ConditionId id,
        out ConditionDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<ConditionDefinition> definitions)
    {
        var ids = new HashSet<ConditionId>();

        foreach (ConditionDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate condition ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
