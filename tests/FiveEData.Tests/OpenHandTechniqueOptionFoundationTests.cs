using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.OpenHandTechniqueOptions;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Tests;

public sealed class OpenHandTechniqueOptionFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new OpenHandTechniqueOptionId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.open-hand-technique-option.test";

        var id = new OpenHandTechniqueOptionId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference> { CreateSource() };

        OpenHandTechniqueOptionDefinition definition = Create(
            "dnd5e2014.open-hand-technique-option.test",
            "Test",
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        OpenHandTechniqueOptionDefinition definition = Create(
            null,
            "Test",
            [CreateSource()]);

        Assert.Contains(
            OpenHandTechniqueOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains("ID", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositivePushDistance()
    {
        OpenHandTechniqueOptionDefinition definition = Create(
            "dnd5e2014.open-hand-technique-option.test",
            "Test",
            [CreateSource()],
            savingThrowAbilityId:
                new AbilityId("dnd5e2014.ability.strength"),
            pushDistanceFeet: 0);

        Assert.Contains(
            OpenHandTechniqueOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "greater than zero",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsConditionWithoutASavingThrow()
    {
        OpenHandTechniqueOptionDefinition definition = Create(
            "dnd5e2014.open-hand-technique-option.test",
            "Test",
            [CreateSource()],
            imposedConditionId:
                new ConditionId("dnd5e2014.condition.prone"));

        Assert.Contains(
            OpenHandTechniqueOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "impose a condition",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDurationWithoutReactionPrevention()
    {
        OpenHandTechniqueOptionDefinition definition = Create(
            "dnd5e2014.open-hand-technique-option.test",
            "Test",
            [CreateSource()],
            preventsReactionsUntil:
                NextTurnDurationTrigger.EndOfYourNextTurn);

        Assert.Contains(
            OpenHandTechniqueOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "reaction",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        OpenHandTechniqueOptionDefinition definition = Create(
            "dnd5e2014.open-hand-technique-option.test",
            "Test",
            []);

        Assert.Contains(
            OpenHandTechniqueOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains("source", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new OpenHandTechniqueOptionCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new OpenHandTechniqueOptionCatalog(
            [
                Create(
                    "dnd5e2014.open-hand-technique-option.z",
                    "Z",
                    [CreateSource()]),
                Create(
                    "dnd5e2014.open-hand-technique-option.a",
                    "A",
                    [CreateSource()])
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.open-hand-technique-option.a",
                "dnd5e2014.open-hand-technique-option.z"
            ],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId = new OpenHandTechniqueOptionId(
            "dnd5e2014.open-hand-technique-option.a");

        OpenHandTechniqueOptionDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(
                aId,
                out OpenHandTechniqueOptionDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId = new OpenHandTechniqueOptionId(
            "dnd5e2014.open-hand-technique-option.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out OpenHandTechniqueOptionDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new OpenHandTechniqueOptionCatalog(
                [
                    Create(
                        "dnd5e2014.open-hand-technique-option.duplicate",
                        "One",
                        [CreateSource()]),
                    Create(
                        "dnd5e2014.open-hand-technique-option.duplicate",
                        "Two",
                        [CreateSource()])
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        OpenHandTechniqueOptionDefinition definition = Create(
            "dnd5e2014.open-hand-technique-option.test",
            "Test",
            []);

        Assert.Throws<InvalidOperationException>(
            () => new OpenHandTechniqueOptionCatalog([definition]));
    }

    private static OpenHandTechniqueOptionDefinition Create(
        string? id,
        string name,
        IEnumerable<SourceReference> sources,
        AbilityId? savingThrowAbilityId = null,
        ConditionId? imposedConditionId = null,
        int? pushDistanceFeet = null,
        bool preventsReactions = false,
        NextTurnDurationTrigger? preventsReactionsUntil = null)
    {
        return new OpenHandTechniqueOptionDefinition(
            id: id is null ? default : new OpenHandTechniqueOptionId(id),
            name: name,
            savingThrowAbilityId: savingThrowAbilityId,
            imposedConditionId: imposedConditionId,
            pushDistanceFeet: pushDistanceFeet,
            preventsReactions: preventsReactions,
            preventsReactionsUntil: preventsReactionsUntil,
            sources: sources);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 79);
    }
}
