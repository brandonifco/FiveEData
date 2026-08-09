using FiveEData.Rules.Adventuring.Resting;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class RestTypeFoundationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Id_RejectsEmptyValue(string value)
    {
        Assert.Throws<ArgumentException>(() => new RestTypeId(value));
    }

    [Fact]
    public void Id_RejectsNullValue()
    {
        Assert.Throws<ArgumentNullException>(() => new RestTypeId(null!));
    }

    [Fact]
    public void Id_RoundTripsValue()
    {
        var id = new RestTypeId("dnd5e2014.rest-type.short-rest");

        Assert.Equal("dnd5e2014.rest-type.short-rest", id.Value);
        Assert.Equal("dnd5e2014.rest-type.short-rest", id.ToString());
    }

    [Fact]
    public void Definition_CopiesSourcesDefensively()
    {
        var sources = new List<SourceReference> { TestSource() };

        RestTypeDefinition definition = Create(sources: sources);

        sources.Add(TestSource());

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        RestTypeDefinition definition = Create(sources: []);

        Assert.Contains(
            RestTypeDefinitionValidator.Validate(definition),
            error => error.Contains(
                "at least one source reference",
                StringComparison.Ordinal));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsNonPositiveMinimumDurationHours(int hours)
    {
        RestTypeDefinition definition = Create(minimumDurationHours: hours);

        Assert.Contains(
            RestTypeDefinitionValidator.Validate(definition),
            error => error.Contains(
                "minimum duration",
                StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsNonPositiveCooldownHours(int hours)
    {
        RestTypeDefinition definition = Create(cooldownHours: hours);

        Assert.Contains(
            RestTypeDefinitionValidator.Validate(definition),
            error => error.Contains(
                "cooldown",
                StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsNonPositiveMinimumHitPointsToBenefit(
        int hitPoints)
    {
        RestTypeDefinition definition = Create(
            minimumHitPointsToBenefit: hitPoints);

        Assert.Contains(
            RestTypeDefinitionValidator.Validate(definition),
            error => error.Contains(
                "minimum hit points",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_OrdersById()
    {
        var catalog = new RestTypeCatalog(
        [
            Create("dnd5e2014.rest-type.short-rest", "Short Rest"),
            Create("dnd5e2014.rest-type.long-rest", "Long Rest")
        ]);

        Assert.Equal(
            ["dnd5e2014.rest-type.long-rest", "dnd5e2014.rest-type.short-rest"],
            catalog.All.Select(definition => definition.Id.Value));
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new RestTypeCatalog(
            [
                Create("dnd5e2014.rest-type.short-rest", "Short Rest"),
                Create("dnd5e2014.rest-type.short-rest", "Short Rest")
            ]));
    }

    [Fact]
    public void Catalog_GetThrowsForMissingId()
    {
        var catalog = new RestTypeCatalog(
            [Create("dnd5e2014.rest-type.short-rest", "Short Rest")]);

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(
                new RestTypeId("dnd5e2014.rest-type.missing")));
    }

    [Fact]
    public void Catalog_TryGetReportsPresence()
    {
        var catalog = new RestTypeCatalog(
            [Create("dnd5e2014.rest-type.short-rest", "Short Rest")]);

        Assert.True(
            catalog.TryGet(
                new RestTypeId("dnd5e2014.rest-type.short-rest"),
                out RestTypeDefinition? found));
        Assert.Equal("Short Rest", found!.Name);

        Assert.False(
            catalog.TryGet(
                new RestTypeId("dnd5e2014.rest-type.missing"),
                out RestTypeDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_ExposesCount()
    {
        var catalog = new RestTypeCatalog(
        [
            Create("dnd5e2014.rest-type.short-rest", "Short Rest"),
            Create("dnd5e2014.rest-type.long-rest", "Long Rest")
        ]);

        Assert.Equal(2, catalog.Count);
    }

    private static RestTypeDefinition Create(
        string id = "dnd5e2014.rest-type.short-rest",
        string name = "Short Rest",
        int minimumDurationHours = 1,
        int? cooldownHours = null,
        int? minimumHitPointsToBenefit = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new RestTypeDefinition(
            new RestTypeId(id),
            name,
            minimumDurationHours,
            cooldownHours,
            minimumHitPointsToBenefit,
            sources ?? [TestSource()]);
    }

    private static SourceReference TestSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            186,
            "Chapter 8: Adventuring — Resting");
    }
}
