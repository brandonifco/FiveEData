using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.Services;
using FiveEData.Rules.Expenses.Services.Serialization;

namespace FiveEData.Tests;

public sealed class MundaneServiceDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactlySevenRows()
    {
        IReadOnlyList<MundaneServiceDefinition> definitions =
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
    public void CanonicalFile_MatchesFirstPrintingTable()
    {
        IReadOnlyDictionary<
            MundaneServiceId,
            MundaneServiceDefinition> actual =
                LoadCanonical().ToDictionary(
                    definition => definition.Id);

        foreach (ExpectedService expected in Expected)
        {
            MundaneServiceDefinition definition =
                actual[new MundaneServiceId(expected.Id)];

            Assert.Equal(expected.Name, definition.Name);
            Assert.Equal(
                expected.CopperPieces,
                definition.Cost.Amount.CopperPieces);
            Assert.Equal(
                expected.CostKind,
                definition.Cost.Kind);
            Assert.Equal(
                expected.PricingUnit,
                definition.PricingUnit);

            Assert.Equal(
                expected.RuleIds,
                definition.SpecialRuleIds
                    .Select(ruleId => ruleId.Value)
                    .ToArray());

            SourceReference source =
                Assert.Single(definition.Sources);

            Assert.Equal(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                source.DocumentId);
            Assert.Equal(159, source.Page);
            Assert.Equal(ServicesSection, source.Section);
        }
    }

    [Fact]
    public void CanonicalFile_UsesMinimumOnlyForSkilledHireling()
    {
        MundaneServiceDefinition minimum =
            Assert.Single(
                LoadCanonical(),
                definition =>
                    definition.Cost.Kind ==
                    ListedCostKind.Minimum);

        Assert.Equal(
            new MundaneServiceId(
                "dnd5e2014.mundane-service.hireling-skilled"),
            minimum.Id);
        Assert.Equal(
            200,
            minimum.Cost.Amount.CopperPieces);
        Assert.Equal(
            ServicePricingUnit.Day,
            minimum.PricingUnit);
    }

    [Fact]
    public void CanonicalFile_UsesExpectedPricingUnits()
    {
        IReadOnlyList<MundaneServiceDefinition> definitions =
            LoadCanonical();

        Assert.Equal(
            2,
            definitions.Count(
                definition =>
                    definition.PricingUnit ==
                    ServicePricingUnit.Flat));

        Assert.Equal(
            3,
            definitions.Count(
                definition =>
                    definition.PricingUnit ==
                    ServicePricingUnit.Mile));

        Assert.Equal(
            2,
            definitions.Count(
                definition =>
                    definition.PricingUnit ==
                    ServicePricingUnit.Day));
    }

    [Fact]
    public void CanonicalFile_DoesNotPriceSpellcastingServices()
    {
        IReadOnlyList<MundaneServiceDefinition> definitions =
            LoadCanonical();

        Assert.DoesNotContain(
            definitions,
            definition =>
                definition.Id.Value.Contains(
                    "spellcasting",
                    StringComparison.OrdinalIgnoreCase));

        Assert.DoesNotContain(
            definitions,
            definition =>
                definition.Name.Contains(
                    "spellcasting",
                    StringComparison.OrdinalIgnoreCase));
    }

    private static IReadOnlyList<MundaneServiceDefinition>
        LoadCanonical()
    {
        string json = File.ReadAllText(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "mundane-services.json"));

        return MundaneServiceDefinitionLoader
            .LoadFromJson(json);
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

    private sealed record ExpectedService(
        string Id,
        string Name,
        long CopperPieces,
        ListedCostKind CostKind,
        ServicePricingUnit PricingUnit,
        string[] RuleIds);

    private const string ServicesSection =
        "Chapter 5: Equipment — Expenses — Services";

    private static readonly ExpectedService[] Expected =
    [
        new(
            "dnd5e2014.mundane-service.coach-between-towns",
            "Coach cab, between towns",
            3,
            ListedCostKind.Exact,
            ServicePricingUnit.Mile,
            []),
        new(
            "dnd5e2014.mundane-service.coach-within-city",
            "Coach cab, within a city",
            1,
            ListedCostKind.Exact,
            ServicePricingUnit.Flat,
            []),
        new(
            "dnd5e2014.mundane-service.hireling-skilled",
            "Hireling, skilled",
            200,
            ListedCostKind.Minimum,
            ServicePricingUnit.Day,
            [
                "dnd5e2014.expense-rule." +
                "skilled-hireling-proficiency-service",
                "dnd5e2014.expense-rule." +
                "skilled-hireling-pay-minimum"
            ]),
        new(
            "dnd5e2014.mundane-service.hireling-untrained",
            "Hireling, untrained",
            20,
            ListedCostKind.Exact,
            ServicePricingUnit.Day,
            [
                "dnd5e2014.expense-rule." +
                "untrained-hireling-menial-work"
            ]),
        new(
            "dnd5e2014.mundane-service.messenger",
            "Messenger",
            2,
            ListedCostKind.Exact,
            ServicePricingUnit.Mile,
            []),
        new(
            "dnd5e2014.mundane-service.road-or-gate-toll",
            "Road or gate toll",
            1,
            ListedCostKind.Exact,
            ServicePricingUnit.Flat,
            []),
        new(
            "dnd5e2014.mundane-service.ship-passage",
            "Ship's passage",
            10,
            ListedCostKind.Exact,
            ServicePricingUnit.Mile,
            [])
    ];
}
