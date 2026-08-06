using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class ExtraAttackProgressionFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new ExtraAttackProgressionId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.extra-attack-progression.test";

        var id = new ExtraAttackProgressionId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void Grant_RejectsOutOfRangeCharacterLevel(int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ExtraAttackGrant(characterLevel, 2));
    }

    [Fact]
    public void Grant_RejectsAttackCountBelowTwo()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ExtraAttackGrant(5, 1));
    }

    [Fact]
    public void Definition_DefensivelySnapshotsGrantsAndSources()
    {
        var grants = new List<ExtraAttackGrant> { new(5, 2) };
        var sources = new List<SourceReference> { CreateSource() };

        ExtraAttackProgressionDefinition definition = Create(
            "dnd5e2014.extra-attack-progression.test",
            grants,
            sources);

        grants.Clear();
        sources.Clear();

        Assert.Single(definition.Grants);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        ExtraAttackProgressionDefinition definition = Create(
            null,
            [new ExtraAttackGrant(5, 2)],
            [CreateSource()]);

        Assert.Contains(
            ExtraAttackProgressionDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsNoGrants()
    {
        ExtraAttackProgressionDefinition definition = Create(
            "dnd5e2014.extra-attack-progression.test",
            [],
            [CreateSource()]);

        Assert.Contains(
            ExtraAttackProgressionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "at least one attack increase",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDuplicateCharacterLevel()
    {
        ExtraAttackProgressionDefinition definition = Create(
            "dnd5e2014.extra-attack-progression.test",
            [new ExtraAttackGrant(5, 2), new ExtraAttackGrant(5, 3)],
            [CreateSource()]);

        Assert.Contains(
            ExtraAttackProgressionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "duplicated",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonIncreasingAttackCount()
    {
        ExtraAttackProgressionDefinition definition = Create(
            "dnd5e2014.extra-attack-progression.test",
            [new ExtraAttackGrant(5, 3), new ExtraAttackGrant(11, 2)],
            [CreateSource()]);

        Assert.Contains(
            ExtraAttackProgressionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "must be greater than",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        ExtraAttackProgressionDefinition definition = Create(
            "dnd5e2014.extra-attack-progression.test",
            [new ExtraAttackGrant(5, 2)],
            []);

        Assert.Contains(
            ExtraAttackProgressionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ExtraAttackProgressionCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new ExtraAttackProgressionCatalog(
            [
                Create(
                    "dnd5e2014.extra-attack-progression.z",
                    [new ExtraAttackGrant(5, 2)],
                    [CreateSource()]),
                Create(
                    "dnd5e2014.extra-attack-progression.a",
                    [new ExtraAttackGrant(5, 2)],
                    [CreateSource()])
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.extra-attack-progression.a",
                "dnd5e2014.extra-attack-progression.z"
            ],
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        var aId = new ExtraAttackProgressionId(
            "dnd5e2014.extra-attack-progression.a");

        ExtraAttackProgressionDefinition found = catalog.Get(aId);

        Assert.True(
            catalog.TryGet(
                aId,
                out ExtraAttackProgressionDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId = new ExtraAttackProgressionId(
            "dnd5e2014.extra-attack-progression.missing");

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out ExtraAttackProgressionDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new ExtraAttackProgressionCatalog(
                [
                    Create(
                        "dnd5e2014.extra-attack-progression.duplicate",
                        [new ExtraAttackGrant(5, 2)],
                        [CreateSource()]),
                    Create(
                        "dnd5e2014.extra-attack-progression.duplicate",
                        [new ExtraAttackGrant(5, 2)],
                        [CreateSource()])
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        ExtraAttackProgressionDefinition definition = Create(
            "dnd5e2014.extra-attack-progression.test",
            [],
            [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => new ExtraAttackProgressionCatalog([definition]));
    }

    private static ExtraAttackProgressionDefinition Create(
        string? id,
        IEnumerable<ExtraAttackGrant> grants,
        IEnumerable<SourceReference> sources)
    {
        return new ExtraAttackProgressionDefinition(
            id is null ? default : new ExtraAttackProgressionId(id),
            "Test",
            grants,
            sources);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 49);
    }
}
