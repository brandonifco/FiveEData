using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Spells.MagicSchools;

namespace FiveEData.Tests;

public sealed class MagicSchoolFoundationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Id_RejectsEmptyValue(string value)
    {
        Assert.Throws<ArgumentException>(() => new MagicSchoolId(value));
    }

    [Fact]
    public void Id_RejectsNullValue()
    {
        Assert.Throws<ArgumentNullException>(() => new MagicSchoolId(null!));
    }

    [Fact]
    public void Id_RoundTripsValue()
    {
        var id = new MagicSchoolId("dnd5e2014.magic-school.evocation");

        Assert.Equal("dnd5e2014.magic-school.evocation", id.Value);
        Assert.Equal("dnd5e2014.magic-school.evocation", id.ToString());
    }

    [Fact]
    public void Definition_CopiesSourcesDefensively()
    {
        var sources = new List<SourceReference> { TestSource() };

        MagicSchoolDefinition definition = Create(sources: sources);

        sources.Add(TestSource());

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        MagicSchoolDefinition definition = Create(sources: []);

        Assert.Contains(
            MagicSchoolDefinitionValidator.Validate(definition),
            error => error.Contains(
                "at least one source reference",
                StringComparison.Ordinal));
    }

    [Fact]
    public void Catalog_OrdersById()
    {
        var catalog = new MagicSchoolCatalog(
        [
            Create("dnd5e2014.magic-school.necromancy", "Necromancy"),
            Create("dnd5e2014.magic-school.abjuration", "Abjuration")
        ]);

        Assert.Equal(
            [
                "dnd5e2014.magic-school.abjuration",
                "dnd5e2014.magic-school.necromancy"
            ],
            catalog.All.Select(definition => definition.Id.Value));
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new MagicSchoolCatalog(
            [
                Create("dnd5e2014.magic-school.illusion", "Illusion"),
                Create("dnd5e2014.magic-school.illusion", "Illusion")
            ]));
    }

    [Fact]
    public void Catalog_GetThrowsForMissingId()
    {
        var catalog = new MagicSchoolCatalog(
            [Create("dnd5e2014.magic-school.illusion", "Illusion")]);

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(
                new MagicSchoolId("dnd5e2014.magic-school.missing")));
    }

    [Fact]
    public void Catalog_TryGetReportsPresence()
    {
        var catalog = new MagicSchoolCatalog(
            [Create("dnd5e2014.magic-school.illusion", "Illusion")]);

        Assert.True(
            catalog.TryGet(
                new MagicSchoolId("dnd5e2014.magic-school.illusion"),
                out MagicSchoolDefinition? found));
        Assert.Equal("Illusion", found!.Name);

        Assert.False(
            catalog.TryGet(
                new MagicSchoolId("dnd5e2014.magic-school.missing"),
                out MagicSchoolDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_ExposesCount()
    {
        var catalog = new MagicSchoolCatalog(
        [
            Create("dnd5e2014.magic-school.illusion", "Illusion"),
            Create("dnd5e2014.magic-school.evocation", "Evocation")
        ]);

        Assert.Equal(2, catalog.Count);
    }

    private static MagicSchoolDefinition Create(
        string id = "dnd5e2014.magic-school.evocation",
        string name = "Evocation",
        IEnumerable<SourceReference>? sources = null)
    {
        return new MagicSchoolDefinition(
            new MagicSchoolId(id),
            name,
            sources ?? [TestSource()]);
    }

    private static SourceReference TestSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            203,
            "Chapter 10: Spellcasting — The Schools of Magic");
    }
}
