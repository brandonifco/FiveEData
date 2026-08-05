using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Tests;

public sealed class DamageTypeFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new DamageTypeId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.damage-type.test";

        var id = new DamageTypeId(value);

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

        var definition = new DamageTypeDefinition(
            new DamageTypeId(
                "dnd5e2014.damage-type.test"),
            "Test",
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        var definition = new DamageTypeDefinition(
            default,
            "Test",
            [CreateSource()]);

        Assert.Contains(
            DamageTypeDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "ID",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        var definition = new DamageTypeDefinition(
            new DamageTypeId(
                "dnd5e2014.damage-type.test"),
            "Test",
            []);

        Assert.Contains(
            DamageTypeDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new DamageTypeCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new DamageTypeCatalog(
            [
                Create(
                    "dnd5e2014.damage-type.z",
                    "Z"),
                Create(
                    "dnd5e2014.damage-type.a",
                    "A")
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.damage-type.a",
                "dnd5e2014.damage-type.z"
            ],
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        var aId =
            new DamageTypeId("dnd5e2014.damage-type.a");

        DamageTypeDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(
                aId,
                out DamageTypeDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new DamageTypeId("dnd5e2014.damage-type.missing");

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out DamageTypeDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<DamageTypeDefinition>
        {
            Create(
                "dnd5e2014.damage-type.one",
                "One")
        };

        var catalog = new DamageTypeCatalog(source);

        source.Add(
            Create(
                "dnd5e2014.damage-type.two",
                "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new DamageTypeCatalog(
                [
                    Create(
                        "dnd5e2014.damage-type.duplicate",
                        "One"),
                    Create(
                        "dnd5e2014.damage-type.duplicate",
                        "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        var definition = new DamageTypeDefinition(
            default,
            "Invalid",
            [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => new DamageTypeCatalog([definition]));
    }

    private static DamageTypeDefinition Create(
        string id,
        string name)
    {
        return new DamageTypeDefinition(
            new DamageTypeId(id),
            name,
            [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            page: 196);
    }
}
