using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.HunterOptions;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Tests;

public sealed class HunterOptionFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new HunterOptionId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.hunter-option.test";

        var id = new HunterOptionId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference> { CreateSource() };

        HunterOptionDefinition definition = Create(
            "dnd5e2014.hunter-option.test",
            "Test",
            requiredLevel: 3,
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Definition_ExposesMechanismFieldsWhenPresent()
    {
        HunterOptionDefinition definition = Create(
            "dnd5e2014.hunter-option.test",
            "Test",
            requiredLevel: 3,
            [CreateSource()],
            extraDamage: new DiceExpression(1, 8),
            oncePerTurn: true,
            requiresTargetBelowHitPointMaximum: true,
            minimumTargetSizeId:
                new CreatureSizeId("dnd5e2014.creature-size.large"));

        Assert.Equal(new DiceExpression(1, 8), definition.ExtraDamage);
        Assert.True(definition.OncePerTurn);
        Assert.True(definition.RequiresTargetBelowHitPointMaximum);
        Assert.Equal(
            "dnd5e2014.creature-size.large",
            definition.MinimumTargetSizeId?.Value);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        HunterOptionDefinition definition = Create(
            null,
            "Test",
            requiredLevel: 3,
            [CreateSource()]);

        Assert.Contains(
            HunterOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains("ID", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void Validator_RejectsOutOfRangeRequiredLevel(int requiredLevel)
    {
        HunterOptionDefinition definition = Create(
            "dnd5e2014.hunter-option.test",
            "Test",
            requiredLevel,
            [CreateSource()]);

        Assert.Contains(
            HunterOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "between 1 and 20",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsOncePerTurnWithNothingToBound()
    {
        HunterOptionDefinition definition = Create(
            "dnd5e2014.hunter-option.test",
            "Test",
            requiredLevel: 3,
            [CreateSource()],
            oncePerTurn: true);

        Assert.Contains(
            HunterOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "once per turn",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsSecondaryRangeWithoutAnExtraAttack()
    {
        HunterOptionDefinition definition = Create(
            "dnd5e2014.hunter-option.test",
            "Test",
            requiredLevel: 3,
            [CreateSource()],
            secondaryTargetRangeFeet: 5);

        Assert.Contains(
            HunterOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "secondary target range",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsHalfOfTheMultiattackPair()
    {
        HunterOptionDefinition definition = Create(
            "dnd5e2014.hunter-option.test",
            "Test",
            requiredLevel: 11,
            [CreateSource()],
            attacksAnyNumberOfCreaturesWithinFeet: 10);

        Assert.Contains(
            HunterOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "multiattack",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsUndefinedMultiattackKind()
    {
        HunterOptionDefinition definition = Create(
            "dnd5e2014.hunter-option.test",
            "Test",
            requiredLevel: 11,
            [CreateSource()],
            attacksAnyNumberOfCreaturesWithinFeet: 10,
            multiattackKind: (HunterMultiattackKind)99);

        Assert.Contains(
            HunterOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "must be defined",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsSaveOutcomeWithoutASavingThrow()
    {
        HunterOptionDefinition definition = Create(
            "dnd5e2014.hunter-option.test",
            "Test",
            requiredLevel: 15,
            [CreateSource()],
            negatesDamageOnSuccessfulSave: true);

        Assert.Contains(
            HunterOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "saving throw",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        HunterOptionDefinition definition = Create(
            "dnd5e2014.hunter-option.test",
            "Test",
            requiredLevel: 3,
            []);

        Assert.Contains(
            HunterOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains("source", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new HunterOptionCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new HunterOptionCatalog(
            [
                Create(
                    "dnd5e2014.hunter-option.z",
                    "Z",
                    requiredLevel: 3,
                    [CreateSource()]),
                Create(
                    "dnd5e2014.hunter-option.a",
                    "A",
                    requiredLevel: 3,
                    [CreateSource()])
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            ["dnd5e2014.hunter-option.a", "dnd5e2014.hunter-option.z"],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId = new HunterOptionId("dnd5e2014.hunter-option.a");

        HunterOptionDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(aId, out HunterOptionDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId = new HunterOptionId("dnd5e2014.hunter-option.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(missingId, out HunterOptionDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new HunterOptionCatalog(
                [
                    Create(
                        "dnd5e2014.hunter-option.duplicate",
                        "One",
                        requiredLevel: 3,
                        [CreateSource()]),
                    Create(
                        "dnd5e2014.hunter-option.duplicate",
                        "Two",
                        requiredLevel: 3,
                        [CreateSource()])
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        HunterOptionDefinition definition = Create(
            "dnd5e2014.hunter-option.test",
            "Test",
            requiredLevel: 3,
            []);

        Assert.Throws<InvalidOperationException>(
            () => new HunterOptionCatalog([definition]));
    }

    private static HunterOptionDefinition Create(
        string? id,
        string name,
        int requiredLevel,
        IEnumerable<SourceReference> sources,
        DiceExpression? extraDamage = null,
        bool oncePerTurn = false,
        bool requiresTargetBelowHitPointMaximum = false,
        CreatureSizeId? minimumTargetSizeId = null,
        bool grantsExtraAttackAgainstDifferentTarget = false,
        int? secondaryTargetRangeFeet = null,
        bool imposesDisadvantageOnOpportunityAttacksAgainstYou = false,
        int? armorClassBonusAgainstSubsequentAttacks = null,
        ConditionId? grantsAdvantageOnSavingThrowsAgainstConditionId = null,
        int? attacksAnyNumberOfCreaturesWithinFeet = null,
        HunterMultiattackKind? multiattackKind = null,
        AbilityId? savingThrowAbilityId = null,
        bool negatesDamageOnSuccessfulSave = false,
        bool halfDamageOnFailedSave = false,
        bool halvesAttackDamageAsReaction = false)
    {
        return new HunterOptionDefinition(
            id: id is null ? default : new HunterOptionId(id),
            name: name,
            requiredLevel: requiredLevel,
            extraDamage: extraDamage,
            oncePerTurn: oncePerTurn,
            requiresTargetBelowHitPointMaximum:
                requiresTargetBelowHitPointMaximum,
            minimumTargetSizeId: minimumTargetSizeId,
            grantsExtraAttackAgainstDifferentTarget:
                grantsExtraAttackAgainstDifferentTarget,
            secondaryTargetRangeFeet: secondaryTargetRangeFeet,
            imposesDisadvantageOnOpportunityAttacksAgainstYou:
                imposesDisadvantageOnOpportunityAttacksAgainstYou,
            armorClassBonusAgainstSubsequentAttacks:
                armorClassBonusAgainstSubsequentAttacks,
            grantsAdvantageOnSavingThrowsAgainstConditionId:
                grantsAdvantageOnSavingThrowsAgainstConditionId,
            attacksAnyNumberOfCreaturesWithinFeet:
                attacksAnyNumberOfCreaturesWithinFeet,
            multiattackKind: multiattackKind,
            savingThrowAbilityId: savingThrowAbilityId,
            negatesDamageOnSuccessfulSave: negatesDamageOnSuccessfulSave,
            halfDamageOnFailedSave: halfDamageOnFailedSave,
            halvesAttackDamageAsReaction: halvesAttackDamageAsReaction,
            sources: sources);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 93);
    }
}
