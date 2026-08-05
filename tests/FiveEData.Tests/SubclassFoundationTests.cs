using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class SubclassFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(() => new SubclassId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.subclass.test";

        var id = new SubclassId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsCollections()
    {
        var levelFeatures = new List<ClassLevelFeature>
        {
            new(3, new RuleId("dnd5e2014.class-rule.test"))
        };
        var sources = new List<SourceReference> { CreateSource() };

        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            levelFeatures: levelFeatures,
            sources: sources);

        levelFeatures.Clear();
        sources.Clear();

        Assert.Single(subclass.LevelFeatures);
        Assert.Single(subclass.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        var subclass = new SubclassDefinition(
            default,
            "Test",
            new ClassId("dnd5e2014.class.fighter"),
            3,
            [],
            [CreateSource()]);

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsDefaultClassId()
    {
        var subclass = new SubclassDefinition(
            new SubclassId("dnd5e2014.subclass.test"),
            "Test",
            default,
            3,
            [],
            [CreateSource()]);

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error => error.Contains("class ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            sources: []);

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error => error.Contains(
                "source",
                StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void Validator_RejectsOutOfRangeChosenAtLevel(int level)
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            chosenAtLevel: level);

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error => error.Contains(
                "chosen-at level",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDuplicateLevelFeature()
    {
        var ruleId = new RuleId("dnd5e2014.class-rule.test");

        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            levelFeatures:
            [
                new ClassLevelFeature(3, ruleId),
                new ClassLevelFeature(3, ruleId)
            ]);

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error => error.Contains("duplicated", StringComparison.Ordinal));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new SubclassCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new SubclassCatalog(
            [
                Create("dnd5e2014.subclass.z", name: "Z"),
                Create("dnd5e2014.subclass.a", name: "A")
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            ["dnd5e2014.subclass.a", "dnd5e2014.subclass.z"],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId = new SubclassId("dnd5e2014.subclass.a");

        SubclassDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(catalog.TryGet(aId, out SubclassDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId = new SubclassId("dnd5e2014.subclass.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(missingId, out SubclassDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<SubclassDefinition>
        {
            Create("dnd5e2014.subclass.one", name: "One")
        };

        var catalog = new SubclassCatalog(source);

        source.Add(Create("dnd5e2014.subclass.two", name: "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new SubclassCatalog(
                [
                    Create("dnd5e2014.subclass.duplicate", name: "One"),
                    Create("dnd5e2014.subclass.duplicate", name: "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            sources: []);

        Assert.Throws<InvalidOperationException>(
            () => new SubclassCatalog([subclass]));
    }

    private static SubclassDefinition Create(
        string id,
        string name = "Test",
        string classId = "dnd5e2014.class.fighter",
        int chosenAtLevel = 3,
        IEnumerable<ClassLevelFeature>? levelFeatures = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new SubclassDefinition(
            new SubclassId(id),
            name,
            new ClassId(classId),
            chosenAtLevel,
            levelFeatures ?? [],
            sources ?? [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 73);
    }
}
