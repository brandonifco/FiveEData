using System.Collections.Frozen;
using FiveEData.Rules.Common;

namespace FiveEData.Rules.Catalog;

public sealed class RuleCatalog
{
    private readonly FrozenDictionary<RuleId, RuleDefinition> _byId;

    internal RuleCatalog(IEnumerable<RuleDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        RuleDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (RuleDefinition definition in ordered)
        {
            RuleDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);

        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<RuleDefinition> All { get; }
    public int Count => All.Count;

    public RuleDefinition Get(RuleId id)
    {
        if (_byId.TryGetValue(id, out RuleDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Rule '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        RuleId id,
        out RuleDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<RuleDefinition> definitions)
    {
        var ids = new HashSet<RuleId>();

        foreach (RuleDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate rule ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
