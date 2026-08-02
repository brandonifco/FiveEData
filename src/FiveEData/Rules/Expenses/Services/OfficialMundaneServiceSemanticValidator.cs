using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses;

namespace FiveEData.Rules.Expenses.Services;

internal static class OfficialMundaneServiceSemanticValidator
{
    private const string ServicesSection =
        "Chapter 5: Equipment — Expenses — Services";

    private static readonly OfficialSourceExpectation
        ServiceSource =
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                159,
                ServicesSection);

    private static readonly ServiceExpectation[] Expectations =
    [
        new(
            new MundaneServiceId(
                "dnd5e2014.mundane-service." +
                "coach-between-towns"),
            "Coach cab, between towns",
            3,
            ListedCostKind.Exact,
            ServicePricingUnit.Mile),
        new(
            new MundaneServiceId(
                "dnd5e2014.mundane-service." +
                "coach-within-city"),
            "Coach cab, within a city",
            1,
            ListedCostKind.Exact,
            ServicePricingUnit.Flat),
        new(
            new MundaneServiceId(
                "dnd5e2014.mundane-service." +
                "hireling-skilled"),
            "Hireling, skilled",
            200,
            ListedCostKind.Minimum,
            ServicePricingUnit.Day),
        new(
            new MundaneServiceId(
                "dnd5e2014.mundane-service." +
                "hireling-untrained"),
            "Hireling, untrained",
            20,
            ListedCostKind.Exact,
            ServicePricingUnit.Day),
        new(
            new MundaneServiceId(
                "dnd5e2014.mundane-service.messenger"),
            "Messenger",
            2,
            ListedCostKind.Exact,
            ServicePricingUnit.Mile),
        new(
            new MundaneServiceId(
                "dnd5e2014.mundane-service." +
                "road-or-gate-toll"),
            "Road or gate toll",
            1,
            ListedCostKind.Exact,
            ServicePricingUnit.Flat),
        new(
            new MundaneServiceId(
                "dnd5e2014.mundane-service.ship-passage"),
            "Ship's passage",
            10,
            ListedCostKind.Exact,
            ServicePricingUnit.Mile)
    ];

    public static IReadOnlyList<string> Validate(
        IReadOnlyList<MundaneServiceDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        var errors = new List<string>();

        if (definitions.Count != Expectations.Length)
        {
            errors.Add(
                "Official mundane-service catalog must contain " +
                $"exactly {Expectations.Length} definitions; " +
                $"found {definitions.Count}.");
        }

        var byId =
            new Dictionary<
                MundaneServiceId,
                MundaneServiceDefinition>();

        foreach (MundaneServiceDefinition definition in definitions)
        {
            if (!byId.TryAdd(definition.Id, definition))
            {
                errors.Add(
                    "Official mundane-service catalog contains " +
                    $"duplicate ID '{definition.Id}'.");
            }

            if (definition.Id.Value.Contains(
                    "spellcasting",
                    StringComparison.OrdinalIgnoreCase) ||
                definition.Name.Contains(
                    "spellcasting",
                    StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(
                    "Official mundane-service catalog must not " +
                    "contain a priced spellcasting service.");
            }
        }

        HashSet<MundaneServiceId> expectedIds =
            Expectations
                .Select(expectation => expectation.Id)
                .ToHashSet();

        foreach (ServiceExpectation expectation in Expectations)
        {
            if (!byId.TryGetValue(
                    expectation.Id,
                    out MundaneServiceDefinition? definition))
            {
                errors.Add(
                    "Official mundane-service catalog is missing " +
                    $"'{expectation.Id}'.");
                continue;
            }

            if (!string.Equals(
                    definition.Name,
                    expectation.Name,
                    StringComparison.Ordinal))
            {
                errors.Add(
                    $"Official mundane service '{expectation.Id}' " +
                    $"must be named '{expectation.Name}'; found " +
                    $"'{definition.Name}'.");
            }

            if (definition.Cost.Amount.CopperPieces !=
                expectation.CopperPieces)
            {
                errors.Add(
                    $"Official mundane service '{expectation.Id}' " +
                    $"must cost {expectation.CopperPieces} cp; " +
                    $"found " +
                    $"{definition.Cost.Amount.CopperPieces} cp.");
            }

            if (definition.Cost.Kind != expectation.CostKind)
            {
                errors.Add(
                    $"Official mundane service '{expectation.Id}' " +
                    $"must use cost kind '{expectation.CostKind}'; " +
                    $"found '{definition.Cost.Kind}'.");
            }

            if (definition.PricingUnit !=
                expectation.PricingUnit)
            {
                errors.Add(
                    $"Official mundane service '{expectation.Id}' " +
                    $"must use pricing unit " +
                    $"'{expectation.PricingUnit}'; found " +
                    $"'{definition.PricingUnit}'.");
            }

            OfficialSourceReferenceSemanticValidator.Validate(
                $"Official mundane service '{expectation.Id}'",
                definition.Sources,
                expectation.Source,
                errors);
        }

        foreach (
            MundaneServiceId unexpectedId
            in byId.Keys
                .Where(id => !expectedIds.Contains(id))
                .OrderBy(
                    id => id.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                "Official mundane-service catalog contains " +
                $"unexpected definition '{unexpectedId}'.");
        }

        return errors;
    }

    private readonly record struct ServiceExpectation(
        MundaneServiceId Id,
        string Name,
        long CopperPieces,
        ListedCostKind CostKind,
        ServicePricingUnit PricingUnit,
        OfficialSourceExpectation Source)
    {
        public ServiceExpectation(
            MundaneServiceId id,
            string name,
            long copperPieces,
            ListedCostKind costKind,
            ServicePricingUnit pricingUnit)
            : this(
                id,
                name,
                copperPieces,
                costKind,
                pricingUnit,
                OfficialMundaneServiceSemanticValidator
                    .ServiceSource)
        {
        }
    }
}
