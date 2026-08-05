using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Tests;

public sealed class ConditionFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new ConditionId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.condition.test";

        var id = new ConditionId(value);

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

        var definition = new ConditionDefinition(
            new ConditionId(
                "dnd5e2014.condition.test"),
            "Test",
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        var definition = new ConditionDefinition(
            default,
            "Test",
            [CreateSource()]);

        Assert.Contains(
            ConditionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "ID",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        var definition = new ConditionDefinition(
            new ConditionId(
                "dnd5e2014.condition.test"),
            "Test",
            []);

        Assert.Contains(
            ConditionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ConditionCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new ConditionCatalog(
            [
                Create(
                    "dnd5e2014.condition.z",
                    "Z"),
                Create(
                    "dnd5e2014.condition.a",
                    "A")
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.condition.a",
                "dnd5e2014.condition.z"
            ],
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        var aId =
            new ConditionId("dnd5e2014.condition.a");

        ConditionDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(
                aId,
                out ConditionDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new ConditionId("dnd5e2014.condition.missing");

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out ConditionDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<ConditionDefinition>
        {
            Create(
                "dnd5e2014.condition.one",
                "One")
        };

        var catalog = new ConditionCatalog(source);

        source.Add(
            Create(
                "dnd5e2014.condition.two",
                "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new ConditionCatalog(
                [
                    Create(
                        "dnd5e2014.condition.duplicate",
                        "One"),
                    Create(
                        "dnd5e2014.condition.duplicate",
                        "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        var definition = new ConditionDefinition(
            default,
            "Invalid",
            [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => new ConditionCatalog([definition]));
    }

    private static ConditionDefinition Create(
        string id,
        string name)
    {
        return new ConditionDefinition(
            new ConditionId(id),
            name,
            [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            page: 290);
    }
}
