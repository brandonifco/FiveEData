using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Auras;
using FiveEData.Rules.Classes.BendLuck;
using FiveEData.Rules.Classes.WrathOfTheStorm;
using FiveEData.Rules.Classes.ThunderboltStrike;
using FiveEData.Rules.Classes.ShadowStep;
using FiveEData.Rules.Classes.ImprovedCritical;
using FiveEData.Rules.Classes.HurlThroughHell;
using FiveEData.Rules.Classes.CircleForms;
using FiveEData.Rules.Classes.CombatSuperiority;
using FiveEData.Rules.Classes.DiscipleOfTheElements;
using FiveEData.Rules.Classes.DraconicResilience;
using FiveEData.Rules.Classes.DivineStrike;
using FiveEData.Rules.Classes.MagicalSecrets;
using FiveEData.Rules.Classes.Portent;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Classes.WardingFlare;
using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;

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
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
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
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
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
    public void Validator_RejectsDivineStrikeProgressionWithNoDamageGrants()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            divineStrikeProgression: new DivineStrikeProgressionDetail(
                [],
                fixedDamageTypeId: new DamageTypeId(
                    "dnd5e2014.damage-type.radiant"),
                choosableDamageTypeIds: null,
                matchesWeaponDamageType: false));

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error => error.Contains(
                "at least one",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDivineStrikeProgressionWithNonAscendingDamage()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            divineStrikeProgression: new DivineStrikeProgressionDetail(
                [
                    new DivineStrikeDamageGrant(8, new DiceExpression(2, 8)),
                    new DivineStrikeDamageGrant(14, new DiceExpression(1, 8))
                ],
                fixedDamageTypeId: new DamageTypeId(
                    "dnd5e2014.damage-type.radiant"),
                choosableDamageTypeIds: null,
                matchesWeaponDamageType: false));

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error => error.Contains(
                "greater than",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDivineStrikeProgressionWithMismatchedDieSizes()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            divineStrikeProgression: new DivineStrikeProgressionDetail(
                [
                    new DivineStrikeDamageGrant(8, new DiceExpression(1, 8)),
                    new DivineStrikeDamageGrant(14, new DiceExpression(2, 6))
                ],
                fixedDamageTypeId: new DamageTypeId(
                    "dnd5e2014.damage-type.radiant"),
                choosableDamageTypeIds: null,
                matchesWeaponDamageType: false));

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error => error.Contains(
                "same damage die size",
                StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(null, null, false)]
    [InlineData(
        "dnd5e2014.damage-type.radiant",
        new[] { "dnd5e2014.damage-type.cold", "dnd5e2014.damage-type.fire" },
        false)]
    [InlineData(
        "dnd5e2014.damage-type.radiant",
        null,
        true)]
    public void Validator_RejectsDivineStrikeProgressionWithoutExactlyOneDamageTypeMechanism(
        string? fixedDamageTypeId,
        string[]? choosableDamageTypeIds,
        bool matchesWeaponDamageType)
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            divineStrikeProgression: new DivineStrikeProgressionDetail(
                [new DivineStrikeDamageGrant(8, new DiceExpression(1, 8))],
                fixedDamageTypeId is null
                    ? null
                    : new DamageTypeId(fixedDamageTypeId),
                choosableDamageTypeIds?.Select(value => new DamageTypeId(value)),
                matchesWeaponDamageType));

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error => error.Contains(
                "exactly one",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDivineStrikeProgressionWithSingleChoosableDamageType()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            divineStrikeProgression: new DivineStrikeProgressionDetail(
                [new DivineStrikeDamageGrant(8, new DiceExpression(1, 8))],
                fixedDamageTypeId: null,
                choosableDamageTypeIds:
                [
                    new DamageTypeId("dnd5e2014.damage-type.cold")
                ],
                matchesWeaponDamageType: false));

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error => error.Contains(
                "at least two options",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedDivineStrikeProgression()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            divineStrikeProgression: new DivineStrikeProgressionDetail(
                [
                    new DivineStrikeDamageGrant(8, new DiceExpression(1, 8)),
                    new DivineStrikeDamageGrant(14, new DiceExpression(2, 8))
                ],
                fixedDamageTypeId: new DamageTypeId(
                    "dnd5e2014.damage-type.radiant"),
                choosableDamageTypeIds: null,
                matchesWeaponDamageType: false));

        Assert.Empty(SubclassDefinitionValidator.Validate(subclass));
    }

    [Fact]
    public void Validator_RejectsCircleFormsProgressionWithNoGrants()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            circleFormsProgression: new CircleFormsProgressionDetail([]));

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error =>
                error.Contains(
                    "Circle Forms progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsCircleFormsProgressionWithNonIncreasingChallengeRating()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            circleFormsProgression: new CircleFormsProgressionDetail(
                [
                    new CircleFormsChallengeRatingGrant(2, 1.0),
                    new CircleFormsChallengeRatingGrant(6, 1.0)
                ]));

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error =>
                error.Contains(
                    "must be greater than",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedCircleFormsProgression()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            circleFormsProgression: new CircleFormsProgressionDetail(
                [
                    new CircleFormsChallengeRatingGrant(2, 1.0),
                    new CircleFormsChallengeRatingGrant(6, 2.0),
                    new CircleFormsChallengeRatingGrant(9, 3.0),
                    new CircleFormsChallengeRatingGrant(12, 4.0),
                    new CircleFormsChallengeRatingGrant(15, 5.0),
                    new CircleFormsChallengeRatingGrant(18, 6.0)
                ]));

        Assert.Empty(SubclassDefinitionValidator.Validate(subclass));
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

    [Fact]
    public void BendLuckDetail_RejectsNonPositiveSorceryPointCost()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new BendLuckDetail(0, new DiceExpression(1, 4)));
    }

    [Fact]
    public void Validator_RejectsImprovedCriticalProgressionWithNoGrants()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            improvedCriticalProgression:
                new ImprovedCriticalProgressionDetail([]));

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error =>
                error.Contains(
                    "Improved Critical progression must grant",
                    StringComparison.Ordinal));
    }

    // The threshold must fall, not rise: a rising value is the bug this
    // guards against, because every sibling progression ascends.
    [Fact]
    public void Validator_RejectsImprovedCriticalProgressionWithRisingThreshold()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            improvedCriticalProgression:
                new ImprovedCriticalProgressionDetail(
                    [
                        new CriticalHitThresholdGrant(3, 18),
                        new CriticalHitThresholdGrant(15, 19)
                    ]));

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error =>
                error.Contains(
                    "must be lower than the value at the previous grant",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedTierBSubclassScalars()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            improvedCriticalProgression:
                new ImprovedCriticalProgressionDetail(
                    [
                        new CriticalHitThresholdGrant(3, 19),
                        new CriticalHitThresholdGrant(15, 18)
                    ]),
            shadowStep: new ShadowStepDetail(
                60,
                grantsAdvantageOnNextMeleeAttack: true),
            hurlThroughHell: new HurlThroughHellDetail(
                new DiceExpression(10, 10),
                new DamageTypeId("dnd5e2014.damage-type.psychic"),
                exemptsFiends: true,
                recoversOnLongRest: true),
            wrathOfTheStorm: new WrathOfTheStormDetail(
                5,
                new DiceExpression(2, 8),
                [new DamageTypeId("dnd5e2014.damage-type.lightning")],
                new AbilityId("dnd5e2014.ability.dexterity"),
                halfDamageOnSuccessfulSave: true,
                usesPerRest: new AbilityModifierUsesGrant(
                    new AbilityId("dnd5e2014.ability.wisdom"),
                    recoversOnLongRest: true)),
            thunderboltStrike: new ThunderboltStrikeDetail(
                10,
                new CreatureSizeId("dnd5e2014.creature-size.large")));

        Assert.Empty(SubclassDefinitionValidator.Validate(subclass));
    }

    [Fact]
    public void CriticalHitThresholdGrant_RejectsOutOfRangeMinimumRoll()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new CriticalHitThresholdGrant(3, 21));
    }

    [Fact]
    public void ShadowStepDetail_RejectsNonPositiveTeleportRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ShadowStepDetail(
                0,
                grantsAdvantageOnNextMeleeAttack: true));
    }

    [Fact]
    public void ThunderboltStrikeDetail_RejectsDefaultMaximumTargetSizeId()
    {
        Assert.Throws<ArgumentException>(
            () => new ThunderboltStrikeDetail(10, default));
    }

    [Fact]
    public void WrathOfTheStormDetail_RejectsNonPositiveTriggerRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new WrathOfTheStormDetail(
                0,
                new DiceExpression(2, 8),
                [],
                new AbilityId("dnd5e2014.ability.dexterity"),
                halfDamageOnSuccessfulSave: true,
                usesPerRest: new AbilityModifierUsesGrant(
                    new AbilityId("dnd5e2014.ability.wisdom"),
                    recoversOnLongRest: true)));
    }

    [Fact]
    public void Validator_RejectsPortentProgressionWithNoGrants()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            portentProgression: new PortentProgressionDetail(
                [],
                oncePerTurn: true,
                recoversOnLongRest: true));

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error =>
                error.Contains(
                    "Portent progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsPortentProgressionWithNonIncreasingRolls()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            portentProgression: new PortentProgressionDetail(
                [
                    new PortentRollGrant(2, 3),
                    new PortentRollGrant(14, 2)
                ],
                oncePerTurn: true,
                recoversOnLongRest: true));

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error =>
                error.Contains(
                    "must be greater than the value at the previous grant",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsSubclassMagicalSecretsProgressionWithNoGrants()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            magicalSecretsProgression: new MagicalSecretsProgressionDetail(
                [],
                countsAgainstSpellsKnown: false));

        Assert.Contains(
            SubclassDefinitionValidator.Validate(subclass),
            error =>
                error.Contains(
                    "Magical Secrets progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_AcceptsWellFormedQuantizedSubclassFeatures()
    {
        SubclassDefinition subclass = Create(
            "dnd5e2014.subclass.test",
            magicalSecretsProgression: new MagicalSecretsProgressionDetail(
                [new MagicalSecretsChoiceGrant(6, 2)],
                countsAgainstSpellsKnown: false),
            portentProgression: new PortentProgressionDetail(
                [
                    new PortentRollGrant(2, 2),
                    new PortentRollGrant(14, 3)
                ],
                oncePerTurn: true,
                recoversOnLongRest: true),
            draconicResilience: new DraconicResilienceDetail(
                hitPointBonusPerLevel: 1,
                unarmoredArmorClass: new ArmorClassFormula(
                    13,
                    includesDexterityModifier: true)));

        Assert.Empty(SubclassDefinitionValidator.Validate(subclass));
    }

    [Fact]
    public void PortentRollGrant_RejectsNonPositiveForetellingRolls()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new PortentRollGrant(2, 0));
    }

    [Fact]
    public void DraconicResilienceDetail_RejectsNonPositiveHitPointBonus()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new DraconicResilienceDetail(
                hitPointBonusPerLevel: 0,
                unarmoredArmorClass: new ArmorClassFormula(
                    13,
                    includesDexterityModifier: true)));
    }

    private static SubclassDefinition Create(
        string id,
        string name = "Test",
        string classId = "dnd5e2014.class.fighter",
        int chosenAtLevel = 3,
        IEnumerable<ClassLevelFeature>? levelFeatures = null,
        SpellSlotProgressionId? spellSlotProgressionId = null,
        AbilityId? spellcastingAbilityId = null,
        DivineStrikeProgressionDetail? divineStrikeProgression = null,
        CircleFormsProgressionDetail? circleFormsProgression = null,
        AuraOfDevotionDetail? auraOfDevotion = null,
        AuraOfWardingDetail? auraOfWarding = null,
        CombatSuperiorityProgressionDetail? combatSuperiorityProgression =
            null,
        DiscipleOfTheElementsProgressionDetail?
            discipleOfTheElementsProgression = null,
        MagicalSecretsProgressionDetail? magicalSecretsProgression = null,
        PortentProgressionDetail? portentProgression = null,
        DraconicResilienceDetail? draconicResilience = null,
        ImprovedCriticalProgressionDetail? improvedCriticalProgression = null,
        ShadowStepDetail? shadowStep = null,
        HurlThroughHellDetail? hurlThroughHell = null,
        WrathOfTheStormDetail? wrathOfTheStorm = null,
        ThunderboltStrikeDetail? thunderboltStrike = null,
        int? shadowArtsKiCost = null,
        int? quiveringPalmKiCost = null,
        int? draconicPresenceSorceryPointCost = null,
        BendLuckDetail? bendLuck = null,
        WardingFlareDetail? wardingFlare = null,
        AbilityModifierUsesGrant? warPriestUsesPerRest = null,
        IEnumerable<SpellGrant>? innateSpellGrants = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new SubclassDefinition(
            new SubclassId(id),
            name,
            new ClassId(classId),
            chosenAtLevel,
            levelFeatures ?? [],
            spellSlotProgressionId,
            spellcastingAbilityId,
            divineStrikeProgression,
            circleFormsProgression,
            auraOfDevotion,
            auraOfWarding,
            combatSuperiorityProgression,
            discipleOfTheElementsProgression,
            magicalSecretsProgression,
            portentProgression,
            draconicResilience,
            improvedCriticalProgression,
            shadowStep,
            hurlThroughHell,
            wrathOfTheStorm,
            thunderboltStrike,
            shadowArtsKiCost,
            quiveringPalmKiCost,
            draconicPresenceSorceryPointCost,
            bendLuck,
            wardingFlare,
            warPriestUsesPerRest,
            innateSpellGrants ?? [],
            sources ?? [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 73);
    }
}
