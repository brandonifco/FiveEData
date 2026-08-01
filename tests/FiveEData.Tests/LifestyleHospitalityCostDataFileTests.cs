using FiveEData.Rules.Common;
using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.FoodAndLodging.Serialization;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Tests;

public sealed class LifestyleHospitalityCostDataFileTests
{
    private const string IncludedRuleId =
        "dnd5e2014.expense-rule." +
        "food-drink-lodging-included-in-lifestyle";

    private const string ExpectedSection =
        "Chapter 5: Equipment — Expenses — " +
        "Food, Drink, and Lodging";

    [Fact]
    public void CanonicalFile_ContainsExactlySixRows()
    {
        IReadOnlyList<
            LifestyleHospitalityCostDefinition> definitions =
                LoadCanonical();

        Assert.Equal(6, definitions.Count);
        Assert.Equal(
            6,
            definitions
                .Select(definition => definition.LifestyleId)
                .Distinct()
                .Count());
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingTables()
    {
        IReadOnlyDictionary<
            LifestyleId,
            LifestyleHospitalityCostDefinition> actual =
                LoadCanonical().ToDictionary(
                    definition => definition.LifestyleId);

        foreach (ExpectedHospitalityCost expected in Expected)
        {
            LifestyleHospitalityCostDefinition definition =
                actual[new LifestyleId(expected.LifestyleId)];

            Assert.Equal(
                expected.InnStayCopperPieces,
                definition.InnStayCostPerDay.CopperPieces);
            Assert.Equal(
                expected.MealsCopperPieces,
                definition.MealsCostPerDay.CopperPieces);

            Assert.Equal(
                [new RuleId(IncludedRuleId)],
                definition.SpecialRuleIds);

            var source = Assert.Single(definition.Sources);

            Assert.Equal(158, source.Page);
            Assert.Equal(ExpectedSection, source.Section);
        }
    }

    [Fact]
    public void CanonicalFile_DoesNotDefineWretchedHospitality()
    {
        Assert.DoesNotContain(
            LoadCanonical(),
            definition =>
                definition.LifestyleId ==
                new LifestyleId(
                    "dnd5e2014.lifestyle.wretched"));
    }

    [Fact]
    public void CanonicalFile_AssociatesEveryRowWithInclusionRule()
    {
        Assert.All(
            LoadCanonical(),
            definition =>
                Assert.Equal(
                    [new RuleId(IncludedRuleId)],
                    definition.SpecialRuleIds));
    }

    private static IReadOnlyList<
        LifestyleHospitalityCostDefinition> LoadCanonical()
    {
        return LifestyleHospitalityCostDefinitionLoader
            .LoadFromFile(
                Path.Combine(
                    FindRepositoryRoot(),
                    "Data",
                    "dnd5e2014",
                    "lifestyle-hospitality-costs.json"));
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

    private sealed record ExpectedHospitalityCost(
        string LifestyleId,
        long InnStayCopperPieces,
        long MealsCopperPieces);

    private static readonly ExpectedHospitalityCost[] Expected =
    [
        new(
            "dnd5e2014.lifestyle.squalid",
            7,
            3),
        new(
            "dnd5e2014.lifestyle.poor",
            10,
            6),
        new(
            "dnd5e2014.lifestyle.modest",
            50,
            30),
        new(
            "dnd5e2014.lifestyle.comfortable",
            80,
            50),
        new(
            "dnd5e2014.lifestyle.wealthy",
            200,
            80),
        new(
            "dnd5e2014.lifestyle.aristocratic",
            400,
            200)
    ];
}
