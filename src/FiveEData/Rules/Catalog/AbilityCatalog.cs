using System.Collections.Frozen;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Catalog;

public sealed class AbilityCatalog
{
    private readonly FrozenDictionary<
        AbilityId,
        AbilityDefinition> _byId;

    internal AbilityCatalog(
        IEnumerable<AbilityDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        AbilityDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (AbilityDefinition definition in ordered)
        {
            AbilityDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<AbilityDefinition> All { get; }
    public int Count => All.Count;

    public AbilityDefinition Get(AbilityId id)
    {
        if (_byId.TryGetValue(
                id,
                out AbilityDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Ability '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        AbilityId id,
        out AbilityDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<AbilityDefinition> definitions)
    {
        var ids = new HashSet<AbilityId>();

        foreach (AbilityDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate ability ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
