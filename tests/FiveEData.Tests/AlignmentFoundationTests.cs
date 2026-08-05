using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Alignments;

namespace FiveEData.Tests;

public sealed class AlignmentFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new AlignmentId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.alignment.test";

        var id = new AlignmentId(value);

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

        var definition = new AlignmentDefinition(
            new AlignmentId(
                "dnd5e2014.alignment.test"),
            "Test",
            AlignmentEthic.Neutral,
            AlignmentMorality.Neutral,
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        var definition = new AlignmentDefinition(
            default,
            "Test",
            AlignmentEthic.Neutral,
            AlignmentMorality.Neutral,
            [CreateSource()]);

        Assert.Contains(
            AlignmentDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "ID",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsUndefinedEthic()
    {
        var definition = new AlignmentDefinition(
            new AlignmentId(
                "dnd5e2014.alignment.test"),
            "Test",
            (AlignmentEthic)99,
            AlignmentMorality.Neutral,
            [CreateSource()]);

        Assert.Contains(
            AlignmentDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "ethic",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsUndefinedMorality()
    {
        var definition = new AlignmentDefinition(
            new AlignmentId(
                "dnd5e2014.alignment.test"),
            "Test",
            AlignmentEthic.Neutral,
            (AlignmentMorality)99,
            [CreateSource()]);

        Assert.Contains(
            AlignmentDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "morality",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        var definition = new AlignmentDefinition(
            new AlignmentId(
                "dnd5e2014.alignment.test"),
            "Test",
            AlignmentEthic.Neutral,
            AlignmentMorality.Neutral,
            []);

        Assert.Contains(
            AlignmentDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new AlignmentCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new AlignmentCatalog(
            [
                Create(
                    "dnd5e2014.alignment.z",
                    "Z",
                    AlignmentEthic.Chaotic,
                    AlignmentMorality.Evil),
                Create(
                    "dnd5e2014.alignment.a",
                    "A",
                    AlignmentEthic.Lawful,
                    AlignmentMorality.Good)
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.alignment.a",
                "dnd5e2014.alignment.z"
            ],
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        var aId =
            new AlignmentId("dnd5e2014.alignment.a");

        AlignmentDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(
                aId,
                out AlignmentDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new AlignmentId("dnd5e2014.alignment.missing");

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out AlignmentDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<AlignmentDefinition>
        {
            Create(
                "dnd5e2014.alignment.one",
                "One",
                AlignmentEthic.Lawful,
                AlignmentMorality.Good)
        };

        var catalog = new AlignmentCatalog(source);

        source.Add(
            Create(
                "dnd5e2014.alignment.two",
                "Two",
                AlignmentEthic.Chaotic,
                AlignmentMorality.Evil));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new AlignmentCatalog(
                [
                    Create(
                        "dnd5e2014.alignment.duplicate",
                        "One",
                        AlignmentEthic.Lawful,
                        AlignmentMorality.Good),
                    Create(
                        "dnd5e2014.alignment.duplicate",
                        "Two",
                        AlignmentEthic.Chaotic,
                        AlignmentMorality.Evil)
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        var defaultId = new AlignmentDefinition(
            default,
            "Invalid",
            AlignmentEthic.Neutral,
            AlignmentMorality.Neutral,
            [CreateSource()]);

        var undefinedEthic = new AlignmentDefinition(
            new AlignmentId(
                "dnd5e2014.alignment.invalid"),
            "Invalid",
            (AlignmentEthic)99,
            AlignmentMorality.Neutral,
            [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => new AlignmentCatalog([defaultId]));
        Assert.Throws<InvalidOperationException>(
            () => new AlignmentCatalog([undefinedEthic]));
    }

    private static AlignmentDefinition Create(
        string id,
        string name,
        AlignmentEthic ethic,
        AlignmentMorality morality)
    {
        return new AlignmentDefinition(
            new AlignmentId(id),
            name,
            ethic,
            morality,
            [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            page: 122);
    }
}
