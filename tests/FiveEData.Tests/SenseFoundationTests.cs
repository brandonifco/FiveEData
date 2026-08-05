using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Senses;

namespace FiveEData.Tests;

public sealed class SenseFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new SenseId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.sense.test";

        var id = new SenseId(value);

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

        var definition = new SenseDefinition(
            new SenseId(
                "dnd5e2014.sense.test"),
            "Test",
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        var definition = new SenseDefinition(
            default,
            "Test",
            [CreateSource()]);

        Assert.Contains(
            SenseDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "ID",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        var definition = new SenseDefinition(
            new SenseId(
                "dnd5e2014.sense.test"),
            "Test",
            []);

        Assert.Contains(
            SenseDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new SenseCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new SenseCatalog(
            [
                Create(
                    "dnd5e2014.sense.z",
                    "Z"),
                Create(
                    "dnd5e2014.sense.a",
                    "A")
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.sense.a",
                "dnd5e2014.sense.z"
            ],
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        var aId =
            new SenseId("dnd5e2014.sense.a");

        SenseDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(
                aId,
                out SenseDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new SenseId("dnd5e2014.sense.missing");

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out SenseDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<SenseDefinition>
        {
            Create(
                "dnd5e2014.sense.one",
                "One")
        };

        var catalog = new SenseCatalog(source);

        source.Add(
            Create(
                "dnd5e2014.sense.two",
                "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new SenseCatalog(
                [
                    Create(
                        "dnd5e2014.sense.duplicate",
                        "One"),
                    Create(
                        "dnd5e2014.sense.duplicate",
                        "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        var definition = new SenseDefinition(
            default,
            "Invalid",
            [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => new SenseCatalog([definition]));
    }

    private static SenseDefinition Create(
        string id,
        string name)
    {
        return new SenseDefinition(
            new SenseId(id),
            name,
            [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            page: 183);
    }
}
