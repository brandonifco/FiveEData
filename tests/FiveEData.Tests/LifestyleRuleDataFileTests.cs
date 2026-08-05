using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Tests;

public sealed class LifestyleRuleDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExpectedRuleCount()
    {
        IReadOnlyList<RuleDefinition> rules = LoadCanonical();

        Assert.Equal(244, rules.Count);
        Assert.Equal(
            244,
            rules.Select(rule => rule.Id).Distinct().Count());
    }

    [Fact]
    public void CanonicalFile_ContainsExactLifestyleRules()
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
    public void CanonicalFile_ContainsExactlyElevenPhase10BLifestyleRules()
    {
        IReadOnlyList<RuleDefinition> rules = LoadCanonical();

        Assert.Equal(
            11,
            rules.Count(
                rule =>
                    rule.Id.Value.StartsWith(
                        "dnd5e2014.expense-rule.lifestyle-",
                        StringComparison.Ordinal) ||
                    rule.Id.Value.StartsWith(
                        "dnd5e2014.lifestyle-rule.",
                        StringComparison.Ordinal)));
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

    private const string LifestyleSection =
        "Chapter 5: Equipment — Expenses — Lifestyle Expenses";

    private static readonly ExpectedRule[] Expected =
    [
        new(
            "dnd5e2014.expense-rule.lifestyle-expense-coverage",
            "Lifestyle expenses cover accommodations, food, drink, " +
            "necessities, and equipment maintenance",
            157,
            LifestyleSection),
        new(
            "dnd5e2014.expense-rule." +
            "lifestyle-selection-and-daily-pricing",
            "Choose and pay for a lifestyle using its listed daily price",
            157,
            LifestyleSection),
        new(
            "dnd5e2014.expense-rule." +
            "lifestyle-thirty-day-calculation",
            "Thirty-day lifestyle cost is thirty times the daily price",
            157,
            LifestyleSection),
        new(
            "dnd5e2014.expense-rule.lifestyle-consequences",
            "Lifestyle choice can have social consequences",
            157,
            LifestyleSection),
        CreateLifestyleRule("wretched", "Wretched", 157),
        CreateLifestyleRule("squalid", "Squalid", 157),
        CreateLifestyleRule("poor", "Poor", 157),
        CreateLifestyleRule("modest", "Modest", 157),
        CreateLifestyleRule("comfortable", "Comfortable", 158),
        CreateLifestyleRule("wealthy", "Wealthy", 158),
        CreateLifestyleRule("aristocratic", "Aristocratic", 158)
    ];

    private static ExpectedRule CreateLifestyleRule(
        string idSuffix,
        string name,
        int page)
    {
        return new ExpectedRule(
            $"dnd5e2014.lifestyle-rule.{idSuffix}-conditions",
            $"{name} lifestyle conditions",
            page,
            $"{LifestyleSection} — {name}");
    }
}
