using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Languages;

namespace FiveEData.Tests;

public sealed class LanguageFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new LanguageId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.language.test";

        var id = new LanguageId(value);

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

        var definition = new LanguageDefinition(
            new LanguageId(
                "dnd5e2014.language.test"),
            "Test",
            LanguageCategory.Standard,
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        var definition = new LanguageDefinition(
            default,
            "Test",
            LanguageCategory.Standard,
            [CreateSource()]);

        Assert.Contains(
            LanguageDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "ID",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsUndefinedCategory()
    {
        var definition = new LanguageDefinition(
            new LanguageId(
                "dnd5e2014.language.test"),
            "Test",
            (LanguageCategory)99,
            [CreateSource()]);

        Assert.Contains(
            LanguageDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "category",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        var definition = new LanguageDefinition(
            new LanguageId(
                "dnd5e2014.language.test"),
            "Test",
            LanguageCategory.Standard,
            []);

        Assert.Contains(
            LanguageDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new LanguageCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new LanguageCatalog(
            [
                Create(
                    "dnd5e2014.language.z",
                    "Z",
                    LanguageCategory.Exotic),
                Create(
                    "dnd5e2014.language.a",
                    "A",
                    LanguageCategory.Standard)
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.language.a",
                "dnd5e2014.language.z"
            ],
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        var aId =
            new LanguageId("dnd5e2014.language.a");

        LanguageDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(
                aId,
                out LanguageDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new LanguageId("dnd5e2014.language.missing");

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out LanguageDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<LanguageDefinition>
        {
            Create(
                "dnd5e2014.language.one",
                "One",
                LanguageCategory.Standard)
        };

        var catalog = new LanguageCatalog(source);

        source.Add(
            Create(
                "dnd5e2014.language.two",
                "Two",
                LanguageCategory.Exotic));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new LanguageCatalog(
                [
                    Create(
                        "dnd5e2014.language.duplicate",
                        "One",
                        LanguageCategory.Standard),
                    Create(
                        "dnd5e2014.language.duplicate",
                        "Two",
                        LanguageCategory.Exotic)
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        var defaultId = new LanguageDefinition(
            default,
            "Invalid",
            LanguageCategory.Standard,
            [CreateSource()]);

        var undefinedCategory = new LanguageDefinition(
            new LanguageId(
                "dnd5e2014.language.invalid"),
            "Invalid",
            (LanguageCategory)99,
            [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => new LanguageCatalog([defaultId]));
        Assert.Throws<InvalidOperationException>(
            () => new LanguageCatalog([undefinedCategory]));
    }

    private static LanguageDefinition Create(
        string id,
        string name,
        LanguageCategory category)
    {
        return new LanguageDefinition(
            new LanguageId(id),
            name,
            category,
            [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            page: 123);
    }
}
