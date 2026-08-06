using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.EldritchInvocations;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class EldritchInvocationFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new EldritchInvocationId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.eldritch-invocation.test";

        var id = new EldritchInvocationId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference> { CreateSource() };

        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Definition_ExposesAllPrerequisitesTogetherWhenPresent()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: true,
            requiredMinimumLevel: 15,
            requiresPactBoon: WarlockPactBoon.Chain,
            [CreateSource()]);

        Assert.True(definition.RequiresEldritchBlastCantrip);
        Assert.Equal(15, definition.RequiredMinimumLevel);
        Assert.Equal(WarlockPactBoon.Chain, definition.RequiresPactBoon);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        EldritchInvocationDefinition definition = Create(
            null,
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            [CreateSource()]);

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsOutOfRangeRequiredMinimumLevel()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: 21,
            requiresPactBoon: null,
            [CreateSource()]);

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "between 1 and 20",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            []);

        Assert.Contains(
            EldritchInvocationDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new EldritchInvocationCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new EldritchInvocationCatalog(
            [
                Create(
                    "dnd5e2014.eldritch-invocation.z",
                    "Z",
                    requiresEldritchBlastCantrip: false,
                    requiredMinimumLevel: null,
                    requiresPactBoon: null,
                    [CreateSource()]),
                Create(
                    "dnd5e2014.eldritch-invocation.a",
                    "A",
                    requiresEldritchBlastCantrip: false,
                    requiredMinimumLevel: null,
                    requiresPactBoon: null,
                    [CreateSource()])
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.eldritch-invocation.a",
                "dnd5e2014.eldritch-invocation.z"
            ],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId = new EldritchInvocationId("dnd5e2014.eldritch-invocation.a");

        EldritchInvocationDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(aId, out EldritchInvocationDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new EldritchInvocationId("dnd5e2014.eldritch-invocation.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out EldritchInvocationDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new EldritchInvocationCatalog(
                [
                    Create(
                        "dnd5e2014.eldritch-invocation.duplicate",
                        "One",
                        requiresEldritchBlastCantrip: false,
                        requiredMinimumLevel: null,
                        requiresPactBoon: null,
                        [CreateSource()]),
                    Create(
                        "dnd5e2014.eldritch-invocation.duplicate",
                        "Two",
                        requiresEldritchBlastCantrip: false,
                        requiredMinimumLevel: null,
                        requiresPactBoon: null,
                        [CreateSource()])
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        EldritchInvocationDefinition definition = Create(
            "dnd5e2014.eldritch-invocation.test",
            "Test",
            requiresEldritchBlastCantrip: false,
            requiredMinimumLevel: null,
            requiresPactBoon: null,
            []);

        Assert.Throws<InvalidOperationException>(
            () => new EldritchInvocationCatalog([definition]));
    }

    private static EldritchInvocationDefinition Create(
        string? id,
        string name,
        bool requiresEldritchBlastCantrip,
        int? requiredMinimumLevel,
        WarlockPactBoon? requiresPactBoon,
        IEnumerable<SourceReference> sources)
    {
        return new EldritchInvocationDefinition(
            id is null ? default : new EldritchInvocationId(id),
            name,
            requiresEldritchBlastCantrip,
            requiredMinimumLevel,
            requiresPactBoon,
            sources);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 110);
    }
}
