using System.Collections.Frozen;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Rules.Catalog;

public sealed class LifestyleCatalog
{
    private readonly FrozenDictionary<
        LifestyleId,
        LifestyleDefinition> _byId;

    internal LifestyleCatalog(
        IEnumerable<LifestyleDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        LifestyleDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (LifestyleDefinition definition in ordered)
        {
            LifestyleDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<LifestyleDefinition> All { get; }
    public int Count => All.Count;

    public LifestyleDefinition Get(LifestyleId id)
    {
        if (_byId.TryGetValue(
                id,
                out LifestyleDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Lifestyle '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        LifestyleId id,
        out LifestyleDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<LifestyleDefinition> definitions)
    {
        var ids = new HashSet<LifestyleId>();

        foreach (LifestyleDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate lifestyle ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
