using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Tests;

public sealed class ServiceRuleDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExpectedRuleCount()
    {
        IReadOnlyList<RuleDefinition> rules =
            LoadCanonical();

        Assert.Equal(90, rules.Count);
        Assert.Equal(
            90,
            rules.Select(rule => rule.Id).Distinct().Count());
    }

    [Fact]
    public void CanonicalFile_ContainsExactPhase10DRules()
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
            Assert.Equal(159, source.Page);
            Assert.Equal(expected.Section, source.Section);
        }
    }

    [Fact]
    public void CanonicalFile_ContainsExactlyEightPhase10DRules()
    {
        HashSet<string> expectedIds =
            Expected
                .Select(rule => rule.Id)
                .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(
            8,
            LoadCanonical().Count(
                rule => expectedIds.Contains(
                    rule.Id.Value)));
    }

    private static IReadOnlyList<RuleDefinition>
        LoadCanonical()
    {
        string json = File.ReadAllText(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "rules.json"));

        return RuleDefinitionLoader.LoadFromJson(json);
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
        string Section);

    private const string ServicesSection =
        "Chapter 5: Equipment — Expenses — Services";

    private const string SpellcastingSection =
        "Chapter 5: Equipment — Expenses — " +
        "Spellcasting Services";

    private static readonly ExpectedRule[] Expected =
    [
        new(
            "dnd5e2014.expense-rule." +
            "skilled-hireling-proficiency-service",
            "Skilled hirelings perform services involving " +
            "a weapon, tool, or skill proficiency",
            ServicesSection),
        new(
            "dnd5e2014.expense-rule." +
            "skilled-hireling-pay-minimum",
            "Listed skilled-hireling pay is a minimum " +
            "and experts can require more",
            ServicesSection),
        new(
            "dnd5e2014.expense-rule." +
            "untrained-hireling-menial-work",
            "Untrained hirelings perform menial work " +
            "requiring no particular skill",
            ServicesSection),
        new(
            "dnd5e2014.expense-rule." +
            "spellcasting-services-not-ordinary-hirelings",
            "Spellcasters offering services are not " +
            "ordinary hirelings",
            SpellcastingSection),
        new(
            "dnd5e2014.expense-rule." +
            "spellcasting-services-no-established-rates",
            "Spellcasting services have no established " +
            "pay rates",
            SpellcastingSection),
        new(
            "dnd5e2014.expense-rule." +
            "spellcasting-services-level-affects-access-and-cost",
            "Higher-level spellcasting services are " +
            "harder to find and cost more",
            SpellcastingSection),
        new(
            "dnd5e2014.expense-rule." +
            "spellcasting-services-common-low-level-cost",
            "Common 1st- or 2nd-level spellcasting " +
            "services may cost 10 to 50 gp plus " +
            "expensive materials",
            SpellcastingSection),
        new(
            "dnd5e2014.expense-rule." +
            "spellcasting-services-higher-level-travel-or-service",
            "Higher-level spellcasting services may " +
            "require travel or an adventuring service",
            SpellcastingSection)
    ];
}
