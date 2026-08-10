using FiveEData.Rules.Adventuring.TravelPace;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.TotemWarriorOptions;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Tests;

public sealed class TotemWarriorOptionFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new TotemWarriorOptionId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.totem-warrior-option.test";

        var id = new TotemWarriorOptionId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference> { CreateSource() };

        TotemWarriorOptionDefinition definition = Create(
            "dnd5e2014.totem-warrior-option.test",
            "Test",
            requiredLevel: 3,
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Definition_ExposesMechanismFieldsWhenPresent()
    {
        TotemWarriorOptionDefinition definition = Create(
            "dnd5e2014.totem-warrior-option.test",
            "Test",
            requiredLevel: 14,
            [CreateSource()],
            requiresRaging: true,
            resistsAllDamageExceptTypeId:
                new DamageTypeId("dnd5e2014.damage-type.psychic"),
            tracksAtTravelPaceId:
                new TravelPaceId("dnd5e2014.travel-pace.fast"),
            imposedConditionId:
                new ConditionId("dnd5e2014.condition.prone"),
            maximumTargetSizeId:
                new CreatureSizeId("dnd5e2014.creature-size.large"),
            imposedConditionRequiresBonusAction: true);

        Assert.True(definition.RequiresRaging);
        Assert.Equal(
            "dnd5e2014.damage-type.psychic",
            definition.ResistsAllDamageExceptTypeId?.Value);
        Assert.Equal(
            "dnd5e2014.travel-pace.fast",
            definition.TracksAtTravelPaceId?.Value);
        Assert.Equal(
            "dnd5e2014.condition.prone",
            definition.ImposedConditionId?.Value);
        Assert.Equal(
            "dnd5e2014.creature-size.large",
            definition.MaximumTargetSizeId?.Value);
        Assert.True(definition.ImposedConditionRequiresBonusAction);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        TotemWarriorOptionDefinition definition = Create(
            null,
            "Test",
            requiredLevel: 3,
            [CreateSource()]);

        Assert.Contains(
            TotemWarriorOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains("ID", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void Validator_RejectsOutOfRangeRequiredLevel(int requiredLevel)
    {
        TotemWarriorOptionDefinition definition = Create(
            "dnd5e2014.totem-warrior-option.test",
            "Test",
            requiredLevel,
            [CreateSource()]);

        Assert.Contains(
            TotemWarriorOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "between 1 and 20",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositiveDistance()
    {
        TotemWarriorOptionDefinition definition = Create(
            "dnd5e2014.totem-warrior-option.test",
            "Test",
            requiredLevel: 3,
            [CreateSource()],
            grantsAlliesAdvantageOnMeleeAttacksWithinFeet: 0);

        Assert.Contains(
            TotemWarriorOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "greater than zero",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsHalfOfTheClearSightPair()
    {
        TotemWarriorOptionDefinition definition = Create(
            "dnd5e2014.totem-warrior-option.test",
            "Test",
            requiredLevel: 6,
            [CreateSource()],
            clearSightRangeFeet: 5280);

        Assert.Contains(
            TotemWarriorOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "clear sight",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMaximumTargetSizeWithoutImposedCondition()
    {
        TotemWarriorOptionDefinition definition = Create(
            "dnd5e2014.totem-warrior-option.test",
            "Test",
            requiredLevel: 14,
            [CreateSource()],
            maximumTargetSizeId:
                new CreatureSizeId("dnd5e2014.creature-size.large"));

        Assert.Contains(
            TotemWarriorOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "maximum target size",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsBonusActionWithoutImposedCondition()
    {
        TotemWarriorOptionDefinition definition = Create(
            "dnd5e2014.totem-warrior-option.test",
            "Test",
            requiredLevel: 14,
            [CreateSource()],
            imposedConditionRequiresBonusAction: true);

        Assert.Contains(
            TotemWarriorOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "bonus action",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        TotemWarriorOptionDefinition definition = Create(
            "dnd5e2014.totem-warrior-option.test",
            "Test",
            requiredLevel: 3,
            []);

        Assert.Contains(
            TotemWarriorOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains("source", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new TotemWarriorOptionCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new TotemWarriorOptionCatalog(
            [
                Create(
                    "dnd5e2014.totem-warrior-option.z",
                    "Z",
                    requiredLevel: 3,
                    [CreateSource()]),
                Create(
                    "dnd5e2014.totem-warrior-option.a",
                    "A",
                    requiredLevel: 3,
                    [CreateSource()])
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.totem-warrior-option.a",
                "dnd5e2014.totem-warrior-option.z"
            ],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId = new TotemWarriorOptionId("dnd5e2014.totem-warrior-option.a");

        TotemWarriorOptionDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(aId, out TotemWarriorOptionDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new TotemWarriorOptionId("dnd5e2014.totem-warrior-option.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out TotemWarriorOptionDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new TotemWarriorOptionCatalog(
                [
                    Create(
                        "dnd5e2014.totem-warrior-option.duplicate",
                        "One",
                        requiredLevel: 3,
                        [CreateSource()]),
                    Create(
                        "dnd5e2014.totem-warrior-option.duplicate",
                        "Two",
                        requiredLevel: 3,
                        [CreateSource()])
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        TotemWarriorOptionDefinition definition = Create(
            "dnd5e2014.totem-warrior-option.test",
            "Test",
            requiredLevel: 3,
            []);

        Assert.Throws<InvalidOperationException>(
            () => new TotemWarriorOptionCatalog([definition]));
    }

    private static TotemWarriorOptionDefinition Create(
        string? id,
        string name,
        int requiredLevel,
        IEnumerable<SourceReference> sources,
        bool requiresRaging = false,
        bool requiresNotWearingHeavyArmor = false,
        DamageTypeId? resistsAllDamageExceptTypeId = null,
        bool imposesDisadvantageOnOpportunityAttacksAgainstYou = false,
        bool grantsDashAsBonusAction = false,
        int? grantsAlliesAdvantageOnMeleeAttacksWithinFeet = null,
        bool doublesCarryingCapacity = false,
        bool grantsAdvantageOnStrengthChecksToMoveObjects = false,
        int? clearSightRangeFeet = null,
        int? clearSightDetailEquivalentRangeFeet = null,
        bool ignoresDimLightPerceptionDisadvantage = false,
        TravelPaceId? tracksAtTravelPaceId = null,
        TravelPaceId? movesStealthilyAtTravelPaceId = null,
        int? imposesDisadvantageOnAttacksAgainstOthersWithinFeet = null,
        bool grantsFlyingSpeedEqualToWalkingSpeed = false,
        ConditionId? imposedConditionId = null,
        CreatureSizeId? maximumTargetSizeId = null,
        bool imposedConditionRequiresBonusAction = false)
    {
        return new TotemWarriorOptionDefinition(
            id: id is null ? default : new TotemWarriorOptionId(id),
            name: name,
            requiredLevel: requiredLevel,
            requiresRaging: requiresRaging,
            requiresNotWearingHeavyArmor: requiresNotWearingHeavyArmor,
            resistsAllDamageExceptTypeId: resistsAllDamageExceptTypeId,
            imposesDisadvantageOnOpportunityAttacksAgainstYou:
                imposesDisadvantageOnOpportunityAttacksAgainstYou,
            grantsDashAsBonusAction: grantsDashAsBonusAction,
            grantsAlliesAdvantageOnMeleeAttacksWithinFeet:
                grantsAlliesAdvantageOnMeleeAttacksWithinFeet,
            doublesCarryingCapacity: doublesCarryingCapacity,
            grantsAdvantageOnStrengthChecksToMoveObjects:
                grantsAdvantageOnStrengthChecksToMoveObjects,
            clearSightRangeFeet: clearSightRangeFeet,
            clearSightDetailEquivalentRangeFeet:
                clearSightDetailEquivalentRangeFeet,
            ignoresDimLightPerceptionDisadvantage:
                ignoresDimLightPerceptionDisadvantage,
            tracksAtTravelPaceId: tracksAtTravelPaceId,
            movesStealthilyAtTravelPaceId: movesStealthilyAtTravelPaceId,
            imposesDisadvantageOnAttacksAgainstOthersWithinFeet:
                imposesDisadvantageOnAttacksAgainstOthersWithinFeet,
            grantsFlyingSpeedEqualToWalkingSpeed:
                grantsFlyingSpeedEqualToWalkingSpeed,
            imposedConditionId: imposedConditionId,
            maximumTargetSizeId: maximumTargetSizeId,
            imposedConditionRequiresBonusAction:
                imposedConditionRequiresBonusAction,
            sources: sources);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 50);
    }
}
