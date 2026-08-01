using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.Lifestyles;
using FiveEData.Rules.Expenses.Lifestyles.Serialization;

namespace FiveEData.Tests;

public sealed class LifestyleDataFileTests
{
    private const string ExpectedSection =
        "Chapter 5: Equipment — Expenses — Lifestyle Expenses";

    [Fact]
    public void CanonicalFile_ContainsExactlySevenLifestyles()
    {
        IReadOnlyList<LifestyleDefinition> definitions =
            LoadCanonical();

        Assert.Equal(7, definitions.Count);
        Assert.Equal(
            7,
            definitions
                .Select(definition => definition.Id)
                .Distinct()
                .Count());
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingLifestyleTable()
    {
        IReadOnlyDictionary<LifestyleId, LifestyleDefinition> actual =
            LoadCanonical().ToDictionary(
                definition => definition.Id);

        foreach (ExpectedLifestyle expected in Expected)
        {
            LifestyleDefinition definition =
                actual[new LifestyleId(expected.Id)];

            Assert.Equal(expected.Name, definition.Name);
            Assert.Equal(
                expected.CopperPieces,
                definition.DailyCost?.Amount.CopperPieces);
            Assert.Equal(
                expected.CostKind,
                definition.DailyCost?.Kind);

            Assert.Equal(
                new[]
                {
                    new RuleId(expected.RuleId)
                },
                definition.SpecialRuleIds);

            SourceReference source =
                Assert.Single(definition.Sources);

            Assert.Equal(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                source.DocumentId);
            Assert.Equal(157, source.Page);
            Assert.Equal(ExpectedSection, source.Section);
        }
    }

    [Fact]
    public void CanonicalFile_PreservesCostSemantics()
    {
        IReadOnlyList<LifestyleDefinition> definitions =
            LoadCanonical();

        Assert.Single(
            definitions,
            definition => definition.DailyCost is null);

        Assert.Equal(
            5,
            definitions.Count(
                definition =>
                    definition.DailyCost?.Kind ==
                    ListedCostKind.Exact));

        Assert.Single(
            definitions,
            definition =>
                definition.DailyCost?.Kind ==
                ListedCostKind.Minimum);
    }

    [Fact]
    public void CanonicalFile_UsesMinimumOnlyForAristocratic()
    {
        LifestyleDefinition minimum =
            Assert.Single(
                LoadCanonical(),
                definition =>
                    definition.DailyCost?.Kind ==
                    ListedCostKind.Minimum);

        Assert.Equal(
            new LifestyleId(
                "dnd5e2014.lifestyle.aristocratic"),
            minimum.Id);
        Assert.Equal(
            1000,
            minimum.DailyCost?.Amount.CopperPieces);
    }

    private static IReadOnlyList<LifestyleDefinition>
        LoadCanonical()
    {
        return LifestyleDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "lifestyles.json"));
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

    private sealed record ExpectedLifestyle(
        string Id,
        string Name,
        long? CopperPieces,
        ListedCostKind? CostKind,
        string RuleId);

    private static readonly ExpectedLifestyle[] Expected =
    [
        new(
            "dnd5e2014.lifestyle.wretched",
            "Wretched",
            null,
            null,
            "dnd5e2014.lifestyle-rule.wretched-conditions"),
        new(
            "dnd5e2014.lifestyle.squalid",
            "Squalid",
            10,
            ListedCostKind.Exact,
            "dnd5e2014.lifestyle-rule.squalid-conditions"),
        new(
            "dnd5e2014.lifestyle.poor",
            "Poor",
            20,
            ListedCostKind.Exact,
            "dnd5e2014.lifestyle-rule.poor-conditions"),
        new(
            "dnd5e2014.lifestyle.modest",
            "Modest",
            100,
            ListedCostKind.Exact,
            "dnd5e2014.lifestyle-rule.modest-conditions"),
        new(
            "dnd5e2014.lifestyle.comfortable",
            "Comfortable",
            200,
            ListedCostKind.Exact,
            "dnd5e2014.lifestyle-rule.comfortable-conditions"),
        new(
            "dnd5e2014.lifestyle.wealthy",
            "Wealthy",
            400,
            ListedCostKind.Exact,
            "dnd5e2014.lifestyle-rule.wealthy-conditions"),
        new(
            "dnd5e2014.lifestyle.aristocratic",
            "Aristocratic",
            1000,
            ListedCostKind.Minimum,
            "dnd5e2014.lifestyle-rule.aristocratic-conditions")
    ];
}
