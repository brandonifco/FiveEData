using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Tests;

public sealed class CreatureSizeFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new CreatureSizeId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value =
            "dnd5e2014.creature-size.test";

        var id = new CreatureSizeId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference>
        {
            CreateSource()
        };

        var definition = new CreatureSizeDefinition(
            new CreatureSizeId(
                "dnd5e2014.creature-size.test"),
            "Test",
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        var definition = new CreatureSizeDefinition(
            default,
            "Test",
            [CreateSource()]);

        Assert.Contains(
            CreatureSizeDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "ID",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        var definition = new CreatureSizeDefinition(
            new CreatureSizeId(
                "dnd5e2014.creature-size.test"),
            "Test",
            []);

        Assert.Contains(
            CreatureSizeDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new CreatureSizeCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new CreatureSizeCatalog(
            [
                Create(
                    "dnd5e2014.creature-size.z",
                    "Z"),
                Create(
                    "dnd5e2014.creature-size.a",
                    "A")
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.creature-size.a",
                "dnd5e2014.creature-size.z"
            ],
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        var aId =
            new CreatureSizeId(
                "dnd5e2014.creature-size.a");

        CreatureSizeDefinition found =
            catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(
                aId,
                out CreatureSizeDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new CreatureSizeId(
                "dnd5e2014.creature-size.missing");

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out CreatureSizeDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<CreatureSizeDefinition>
        {
            Create(
                "dnd5e2014.creature-size.one",
                "One")
        };

        var catalog = new CreatureSizeCatalog(source);

        source.Add(
            Create(
                "dnd5e2014.creature-size.two",
                "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new CreatureSizeCatalog(
                [
                    Create(
                        "dnd5e2014.creature-size.duplicate",
                        "One"),
                    Create(
                        "dnd5e2014.creature-size.duplicate",
                        "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        var definition = new CreatureSizeDefinition(
            default,
            "Invalid",
            [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => new CreatureSizeCatalog([definition]));
    }

    private static CreatureSizeDefinition Create(
        string id,
        string name)
    {
        return new CreatureSizeDefinition(
            new CreatureSizeId(id),
            name,
            [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            page: 191);
    }
}
