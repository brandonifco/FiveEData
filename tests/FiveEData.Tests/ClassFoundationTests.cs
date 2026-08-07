using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.ActionSurge;
using FiveEData.Rules.Classes.Auras;
using FiveEData.Rules.Classes.EmptyBody;
using FiveEData.Rules.Classes.PrimalChampion;
using FiveEData.Rules.Classes.ImprovedDivineSmite;
using FiveEData.Rules.Classes.FeralSenses;
using FiveEData.Rules.Classes.DivineSense;
using FiveEData.Rules.Classes.Blindsense;
using FiveEData.Rules.Classes.BardicInspiration;
using FiveEData.Rules.Classes.BrutalCritical;
using FiveEData.Rules.Classes.ChannelDivinity;
using FiveEData.Rules.Classes.DestroyUndead;
using FiveEData.Rules.Classes.EldritchInvocationsKnown;
using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Classes.FastMovement;
using FiveEData.Rules.Classes.FavoredEnemy;
using FiveEData.Rules.Classes.FontOfMagic;
using FiveEData.Rules.Classes.Indomitable;
using FiveEData.Rules.Classes.Ki;
using FiveEData.Rules.Classes.MagicalSecrets;
using FiveEData.Rules.Classes.MartialArts;
using FiveEData.Rules.Classes.MysticArcanum;
using FiveEData.Rules.Classes.NaturalExplorer;
using FiveEData.Rules.Classes.Rage;
using FiveEData.Rules.Classes.SneakAttack;
using FiveEData.Rules.Classes.SongOfRest;
using FiveEData.Rules.Classes.SorceryPoints;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Classes.UnarmoredMovement;
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
    public void EmptyBodyDetail_RejectsNonPositiveAstralProjectionKiCost()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new EmptyBodyDetail(
                invisibilityKiCost: 4,
                invisibilityDurationMinutes: 1,
                astralProjectionKiCost: 0));
    }

    [Fact]
    public void BlindsenseDetail_RejectsNonPositiveRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new BlindsenseDetail(0, requiresHearing: true));
    }

    [Fact]
    public void FeralSensesDetail_RejectsNonPositiveRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new FeralSensesDetail(
                0,
                negatesUnseenAttackDisadvantage: true));
    }

    [Fact]
    public void DivineSenseDetail_RejectsNonPositiveRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new DivineSenseDetail(0, recoversOnLongRest: true));
    }

    [Fact]
    public void ImprovedDivineSmiteDetail_RejectsDefaultDamageTypeId()
    {
        Assert.Throws<ArgumentException>(
            () => new ImprovedDivineSmiteDetail(
                new DiceExpression(1, 8),
                default,
                requiresMeleeWeapon: true));
    }

    [Fact]
    public void PrimalChampionDetail_RejectsNonPositiveIncrease()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new PrimalChampionDetail(
                [new AbilityId("dnd5e2014.ability.strength")],
                abilityScoreIncrease: 0,
                maximumAbilityScore: 24));
    }

    [Fact]
    public void PrimalChampionDetail_DefensivelySnapshotsAbilityIds()
    {
        var abilityIds = new List<AbilityId>
        {
            new("dnd5e2014.ability.strength")
        };

        var detail = new PrimalChampionDetail(abilityIds, 4, 24);

        abilityIds.Clear();

        Assert.Single(detail.AbilityIds);
    }

    [Fact]
    public void Validator_RejectsMagicalSecretsProgressionWithNoGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            magicalSecretsProgression: new MagicalSecretsProgressionDetail(
                [],
                countsAgainstSpellsKnown: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Magical Secrets spells known progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsMagicalSecretsProgressionWithNonIncreasingCount()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            magicalSecretsProgression: new MagicalSecretsProgressionDetail(
                [
                    new MagicalSecretsChoiceGrant(10, 4),
                    new MagicalSecretsChoiceGrant(14, 2)
                ],
                countsAgainstSpellsKnown: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must be greater than the value at the previous grant",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedMagicalSecretsProgression()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            magicalSecretsProgression: new MagicalSecretsProgressionDetail(
                [
                    new MagicalSecretsChoiceGrant(10, 2),
                    new MagicalSecretsChoiceGrant(14, 4),
                    new MagicalSecretsChoiceGrant(18, 6)
                ],
                countsAgainstSpellsKnown: true));

        Assert.Empty(ClassDefinitionValidator.Validate(@class));
    }

    [Fact]
    public void MagicalSecretsChoiceGrant_RejectsNonPositiveSpellsKnown()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new MagicalSecretsChoiceGrant(10, 0));
    }

    [Fact]
    public void Validator_RejectsFavoredEnemyProgressionWithNoGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            favoredEnemyProgression: new FavoredEnemyProgressionDetail(
                [],
                grantsAssociatedLanguagePerChoice: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Favored Enemy types known progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsFavoredEnemyProgressionWithNonIncreasingCount()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            favoredEnemyProgression: new FavoredEnemyProgressionDetail(
                [
                    new FavoredEnemyChoiceGrant(1, 2),
                    new FavoredEnemyChoiceGrant(6, 1)
                ],
                grantsAssociatedLanguagePerChoice: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must be greater than the value at the previous grant",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNaturalExplorerProgressionWithDuplicateLevels()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            naturalExplorerProgression:
                new NaturalExplorerProgressionDetail(
                    [
                        new NaturalExplorerChoiceGrant(1, 1),
                        new NaturalExplorerChoiceGrant(1, 2)
                    ]));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "is duplicated",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedFavoredEnemyAndNaturalExplorerProgressions()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            favoredEnemyProgression: new FavoredEnemyProgressionDetail(
                [
                    new FavoredEnemyChoiceGrant(1, 1),
                    new FavoredEnemyChoiceGrant(6, 2),
                    new FavoredEnemyChoiceGrant(14, 3)
                ],
                grantsAssociatedLanguagePerChoice: true),
            naturalExplorerProgression:
                new NaturalExplorerProgressionDetail(
                    [
                        new NaturalExplorerChoiceGrant(1, 1),
                        new NaturalExplorerChoiceGrant(6, 2),
                        new NaturalExplorerChoiceGrant(10, 3)
                    ]));

        Assert.Empty(ClassDefinitionValidator.Validate(@class));
    }

    [Fact]
    public void FavoredEnemyChoiceGrant_RejectsNonPositiveCount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new FavoredEnemyChoiceGrant(1, 0));
    }

    [Fact]
    public void NaturalExplorerChoiceGrant_RejectsOutOfRangeCharacterLevel()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new NaturalExplorerChoiceGrant(21, 1));
    }

    [Fact]
    public void Validator_RejectsDestroyUndeadProgressionWithNoThresholds()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            destroyUndeadProgression: new DestroyUndeadProgressionDetail([]));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Destroy Undead progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsDestroyUndeadProgressionWithNonIncreasingChallengeRating()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            destroyUndeadProgression: new DestroyUndeadProgressionDetail(
                [
                    new DestroyUndeadThresholdGrant(5, 1),
                    new DestroyUndeadThresholdGrant(8, 0.5)
                ]));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must be greater than the value at the previous grant",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedDestroyUndeadProgression()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            destroyUndeadProgression: new DestroyUndeadProgressionDetail(
                [
                    new DestroyUndeadThresholdGrant(5, 0.5),
                    new DestroyUndeadThresholdGrant(8, 1),
                    new DestroyUndeadThresholdGrant(11, 2)
                ]));

        Assert.Empty(ClassDefinitionValidator.Validate(@class));
    }

    [Fact]
    public void DestroyUndeadThresholdGrant_RejectsNonPositiveChallengeRating()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new DestroyUndeadThresholdGrant(5, 0));
    }

    [Fact]
    public void Validator_RejectsActionSurgeProgressionWithNoUseGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            actionSurgeProgression: new ActionSurgeProgressionDetail(
                [],
                recoversOnShortRest: true,
                oncePerTurn: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Action Surge uses progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsActionSurgeProgressionWithNonIncreasingUses()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            actionSurgeProgression: new ActionSurgeProgressionDetail(
                [
                    new ActionSurgeUseGrant(2, 2),
                    new ActionSurgeUseGrant(17, 1)
                ],
                recoversOnShortRest: true,
                oncePerTurn: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must be greater than the value at the previous grant",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsIndomitableProgressionWithDuplicateLevels()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            indomitableProgression: new IndomitableProgressionDetail(
                [
                    new IndomitableUseGrant(9, 1),
                    new IndomitableUseGrant(9, 2)
                ],
                recoversOnShortRest: false));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "is duplicated",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedActionSurgeAndIndomitableProgressions()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            actionSurgeProgression: new ActionSurgeProgressionDetail(
                [
                    new ActionSurgeUseGrant(2, 1),
                    new ActionSurgeUseGrant(17, 2)
                ],
                recoversOnShortRest: true,
                oncePerTurn: true),
            indomitableProgression: new IndomitableProgressionDetail(
                [
                    new IndomitableUseGrant(9, 1),
                    new IndomitableUseGrant(13, 2),
                    new IndomitableUseGrant(17, 3)
                ],
                recoversOnShortRest: false));

        Assert.Empty(ClassDefinitionValidator.Validate(@class));
    }

    [Fact]
    public void ActionSurgeUseGrant_RejectsNonPositiveUsesPerRest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ActionSurgeUseGrant(2, 0));
    }

    [Fact]
    public void IndomitableUseGrant_RejectsOutOfRangeCharacterLevel()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new IndomitableUseGrant(0, 1));
    }

    [Fact]
    public void Validator_RejectsBrutalCriticalProgressionWithNoDiceGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            brutalCriticalProgression: new BrutalCriticalProgressionDetail(
                [],
                requiresMeleeAttack: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Brutal Critical additional dice progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsBrutalCriticalProgressionWithNonIncreasingDice()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            brutalCriticalProgression: new BrutalCriticalProgressionDetail(
                [
                    new BrutalCriticalDiceGrant(9, 2),
                    new BrutalCriticalDiceGrant(13, 1)
                ],
                requiresMeleeAttack: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must be greater than the value at the previous grant",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsBrutalCriticalProgressionWithDuplicateLevels()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            brutalCriticalProgression: new BrutalCriticalProgressionDetail(
                [
                    new BrutalCriticalDiceGrant(9, 1),
                    new BrutalCriticalDiceGrant(9, 2)
                ],
                requiresMeleeAttack: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "is duplicated",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedBrutalCriticalProgressionAndFastMovement()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            brutalCriticalProgression: new BrutalCriticalProgressionDetail(
                [
                    new BrutalCriticalDiceGrant(9, 1),
                    new BrutalCriticalDiceGrant(13, 2),
                    new BrutalCriticalDiceGrant(17, 3)
                ],
                requiresMeleeAttack: true),
            fastMovement: new FastMovementDetail(
                speedBonusFeet: 10,
                requiresNotWearingHeavyArmor: true));

        Assert.Empty(ClassDefinitionValidator.Validate(@class));
    }

    [Fact]
    public void BrutalCriticalDiceGrant_RejectsNonPositiveAdditionalDice()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new BrutalCriticalDiceGrant(9, 0));
    }

    [Fact]
    public void FastMovementDetail_RejectsNonPositiveSpeedBonus()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new FastMovementDetail(
                speedBonusFeet: 0,
                requiresNotWearingHeavyArmor: true));
    }

    [Fact]
    public void Validator_RejectsMartialArtsProgressionWithNoDieGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            martialArtsProgression: new MartialArtsProgressionDetail(
                [],
                canUseDexterityForAttackAndDamage: true,
                grantsBonusActionUnarmedStrike: true,
                requiresNotWearingArmor: true,
                requiresNotWieldingShield: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Martial Arts progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsMartialArtsProgressionWithNonIncreasingDieSize()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            martialArtsProgression: new MartialArtsProgressionDetail(
                [
                    new MartialArtsDieGrant(1, new DiceExpression(1, 6)),
                    new MartialArtsDieGrant(5, new DiceExpression(1, 4))
                ],
                canUseDexterityForAttackAndDamage: true,
                grantsBonusActionUnarmedStrike: true,
                requiresNotWearingArmor: true,
                requiresNotWieldingShield: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must use a larger die",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMartialArtsProgressionWithMixedDieCounts()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            martialArtsProgression: new MartialArtsProgressionDetail(
                [
                    new MartialArtsDieGrant(1, new DiceExpression(1, 4)),
                    new MartialArtsDieGrant(5, new DiceExpression(2, 6))
                ],
                canUseDexterityForAttackAndDamage: true,
                grantsBonusActionUnarmedStrike: true,
                requiresNotWearingArmor: true,
                requiresNotWieldingShield: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "same number of dice",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsUnarmoredMovementProgressionWithNoSpeedBonusGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            unarmoredMovementProgression:
                new UnarmoredMovementProgressionDetail(
                    [],
                    requiresNotWearingArmor: true,
                    requiresNotWieldingShield: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Unarmored Movement progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsUnarmoredMovementProgressionWithNonIncreasingSpeedBonus()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            unarmoredMovementProgression:
                new UnarmoredMovementProgressionDetail(
                    [
                        new UnarmoredMovementSpeedBonusGrant(2, 15),
                        new UnarmoredMovementSpeedBonusGrant(6, 10)
                    ],
                    requiresNotWearingArmor: true,
                    requiresNotWieldingShield: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must be greater than the value at the previous grant",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsUnarmoredMovementProgressionWithDuplicateLevels()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            unarmoredMovementProgression:
                new UnarmoredMovementProgressionDetail(
                    [
                        new UnarmoredMovementSpeedBonusGrant(2, 10),
                        new UnarmoredMovementSpeedBonusGrant(2, 15)
                    ],
                    requiresNotWearingArmor: true,
                    requiresNotWieldingShield: true));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "is duplicated",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedMartialArtsAndUnarmoredMovementProgressions()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            martialArtsProgression: new MartialArtsProgressionDetail(
                [
                    new MartialArtsDieGrant(1, new DiceExpression(1, 4)),
                    new MartialArtsDieGrant(5, new DiceExpression(1, 6)),
                    new MartialArtsDieGrant(11, new DiceExpression(1, 8)),
                    new MartialArtsDieGrant(17, new DiceExpression(1, 10))
                ],
                canUseDexterityForAttackAndDamage: true,
                grantsBonusActionUnarmedStrike: true,
                requiresNotWearingArmor: true,
                requiresNotWieldingShield: true),
            unarmoredMovementProgression:
                new UnarmoredMovementProgressionDetail(
                    [
                        new UnarmoredMovementSpeedBonusGrant(2, 10),
                        new UnarmoredMovementSpeedBonusGrant(6, 15),
                        new UnarmoredMovementSpeedBonusGrant(10, 20),
                        new UnarmoredMovementSpeedBonusGrant(14, 25),
                        new UnarmoredMovementSpeedBonusGrant(18, 30)
                    ],
                    requiresNotWearingArmor: true,
                    requiresNotWieldingShield: true));

        Assert.Empty(ClassDefinitionValidator.Validate(@class));
    }

    [Fact]
    public void MartialArtsDieGrant_RejectsOutOfRangeCharacterLevel()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new MartialArtsDieGrant(21, new DiceExpression(1, 4)));
    }

    [Fact]
    public void UnarmoredMovementSpeedBonusGrant_RejectsNonPositiveSpeedBonus()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new UnarmoredMovementSpeedBonusGrant(2, 0));
    }

    [Fact]
    public void Validator_RejectsSongOfRestProgressionWithNoDieGrants()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            songOfRestProgression: new SongOfRestProgressionDetail([]));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "Song of Rest progression must grant",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsSongOfRestProgressionWithNonIncreasingDieSize()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            songOfRestProgression: new SongOfRestProgressionDetail(
                [
                    new SongOfRestDieGrant(2, new DiceExpression(1, 8)),
                    new SongOfRestDieGrant(9, new DiceExpression(1, 6))
                ]));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "must use a larger die",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsSongOfRestProgressionWithMixedDieCounts()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            songOfRestProgression: new SongOfRestProgressionDetail(
                [
                    new SongOfRestDieGrant(2, new DiceExpression(1, 6)),
                    new SongOfRestDieGrant(9, new DiceExpression(2, 8))
                ]));

        Assert.Contains(
            ClassDefinitionValidator.Validate(@class),
            error =>
                error.Contains(
                    "same number of dice",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsWellFormedSongOfRestProgression()
    {
        ClassDefinition @class = Create(
            "dnd5e2014.class.test",
            songOfRestProgression: new SongOfRestProgressionDetail(
                [
                    new SongOfRestDieGrant(2, new DiceExpression(1, 6)),
                    new SongOfRestDieGrant(9, new DiceExpression(1, 8)),
                    new SongOfRestDieGrant(13, new DiceExpression(1, 10)),
                    new SongOfRestDieGrant(17, new DiceExpression(1, 12))
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
        ActionSurgeProgressionDetail? actionSurgeProgression = null,
        IndomitableProgressionDetail? indomitableProgression = null,
        RageProgressionDetail? rageProgression = null,
        BrutalCriticalProgressionDetail? brutalCriticalProgression
            = null,
        FastMovementDetail? fastMovement = null,
        FavoredEnemyProgressionDetail? favoredEnemyProgression
            = null,
        NaturalExplorerProgressionDetail? naturalExplorerProgression
            = null,
        SneakAttackProgressionDetail? sneakAttackProgression = null,
        KiProgressionDetail? kiProgression = null,
        MartialArtsProgressionDetail? martialArtsProgression = null,
        UnarmoredMovementProgressionDetail? unarmoredMovementProgression
            = null,
        SorceryPointsProgressionDetail? sorceryPointsProgression = null,
        WildShapeProgressionDetail? wildShapeProgression = null,
        AuraOfProtectionDetail? auraOfProtection = null,
        AuraOfCourageDetail? auraOfCourage = null,
        BardicInspirationProgressionDetail? bardicInspirationProgression =
            null,
        MagicalSecretsProgressionDetail? magicalSecretsProgression = null,
        ChannelDivinityProgressionDetail? channelDivinityProgression = null,
        DestroyUndeadProgressionDetail? destroyUndeadProgression = null,
        MysticArcanumProgressionDetail? mysticArcanumProgression = null,
        FontOfMagicConversionDetail? fontOfMagicConversion = null,
        SongOfRestProgressionDetail? songOfRestProgression = null,
        EldritchInvocationsKnownProgressionDetail?
            eldritchInvocationsKnownProgression = null,
        BlindsenseDetail? blindsense = null,
        int? reliableTalentMinimumD20Roll = null,
        FeralSensesDetail? feralSenses = null,
        DivineSenseDetail? divineSense = null,
        ImprovedDivineSmiteDetail? improvedDivineSmite = null,
        PrimalChampionDetail? primalChampion = null,
        int? stunningStrikeKiCost = null,
        int? diamondSoulRerollKiCost = null,
        EmptyBodyDetail? emptyBody = null,
        int? perfectSelfKiPointsRegained = null,
        int? sorcerousRestorationSorceryPointsRegained = null,
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
            actionSurgeProgression,
            indomitableProgression,
            rageProgression,
            brutalCriticalProgression,
            fastMovement,
            favoredEnemyProgression,
            naturalExplorerProgression,
            sneakAttackProgression,
            kiProgression,
            martialArtsProgression,
            unarmoredMovementProgression,
            sorceryPointsProgression,
            wildShapeProgression,
            auraOfProtection,
            auraOfCourage,
            bardicInspirationProgression,
            magicalSecretsProgression,
            channelDivinityProgression,
            destroyUndeadProgression,
            mysticArcanumProgression,
            fontOfMagicConversion,
            songOfRestProgression,
            eldritchInvocationsKnownProgression,
            blindsense,
            reliableTalentMinimumD20Roll,
            feralSenses,
            divineSense,
            improvedDivineSmite,
            primalChampion,
            stunningStrikeKiCost,
            diamondSoulRerollKiCost,
            emptyBody,
            perfectSelfKiPointsRegained,
            sorcerousRestorationSorceryPointsRegained,
            sources ?? [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 71);
    }
}
