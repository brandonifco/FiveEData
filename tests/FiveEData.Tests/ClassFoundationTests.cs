using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Auras;
using FiveEData.Rules.Classes.BardicInspiration;
using FiveEData.Rules.Classes.ChannelDivinity;
using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Classes.FontOfMagic;
using FiveEData.Rules.Classes.Ki;
using FiveEData.Rules.Classes.MysticArcanum;
using FiveEData.Rules.Classes.Rage;
using FiveEData.Rules.Classes.SneakAttack;
using FiveEData.Rules.Classes.SorceryPoints;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Classes.WildShape;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Tests;

public sealed class ClassFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(() => new ClassId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.class.test";

        var id = new ClassId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void LevelFeature_RejectsOutOfRangeLevel(int level)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ClassLevelFeature(
                level,
                new RuleId("dnd5e2014.class-rule.test")));
    }

    [Fact]
    public void LevelFeature_RejectsDefaultFeatureRuleId()
    {
        Assert.Throws<ArgumentException>(
            () => new ClassLevelFeature(1, default));
    }

    [Fact]
    public void Definition_DefensivelySnapshotsCollections()
    {
        var primaryAbilityIds = new List<AbilityId>
        {
            new("dnd5e2014.ability.strength")
        };
        var savingThrows = new List<AbilityId>
        {
            new("dnd5e2014.ability.strength"),
            new("dnd5e2014.ability.constitution")
        };
        var armorCategories = new List<ArmorCategory> { ArmorCategory.Light };
        var weaponCategories = new List<WeaponProficiencyCategory>
        {
            WeaponProficiencyCategory.Simple
        };
        var weaponIds = new List<WeaponId>
        {
            new("dnd5e2014.weapon.longsword")
        };
        var skillOptions = new List<SkillId>
        {
            new("dnd5e2014.skill.athletics"),
            new("dnd5e2014.skill.perception")
        };
        var levelFeatures = new List<ClassLevelFeature>
        {
            new(1, new RuleId("dnd5e2014.class-rule.test"))
        };
        var sources = new List<SourceReference> { CreateSource() };

        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            primaryAbilityIds: primaryAbilityIds,
            savingThrowProficiencyIds: savingThrows,
            armorProficiencyCategories: armorCategories,
            weaponProficiencyCategories: weaponCategories,
            weaponProficiencyIds: weaponIds,
            skillChoiceOptionIds: skillOptions,
            levelFeatures: levelFeatures,
            sources: sources);

        primaryAbilityIds.Clear();
        savingThrows.Clear();
        armorCategories.Clear();
        weaponCategories.Clear();
        weaponIds.Clear();
        skillOptions.Clear();
        levelFeatures.Clear();
        sources.Clear();

        Assert.Single(@class.PrimaryAbilityIds);
        Assert.Equal(2, @class.SavingThrowProficiencyIds.Count);
        Assert.Single(@class.ArmorProficiencyCategories);
        Assert.Single(@class.WeaponProficiencyCategories);
        Assert.Single(@class.WeaponProficiencyIds);
        Assert.Equal(2, @class.SkillChoiceOptionIds.Count);
        Assert.Single(@class.LevelFeatures);
        Assert.Single(@class.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        var @class = new ClassDefinition(
            default,
            "Test",
            new DiceExpression(1, 10),
            [new AbilityId("dnd5e2014.ability.strength")],
            false,
            [
                new AbilityId("dnd5e2014.ability.strength"),
                new AbilityId("dnd5e2014.ability.constitution")
            ],
            [],
            false,
            [],
            [],
            0,
            [],
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
            [CreateSource()]);

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsEmptySources()
    {
        ClassDefinition @class = Create("dnd5e2014.class.test", sources: []);

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error => error.Contains(
                "source",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsRageProgressionWithNoUseGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            rageProgression: new RageProgressionDetail(
                [],
                [new RageDamageBonusGrant(1, 2)],
                1,
                [new DamageTypeId("dnd5e2014.damage-type.bludgeoning")],
                true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "at least one uses-per-long-rest",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsRageProgressionWithNoDamageBonusGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            rageProgression: new RageProgressionDetail(
                [new RageUseGrant(1, 2)],
                [],
                1,
                [new DamageTypeId("dnd5e2014.damage-type.bludgeoning")],
                true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "at least one damage bonus",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsRageProgressionWithNonIncreasingDamageBonus()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            rageProgression: new RageProgressionDetail(
                [new RageUseGrant(1, 2)],
                [
                    new RageDamageBonusGrant(9, 3),
                    new RageDamageBonusGrant(16, 3)
                ],
                1,
                [new DamageTypeId("dnd5e2014.damage-type.bludgeoning")],
                true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must be greater than",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AllowsRageUsesToStayUnlimitedAfterReachingIt()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            rageProgression: new RageProgressionDetail(
                [
                    new RageUseGrant(1, 2),
                    new RageUseGrant(20, null)
                ],
                [new RageDamageBonusGrant(1, 2)],
                1,
                [new DamageTypeId("dnd5e2014.damage-type.bludgeoning")],
                true));

        Assert.DoesNotContain(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Rage uses",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsRageProgressionWithDuplicateResistedDamageType()
    {
        var bludgeoning =
            new DamageTypeId("dnd5e2014.damage-type.bludgeoning");

        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            rageProgression: new RageProgressionDetail(
                [new RageUseGrant(1, 2)],
                [new RageDamageBonusGrant(1, 2)],
                1,
                [bludgeoning, bludgeoning],
                true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "duplicated",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsSneakAttackProgressionWithNoDiceGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            sneakAttackProgression: new SneakAttackProgressionDetail(
                [],
                true,
                true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "at least one dice increase",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsSneakAttackProgressionWithNonIncreasingDice()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            sneakAttackProgression: new SneakAttackProgressionDetail(
                [
                    new SneakAttackDiceGrant(3, new DiceExpression(2, 6)),
                    new SneakAttackDiceGrant(5, new DiceExpression(2, 6))
                ],
                true,
                true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must be greater than",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsSneakAttackProgressionWithMixedDieSizes()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            sneakAttackProgression: new SneakAttackProgressionDetail(
                [
                    new SneakAttackDiceGrant(1, new DiceExpression(1, 6)),
                    new SneakAttackDiceGrant(3, new DiceExpression(2, 8))
                ],
                true,
                true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "same damage die size",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsKiProgressionWithNoPointsGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            kiProgression: new KiProgressionDetail([], true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Ki points progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsKiProgressionWithNonIncreasingPoints()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            kiProgression: new KiProgressionDetail(
                [
                    new KiPointsGrant(2, 3),
                    new KiPointsGrant(3, 3)
                ],
                true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must be greater than",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsSorceryPointsProgressionWithNoPointsGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            sorceryPointsProgression: new SorceryPointsProgressionDetail(
                [],
                false));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Sorcery points progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsSorceryPointsProgressionWithNonIncreasingPoints()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            sorceryPointsProgression: new SorceryPointsProgressionDetail(
                [
                    new SorceryPointsGrant(2, 3),
                    new SorceryPointsGrant(3, 3)
                ],
                false));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must be greater than",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsChannelDivinityProgressionWithNoUseGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            channelDivinityProgression: new ChannelDivinityProgressionDetail(
                [],
                recoversOnShortRest: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Channel Divinity uses progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsChannelDivinityProgressionWithNonIncreasingUses()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            channelDivinityProgression: new ChannelDivinityProgressionDetail(
                [
                    new ChannelDivinityUseGrant(2, 2),
                    new ChannelDivinityUseGrant(6, 2)
                ],
                recoversOnShortRest: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must be greater than",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedChannelDivinityProgression()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            channelDivinityProgression: new ChannelDivinityProgressionDetail(
                [
                    new ChannelDivinityUseGrant(2, 1),
                    new ChannelDivinityUseGrant(6, 2),
                    new ChannelDivinityUseGrant(18, 3)
                ],
                recoversOnShortRest: true));

        Assert.Empty(ClassDefinitionValidator.Validate(@class));
    }

    [Fact]
    public void Validator_RejectsMysticArcanumProgressionWithNoGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            mysticArcanumProgression: new MysticArcanumProgressionDetail(
                [],
                recoversOnShortRest: false));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Mystic Arcanum spell level progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsMysticArcanumProgressionWithNonIncreasingSpellLevel()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            mysticArcanumProgression: new MysticArcanumProgressionDetail(
                [
                    new MysticArcanumGrant(11, 7),
                    new MysticArcanumGrant(13, 6)
                ],
                recoversOnShortRest: false));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must be greater than",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedMysticArcanumProgression()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            mysticArcanumProgression: new MysticArcanumProgressionDetail(
                [
                    new MysticArcanumGrant(11, 6),
                    new MysticArcanumGrant(13, 7),
                    new MysticArcanumGrant(15, 8),
                    new MysticArcanumGrant(17, 9)
                ],
                recoversOnShortRest: false));

        Assert.Empty(ClassDefinitionValidator.Validate(@class));
    }

    [Fact]
    public void Validator_RejectsFontOfMagicConversionWithNoSlotCostGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            fontOfMagicConversion: new FontOfMagicConversionDetail([]));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Font of Magic conversion must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsFontOfMagicConversionWithNonIncreasingCost()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            fontOfMagicConversion: new FontOfMagicConversionDetail(
                [
                    new FontOfMagicSlotCostGrant(1, 3),
                    new FontOfMagicSlotCostGrant(2, 3)
                ]));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must be greater than",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedFontOfMagicConversion()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            fontOfMagicConversion: new FontOfMagicConversionDetail(
                [
                    new FontOfMagicSlotCostGrant(1, 2),
                    new FontOfMagicSlotCostGrant(2, 3),
                    new FontOfMagicSlotCostGrant(3, 5),
                    new FontOfMagicSlotCostGrant(4, 6),
                    new FontOfMagicSlotCostGrant(5, 7)
                ]));

        Assert.Empty(ClassDefinitionValidator.Validate(@class));
    }

    [Fact]
    public void Validator_RejectsWildShapeProgressionWithNoFormLimits()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            wildShapeProgression: new WildShapeProgressionDetail(
                [],
                usesPerRest: 2,
                recoversOnShortRest: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Wild Shape progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsWildShapeProgressionWithNonIncreasingChallengeRating()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            wildShapeProgression: new WildShapeProgressionDetail(
                [
                    new WildShapeFormLimit(2, 0.5, false, false),
                    new WildShapeFormLimit(4, 0.5, false, true)
                ],
                usesPerRest: 2,
                recoversOnShortRest: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must be greater than",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedWildShapeProgression()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            wildShapeProgression: new WildShapeProgressionDetail(
                [
                    new WildShapeFormLimit(2, 0.25, false, false),
                    new WildShapeFormLimit(4, 0.5, false, true),
                    new WildShapeFormLimit(8, 1.0, true, true)
                ],
                usesPerRest: 2,
                recoversOnShortRest: true));

        Assert.Empty(ClassDefinitionValidator.Validate(@class));
    }

    [Fact]
    public void Validator_RejectsBardicInspirationProgressionWithNoDieGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            bardicInspirationProgression:
                new BardicInspirationProgressionDetail(
                    [],
                    rangeFeet: 60,
                    durationMinutes: 10));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Bardic Inspiration progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsBardicInspirationProgressionWithNonIncreasingDieSize()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            bardicInspirationProgression:
                new BardicInspirationProgressionDetail(
                    [
                        new BardicInspirationDieGrant(
                            1,
                            new DiceExpression(1, 8)),
                        new BardicInspirationDieGrant(
                            5,
                            new DiceExpression(1, 6))
                    ],
                    rangeFeet: 60,
                    durationMinutes: 10));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must use a larger die",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsBardicInspirationProgressionWithMixedDieCounts()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            bardicInspirationProgression:
                new BardicInspirationProgressionDetail(
                    [
                        new BardicInspirationDieGrant(
                            1,
                            new DiceExpression(1, 6)),
                        new BardicInspirationDieGrant(
                            5,
                            new DiceExpression(2, 8))
                    ],
                    rangeFeet: 60,
                    durationMinutes: 10));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "same number of dice",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedBardicInspirationProgression()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            bardicInspirationProgression:
                new BardicInspirationProgressionDetail(
                    [
                        new BardicInspirationDieGrant(
                            1,
                            new DiceExpression(1, 6)),
                        new BardicInspirationDieGrant(
                            5,
                            new DiceExpression(1, 8)),
                        new BardicInspirationDieGrant(
                            10,
                            new DiceExpression(1, 10)),
                        new BardicInspirationDieGrant(
                            15,
                            new DiceExpression(1, 12))
                    ],
                    rangeFeet: 60,
                    durationMinutes: 10));

        Assert.Empty(ClassDefinitionValidator.Validate(@class));
    }

    [Fact]
    public void Validator_RejectsNoPrimaryAbilities()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            primaryAbilityIds: []);

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error => error.Contains(
                "primary ability",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsSavingThrowCountOtherThanTwo()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            savingThrowProficiencyIds:
            [
                new AbilityId("dnd5e2014.ability.strength")
            ]);

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error => error.Contains(
                "exactly two saving throw",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDuplicateArmorProficiencyCategory()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            armorProficiencyCategories:
            [ArmorCategory.Light, ArmorCategory.Light]);

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error => error.Contains("duplicated", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsSkillChoiceCountExceedingOptions()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            skillChoiceCount: 3,
            skillChoiceOptionIds:
            [
                new SkillId("dnd5e2014.skill.athletics"),
                new SkillId("dnd5e2014.skill.perception")
            ]);

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error => error.Contains(
                "cannot exceed",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDuplicateLevelFeature()
    {
        var ruleId = new RuleId("dnd5e2014.class-rule.test");

        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            levelFeatures:
            [
                new ClassLevelFeature(1, ruleId),
                new ClassLevelFeature(1, ruleId)
            ]);

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error => error.Contains("duplicated", StringComparison.Ordinal));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new ClassCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new ClassCatalog(
            [
                Create("dnd5e2014.class.z", name: "Z"),
                Create("dnd5e2014.class.a", name: "A")
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            ["dnd5e2014.class.a", "dnd5e2014.class.z"],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId = new ClassId("dnd5e2014.class.a");

        ClassDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(catalog.TryGet(aId, out ClassDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId = new ClassId("dnd5e2014.class.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(catalog.TryGet(missingId, out ClassDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<ClassDefinition>
        {
            Create("dnd5e2014.class.one", name: "One")
        };

        var catalog = new ClassCatalog(source);

        source.Add(Create("dnd5e2014.class.two", name: "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new ClassCatalog(
                [
                    Create("dnd5e2014.class.duplicate", name: "One"),
                    Create("dnd5e2014.class.duplicate", name: "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        ClassDefinition @class = Create("dnd5e2014.class.test", sources: []);

        Assert.Throws<InvalidOperationException>(
            () => new ClassCatalog([@class]));
    }

    private static ClassDefinition Create(
        string id,
        string name = "Test",
        DiceExpression? hitDie = null,
        IEnumerable<AbilityId>? primaryAbilityIds = null,
        bool requiresAllPrimaryAbilities = false,
        IEnumerable<AbilityId>? savingThrowProficiencyIds = null,
        IEnumerable<ArmorCategory>? armorProficiencyCategories = null,
        bool proficientWithShields = false,
        IEnumerable<WeaponProficiencyCategory>? weaponProficiencyCategories = null,
        IEnumerable<WeaponId>? weaponProficiencyIds = null,
        int skillChoiceCount = 0,
        IEnumerable<SkillId>? skillChoiceOptionIds = null,
        IEnumerable<ClassLevelFeature>? levelFeatures = null,
        SpellSlotProgressionId? spellSlotProgressionId = null,
        AbilityId? spellcastingAbilityId = null,
        ExtraAttackProgressionId? extraAttackProgressionId = null,
        RageProgressionDetail? rageProgression = null,
        SneakAttackProgressionDetail? sneakAttackProgression = null,
        KiProgressionDetail? kiProgression = null,
        SorceryPointsProgressionDetail? sorceryPointsProgression = null,
        WildShapeProgressionDetail? wildShapeProgression = null,
        AuraOfProtectionDetail? auraOfProtection = null,
        AuraOfCourageDetail? auraOfCourage = null,
        BardicInspirationProgressionDetail? bardicInspirationProgression =
            null,
        ChannelDivinityProgressionDetail? channelDivinityProgression = null,
        MysticArcanumProgressionDetail? mysticArcanumProgression = null,
        FontOfMagicConversionDetail? fontOfMagicConversion = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new ClassDefinition(
            new ClassId(id),
            name,
            hitDie ?? new DiceExpression(1, 10),
            primaryAbilityIds
                ?? [new AbilityId("dnd5e2014.ability.strength")],
            requiresAllPrimaryAbilities,
            savingThrowProficiencyIds
                ?? [
                    new AbilityId("dnd5e2014.ability.strength"),
                    new AbilityId("dnd5e2014.ability.constitution")
                ],
            armorProficiencyCategories ?? [],
            proficientWithShields,
            weaponProficiencyCategories ?? [],
            weaponProficiencyIds ?? [],
            skillChoiceCount,
            skillChoiceOptionIds ?? [],
            levelFeatures ?? [],
            spellSlotProgressionId,
            spellcastingAbilityId,
            extraAttackProgressionId,
            rageProgression,
            sneakAttackProgression,
            kiProgression,
            sorceryPointsProgression,
            wildShapeProgression,
            auraOfProtection,
            auraOfCourage,
            bardicInspirationProgression,
            channelDivinityProgression,
            mysticArcanumProgression,
            fontOfMagicConversion,
            sources ?? [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 71);
    }
}
