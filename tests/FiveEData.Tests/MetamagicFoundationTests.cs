using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.Metamagic;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class MetamagicFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new MetamagicOptionId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.metamagic-option.test";

        var id = new MetamagicOptionId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference> { CreateSource() };

        MetamagicOptionDefinition definition = CreateFixedCost(
            "dnd5e2014.metamagic-option.test",
            "Test",
            1,
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        MetamagicOptionDefinition definition = CreateFixedCost(
            null,
            "Test",
            1,
            [CreateSource()]);

        Assert.Contains(
            MetamagicOptionDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsNonPositiveFixedCost()
    {
        var definition = new MetamagicOptionDefinition(
            new MetamagicOptionId("dnd5e2014.metamagic-option.test"),
            "Test",
            fixedSorceryPointCost: 0,
            costEqualsSpellLevelWithCantripMinimum: false,
            [CreateSource()]);

        Assert.Contains(
            MetamagicOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "greater than zero",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsZeroCostRepresentations()
    {
        var definition = new MetamagicOptionDefinition(
            new MetamagicOptionId("dnd5e2014.metamagic-option.test"),
            "Test",
            fixedSorceryPointCost: null,
            costEqualsSpellLevelWithCantripMinimum: false,
            [CreateSource()]);

        Assert.Contains(
            MetamagicOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "exactly one cost",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsBothCostRepresentations()
    {
        var definition = new MetamagicOptionDefinition(
            new MetamagicOptionId("dnd5e2014.metamagic-option.test"),
            "Test",
            fixedSorceryPointCost: 1,
            costEqualsSpellLevelWithCantripMinimum: true,
            [CreateSource()]);

        Assert.Contains(
            MetamagicOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "exactly one cost",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        MetamagicOptionDefinition definition = CreateFixedCost(
            "dnd5e2014.metamagic-option.test",
            "Test",
            1,
            []);

        Assert.Contains(
            MetamagicOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new MetamagicOptionCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new MetamagicOptionCatalog(
            [
                CreateFixedCost(
                    "dnd5e2014.metamagic-option.z",
                    "Z",
                    1,
                    [CreateSource()]),
                CreateFixedCost(
                    "dnd5e2014.metamagic-option.a",
                    "A",
                    1,
                    [CreateSource()])
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.metamagic-option.a",
                "dnd5e2014.metamagic-option.z"
            ],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId = new MetamagicOptionId("dnd5e2014.metamagic-option.a");

        MetamagicOptionDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(aId, out MetamagicOptionDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new MetamagicOptionId("dnd5e2014.metamagic-option.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out MetamagicOptionDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new MetamagicOptionCatalog(
                [
                    CreateFixedCost(
                        "dnd5e2014.metamagic-option.duplicate",
                        "One",
                        1,
                        [CreateSource()]),
                    CreateFixedCost(
                        "dnd5e2014.metamagic-option.duplicate",
                        "Two",
                        1,
                        [CreateSource()])
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        MetamagicOptionDefinition definition = CreateFixedCost(
            "dnd5e2014.metamagic-option.test",
            "Test",
            1,
            []);

        Assert.Throws<InvalidOperationException>(
            () => new MetamagicOptionCatalog([definition]));
    }

    private static MetamagicOptionDefinition CreateFixedCost(
        string? id,
        string name,
        int fixedSorceryPointCost,
        IEnumerable<SourceReference> sources)
    {
        return new MetamagicOptionDefinition(
            id is null ? default : new MetamagicOptionId(id),
            name,
            fixedSorceryPointCost,
            costEqualsSpellLevelWithCantripMinimum: false,
            sources);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 102);
    }
}
