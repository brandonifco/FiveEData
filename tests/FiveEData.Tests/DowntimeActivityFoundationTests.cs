using FiveEData.Rules.Adventuring.DowntimeActivities;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Tests;

public sealed class DowntimeActivityFoundationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Id_RejectsEmptyValue(string value)
    {
        Assert.Throws<ArgumentException>(
            () => new DowntimeActivityId(value));
    }

    [Fact]
    public void Id_RejectsNullValue()
    {
        Assert.Throws<ArgumentNullException>(
            () => new DowntimeActivityId(null!));
    }

    [Fact]
    public void Id_RoundTripsValue()
    {
        var id = new DowntimeActivityId(
            "dnd5e2014.downtime-activity.crafting");

        Assert.Equal("dnd5e2014.downtime-activity.crafting", id.Value);
        Assert.Equal("dnd5e2014.downtime-activity.crafting", id.ToString());
    }

    [Fact]
    public void Definition_CopiesSourcesDefensively()
    {
        var sources = new List<SourceReference> { TestSource() };

        DowntimeActivityDefinition definition = Create(sources: sources);

        sources.Add(TestSource());

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        DowntimeActivityDefinition definition = Create(sources: []);

        Assert.Contains(
            DowntimeActivityDefinitionValidator.Validate(definition),
            error => error.Contains(
                "at least one source reference",
                StringComparison.Ordinal));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsNonPositiveRequiredDays(int days)
    {
        DowntimeActivityDefinition definition = Create(requiredDays: days);

        Assert.Contains(
            DowntimeActivityDefinitionValidator.Validate(definition),
            error => error.Contains(
                "required days",
                StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsNonPositiveCostPerDay(int cost)
    {
        DowntimeActivityDefinition definition = Create(
            costPerDayGoldPieces: cost);

        Assert.Contains(
            DowntimeActivityDefinitionValidator.Validate(definition),
            error => error.Contains(
                "cost per day",
                StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsNonPositiveSavingThrowDC(int dc)
    {
        DowntimeActivityDefinition definition = Create(
            savingThrowAbilityId:
                new AbilityId("dnd5e2014.ability.constitution"),
            savingThrowDC: dc);

        Assert.Contains(
            DowntimeActivityDefinitionValidator.Validate(definition),
            error => error.Contains(
                "saving throw DC",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsSavingThrowAbilityWithoutDC()
    {
        DowntimeActivityDefinition definition = Create(
            savingThrowAbilityId:
                new AbilityId("dnd5e2014.ability.constitution"),
            savingThrowDC: null);

        Assert.Contains(
            DowntimeActivityDefinitionValidator.Validate(definition),
            error => error.Contains(
                "must be both present or both absent",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsSavingThrowDCWithoutAbility()
    {
        DowntimeActivityDefinition definition = Create(
            savingThrowAbilityId: null,
            savingThrowDC: 15);

        Assert.Contains(
            DowntimeActivityDefinitionValidator.Validate(definition),
            error => error.Contains(
                "must be both present or both absent",
                StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsNonPositiveMarketValueProgressPerDay(
        int value)
    {
        DowntimeActivityDefinition definition = Create(
            marketValueProgressPerDayGoldPieces: value);

        Assert.Contains(
            DowntimeActivityDefinitionValidator.Validate(definition),
            error => error.Contains(
                "market value progress",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsAllFactsDeclined()
    {
        DowntimeActivityDefinition definition = Create(
            requiredDays: null,
            costPerDayGoldPieces: null,
            savingThrowAbilityId: null,
            savingThrowDC: null,
            marketValueProgressPerDayGoldPieces: null);

        Assert.Empty(
            DowntimeActivityDefinitionValidator.Validate(definition));
    }

    [Fact]
    public void Catalog_OrdersById()
    {
        var catalog = new DowntimeActivityCatalog(
        [
            Create("dnd5e2014.downtime-activity.training", "Training"),
            Create("dnd5e2014.downtime-activity.crafting", "Crafting")
        ]);

        Assert.Equal(
            [
                "dnd5e2014.downtime-activity.crafting",
                "dnd5e2014.downtime-activity.training"
            ],
            catalog.All.Select(definition => definition.Id.Value));
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new DowntimeActivityCatalog(
            [
                Create("dnd5e2014.downtime-activity.crafting", "Crafting"),
                Create("dnd5e2014.downtime-activity.crafting", "Crafting")
            ]));
    }

    [Fact]
    public void Catalog_GetThrowsForMissingId()
    {
        var catalog = new DowntimeActivityCatalog(
            [Create("dnd5e2014.downtime-activity.crafting", "Crafting")]);

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(
                new DowntimeActivityId(
                    "dnd5e2014.downtime-activity.missing")));
    }

    [Fact]
    public void Catalog_TryGetReportsPresence()
    {
        var catalog = new DowntimeActivityCatalog(
            [Create("dnd5e2014.downtime-activity.crafting", "Crafting")]);

        Assert.True(
            catalog.TryGet(
                new DowntimeActivityId(
                    "dnd5e2014.downtime-activity.crafting"),
                out DowntimeActivityDefinition? found));
        Assert.Equal("Crafting", found!.Name);

        Assert.False(
            catalog.TryGet(
                new DowntimeActivityId(
                    "dnd5e2014.downtime-activity.missing"),
                out DowntimeActivityDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_ExposesCount()
    {
        var catalog = new DowntimeActivityCatalog(
        [
            Create("dnd5e2014.downtime-activity.crafting", "Crafting"),
            Create("dnd5e2014.downtime-activity.training", "Training")
        ]);

        Assert.Equal(2, catalog.Count);
    }

    private static DowntimeActivityDefinition Create(
        string id = "dnd5e2014.downtime-activity.crafting",
        string name = "Crafting",
        int? requiredDays = null,
        int? costPerDayGoldPieces = null,
        AbilityId? savingThrowAbilityId = null,
        int? savingThrowDC = null,
        int? marketValueProgressPerDayGoldPieces = 5,
        IEnumerable<SourceReference>? sources = null)
    {
        return new DowntimeActivityDefinition(
            new DowntimeActivityId(id),
            name,
            requiredDays,
            costPerDayGoldPieces,
            savingThrowAbilityId,
            savingThrowDC,
            marketValueProgressPerDayGoldPieces,
            sources ?? [TestSource()]);
    }

    private static SourceReference TestSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            187,
            "Chapter 8: Adventuring — Between Adventures — Downtime " +
                "Activities");
    }
}
