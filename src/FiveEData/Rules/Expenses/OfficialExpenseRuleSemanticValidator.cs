using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Expenses;

internal static class OfficialExpenseRuleSemanticValidator
{
    private const string ExpenseRulePrefix =
        "dnd5e2014.expense-rule.";

    private const string LifestyleRulePrefix =
        "dnd5e2014.lifestyle-rule.";

    private const string LifestyleSection =
        "Chapter 5: Equipment — Expenses — Lifestyle Expenses";

    private const string FoodDrinkSection =
        "Chapter 5: Equipment — Expenses — Food, Drink, and Lodging";

    private const string SelfSufficiencySection =
        "Chapter 5: Equipment — Expenses — Self-Sufficiency";

    private const string ServicesSection =
        "Chapter 5: Equipment — Expenses — Services";

    private const string SpellcastingServicesSection =
        "Chapter 5: Equipment — Expenses — Spellcasting Services";

    private static readonly SourceDocumentId SourceDocumentId =
        new("dnd5e2014.source.phb-first-printing");

    private static readonly RuleExpectation[] Expectations =
    [
        ExpenseRule(
            "food-drink-lodging-included-in-lifestyle",
            "Food, drink, and lodging costs are included in " +
            "lifestyle expenses",
            158,
            FoodDrinkSection),
        ExpenseRule(
            "lifestyle-consequences",
            "Lifestyle choice can have social consequences",
            157,
            LifestyleSection),
        ExpenseRule(
            "lifestyle-expense-coverage",
            "Lifestyle expenses cover accommodations, food, " +
            "drink, necessities, and equipment maintenance",
            157,
            LifestyleSection),
        ExpenseRule(
            "lifestyle-selection-and-daily-pricing",
            "Choose and pay for a lifestyle using its listed " +
            "daily price",
            157,
            LifestyleSection),
        ExpenseRule(
            "lifestyle-thirty-day-calculation",
            "Thirty-day lifestyle cost is thirty times the " +
            "daily price",
            157,
            LifestyleSection),
        ExpenseRule(
            "profession-poor-lifestyle-equivalent",
            "Practicing a profession supports a poor lifestyle " +
            "equivalent",
            159,
            SelfSufficiencySection),
        ExpenseRule(
            "self-sufficiency",
            "Self-sufficiency can replace coin-paid lifestyle " +
            "expenses",
            159,
            SelfSufficiencySection),
        ExpenseRule(
            "skilled-hireling-pay-minimum",
            "Listed skilled-hireling pay is a minimum and " +
            "experts can require more",
            159,
            ServicesSection),
        ExpenseRule(
            "skilled-hireling-proficiency-service",
            "Skilled hirelings perform services involving a " +
            "weapon, tool, or skill proficiency",
            159,
            ServicesSection),
        ExpenseRule(
            "spellcasting-services-common-low-level-cost",
            "Common 1st- or 2nd-level spellcasting services may " +
            "cost 10 to 50 gp plus expensive materials",
            159,
            SpellcastingServicesSection),
        ExpenseRule(
            "spellcasting-services-higher-level-travel-or-service",
            "Higher-level spellcasting services may require " +
            "travel or an adventuring service",
            159,
            SpellcastingServicesSection),
        ExpenseRule(
            "spellcasting-services-level-affects-access-and-cost",
            "Higher-level spellcasting services are harder to " +
            "find and cost more",
            159,
            SpellcastingServicesSection),
        ExpenseRule(
            "spellcasting-services-no-established-rates",
            "Spellcasting services have no established pay rates",
            159,
            SpellcastingServicesSection),
        ExpenseRule(
            "spellcasting-services-not-ordinary-hirelings",
            "Spellcasters offering services are not ordinary " +
            "hirelings",
            159,
            SpellcastingServicesSection),
        ExpenseRule(
            "survival-comfortable-lifestyle-equivalent",
            "Survival proficiency supports a comfortable " +
            "lifestyle equivalent",
            159,
            SelfSufficiencySection),
        ExpenseRule(
            "untrained-hireling-menial-work",
            "Untrained hirelings perform menial work requiring " +
            "no particular skill",
            159,
            ServicesSection),
        LifestyleRule(
            "aristocratic",
            "Aristocratic",
            158),
        LifestyleRule(
            "comfortable",
            "Comfortable",
            158),
        LifestyleRule(
            "modest",
            "Modest",
            157),
        LifestyleRule(
            "poor",
            "Poor",
            157),
        LifestyleRule(
            "squalid",
            "Squalid",
            157),
        LifestyleRule(
            "wealthy",
            "Wealthy",
            158),
        LifestyleRule(
            "wretched",
            "Wretched",
            157)
    ];

    public static IReadOnlyList<string> Validate(
        IReadOnlyList<RuleDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        var errors = new List<string>();

        RuleDefinition[] managedDefinitions =
            definitions
                .Where(
                    definition =>
                        IsManagedIdentity(definition.Id))
                .ToArray();

        if (managedDefinitions.Length != Expectations.Length)
        {
            errors.Add(
                "Official Phase 10 expense-rule set must contain " +
                $"exactly {Expectations.Length} managed " +
                $"definitions; found {managedDefinitions.Length}.");
        }

        var byId =
            new Dictionary<RuleId, RuleDefinition>();

        foreach (RuleDefinition definition in managedDefinitions)
        {
            if (!byId.TryAdd(definition.Id, definition))
            {
                errors.Add(
                    "Official Phase 10 expense-rule set contains " +
                    $"duplicate ID '{definition.Id}'.");
            }
        }

        HashSet<RuleId> expectedIds =
            Expectations
                .Select(expectation => expectation.Id)
                .ToHashSet();

        foreach (RuleExpectation expectation in Expectations)
        {
            if (!byId.TryGetValue(
                    expectation.Id,
                    out RuleDefinition? definition))
            {
                errors.Add(
                    "Official Phase 10 expense-rule set is missing " +
                    $"'{expectation.Id}'.");
                continue;
            }

            ValidateDefinition(
                definition,
                expectation,
                errors);
        }

        foreach (
            RuleId unexpectedId
            in byId.Keys
                .Where(id => !expectedIds.Contains(id))
                .OrderBy(
                    id => id.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                "Official Phase 10 expense-rule set contains " +
                $"unexpected definition '{unexpectedId}'.");
        }

        return errors;
    }

    private static bool IsManagedIdentity(RuleId id)
    {
        return
            id.Value.StartsWith(
                ExpenseRulePrefix,
                StringComparison.Ordinal) ||
            id.Value.StartsWith(
                LifestyleRulePrefix,
                StringComparison.Ordinal);
    }

    private static void ValidateDefinition(
        RuleDefinition definition,
        RuleExpectation expectation,
        ICollection<string> errors)
    {
        if (!string.Equals(
                definition.Name,
                expectation.Name,
                StringComparison.Ordinal))
        {
            errors.Add(
                $"Official Phase 10 rule '{expectation.Id}' must " +
                $"be named '{expectation.Name}'; found " +
                $"'{definition.Name}'.");
        }

        OfficialSourceReferenceSemanticValidator.Validate(
            $"Official Phase 10 rule '{expectation.Id}'",
            definition.Sources,
            expectation.Source,
            errors);
    }

    private static RuleExpectation ExpenseRule(
        string idSuffix,
        string name,
        int page,
        string section)
    {
        return new RuleExpectation(
            new RuleId(ExpenseRulePrefix + idSuffix),
            name,
            new OfficialSourceExpectation(
                SourceDocumentId,
                page,
                section));
    }

    private static RuleExpectation LifestyleRule(
        string idSuffix,
        string name,
        int page)
    {
        return new RuleExpectation(
            new RuleId(
                LifestyleRulePrefix +
                idSuffix +
                "-conditions"),
            $"{name} lifestyle conditions",
            new OfficialSourceExpectation(
                SourceDocumentId,
                page,
                $"{LifestyleSection} — {name}"));
    }

    private readonly record struct RuleExpectation(
        RuleId Id,
        string Name,
        OfficialSourceExpectation Source);
}
