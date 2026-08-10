using System.Collections.Frozen;
using FiveEData.Rules.Classes.HunterOptions;

namespace FiveEData.Rules.Catalog;

public sealed class HunterOptionCatalog
{
    private readonly FrozenDictionary<
        HunterOptionId,
        HunterOptionDefinition> _byId;

    internal HunterOptionCatalog(
        IEnumerable<HunterOptionDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        HunterOptionDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (HunterOptionDefinition definition in ordered)
        {
            HunterOptionDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<HunterOptionDefinition> All { get; }
    public int Count => All.Count;

    public HunterOptionDefinition Get(HunterOptionId id)
    {
        if (_byId.TryGetValue(id, out HunterOptionDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Hunter option '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        HunterOptionId id,
        out HunterOptionDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<HunterOptionDefinition> definitions)
    {
        var ids = new HashSet<HunterOptionId>();

        foreach (HunterOptionDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate hunter option ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
