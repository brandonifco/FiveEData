using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.ThirdEyeOptions;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class ThirdEyeOptionFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new ThirdEyeOptionId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.third-eye-option.test";

        var id = new ThirdEyeOptionId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference> { CreateSource() };

        ThirdEyeOptionDefinition definition = Create(
            "dnd5e2014.third-eye-option.test",
            "Test",
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        ThirdEyeOptionDefinition definition = Create(
            null,
            "Test",
            [CreateSource()]);

        Assert.Contains(
            ThirdEyeOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains("ID", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Validator_RejectsNonPositiveDarkvisionRange(int range)
    {
        ThirdEyeOptionDefinition definition = Create(
            "dnd5e2014.third-eye-option.test",
            "Test",
            [CreateSource()],
            darkvisionRangeFeet: range);

        Assert.Contains(
            ThirdEyeOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "greater than zero",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        ThirdEyeOptionDefinition definition = Create(
            "dnd5e2014.third-eye-option.test",
            "Test",
            []);

        Assert.Contains(
            ThirdEyeOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains("source", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ThirdEyeOptionCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new ThirdEyeOptionCatalog(
            [
                Create("dnd5e2014.third-eye-option.z", "Z", [CreateSource()]),
                Create("dnd5e2014.third-eye-option.a", "A", [CreateSource()])
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            ["dnd5e2014.third-eye-option.a", "dnd5e2014.third-eye-option.z"],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId = new ThirdEyeOptionId("dnd5e2014.third-eye-option.a");

        ThirdEyeOptionDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(aId, out ThirdEyeOptionDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new ThirdEyeOptionId("dnd5e2014.third-eye-option.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(missingId, out ThirdEyeOptionDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new ThirdEyeOptionCatalog(
                [
                    Create(
                        "dnd5e2014.third-eye-option.duplicate",
                        "One",
                        [CreateSource()]),
                    Create(
                        "dnd5e2014.third-eye-option.duplicate",
                        "Two",
                        [CreateSource()])
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        ThirdEyeOptionDefinition definition = Create(
            "dnd5e2014.third-eye-option.test",
            "Test",
            []);

        Assert.Throws<InvalidOperationException>(
            () => new ThirdEyeOptionCatalog([definition]));
    }

    private static ThirdEyeOptionDefinition Create(
        string? id,
        string name,
        IEnumerable<SourceReference> sources,
        int? darkvisionRangeFeet = null,
        int? etherealSightRangeFeet = null,
        int? seeInvisibilityRangeFeet = null,
        bool canReadAllLanguages = false)
    {
        return new ThirdEyeOptionDefinition(
            id: id is null ? default : new ThirdEyeOptionId(id),
            name: name,
            darkvisionRangeFeet: darkvisionRangeFeet,
            etherealSightRangeFeet: etherealSightRangeFeet,
            seeInvisibilityRangeFeet: seeInvisibilityRangeFeet,
            canReadAllLanguages: canReadAllLanguages,
            sources: sources);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 116);
    }
}
