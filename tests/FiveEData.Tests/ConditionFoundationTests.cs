using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Tests;

public sealed class ConditionFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new ConditionId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.condition.test";

        var id = new ConditionId(value);

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

        var definition = Create(
            "dnd5e2014.condition.test",
            "Test",
            sources: sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        ConditionDefinition definition = CreateWithDefaultId();

        Assert.Contains(
            ConditionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "ID",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        var definition = Create(
            "dnd5e2014.condition.test",
            "Test",
            sources: []);

        Assert.Contains(
            ConditionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_RejectsNonPositiveWeightMultiplier(int value)
    {
        var definition = Create(
            "dnd5e2014.condition.test",
            "Test",
            weightMultiplier: value);

        Assert.Contains(
            ConditionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "weight multiplier",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ConditionCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new ConditionCatalog(
            [
                Create(
                    "dnd5e2014.condition.z",
                    "Z"),
                Create(
                    "dnd5e2014.condition.a",
                    "A")
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.condition.a",
                "dnd5e2014.condition.z"
            ],
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        var aId =
            new ConditionId("dnd5e2014.condition.a");

        ConditionDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(
                aId,
                out ConditionDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new ConditionId("dnd5e2014.condition.missing");

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out ConditionDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<ConditionDefinition>
        {
            Create(
                "dnd5e2014.condition.one",
                "One")
        };

        var catalog = new ConditionCatalog(source);

        source.Add(
            Create(
                "dnd5e2014.condition.two",
                "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new ConditionCatalog(
                [
                    Create(
                        "dnd5e2014.condition.duplicate",
                        "One"),
                    Create(
                        "dnd5e2014.condition.duplicate",
                        "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        ConditionDefinition definition = CreateWithDefaultId();

        Assert.Throws<InvalidOperationException>(
            () => new ConditionCatalog([definition]));
    }

    private static ConditionDefinition Create(
        string id,
        string name,
        bool preventsActionsAndReactions = false,
        bool preventsMovement = false,
        bool onlyMovementOptionIsToCrawl = false,
        bool speedBecomesZero = false,
        bool ignoresBonusesToSpeed = false,
        SpeechRestriction speechRestriction = SpeechRestriction.None,
        bool unawareOfSurroundings = false,
        bool automaticallyFailsStrengthAndDexteritySavingThrows = false,
        bool dexteritySavingThrowsHaveDisadvantage = false,
        bool automaticallyFailsAbilityChecksRequiringSight = false,
        bool automaticallyFailsAbilityChecksRequiringHearing = false,
        bool ownAbilityChecksHaveDisadvantage = false,
        RollModifier attackRollsAgainstTheCreature = RollModifier.None,
        RollModifier theCreaturesOwnAttackRolls = RollModifier.None,
        bool anyHitIsACriticalHitIfAttackerIsWithinFiveFeet = false,
        bool requiresSourceInLineOfSightForRollEffects = false,
        bool cannotWillinglyMoveCloserToSource = false,
        bool cannotAttackOrTargetSourceWithHarmfulEffects = false,
        bool sourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature =
            false,
        bool endsIfSourceCreatureIsIncapacitated = false,
        bool resistantToAllDamage = false,
        bool immuneToPoisonAndDisease = false,
        int? weightMultiplier = null,
        bool dropsHeldItemsAndFallsProne = false,
        bool heavilyObscuredForHidingPurposes = false,
        ExhaustionEffectDetail? exhaustionEffect = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new ConditionDefinition(
            id: new ConditionId(id),
            name: name,
            preventsActionsAndReactions: preventsActionsAndReactions,
            preventsMovement: preventsMovement,
            onlyMovementOptionIsToCrawl: onlyMovementOptionIsToCrawl,
            speedBecomesZero: speedBecomesZero,
            ignoresBonusesToSpeed: ignoresBonusesToSpeed,
            speechRestriction: speechRestriction,
            unawareOfSurroundings: unawareOfSurroundings,
            automaticallyFailsStrengthAndDexteritySavingThrows:
                automaticallyFailsStrengthAndDexteritySavingThrows,
            dexteritySavingThrowsHaveDisadvantage:
                dexteritySavingThrowsHaveDisadvantage,
            automaticallyFailsAbilityChecksRequiringSight:
                automaticallyFailsAbilityChecksRequiringSight,
            automaticallyFailsAbilityChecksRequiringHearing:
                automaticallyFailsAbilityChecksRequiringHearing,
            ownAbilityChecksHaveDisadvantage:
                ownAbilityChecksHaveDisadvantage,
            attackRollsAgainstTheCreature: attackRollsAgainstTheCreature,
            theCreaturesOwnAttackRolls: theCreaturesOwnAttackRolls,
            anyHitIsACriticalHitIfAttackerIsWithinFiveFeet:
                anyHitIsACriticalHitIfAttackerIsWithinFiveFeet,
            requiresSourceInLineOfSightForRollEffects:
                requiresSourceInLineOfSightForRollEffects,
            cannotWillinglyMoveCloserToSource:
                cannotWillinglyMoveCloserToSource,
            cannotAttackOrTargetSourceWithHarmfulEffects:
                cannotAttackOrTargetSourceWithHarmfulEffects,
            sourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature:
                sourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature,
            endsIfSourceCreatureIsIncapacitated:
                endsIfSourceCreatureIsIncapacitated,
            resistantToAllDamage: resistantToAllDamage,
            immuneToPoisonAndDisease: immuneToPoisonAndDisease,
            weightMultiplier: weightMultiplier,
            dropsHeldItemsAndFallsProne: dropsHeldItemsAndFallsProne,
            heavilyObscuredForHidingPurposes:
                heavilyObscuredForHidingPurposes,
            exhaustionEffect: exhaustionEffect,
            sources: sources ?? [CreateSource()]);
    }

    private static ConditionDefinition CreateWithDefaultId()
    {
        return new ConditionDefinition(
            id: default,
            name: "Test",
            preventsActionsAndReactions: false,
            preventsMovement: false,
            onlyMovementOptionIsToCrawl: false,
            speedBecomesZero: false,
            ignoresBonusesToSpeed: false,
            speechRestriction: SpeechRestriction.None,
            unawareOfSurroundings: false,
            automaticallyFailsStrengthAndDexteritySavingThrows: false,
            dexteritySavingThrowsHaveDisadvantage: false,
            automaticallyFailsAbilityChecksRequiringSight: false,
            automaticallyFailsAbilityChecksRequiringHearing: false,
            ownAbilityChecksHaveDisadvantage: false,
            attackRollsAgainstTheCreature: RollModifier.None,
            theCreaturesOwnAttackRolls: RollModifier.None,
            anyHitIsACriticalHitIfAttackerIsWithinFiveFeet: false,
            requiresSourceInLineOfSightForRollEffects: false,
            cannotWillinglyMoveCloserToSource: false,
            cannotAttackOrTargetSourceWithHarmfulEffects: false,
            sourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature:
                false,
            endsIfSourceCreatureIsIncapacitated: false,
            resistantToAllDamage: false,
            immuneToPoisonAndDisease: false,
            weightMultiplier: null,
            dropsHeldItemsAndFallsProne: false,
            heavilyObscuredForHidingPurposes: false,
            exhaustionEffect: null,
            sources: [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            page: 290);
    }
}
