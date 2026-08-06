using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Tests;

public sealed class ExpenseRuleDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExpectedRuleCount()
    {
        IReadOnlyList<RuleDefinition> rules = LoadCanonical();

        Assert.Equal(289, rules.Count);
        Assert.Equal(
            289,
            rules.Select(rule => rule.Id).Distinct().Count());
    }

    [Fact]
    public void CanonicalFile_ContainsExactPhase10CExpenseRules()
    {
        IReadOnlyDictionary<RuleId, RuleDefinition> actual =
            LoadCanonical().ToDictionary(rule => rule.Id);

        foreach (ExpectedRule expected in Expected)
        {
            RuleDefinition rule =
                actual[new RuleId(expected.Id)];

            Assert.Equal(expected.Name, rule.Name);

            SourceReference source =
                Assert.Single(rule.Sources);

            Assert.Equal(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                source.DocumentId);
            Assert.Equal(expected.Page, source.Page);
            Assert.Equal(expected.Section, source.Section);
        }
    }

    [Fact]
    public void CanonicalFile_ContainsExactlyFourPhase10CExpenseRules()
    {
        HashSet<string> expectedIds =
            Expected.Select(rule => rule.Id).ToHashSet(
                StringComparer.Ordinal);

        Assert.Equal(
            4,
            LoadCanonical().Count(
                rule => expectedIds.Contains(rule.Id.Value)));
    }

    private static IReadOnlyList<RuleDefinition> LoadCanonical()
    {
        string rulesDirectory = Path.Combine(
            FindRepositoryRoot(),
            "Data",
            "dnd5e2014",
            "rules");

        return RuleDefinitionLoader.LoadAndMergeFromFiles(
            [
                Path.Combine(rulesDirectory, "weapon-rule.json"),
                Path.Combine(rulesDirectory, "armor-rule.json"),
                Path.Combine(rulesDirectory, "adventuring-gear-rule.json"),
                Path.Combine(rulesDirectory, "tool-rule.json"),
                Path.Combine(rulesDirectory, "mount-vehicle-rule.json"),
                Path.Combine(rulesDirectory, "trade-good-rule.json"),
                Path.Combine(rulesDirectory, "expense-rule.json"),
                Path.Combine(rulesDirectory, "lifestyle-rule.json"),
                Path.Combine(rulesDirectory, "race-rule.json"),
                Path.Combine(rulesDirectory, "class-rule.json")
            ]);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(
                        directory.FullName,
                        "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }

    private sealed record ExpectedRule(
        string Id,
        string Name,
        int Page,
        string Section);

    private const string FoodDrinkSection =
        "Chapter 5: Equipment — Expenses — " +
        "Food, Drink, and Lodging";

    private const string SelfSufficiencySection =
        "Chapter 5: Equipment — Expenses — Self-Sufficiency";

    private static readonly ExpectedRule[] Expected =
    [
        new(
            "dnd5e2014.expense-rule." +
            "food-drink-lodging-included-in-lifestyle",
            "Food, drink, and lodging costs are included in " +
            "lifestyle expenses",
            158,
            FoodDrinkSection),
        new(
            "dnd5e2014.expense-rule.self-sufficiency",
            "Self-sufficiency can replace coin-paid lifestyle expenses",
            159,
            SelfSufficiencySection),
        new(
            "dnd5e2014.expense-rule." +
            "profession-poor-lifestyle-equivalent",
            "Practicing a profession supports a poor lifestyle equivalent",
            159,
            SelfSufficiencySection),
        new(
            "dnd5e2014.expense-rule." +
            "survival-comfortable-lifestyle-equivalent",
            "Survival proficiency supports a comfortable " +
            "lifestyle equivalent",
            159,
            SelfSufficiencySection)
    ];
}
