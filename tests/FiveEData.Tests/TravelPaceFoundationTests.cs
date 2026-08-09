using FiveEData.Rules.Adventuring.TravelPace;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class TravelPaceFoundationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Id_RejectsEmptyValue(string value)
    {
        Assert.Throws<ArgumentException>(() => new TravelPaceId(value));
    }

    [Fact]
    public void Id_RejectsNullValue()
    {
        Assert.Throws<ArgumentNullException>(() => new TravelPaceId(null!));
    }

    [Fact]
    public void Id_RoundTripsValue()
    {
        var id = new TravelPaceId("dnd5e2014.travel-pace.normal");

        Assert.Equal("dnd5e2014.travel-pace.normal", id.Value);
        Assert.Equal("dnd5e2014.travel-pace.normal", id.ToString());
    }

    [Fact]
    public void Definition_CopiesSourcesDefensively()
    {
        var sources = new List<SourceReference> { TestSource() };

        TravelPaceDefinition definition = Create(sources: sources);

        sources.Add(TestSource());

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        TravelPaceDefinition definition = Create(sources: []);

        Assert.Contains(
            TravelPaceDefinitionValidator.Validate(definition),
            error => error.Contains(
                "at least one source reference",
                StringComparison.Ordinal));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsNonPositiveFeetPerMinute(int feetPerMinute)
    {
        TravelPaceDefinition definition = Create(
            feetPerMinute: feetPerMinute);

        Assert.Contains(
            TravelPaceDefinitionValidator.Validate(definition),
            error => error.Contains(
                "feet per minute",
                StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsNonPositiveMilesPerHour(int milesPerHour)
    {
        TravelPaceDefinition definition = Create(
            milesPerHour: milesPerHour);

        Assert.Contains(
            TravelPaceDefinitionValidator.Validate(definition),
            error => error.Contains(
                "miles per hour",
                StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsNonPositiveMilesPerDay(int milesPerDay)
    {
        TravelPaceDefinition definition = Create(milesPerDay: milesPerDay);

        Assert.Contains(
            TravelPaceDefinitionValidator.Validate(definition),
            error => error.Contains(
                "miles per day",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_OrdersById()
    {
        var catalog = new TravelPaceCatalog(
        [
            Create("dnd5e2014.travel-pace.slow", "Slow"),
            Create("dnd5e2014.travel-pace.fast", "Fast")
        ]);

        Assert.Equal(
            ["dnd5e2014.travel-pace.fast", "dnd5e2014.travel-pace.slow"],
            catalog.All.Select(definition => definition.Id.Value));
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new TravelPaceCatalog(
            [
                Create("dnd5e2014.travel-pace.fast", "Fast"),
                Create("dnd5e2014.travel-pace.fast", "Fast")
            ]));
    }

    [Fact]
    public void Catalog_GetThrowsForMissingId()
    {
        var catalog = new TravelPaceCatalog(
            [Create("dnd5e2014.travel-pace.fast", "Fast")]);

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(
                new TravelPaceId("dnd5e2014.travel-pace.missing")));
    }

    [Fact]
    public void Catalog_TryGetReportsPresence()
    {
        var catalog = new TravelPaceCatalog(
            [Create("dnd5e2014.travel-pace.fast", "Fast")]);

        Assert.True(
            catalog.TryGet(
                new TravelPaceId("dnd5e2014.travel-pace.fast"),
                out TravelPaceDefinition? found));
        Assert.Equal("Fast", found!.Name);

        Assert.False(
            catalog.TryGet(
                new TravelPaceId("dnd5e2014.travel-pace.missing"),
                out TravelPaceDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_ExposesCount()
    {
        var catalog = new TravelPaceCatalog(
        [
            Create("dnd5e2014.travel-pace.fast", "Fast"),
            Create("dnd5e2014.travel-pace.slow", "Slow")
        ]);

        Assert.Equal(2, catalog.Count);
    }

    private static TravelPaceDefinition Create(
        string id = "dnd5e2014.travel-pace.normal",
        string name = "Normal",
        int feetPerMinute = 300,
        int milesPerHour = 3,
        int milesPerDay = 24,
        int? passiveWisdomPerceptionPenalty = null,
        bool allowsStealth = false,
        IEnumerable<SourceReference>? sources = null)
    {
        return new TravelPaceDefinition(
            new TravelPaceId(id),
            name,
            feetPerMinute,
            milesPerHour,
            milesPerDay,
            passiveWisdomPerceptionPenalty,
            allowsStealth,
            sources ?? [TestSource()]);
    }

    private static SourceReference TestSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            182,
            "Chapter 8: Adventuring — Movement — Travel Pace");
    }
}
