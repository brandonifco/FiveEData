using System.Collections.Frozen;
using FiveEData.Rules.Expenses.FoodAndLodging;

namespace FiveEData.Rules.Catalog;

public sealed class FoodDrinkCatalog
{
    private readonly FrozenDictionary<
        FoodDrinkId,
        FoodDrinkDefinition> _byId;

    internal FoodDrinkCatalog(
        IEnumerable<FoodDrinkDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        FoodDrinkDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (FoodDrinkDefinition definition in ordered)
        {
            FoodDrinkDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<FoodDrinkDefinition> All { get; }
    public int Count => All.Count;

    public FoodDrinkDefinition Get(FoodDrinkId id)
    {
        if (_byId.TryGetValue(
                id,
                out FoodDrinkDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Food and drink '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        FoodDrinkId id,
        out FoodDrinkDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<FoodDrinkDefinition> definitions)
    {
        var ids = new HashSet<FoodDrinkId>();

        foreach (FoodDrinkDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate food-and-drink ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
