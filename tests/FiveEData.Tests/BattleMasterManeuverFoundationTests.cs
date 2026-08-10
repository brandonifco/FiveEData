using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.BattleMasterManeuvers;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Tests;

public sealed class BattleMasterManeuverFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new BattleMasterManeuverId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.battle-master-maneuver.test";

        var id = new BattleMasterManeuverId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference> { CreateSource() };

        BattleMasterManeuverDefinition definition = Create(
            "dnd5e2014.battle-master-maneuver.test",
            "Test",
            BattleMasterManeuverEffectTarget.DamageRoll,
            null,
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Definition_ExposesSavingThrowAbilityWhenPresent()
    {
        BattleMasterManeuverDefinition definition = Create(
            "dnd5e2014.battle-master-maneuver.test",
            "Test",
            BattleMasterManeuverEffectTarget.DamageRoll,
            new AbilityId("dnd5e2014.ability.strength"),
            [CreateSource()]);

        Assert.Equal(
            "dnd5e2014.ability.strength",
            definition.SavingThrowAbilityId?.Value);
    }

    [Fact]
    public void Definition_ExposesImposedConditionAndMaximumTargetSize()
    {
        BattleMasterManeuverDefinition definition = Create(
            "dnd5e2014.battle-master-maneuver.test",
            "Test",
            BattleMasterManeuverEffectTarget.DamageRoll,
            new AbilityId("dnd5e2014.ability.strength"),
            [CreateSource()],
            imposedConditionId: new ConditionId("dnd5e2014.condition.prone"),
            maximumTargetSizeId:
                new CreatureSizeId("dnd5e2014.creature-size.large"));

        Assert.Equal(
            "dnd5e2014.condition.prone",
            definition.ImposedConditionId?.Value);
        Assert.Equal(
            "dnd5e2014.creature-size.large",
            definition.MaximumTargetSizeId?.Value);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        BattleMasterManeuverDefinition definition = Create(
            null,
            "Test",
            BattleMasterManeuverEffectTarget.DamageRoll,
            null,
            [CreateSource()]);

        Assert.Contains(
            BattleMasterManeuverDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        BattleMasterManeuverDefinition definition = Create(
            "dnd5e2014.battle-master-maneuver.test",
            "Test",
            BattleMasterManeuverEffectTarget.DamageRoll,
            null,
            []);

        Assert.Contains(
            BattleMasterManeuverDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositivePushDistanceFeet()
    {
        BattleMasterManeuverDefinition definition = Create(
            "dnd5e2014.battle-master-maneuver.test",
            "Test",
            BattleMasterManeuverEffectTarget.DamageRoll,
            null,
            [CreateSource()],
            pushDistanceFeet: 0);

        Assert.Contains(
            BattleMasterManeuverDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "push distance",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositiveReachIncreaseFeet()
    {
        BattleMasterManeuverDefinition definition = Create(
            "dnd5e2014.battle-master-maneuver.test",
            "Test",
            BattleMasterManeuverEffectTarget.DamageRoll,
            null,
            [CreateSource()],
            reachIncreaseFeet: 0);

        Assert.Contains(
            BattleMasterManeuverDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "reach increase",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositiveSecondaryTargetRangeFeet()
    {
        BattleMasterManeuverDefinition definition = Create(
            "dnd5e2014.battle-master-maneuver.test",
            "Test",
            BattleMasterManeuverEffectTarget.DamageRoll,
            null,
            [CreateSource()],
            secondaryTargetRangeFeet: 0);

        Assert.Contains(
            BattleMasterManeuverDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "secondary target range",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void
        Validator_RejectsDurationTriggerWithoutABoundedSecondaryEffect()
    {
        BattleMasterManeuverDefinition definition = Create(
            "dnd5e2014.battle-master-maneuver.test",
            "Test",
            BattleMasterManeuverEffectTarget.DamageRoll,
            null,
            [CreateSource()],
            secondaryEffectDurationTrigger:
                BattleMasterManeuverDurationTrigger.EndOfYourNextTurn);

        Assert.Contains(
            BattleMasterManeuverDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "duration trigger",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new BattleMasterManeuverCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new BattleMasterManeuverCatalog(
            [
                Create(
                    "dnd5e2014.battle-master-maneuver.z",
                    "Z",
                    BattleMasterManeuverEffectTarget.DamageRoll,
                    null,
                    [CreateSource()]),
                Create(
                    "dnd5e2014.battle-master-maneuver.a",
                    "A",
                    BattleMasterManeuverEffectTarget.DamageRoll,
                    null,
                    [CreateSource()])
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.battle-master-maneuver.a",
                "dnd5e2014.battle-master-maneuver.z"
            ],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId = new BattleMasterManeuverId(
            "dnd5e2014.battle-master-maneuver.a");

        BattleMasterManeuverDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(
                aId,
                out BattleMasterManeuverDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId = new BattleMasterManeuverId(
            "dnd5e2014.battle-master-maneuver.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out BattleMasterManeuverDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new BattleMasterManeuverCatalog(
                [
                    Create(
                        "dnd5e2014.battle-master-maneuver.duplicate",
                        "One",
                        BattleMasterManeuverEffectTarget.DamageRoll,
                        null,
                        [CreateSource()]),
                    Create(
                        "dnd5e2014.battle-master-maneuver.duplicate",
                        "Two",
                        BattleMasterManeuverEffectTarget.DamageRoll,
                        null,
                        [CreateSource()])
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        BattleMasterManeuverDefinition definition = Create(
            "dnd5e2014.battle-master-maneuver.test",
            "Test",
            BattleMasterManeuverEffectTarget.DamageRoll,
            null,
            []);

        Assert.Throws<InvalidOperationException>(
            () => new BattleMasterManeuverCatalog([definition]));
    }

    private static BattleMasterManeuverDefinition Create(
        string? id,
        string name,
        BattleMasterManeuverEffectTarget effectTarget,
        AbilityId? savingThrowAbilityId,
        IEnumerable<SourceReference> sources,
        ConditionId? imposedConditionId = null,
        CreatureSizeId? maximumTargetSizeId = null,
        int? pushDistanceFeet = null,
        int? reachIncreaseFeet = null,
        int? secondaryTargetRangeFeet = null,
        bool forcesDroppedItem = false,
        bool grantsAdvantageOnNextAttackRoll = false,
        bool grantsAdvantageToNextAttackAgainstTarget = false,
        bool imposesDisadvantageOnAttacksAgainstOthers = false,
        bool allowsAllyReactionMovement = false,
        BattleMasterManeuverDurationTrigger?
            secondaryEffectDurationTrigger = null)
    {
        return new BattleMasterManeuverDefinition(
            id: id is null ? default : new BattleMasterManeuverId(id),
            name: name,
            effectTarget: effectTarget,
            savingThrowAbilityId: savingThrowAbilityId,
            imposedConditionId: imposedConditionId,
            maximumTargetSizeId: maximumTargetSizeId,
            pushDistanceFeet: pushDistanceFeet,
            reachIncreaseFeet: reachIncreaseFeet,
            secondaryTargetRangeFeet: secondaryTargetRangeFeet,
            forcesDroppedItem: forcesDroppedItem,
            grantsAdvantageOnNextAttackRoll:
                grantsAdvantageOnNextAttackRoll,
            grantsAdvantageToNextAttackAgainstTarget:
                grantsAdvantageToNextAttackAgainstTarget,
            imposesDisadvantageOnAttacksAgainstOthers:
                imposesDisadvantageOnAttacksAgainstOthers,
            allowsAllyReactionMovement: allowsAllyReactionMovement,
            secondaryEffectDurationTrigger: secondaryEffectDurationTrigger,
            sources: sources);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 74);
    }
}
