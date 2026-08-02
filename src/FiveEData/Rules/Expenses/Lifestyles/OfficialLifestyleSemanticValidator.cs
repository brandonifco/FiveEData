using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses;

namespace FiveEData.Rules.Expenses.Lifestyles;

internal static class OfficialLifestyleSemanticValidator
{
    private const string LifestyleSection =
        "Chapter 5: Equipment — Expenses — Lifestyle Expenses";

    private static readonly OfficialSourceExpectation
        LifestyleSource =
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                157,
                LifestyleSection);

    private static readonly LifestyleExpectation[] Expectations =
    [
        new(
            new LifestyleId(
                "dnd5e2014.lifestyle.wretched"),
            "Wretched",
            copperPieces: null,
            costKind: null),
        new(
            new LifestyleId(
                "dnd5e2014.lifestyle.squalid"),
            "Squalid",
            10,
            ListedCostKind.Exact),
        new(
            new LifestyleId(
                "dnd5e2014.lifestyle.poor"),
            "Poor",
            20,
            ListedCostKind.Exact),
        new(
            new LifestyleId(
                "dnd5e2014.lifestyle.modest"),
            "Modest",
            100,
            ListedCostKind.Exact),
        new(
            new LifestyleId(
                "dnd5e2014.lifestyle.comfortable"),
            "Comfortable",
            200,
            ListedCostKind.Exact),
        new(
            new LifestyleId(
                "dnd5e2014.lifestyle.wealthy"),
            "Wealthy",
            400,
            ListedCostKind.Exact),
        new(
            new LifestyleId(
                "dnd5e2014.lifestyle.aristocratic"),
            "Aristocratic",
            1000,
            ListedCostKind.Minimum)
    ];

    public static IReadOnlyList<string> Validate(
        IReadOnlyList<LifestyleDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        var errors = new List<string>();

        if (definitions.Count != Expectations.Length)
        {
            errors.Add(
                "Official lifestyle catalog must contain " +
                $"exactly {Expectations.Length} definitions; " +
                $"found {definitions.Count}.");
        }

        var byId =
            new Dictionary<
                LifestyleId,
                LifestyleDefinition>();

        foreach (LifestyleDefinition definition in definitions)
        {
            if (!byId.TryAdd(definition.Id, definition))
            {
                errors.Add(
                    "Official lifestyle catalog contains " +
                    $"duplicate ID '{definition.Id}'.");
            }
        }

        HashSet<LifestyleId> expectedIds =
            Expectations
                .Select(expectation => expectation.Id)
                .ToHashSet();

        foreach (LifestyleExpectation expectation in Expectations)
        {
            if (!byId.TryGetValue(
                    expectation.Id,
                    out LifestyleDefinition? definition))
            {
                errors.Add(
                    "Official lifestyle catalog is missing " +
                    $"'{expectation.Id}'.");
                continue;
            }

            ValidateDefinition(
                definition,
                expectation,
                errors);
        }

        foreach (
            LifestyleId unexpectedId
            in byId.Keys
                .Where(id => !expectedIds.Contains(id))
                .OrderBy(
                    id => id.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                "Official lifestyle catalog contains " +
                $"unexpected definition '{unexpectedId}'.");
        }

        return errors;
    }

    private static void ValidateDefinition(
        LifestyleDefinition definition,
        LifestyleExpectation expectation,
        ICollection<string> errors)
    {
        if (!string.Equals(
                definition.Name,
                expectation.Name,
                StringComparison.Ordinal))
        {
            errors.Add(
                $"Official lifestyle '{expectation.Id}' must be " +
                $"named '{expectation.Name}'; found " +
                $"'{definition.Name}'.");
        }

        OfficialSourceReferenceSemanticValidator.Validate(
            $"Official lifestyle '{expectation.Id}'",
            definition.Sources,
            expectation.Source,
            errors);

        if (expectation.CopperPieces is null)
        {
            if (definition.DailyCost is not null)
            {
                errors.Add(
                    $"Official lifestyle '{expectation.Id}' must " +
                    "have no listed daily cost.");
            }

            return;
        }

        if (definition.DailyCost is not { } dailyCost)
        {
            errors.Add(
                $"Official lifestyle '{expectation.Id}' must " +
                "have a listed daily cost.");
            return;
        }

        if (dailyCost.Amount.CopperPieces !=
            expectation.CopperPieces.Value)
        {
            errors.Add(
                $"Official lifestyle '{expectation.Id}' must " +
                $"cost {expectation.CopperPieces.Value} cp per " +
                $"day; found {dailyCost.Amount.CopperPieces} cp.");
        }

        if (dailyCost.Kind != expectation.CostKind)
        {
            errors.Add(
                $"Official lifestyle '{expectation.Id}' must use " +
                $"cost kind '{expectation.CostKind}'; found " +
                $"'{dailyCost.Kind}'.");
        }
    }

    private readonly record struct LifestyleExpectation(
        LifestyleId Id,
        string Name,
        long? CopperPieces,
        ListedCostKind? CostKind,
        OfficialSourceExpectation Source)
    {
        public LifestyleExpectation(
            LifestyleId id,
            string name,
            long? copperPieces,
            ListedCostKind? costKind)
            : this(
                id,
                name,
                copperPieces,
                costKind,
                OfficialLifestyleSemanticValidator
                    .LifestyleSource)
        {
        }
    }
}
