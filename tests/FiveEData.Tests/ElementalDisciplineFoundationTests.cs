using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.ElementalDisciplines;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class ElementalDisciplineFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new ElementalDisciplineId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.elemental-discipline.test";

        var id = new ElementalDisciplineId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference> { CreateSource() };

        ElementalDisciplineDefinition definition = Create(
            "dnd5e2014.elemental-discipline.test",
            "Test",
            kiPointCost: 2,
            requiredMinimumLevel: null,
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Definition_AllowsNoKiCostAndNoLevelRequirement()
    {
        ElementalDisciplineDefinition definition = Create(
            "dnd5e2014.elemental-discipline.test",
            "Test",
            kiPointCost: null,
            requiredMinimumLevel: null,
            [CreateSource()]);

        Assert.Null(definition.KiPointCost);
        Assert.Null(definition.RequiredMinimumLevel);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        ElementalDisciplineDefinition definition = Create(
            null,
            "Test",
            kiPointCost: 2,
            requiredMinimumLevel: null,
            [CreateSource()]);

        Assert.Contains(
            ElementalDisciplineDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsNonPositiveKiPointCost()
    {
        var definition = new ElementalDisciplineDefinition(
            new ElementalDisciplineId("dnd5e2014.elemental-discipline.test"),
            "Test",
            kiPointCost: 0,
            requiredMinimumLevel: null,
            [CreateSource()]);

        Assert.Contains(
            ElementalDisciplineDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "greater than zero",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsOutOfRangeRequiredMinimumLevel()
    {
        ElementalDisciplineDefinition definition = Create(
            "dnd5e2014.elemental-discipline.test",
            "Test",
            kiPointCost: 2,
            requiredMinimumLevel: 21,
            [CreateSource()]);

        Assert.Contains(
            ElementalDisciplineDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "between 1 and 20",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        ElementalDisciplineDefinition definition = Create(
            "dnd5e2014.elemental-discipline.test",
            "Test",
            kiPointCost: 2,
            requiredMinimumLevel: null,
            []);

        Assert.Contains(
            ElementalDisciplineDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ElementalDisciplineCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new ElementalDisciplineCatalog(
            [
                Create(
                    "dnd5e2014.elemental-discipline.z",
                    "Z",
                    kiPointCost: 2,
                    requiredMinimumLevel: null,
                    [CreateSource()]),
                Create(
                    "dnd5e2014.elemental-discipline.a",
                    "A",
                    kiPointCost: 2,
                    requiredMinimumLevel: null,
                    [CreateSource()])
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.elemental-discipline.a",
                "dnd5e2014.elemental-discipline.z"
            ],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId =
            new ElementalDisciplineId("dnd5e2014.elemental-discipline.a");

        ElementalDisciplineDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(aId, out ElementalDisciplineDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId = new ElementalDisciplineId(
            "dnd5e2014.elemental-discipline.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out ElementalDisciplineDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new ElementalDisciplineCatalog(
                [
                    Create(
                        "dnd5e2014.elemental-discipline.duplicate",
                        "One",
                        kiPointCost: 2,
                        requiredMinimumLevel: null,
                        [CreateSource()]),
                    Create(
                        "dnd5e2014.elemental-discipline.duplicate",
                        "Two",
                        kiPointCost: 2,
                        requiredMinimumLevel: null,
                        [CreateSource()])
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        ElementalDisciplineDefinition definition = Create(
            "dnd5e2014.elemental-discipline.test",
            "Test",
            kiPointCost: 2,
            requiredMinimumLevel: null,
            []);

        Assert.Throws<InvalidOperationException>(
            () => new ElementalDisciplineCatalog([definition]));
    }

    private static ElementalDisciplineDefinition Create(
        string? id,
        string name,
        int? kiPointCost,
        int? requiredMinimumLevel,
        IEnumerable<SourceReference> sources)
    {
        return new ElementalDisciplineDefinition(
            id is null ? default : new ElementalDisciplineId(id),
            name,
            kiPointCost,
            requiredMinimumLevel,
            sources);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 81);
    }
}
