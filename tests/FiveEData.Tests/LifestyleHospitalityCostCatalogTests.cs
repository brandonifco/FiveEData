using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Tests;

public sealed class LifestyleHospitalityCostCatalogTests
{
    [Fact]
    public void Catalog_OrdersByLifestyleIdAndProvidesLookup()
    {
        var catalog =
            new LifestyleHospitalityCostCatalog(
                [
                    Create(
                        "dnd5e2014.lifestyle.wealthy"),
                    Create(
                        "dnd5e2014.lifestyle.modest")
                ]);

        Assert.Equal(
            new[]
            {
                "dnd5e2014.lifestyle.modest",
                "dnd5e2014.lifestyle.wealthy"
            },
            catalog.All
                .Select(
                    definition =>
                        definition.LifestyleId.Value)
                .ToArray());

        Assert.Equal(
            50,
            catalog.Get(
                new LifestyleId(
                    "dnd5e2014.lifestyle.modest"))
                .InnStayCostPerDay
                .CopperPieces);

        Assert.True(
            catalog.TryGet(
                new LifestyleId(
                    "dnd5e2014.lifestyle.wealthy"),
                out LifestyleHospitalityCostDefinition?
                    found));
        Assert.NotNull(found);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source =
            new List<
                LifestyleHospitalityCostDefinition>
            {
                Create(
                    "dnd5e2014.lifestyle.modest")
            };

        var catalog =
            new LifestyleHospitalityCostCatalog(source);

        source.Add(
            Create(
                "dnd5e2014.lifestyle.wealthy"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateLifestyleIds()
    {
        Assert.Throws<ArgumentException>(
            () =>
                new LifestyleHospitalityCostCatalog(
                    [
                        Create(
                            "dnd5e2014.lifestyle.modest"),
                        Create(
                            "dnd5e2014.lifestyle.modest")
                    ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinition()
    {
        LifestyleHospitalityCostDefinition definition =
            new(
                default,
                new Money(50),
                new Money(30),
                specialRuleIds: [],
                sources: [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () =>
                new LifestyleHospitalityCostCatalog(
                    [definition]));
    }

    private static LifestyleHospitalityCostDefinition
        Create(string lifestyleId)
    {
        return new LifestyleHospitalityCostDefinition(
            new LifestyleId(lifestyleId),
            new Money(50),
            new Money(30),
            specialRuleIds: [],
            sources: [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            page: 158);
    }
}
