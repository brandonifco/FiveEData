using System.Collections.Frozen;
using FiveEData.Rules.Equipment.TradeGoods;

namespace FiveEData.Rules.Catalog;

public sealed class TradeGoodCatalog
{
    private readonly FrozenDictionary<
        TradeGoodId,
        TradeGoodDefinition> _byId;

    internal TradeGoodCatalog(
        IEnumerable<TradeGoodDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        TradeGoodDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (TradeGoodDefinition definition in ordered)
        {
            TradeGoodDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<TradeGoodDefinition> All { get; }
    public int Count => All.Count;

    public TradeGoodDefinition Get(TradeGoodId id)
    {
        if (_byId.TryGetValue(id, out TradeGoodDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Trade good '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        TradeGoodId id,
        out TradeGoodDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<TradeGoodDefinition> definitions)
    {
        var ids = new HashSet<TradeGoodId>();

        foreach (TradeGoodDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate trade-good ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
