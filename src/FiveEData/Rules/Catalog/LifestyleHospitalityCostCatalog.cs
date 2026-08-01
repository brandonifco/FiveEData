using System.Collections.Frozen;
using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Rules.Catalog;

public sealed class LifestyleHospitalityCostCatalog
{
    private readonly FrozenDictionary<
        LifestyleId,
        LifestyleHospitalityCostDefinition> _byLifestyleId;

    internal LifestyleHospitalityCostCatalog(
        IEnumerable<LifestyleHospitalityCostDefinition>
            definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        LifestyleHospitalityCostDefinition[] ordered =
            definitions
                .OrderBy(
                    definition =>
                        definition.LifestyleId.Value,
                    StringComparer.Ordinal)
                .ToArray();

        EnsureUniqueLifestyleIds(ordered);

        foreach (
            LifestyleHospitalityCostDefinition definition
            in ordered)
        {
            LifestyleHospitalityCostDefinitionValidator
                .EnsureValid(definition);
        }

        _byLifestyleId = ordered.ToFrozenDictionary(
            definition => definition.LifestyleId);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<
        LifestyleHospitalityCostDefinition> All { get; }

    public int Count => All.Count;

    public LifestyleHospitalityCostDefinition Get(
        LifestyleId lifestyleId)
    {
        if (_byLifestyleId.TryGetValue(
                lifestyleId,
                out LifestyleHospitalityCostDefinition?
                    definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            "Hospitality cost for lifestyle " +
            $"'{lifestyleId}' does not exist in this catalog.");
    }

    public bool TryGet(
        LifestyleId lifestyleId,
        out LifestyleHospitalityCostDefinition? definition)
    {
        return _byLifestyleId.TryGetValue(
            lifestyleId,
            out definition);
    }

    private static void EnsureUniqueLifestyleIds(
        IEnumerable<LifestyleHospitalityCostDefinition>
            definitions)
    {
        var lifestyleIds = new HashSet<LifestyleId>();

        foreach (
            LifestyleHospitalityCostDefinition definition
            in definitions)
        {
            if (!lifestyleIds.Add(definition.LifestyleId))
            {
                throw new ArgumentException(
                    "Duplicate hospitality-cost lifestyle ID " +
                    $"'{definition.LifestyleId}'.",
                    nameof(definitions));
            }
        }
    }
}
