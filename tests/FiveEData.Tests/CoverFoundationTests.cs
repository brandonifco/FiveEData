using FiveEData.Rules.Catalog;
using FiveEData.Rules.Combat.Cover;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class CoverFoundationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Id_RejectsEmptyValue(string value)
    {
        Assert.Throws<ArgumentException>(() => new CoverId(value));
    }

    [Fact]
    public void Id_RejectsNullValue()
    {
        Assert.Throws<ArgumentNullException>(() => new CoverId(null!));
    }

    [Fact]
    public void Id_RoundTripsValue()
    {
        var id = new CoverId("dnd5e2014.cover.half");

        Assert.Equal("dnd5e2014.cover.half", id.Value);
        Assert.Equal("dnd5e2014.cover.half", id.ToString());
    }

    [Fact]
    public void Definition_CopiesSourcesDefensively()
    {
        var sources = new List<SourceReference> { TestSource() };

        CoverDefinition definition = Create(sources: sources);

        sources.Add(TestSource());

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        CoverDefinition definition = Create(sources: []);

        Assert.Contains(
            CoverDefinitionValidator.Validate(definition),
            error => error.Contains(
                "at least one source reference",
                StringComparison.Ordinal));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsNonPositiveArmorClassBonus(int bonus)
    {
        CoverDefinition definition = Create(armorClassBonus: bonus);

        Assert.Contains(
            CoverDefinitionValidator.Validate(definition),
            error => error.Contains(
                "armor class bonus",
                StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsNonPositiveDexteritySavingThrowBonus(
        int bonus)
    {
        CoverDefinition definition = Create(
            dexteritySavingThrowBonus: bonus);

        Assert.Contains(
            CoverDefinitionValidator.Validate(definition),
            error => error.Contains(
                "Dexterity saving throw bonus",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsNullBonusesWithPreventsBeingTargeted()
    {
        CoverDefinition definition = Create(
            armorClassBonus: null,
            dexteritySavingThrowBonus: null,
            preventsBeingTargeted: true);

        Assert.Empty(CoverDefinitionValidator.Validate(definition));
    }

    [Fact]
    public void Catalog_OrdersById()
    {
        var catalog = new CoverCatalog(
        [
            Create("dnd5e2014.cover.total", "Total Cover"),
            Create("dnd5e2014.cover.half", "Half Cover")
        ]);

        Assert.Equal(
            ["dnd5e2014.cover.half", "dnd5e2014.cover.total"],
            catalog.All.Select(definition => definition.Id.Value));
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new CoverCatalog(
            [
                Create("dnd5e2014.cover.half", "Half Cover"),
                Create("dnd5e2014.cover.half", "Half Cover")
            ]));
    }

    [Fact]
    public void Catalog_GetThrowsForMissingId()
    {
        var catalog = new CoverCatalog(
            [Create("dnd5e2014.cover.half", "Half Cover")]);

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(new CoverId("dnd5e2014.cover.missing")));
    }

    [Fact]
    public void Catalog_TryGetReportsPresence()
    {
        var catalog = new CoverCatalog(
            [Create("dnd5e2014.cover.half", "Half Cover")]);

        Assert.True(
            catalog.TryGet(
                new CoverId("dnd5e2014.cover.half"),
                out CoverDefinition? found));
        Assert.Equal("Half Cover", found!.Name);

        Assert.False(
            catalog.TryGet(
                new CoverId("dnd5e2014.cover.missing"),
                out CoverDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_ExposesCount()
    {
        var catalog = new CoverCatalog(
        [
            Create("dnd5e2014.cover.half", "Half Cover"),
            Create("dnd5e2014.cover.total", "Total Cover")
        ]);

        Assert.Equal(2, catalog.Count);
    }

    private static CoverDefinition Create(
        string id = "dnd5e2014.cover.half",
        string name = "Half Cover",
        int? armorClassBonus = 2,
        int? dexteritySavingThrowBonus = 2,
        bool preventsBeingTargeted = false,
        IEnumerable<SourceReference>? sources = null)
    {
        return new CoverDefinition(
            new CoverId(id),
            name,
            armorClassBonus,
            dexteritySavingThrowBonus,
            preventsBeingTargeted,
            sources ?? [TestSource()]);
    }

    private static SourceReference TestSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            196,
            "Chapter 9: Combat — Cover");
    }
}
