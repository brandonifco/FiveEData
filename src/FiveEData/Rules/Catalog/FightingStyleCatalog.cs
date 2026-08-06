using System.Collections.Frozen;
using FiveEData.Rules.Classes.FightingStyles;

namespace FiveEData.Rules.Catalog;

public sealed class FightingStyleCatalog
{
    private readonly FrozenDictionary<
        FightingStyleId,
        FightingStyleDefinition> _byId;

    internal FightingStyleCatalog(
        IEnumerable<FightingStyleDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        FightingStyleDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (FightingStyleDefinition definition in ordered)
        {
            FightingStyleDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<FightingStyleDefinition> All { get; }
    public int Count => All.Count;

    public FightingStyleDefinition Get(FightingStyleId id)
    {
        if (_byId.TryGetValue(
                id,
                out FightingStyleDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Fighting style '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        FightingStyleId id,
        out FightingStyleDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<FightingStyleDefinition> definitions)
    {
        var ids = new HashSet<FightingStyleId>();

        foreach (FightingStyleDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate fighting style ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
