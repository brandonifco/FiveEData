using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Auras;
using FiveEData.Rules.Classes.BardicInspiration;
using FiveEData.Rules.Classes.ChannelDivinity;
using FiveEData.Rules.Classes.EldritchInvocationsKnown;
using FiveEData.Rules.Classes.FontOfMagic;
using FiveEData.Rules.Classes.Ki;
using FiveEData.Rules.Classes.MartialArts;
using FiveEData.Rules.Classes.MysticArcanum;
using FiveEData.Rules.Classes.Rage;
using FiveEData.Rules.Classes.Serialization;
using FiveEData.Rules.Classes.SneakAttack;
using FiveEData.Rules.Classes.SongOfRest;
using FiveEData.Rules.Classes.SorceryPoints;
using FiveEData.Rules.Classes.UnarmoredMovement;
using FiveEData.Rules.Classes.WildShape;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Tests;

public sealed class ClassDataFileTests
{
    private static readonly string[] ExpectedClassIds =
    [
        "dnd5e2014.class.fighter",
        "dnd5e2014.class.barbarian",
        "dnd5e2014.class.monk",
        "dnd5e2014.class.rogue",
        "dnd5e2014.class.bard",
        "dnd5e2014.class.wizard",
        "dnd5e2014.class.cleric",
        "dnd5e2014.class.warlock",
        "dnd5e2014.class.druid",
        "dnd5e2014.class.ranger",
        "dnd5e2014.class.paladin",
        "dnd5e2014.class.sorcerer"
    ];

    [Fact]
    public void CanonicalFile_ContainsExactClassClosure()
    {
        IReadOnlyList<ClassDefinition> classes = LoadClasses();

        Assert.Equal(12, classes.Count);
        Assert.Equal(
            ExpectedClassIds.OrderBy(id => id, StringComparer.Ordinal),
            classes
                .Select(@class => @class.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_PreservesFighterMechanics()
    {
        ClassDefinition fighter = GetClass(LoadClasses(), "dnd5e2014.class.fighter");

        Assert.Equal("Fighter", fighter.Name);
        Assert.Equal(1, fighter.HitDie.Count);
        Assert.Equal(10, fighter.HitDie.Sides);
        Assert.Equal(
            [
                "dnd5e2014.ability.strength",
                "dnd5e2014.ability.dexterity"
            ],
            fighter.PrimaryAbilityIds.Select(id => id.Value).ToArray());
        Assert.False(fighter.RequiresAllPrimaryAbilities);
        Assert.Equal(
            [
                "dnd5e2014.ability.strength",
                "dnd5e2014.ability.constitution"
            ],
            fighter.SavingThrowProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(
            [ArmorCategory.Light, ArmorCategory.Medium, ArmorCategory.Heavy],
            fighter.ArmorProficiencyCategories);
        Assert.True(fighter.ProficientWithShields);
        Assert.Equal(
            [WeaponProficiencyCategory.Simple, WeaponProficiencyCategory.Martial],
            fighter.WeaponProficiencyCategories);
        Assert.Empty(fighter.WeaponProficiencyIds);
        Assert.Equal(2, fighter.SkillChoiceCount);
        Assert.Equal(8, fighter.SkillChoiceOptionIds.Count);

        var source = Assert.Single(fighter.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(71, source.Page);
        Assert.Equal("Chapter 3: Classes", source.Section);
    }

    [Fact]
    public void CanonicalFile_PreservesAbilityScoreImprovementAtStandardLevels()
    {
        ClassDefinition fighter = GetClass(LoadClasses(), "dnd5e2014.class.fighter");

        int[] expectedLevels = [4, 6, 8, 12, 14, 16, 19];

        int[] actualLevels = fighter.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.fighter-ability-score-improvement")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesExtraAttackScalingLevels()
    {
        ClassDefinition fighter = GetClass(LoadClasses(), "dnd5e2014.class.fighter");

        int[] expectedLevels = [5, 11, 20];

        int[] actualLevels = fighter.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.fighter-extra-attack")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesMartialArchetypeChoicePoint()
    {
        ClassDefinition fighter = GetClass(LoadClasses(), "dnd5e2014.class.fighter");

        Assert.Contains(
            fighter.LevelFeatures,
            feature =>
                feature.Level == 3 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.martial-archetype");
    }

    [Fact]
    public void CanonicalFile_PreservesBarbarianMechanics()
    {
        ClassDefinition barbarian =
            GetClass(LoadClasses(), "dnd5e2014.class.barbarian");

        Assert.Equal("Barbarian", barbarian.Name);
        Assert.Equal(1, barbarian.HitDie.Count);
        Assert.Equal(12, barbarian.HitDie.Sides);
        Assert.Equal(
            ["dnd5e2014.ability.strength"],
            barbarian.PrimaryAbilityIds.Select(id => id.Value).ToArray());
        Assert.True(barbarian.RequiresAllPrimaryAbilities);
        Assert.Equal(
            [
                "dnd5e2014.ability.strength",
                "dnd5e2014.ability.constitution"
            ],
            barbarian.SavingThrowProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(
            [ArmorCategory.Light, ArmorCategory.Medium],
            barbarian.ArmorProficiencyCategories);
        Assert.True(barbarian.ProficientWithShields);
        Assert.Equal(
            [WeaponProficiencyCategory.Simple, WeaponProficiencyCategory.Martial],
            barbarian.WeaponProficiencyCategories);
        Assert.Empty(barbarian.WeaponProficiencyIds);
        Assert.Equal(2, barbarian.SkillChoiceCount);
        Assert.Equal(6, barbarian.SkillChoiceOptionIds.Count);

        var source = Assert.Single(barbarian.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(47, source.Page);
        Assert.Equal("Chapter 3: Classes", source.Section);
    }

    [Fact]
    public void CanonicalFile_PreservesBarbarianAbilityScoreImprovementAtStandardLevels()
    {
        ClassDefinition barbarian =
            GetClass(LoadClasses(), "dnd5e2014.class.barbarian");

        int[] expectedLevels = [4, 8, 12, 16, 19];

        int[] actualLevels = barbarian.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.ability-score-improvement")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesBrutalCriticalScalingLevels()
    {
        ClassDefinition barbarian =
            GetClass(LoadClasses(), "dnd5e2014.class.barbarian");

        int[] expectedLevels = [9, 13, 17];

        int[] actualLevels = barbarian.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.brutal-critical")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesPrimalPathChoicePoint()
    {
        ClassDefinition barbarian =
            GetClass(LoadClasses(), "dnd5e2014.class.barbarian");

        Assert.Contains(
            barbarian.LevelFeatures,
            feature =>
                feature.Level == 3 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.primal-path");
    }

    [Fact]
    public void CanonicalFile_PreservesMonkMechanics()
    {
        ClassDefinition monk = GetClass(LoadClasses(), "dnd5e2014.class.monk");

        Assert.Equal("Monk", monk.Name);
        Assert.Equal(1, monk.HitDie.Count);
        Assert.Equal(8, monk.HitDie.Sides);
        Assert.Equal(
            [
                "dnd5e2014.ability.dexterity",
                "dnd5e2014.ability.wisdom"
            ],
            monk.PrimaryAbilityIds.Select(id => id.Value).ToArray());
        Assert.True(monk.RequiresAllPrimaryAbilities);
        Assert.Equal(
            [
                "dnd5e2014.ability.strength",
                "dnd5e2014.ability.dexterity"
            ],
            monk.SavingThrowProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Empty(monk.ArmorProficiencyCategories);
        Assert.False(monk.ProficientWithShields);
        Assert.Equal(
            [WeaponProficiencyCategory.Simple],
            monk.WeaponProficiencyCategories);
        Assert.Equal(
            ["dnd5e2014.weapon.shortsword"],
            monk.WeaponProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(2, monk.SkillChoiceCount);
        Assert.Equal(6, monk.SkillChoiceOptionIds.Count);

        var source = Assert.Single(monk.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(77, source.Page);
        Assert.Equal("Chapter 3: Classes", source.Section);

        Assert.Null(monk.SorceryPointsProgression);

        KiProgressionDetail kiProgression =
            monk.KiProgression
            ?? throw new InvalidOperationException(
                "Expected Monk to have a Ki progression.");
        Assert.Equal(19, kiProgression.PointsByLevel.Count);
        Assert.All(
            kiProgression.PointsByLevel,
            grant => Assert.Equal(grant.CharacterLevel, grant.Points));
        Assert.Equal(2, kiProgression.PointsByLevel[0].CharacterLevel);
        Assert.Equal(20, kiProgression.PointsByLevel[^1].CharacterLevel);
        Assert.True(kiProgression.RecoversOnShortRest);

        MartialArtsProgressionDetail martialArtsProgression =
            monk.MartialArtsProgression
            ?? throw new InvalidOperationException(
                "Expected Monk to have a Martial Arts progression.");
        Assert.Equal(
            [(1, 4), (5, 6), (11, 8), (17, 10)],
            martialArtsProgression.DieByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.Die.Sides)));
        Assert.All(
            martialArtsProgression.DieByLevel,
            grant => Assert.Equal(1, grant.Die.Count));
        Assert.True(martialArtsProgression.CanUseDexterityForAttackAndDamage);
        Assert.True(martialArtsProgression.GrantsBonusActionUnarmedStrike);
        Assert.True(martialArtsProgression.RequiresNotWearingArmor);
        Assert.True(martialArtsProgression.RequiresNotWieldingShield);

        UnarmoredMovementProgressionDetail unarmoredMovementProgression =
            monk.UnarmoredMovementProgression
            ?? throw new InvalidOperationException(
                "Expected Monk to have an Unarmored Movement progression.");
        Assert.Equal(
            [(2, 10), (6, 15), (10, 20), (14, 25), (18, 30)],
            unarmoredMovementProgression.SpeedBonusByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant => (grant.CharacterLevel, grant.SpeedBonusFeet)));
        Assert.True(unarmoredMovementProgression.RequiresNotWearingArmor);
        Assert.True(unarmoredMovementProgression.RequiresNotWieldingShield);
    }

    // The Monk table's Unarmored Movement column and its Features column
    // disagree on purpose: the speed bonus grows at 2nd/6th/10th/14th/18th,
    // while the Features column names the feature at 2nd and again at 9th for
    // the vertical-surfaces and across-liquids clause, which carries no
    // number. Neither list is a stale copy of the other.
    [Fact]
    public void CanonicalFile_MonkUnarmoredMovementSpeedLevelsDifferFromFeatureLevels()
    {
        ClassDefinition monk = GetClass(LoadClasses(), "dnd5e2014.class.monk");

        UnarmoredMovementProgressionDetail unarmoredMovementProgression =
            monk.UnarmoredMovementProgression
            ?? throw new InvalidOperationException(
                "Expected Monk to have an Unarmored Movement progression.");

        int[] speedBonusLevels = unarmoredMovementProgression.SpeedBonusByLevel
            .Select(grant => grant.CharacterLevel)
            .OrderBy(level => level)
            .ToArray();

        int[] featureLevels = monk.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.unarmored-movement")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal([2, 6, 10, 14, 18], speedBonusLevels);
        Assert.Equal([2, 9], featureLevels);
    }

    // Martial Arts is granted once, at 1st level, even though its die grows at
    // 5th, 11th, and 17th — the Monk table has no Features-column row for
    // those upgrades, so the progression is the only place they are recorded.
    [Fact]
    public void CanonicalFile_MonkGrantsMartialArtsOnlyAtFirstLevelDespiteDieUpgrades()
    {
        ClassDefinition monk = GetClass(LoadClasses(), "dnd5e2014.class.monk");

        int[] featureLevels = monk.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.martial-arts")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal([1], featureLevels);

        MartialArtsProgressionDetail martialArtsProgression =
            monk.MartialArtsProgression
            ?? throw new InvalidOperationException(
                "Expected Monk to have a Martial Arts progression.");
        Assert.Equal(
            [1, 5, 11, 17],
            martialArtsProgression.DieByLevel
                .Select(grant => grant.CharacterLevel)
                .OrderBy(level => level));
    }

    [Fact]
    public void CanonicalFile_PreservesMonkAbilityScoreImprovementAtStandardLevels()
    {
        ClassDefinition monk = GetClass(LoadClasses(), "dnd5e2014.class.monk");

        int[] expectedLevels = [4, 8, 12, 16, 19];

        int[] actualLevels = monk.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.ability-score-improvement")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesUnarmoredMovementAtInitialAndImprovementLevels()
    {
        ClassDefinition monk = GetClass(LoadClasses(), "dnd5e2014.class.monk");

        int[] expectedLevels = [2, 9];

        int[] actualLevels = monk.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.unarmored-movement")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesMonasticTraditionChoicePoint()
    {
        ClassDefinition monk = GetClass(LoadClasses(), "dnd5e2014.class.monk");

        Assert.Contains(
            monk.LevelFeatures,
            feature =>
                feature.Level == 3 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.monastic-tradition");
    }

    [Fact]
    public void CanonicalFile_SharesAbilityScoreImprovementRuleIdAcrossClassesWithStandardWording()
    {
        IReadOnlyList<ClassDefinition> classes = LoadClasses();

        string[] classIdsSharingStandardWording =
        [
            "dnd5e2014.class.barbarian",
            "dnd5e2014.class.monk",
            "dnd5e2014.class.bard",
            "dnd5e2014.class.wizard",
            "dnd5e2014.class.cleric",
            "dnd5e2014.class.warlock",
            "dnd5e2014.class.druid",
            "dnd5e2014.class.ranger",
            "dnd5e2014.class.paladin",
            "dnd5e2014.class.sorcerer"
        ];

        Assert.All(
            classIdsSharingStandardWording,
            classId => Assert.Contains(
                GetClass(classes, classId).LevelFeatures,
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.ability-score-improvement"));

        // Fighter and Rogue have their own divergent-wording ASI text
        // (extra trigger levels named in their own sentences) and so
        // keep prefixed RuleIds instead of the shared one.
        ClassDefinition fighter =
            GetClass(classes, "dnd5e2014.class.fighter");
        ClassDefinition rogue =
            GetClass(classes, "dnd5e2014.class.rogue");

        Assert.DoesNotContain(
            fighter.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.ability-score-improvement");
        Assert.DoesNotContain(
            rogue.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.ability-score-improvement");
    }

    [Fact]
    public void CanonicalFile_PreservesRogueMechanics()
    {
        ClassDefinition rogue = GetClass(LoadClasses(), "dnd5e2014.class.rogue");

        Assert.Equal("Rogue", rogue.Name);
        Assert.Equal(1, rogue.HitDie.Count);
        Assert.Equal(8, rogue.HitDie.Sides);
        Assert.Equal(
            ["dnd5e2014.ability.dexterity"],
            rogue.PrimaryAbilityIds.Select(id => id.Value).ToArray());
        Assert.True(rogue.RequiresAllPrimaryAbilities);
        Assert.Equal(
            [
                "dnd5e2014.ability.dexterity",
                "dnd5e2014.ability.intelligence"
            ],
            rogue.SavingThrowProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(
            [ArmorCategory.Light],
            rogue.ArmorProficiencyCategories);
        Assert.False(rogue.ProficientWithShields);
        Assert.Equal(
            [WeaponProficiencyCategory.Simple],
            rogue.WeaponProficiencyCategories);
        Assert.Equal(
            [
                "dnd5e2014.weapon.hand-crossbow",
                "dnd5e2014.weapon.longsword",
                "dnd5e2014.weapon.rapier",
                "dnd5e2014.weapon.shortsword"
            ],
            rogue.WeaponProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(4, rogue.SkillChoiceCount);
        Assert.Equal(11, rogue.SkillChoiceOptionIds.Count);

        var source = Assert.Single(rogue.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(95, source.Page);
        Assert.Equal("Chapter 3: Classes", source.Section);
    }

    [Fact]
    public void CanonicalFile_PreservesRogueAbilityScoreImprovementAtItsOwnLevels()
    {
        ClassDefinition rogue = GetClass(LoadClasses(), "dnd5e2014.class.rogue");

        int[] expectedLevels = [4, 8, 10, 12, 16, 19];

        int[] actualLevels = rogue.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.rogue-ability-score-improvement")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesExpertiseAtInitialAndSecondGrantLevels()
    {
        ClassDefinition rogue = GetClass(LoadClasses(), "dnd5e2014.class.rogue");

        int[] expectedLevels = [1, 6];

        int[] actualLevels = rogue.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.rogue-expertise")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesRoguishArchetypeChoicePoint()
    {
        ClassDefinition rogue = GetClass(LoadClasses(), "dnd5e2014.class.rogue");

        Assert.Contains(
            rogue.LevelFeatures,
            feature =>
                feature.Level == 3 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.roguish-archetype");
    }

    [Fact]
    public void CanonicalFile_SharesEvasionRuleIdWithMonkButKeepsItsOwnDistinctAbilityScoreImprovement()
    {
        IReadOnlyList<ClassDefinition> classes = LoadClasses();

        ClassDefinition monk = GetClass(classes, "dnd5e2014.class.monk");
        ClassDefinition rogue = GetClass(classes, "dnd5e2014.class.rogue");

        Assert.Contains(
            monk.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.evasion");
        Assert.Contains(
            rogue.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.evasion");

        Assert.DoesNotContain(
            rogue.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.ability-score-improvement");
        Assert.DoesNotContain(
            monk.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.rogue-ability-score-improvement");
    }

    [Fact]
    public void CanonicalFile_PreservesBardMechanics()
    {
        ClassDefinition bard = GetClass(LoadClasses(), "dnd5e2014.class.bard");

        Assert.Equal("Bard", bard.Name);
        Assert.Equal(1, bard.HitDie.Count);
        Assert.Equal(8, bard.HitDie.Sides);
        Assert.Equal(
            ["dnd5e2014.ability.charisma"],
            bard.PrimaryAbilityIds.Select(id => id.Value).ToArray());
        Assert.True(bard.RequiresAllPrimaryAbilities);
        Assert.Equal(
            [
                "dnd5e2014.ability.dexterity",
                "dnd5e2014.ability.charisma"
            ],
            bard.SavingThrowProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(
            [ArmorCategory.Light],
            bard.ArmorProficiencyCategories);
        Assert.False(bard.ProficientWithShields);
        Assert.Equal(
            [WeaponProficiencyCategory.Simple],
            bard.WeaponProficiencyCategories);
        Assert.Equal(
            [
                "dnd5e2014.weapon.hand-crossbow",
                "dnd5e2014.weapon.longsword",
                "dnd5e2014.weapon.rapier",
                "dnd5e2014.weapon.shortsword"
            ],
            bard.WeaponProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(3, bard.SkillChoiceCount);
        Assert.Equal(18, bard.SkillChoiceOptionIds.Count);

        var source = Assert.Single(bard.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(52, source.Page);
        Assert.Equal("Chapter 3: Classes", source.Section);

        BardicInspirationProgressionDetail bardicInspirationProgression =
            bard.BardicInspirationProgression
            ?? throw new InvalidOperationException(
                "Expected Bard to have a Bardic Inspiration progression.");
        Assert.Equal(
            [(1, 6), (5, 8), (10, 10), (15, 12)],
            bardicInspirationProgression.DieByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.Die.Sides)));
        Assert.All(
            bardicInspirationProgression.DieByLevel,
            grant => Assert.Equal(1, grant.Die.Count));
        Assert.Equal(60, bardicInspirationProgression.RangeFeet);
        Assert.Equal(10, bardicInspirationProgression.DurationMinutes);

        SongOfRestProgressionDetail songOfRestProgression =
            bard.SongOfRestProgression
            ?? throw new InvalidOperationException(
                "Expected Bard to have a Song of Rest progression.");
        Assert.Equal(
            [(2, 6), (9, 8), (13, 10), (17, 12)],
            songOfRestProgression.DieByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.Die.Sides)));
        Assert.All(
            songOfRestProgression.DieByLevel,
            grant => Assert.Equal(1, grant.Die.Count));
    }

    [Fact]
    public void CanonicalFile_PreservesBardAbilityScoreImprovementAtStandardLevels()
    {
        ClassDefinition bard = GetClass(LoadClasses(), "dnd5e2014.class.bard");

        int[] expectedLevels = [4, 8, 12, 16, 19];

        int[] actualLevels = bard.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.ability-score-improvement")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesBardicInspirationAtEachDieUpgradeLevel()
    {
        ClassDefinition bard = GetClass(LoadClasses(), "dnd5e2014.class.bard");

        int[] expectedLevels = [1, 5, 10, 15];

        int[] actualLevels = bard.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.bardic-inspiration")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesBardExpertiseAtInitialAndSecondGrantLevels()
    {
        ClassDefinition bard = GetClass(LoadClasses(), "dnd5e2014.class.bard");

        int[] expectedLevels = [3, 10];

        int[] actualLevels = bard.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.bard-expertise")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesBardCollegeChoicePoint()
    {
        ClassDefinition bard = GetClass(LoadClasses(), "dnd5e2014.class.bard");

        Assert.Contains(
            bard.LevelFeatures,
            feature =>
                feature.Level == 3 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.bard-college");
    }

    [Fact]
    public void CanonicalFile_KeepsBardAndRogueExpertiseAsDistinctRuleIdsDespiteSharedName()
    {
        IReadOnlyList<ClassDefinition> classes = LoadClasses();

        ClassDefinition bard = GetClass(classes, "dnd5e2014.class.bard");
        ClassDefinition rogue = GetClass(classes, "dnd5e2014.class.rogue");

        Assert.DoesNotContain(
            bard.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.rogue-expertise");
        Assert.DoesNotContain(
            rogue.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.bard-expertise");
    }

    [Fact]
    public void CanonicalFile_PreservesWizardMechanics()
    {
        ClassDefinition wizard = GetClass(LoadClasses(), "dnd5e2014.class.wizard");

        Assert.Equal("Wizard", wizard.Name);
        Assert.Equal(1, wizard.HitDie.Count);
        Assert.Equal(6, wizard.HitDie.Sides);
        Assert.Equal(
            ["dnd5e2014.ability.intelligence"],
            wizard.PrimaryAbilityIds.Select(id => id.Value).ToArray());
        Assert.True(wizard.RequiresAllPrimaryAbilities);
        Assert.Equal(
            [
                "dnd5e2014.ability.intelligence",
                "dnd5e2014.ability.wisdom"
            ],
            wizard.SavingThrowProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Empty(wizard.ArmorProficiencyCategories);
        Assert.False(wizard.ProficientWithShields);
        Assert.Empty(wizard.WeaponProficiencyCategories);
        Assert.Equal(
            [
                "dnd5e2014.weapon.dagger",
                "dnd5e2014.weapon.dart",
                "dnd5e2014.weapon.sling",
                "dnd5e2014.weapon.quarterstaff",
                "dnd5e2014.weapon.light-crossbow"
            ],
            wizard.WeaponProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(2, wizard.SkillChoiceCount);
        Assert.Equal(6, wizard.SkillChoiceOptionIds.Count);

        var source = Assert.Single(wizard.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(113, source.Page);
        Assert.Equal("Chapter 3: Classes", source.Section);
    }

    [Fact]
    public void CanonicalFile_PreservesWizardAbilityScoreImprovementAtStandardLevels()
    {
        ClassDefinition wizard = GetClass(LoadClasses(), "dnd5e2014.class.wizard");

        int[] expectedLevels = [4, 8, 12, 16, 19];

        int[] actualLevels = wizard.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.ability-score-improvement")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesArcaneTraditionChoicePoint()
    {
        ClassDefinition wizard = GetClass(LoadClasses(), "dnd5e2014.class.wizard");

        Assert.Contains(
            wizard.LevelFeatures,
            feature =>
                feature.Level == 2 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.arcane-tradition");
    }

    [Fact]
    public void CanonicalFile_PreservesSpellMasteryAndSignatureSpellsCapstones()
    {
        ClassDefinition wizard = GetClass(LoadClasses(), "dnd5e2014.class.wizard");

        Assert.Contains(
            wizard.LevelFeatures,
            feature =>
                feature.Level == 18 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.spell-mastery");
        Assert.Contains(
            wizard.LevelFeatures,
            feature =>
                feature.Level == 20 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.signature-spells");
    }

    [Fact]
    public void CanonicalFile_PreservesClericMechanics()
    {
        ClassDefinition cleric = GetClass(LoadClasses(), "dnd5e2014.class.cleric");

        Assert.Equal("Cleric", cleric.Name);
        Assert.Equal(1, cleric.HitDie.Count);
        Assert.Equal(8, cleric.HitDie.Sides);
        Assert.Equal(
            ["dnd5e2014.ability.wisdom"],
            cleric.PrimaryAbilityIds.Select(id => id.Value).ToArray());
        Assert.True(cleric.RequiresAllPrimaryAbilities);
        Assert.Equal(
            [
                "dnd5e2014.ability.wisdom",
                "dnd5e2014.ability.charisma"
            ],
            cleric.SavingThrowProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(
            [ArmorCategory.Light, ArmorCategory.Medium],
            cleric.ArmorProficiencyCategories);
        Assert.True(cleric.ProficientWithShields);
        Assert.Equal(
            [WeaponProficiencyCategory.Simple],
            cleric.WeaponProficiencyCategories);
        Assert.Empty(cleric.WeaponProficiencyIds);
        Assert.Equal(2, cleric.SkillChoiceCount);
        Assert.Equal(5, cleric.SkillChoiceOptionIds.Count);

        var source = Assert.Single(cleric.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(57, source.Page);
        Assert.Equal("Chapter 3: Classes", source.Section);

        ChannelDivinityProgressionDetail channelDivinityProgression =
            cleric.ChannelDivinityProgression
            ?? throw new InvalidOperationException(
                "Expected Cleric to have a Channel Divinity progression.");
        Assert.Equal(
            [(2, 1), (6, 2), (18, 3)],
            channelDivinityProgression.UsesByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.UsesPerRest)));
        Assert.True(channelDivinityProgression.RecoversOnShortRest);
    }

    [Fact]
    public void CanonicalFile_PreservesClericAbilityScoreImprovementAtStandardLevels()
    {
        ClassDefinition cleric = GetClass(LoadClasses(), "dnd5e2014.class.cleric");

        int[] expectedLevels = [4, 8, 12, 16, 19];

        int[] actualLevels = cleric.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.ability-score-improvement")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesDivineDomainChoicePoint()
    {
        ClassDefinition cleric = GetClass(LoadClasses(), "dnd5e2014.class.cleric");

        Assert.Contains(
            cleric.LevelFeatures,
            feature =>
                feature.Level == 1 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.divine-domain");
    }

    [Fact]
    public void CanonicalFile_PreservesDivineInterventionAtGrantAndImprovementLevels()
    {
        ClassDefinition cleric = GetClass(LoadClasses(), "dnd5e2014.class.cleric");

        int[] expectedLevels = [10, 20];

        int[] actualLevels = cleric.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.divine-intervention")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesWarlockMechanics()
    {
        ClassDefinition warlock = GetClass(LoadClasses(), "dnd5e2014.class.warlock");

        Assert.Equal("Warlock", warlock.Name);
        Assert.Equal(1, warlock.HitDie.Count);
        Assert.Equal(8, warlock.HitDie.Sides);
        Assert.Equal(
            ["dnd5e2014.ability.charisma"],
            warlock.PrimaryAbilityIds.Select(id => id.Value).ToArray());
        Assert.True(warlock.RequiresAllPrimaryAbilities);
        Assert.Equal(
            [
                "dnd5e2014.ability.wisdom",
                "dnd5e2014.ability.charisma"
            ],
            warlock.SavingThrowProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(
            [ArmorCategory.Light],
            warlock.ArmorProficiencyCategories);
        Assert.False(warlock.ProficientWithShields);
        Assert.Equal(
            [WeaponProficiencyCategory.Simple],
            warlock.WeaponProficiencyCategories);
        Assert.Empty(warlock.WeaponProficiencyIds);
        Assert.Equal(2, warlock.SkillChoiceCount);
        Assert.Equal(7, warlock.SkillChoiceOptionIds.Count);

        var source = Assert.Single(warlock.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(106, source.Page);
        Assert.Equal("Chapter 3: Classes", source.Section);

        MysticArcanumProgressionDetail mysticArcanumProgression =
            warlock.MysticArcanumProgression
            ?? throw new InvalidOperationException(
                "Expected Warlock to have a Mystic Arcanum progression.");
        Assert.Equal(
            [(11, 6), (13, 7), (15, 8), (17, 9)],
            mysticArcanumProgression.ArcanumByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.SpellLevel)));
        Assert.False(mysticArcanumProgression.RecoversOnShortRest);

        EldritchInvocationsKnownProgressionDetail
            eldritchInvocationsKnownProgression =
                warlock.EldritchInvocationsKnownProgression
                ?? throw new InvalidOperationException(
                    "Expected Warlock to have an Eldritch Invocations " +
                    "known progression.");
        Assert.Equal(
            [(2, 2), (5, 3), (7, 4), (9, 5), (12, 6), (15, 7), (17, 8)],
            eldritchInvocationsKnownProgression.InvocationsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant =>
                        (grant.CharacterLevel, grant.InvocationsKnown)));
    }

    [Fact]
    public void CanonicalFile_OnlyWarlockHasAnEldritchInvocationsKnownProgression()
    {
        IReadOnlyList<ClassDefinition> classes = LoadClasses();

        IEnumerable<string> otherClassIds = classes
            .Select(@class => @class.Id.Value)
            .Where(id => id != "dnd5e2014.class.warlock");

        foreach (string id in otherClassIds)
        {
            ClassDefinition @class = GetClass(classes, id);

            Assert.Null(@class.EldritchInvocationsKnownProgression);
        }
    }

    [Fact]
    public void CanonicalFile_PreservesWarlockAbilityScoreImprovementAtStandardLevelsDespiteTableOmission()
    {
        ClassDefinition warlock = GetClass(LoadClasses(), "dnd5e2014.class.warlock");

        // The printed Warlock table's Features column omits a 19th-level
        // row, but the Ability Score Improvement feature's own body text
        // names 19th level explicitly, word-for-word identical to every
        // other class using the shared RuleId. The prose is treated as
        // authoritative over the apparently-incomplete table, the same
        // way the Dwarf throwing-hammer/light-hammer errata was resolved
        // in favor of the corrected text over the original printing.
        int[] expectedLevels = [4, 8, 12, 16, 19];

        int[] actualLevels = warlock.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.ability-score-improvement")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesMysticArcanumAtEachGrantLevel()
    {
        ClassDefinition warlock = GetClass(LoadClasses(), "dnd5e2014.class.warlock");

        int[] expectedLevels = [11, 13, 15, 17];

        int[] actualLevels = warlock.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.mystic-arcanum")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesPactBoonChoicePoint()
    {
        ClassDefinition warlock = GetClass(LoadClasses(), "dnd5e2014.class.warlock");

        Assert.Contains(
            warlock.LevelFeatures,
            feature =>
                feature.Level == 3 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.pact-boon");
    }

    [Fact]
    public void CanonicalFile_PreservesDruidMechanics()
    {
        ClassDefinition druid = GetClass(LoadClasses(), "dnd5e2014.class.druid");

        Assert.Equal("Druid", druid.Name);
        Assert.Equal(1, druid.HitDie.Count);
        Assert.Equal(8, druid.HitDie.Sides);
        Assert.Equal(
            ["dnd5e2014.ability.wisdom"],
            druid.PrimaryAbilityIds.Select(id => id.Value).ToArray());
        Assert.True(druid.RequiresAllPrimaryAbilities);
        Assert.Equal(
            [
                "dnd5e2014.ability.intelligence",
                "dnd5e2014.ability.wisdom"
            ],
            druid.SavingThrowProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(
            [ArmorCategory.Light, ArmorCategory.Medium],
            druid.ArmorProficiencyCategories);
        Assert.True(druid.ProficientWithShields);
        Assert.Empty(druid.WeaponProficiencyCategories);
        Assert.Equal(
            [
                "dnd5e2014.weapon.club",
                "dnd5e2014.weapon.dagger",
                "dnd5e2014.weapon.dart",
                "dnd5e2014.weapon.javelin",
                "dnd5e2014.weapon.mace",
                "dnd5e2014.weapon.quarterstaff",
                "dnd5e2014.weapon.scimitar",
                "dnd5e2014.weapon.sickle",
                "dnd5e2014.weapon.sling",
                "dnd5e2014.weapon.spear"
            ],
            druid.WeaponProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(2, druid.SkillChoiceCount);
        Assert.Equal(8, druid.SkillChoiceOptionIds.Count);

        var source = Assert.Single(druid.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(65, source.Page);
        Assert.Equal("Chapter 3: Classes", source.Section);

        WildShapeProgressionDetail wildShapeProgression =
            druid.WildShapeProgression
            ?? throw new InvalidOperationException(
                "Expected Druid to have a Wild Shape progression.");

        Assert.Equal(
            [
                (2, 0.25, false, false),
                (4, 0.5, false, true),
                (8, 1.0, true, true)
            ],
            wildShapeProgression.FormLimitsByLevel
                .OrderBy(limit => limit.CharacterLevel)
                .Select(
                    limit => (
                        limit.CharacterLevel,
                        limit.MaxChallengeRating,
                        limit.AllowsFlyingSpeed,
                        limit.AllowsSwimmingSpeed)));

        Assert.Equal(2, wildShapeProgression.UsesPerRest);
        Assert.True(wildShapeProgression.RecoversOnShortRest);
    }

    [Fact]
    public void CanonicalFile_PreservesWildShapeAtEachImprovementLevel()
    {
        ClassDefinition druid = GetClass(LoadClasses(), "dnd5e2014.class.druid");

        int[] expectedLevels = [2, 4, 8];

        int[] actualLevels = druid.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.wild-shape")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesDruidCircleChoicePoint()
    {
        ClassDefinition druid = GetClass(LoadClasses(), "dnd5e2014.class.druid");

        Assert.Contains(
            druid.LevelFeatures,
            feature =>
                feature.Level == 2 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.druid-circle");
    }

    [Fact]
    public void CanonicalFile_KeepsDruidAndMonkTimelessBodyAsDistinctRuleIdsDespiteSharedName()
    {
        IReadOnlyList<ClassDefinition> classes = LoadClasses();

        ClassDefinition druid = GetClass(classes, "dnd5e2014.class.druid");
        ClassDefinition monk = GetClass(classes, "dnd5e2014.class.monk");

        Assert.Contains(
            druid.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.druid-timeless-body");
        Assert.Contains(
            monk.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.monk-timeless-body");
        Assert.DoesNotContain(
            druid.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.monk-timeless-body");
        Assert.DoesNotContain(
            monk.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.druid-timeless-body");
    }

    [Fact]
    public void CanonicalFile_PreservesRangerMechanics()
    {
        ClassDefinition ranger = GetClass(LoadClasses(), "dnd5e2014.class.ranger");

        Assert.Equal("Ranger", ranger.Name);
        Assert.Equal(1, ranger.HitDie.Count);
        Assert.Equal(10, ranger.HitDie.Sides);
        Assert.Equal(
            [
                "dnd5e2014.ability.dexterity",
                "dnd5e2014.ability.wisdom"
            ],
            ranger.PrimaryAbilityIds.Select(id => id.Value).ToArray());
        Assert.True(ranger.RequiresAllPrimaryAbilities);
        Assert.Equal(
            [
                "dnd5e2014.ability.strength",
                "dnd5e2014.ability.dexterity"
            ],
            ranger.SavingThrowProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(
            [ArmorCategory.Light, ArmorCategory.Medium],
            ranger.ArmorProficiencyCategories);
        Assert.True(ranger.ProficientWithShields);
        Assert.Equal(
            [WeaponProficiencyCategory.Simple, WeaponProficiencyCategory.Martial],
            ranger.WeaponProficiencyCategories);
        Assert.Empty(ranger.WeaponProficiencyIds);
        Assert.Equal(3, ranger.SkillChoiceCount);
        Assert.Equal(8, ranger.SkillChoiceOptionIds.Count);

        var source = Assert.Single(ranger.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(90, source.Page);
        Assert.Equal("Chapter 3: Classes", source.Section);
    }

    [Fact]
    public void CanonicalFile_PreservesFavoredEnemyAndNaturalExplorerAtEachImprovementLevel()
    {
        ClassDefinition ranger = GetClass(LoadClasses(), "dnd5e2014.class.ranger");

        int[] expectedFavoredEnemyLevels = [1, 6, 14];
        int[] expectedNaturalExplorerLevels = [1, 6, 10];

        int[] actualFavoredEnemyLevels = ranger.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.favored-enemy")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();
        int[] actualNaturalExplorerLevels = ranger.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.natural-explorer")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedFavoredEnemyLevels, actualFavoredEnemyLevels);
        Assert.Equal(expectedNaturalExplorerLevels, actualNaturalExplorerLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesRangerArchetypeChoicePoint()
    {
        ClassDefinition ranger = GetClass(LoadClasses(), "dnd5e2014.class.ranger");

        Assert.Contains(
            ranger.LevelFeatures,
            feature =>
                feature.Level == 3 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.ranger-archetype");
    }

    [Fact]
    public void CanonicalFile_SharesFightingStyleExtraAttackAndLandsStrideRuleIdsAcrossEarlierClasses()
    {
        IReadOnlyList<ClassDefinition> classes = LoadClasses();
        ClassDefinition ranger = GetClass(classes, "dnd5e2014.class.ranger");

        // Fighting Style: shared with Fighter (word-for-word identical
        // gateway text, options folded into the same citation).
        Assert.Contains(
            ranger.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.fighting-style");

        // Extra Attack: shared with Barbarian/Monk's "Beginning at 5th
        // level..." wording, not Fighter's own scaling version.
        Assert.Contains(
            ranger.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.extra-attack");

        // Land's Stride: shared with Circle of the Land's own entry
        // (Druid subclass) on word-for-word identical text discovered
        // while building a later, unrelated class - not caught within
        // Druid's own build.
        Assert.Contains(
            ranger.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.lands-stride");
    }

    [Fact]
    public void CanonicalFile_PreservesPaladinMechanics()
    {
        ClassDefinition paladin = GetClass(LoadClasses(), "dnd5e2014.class.paladin");

        Assert.Equal("Paladin", paladin.Name);
        Assert.Equal(1, paladin.HitDie.Count);
        Assert.Equal(10, paladin.HitDie.Sides);
        Assert.Equal(
            [
                "dnd5e2014.ability.strength",
                "dnd5e2014.ability.charisma"
            ],
            paladin.PrimaryAbilityIds.Select(id => id.Value).ToArray());
        Assert.True(paladin.RequiresAllPrimaryAbilities);
        Assert.Equal(
            [
                "dnd5e2014.ability.wisdom",
                "dnd5e2014.ability.charisma"
            ],
            paladin.SavingThrowProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(
            [ArmorCategory.Light, ArmorCategory.Medium, ArmorCategory.Heavy],
            paladin.ArmorProficiencyCategories);
        Assert.True(paladin.ProficientWithShields);
        Assert.Equal(
            [WeaponProficiencyCategory.Simple, WeaponProficiencyCategory.Martial],
            paladin.WeaponProficiencyCategories);
        Assert.Empty(paladin.WeaponProficiencyIds);
        Assert.Equal(2, paladin.SkillChoiceCount);
        Assert.Equal(6, paladin.SkillChoiceOptionIds.Count);

        var source = Assert.Single(paladin.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(84, source.Page);
        Assert.Equal("Chapter 3: Classes", source.Section);

        AuraOfProtectionDetail auraOfProtection =
            paladin.AuraOfProtection
            ?? throw new InvalidOperationException(
                "Expected Paladin to have an Aura of Protection.");
        Assert.Equal(10, auraOfProtection.Range.BaseRangeFeet);
        Assert.Equal(30, auraOfProtection.Range.ExpandedRangeFeet);
        Assert.Equal(18, auraOfProtection.Range.ExpandedAtLevel);
        Assert.True(auraOfProtection.RequiresConsciousness);
        Assert.Equal(1, auraOfProtection.SavingThrowBonusMinimum);

        AuraOfCourageDetail auraOfCourage =
            paladin.AuraOfCourage
            ?? throw new InvalidOperationException(
                "Expected Paladin to have an Aura of Courage.");
        Assert.Equal(10, auraOfCourage.Range.BaseRangeFeet);
        Assert.Equal(30, auraOfCourage.Range.ExpandedRangeFeet);
        Assert.Equal(18, auraOfCourage.Range.ExpandedAtLevel);
        Assert.True(auraOfCourage.RequiresConsciousness);
    }

    [Fact]
    public void CanonicalFile_PreservesAuraOfProtectionAndCourageAt18thLevelImprovement()
    {
        ClassDefinition paladin = GetClass(LoadClasses(), "dnd5e2014.class.paladin");

        int[] expectedAuraOfProtectionLevels = [6, 18];
        int[] expectedAuraOfCourageLevels = [10, 18];

        int[] actualAuraOfProtectionLevels = paladin.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.aura-of-protection")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();
        int[] actualAuraOfCourageLevels = paladin.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.aura-of-courage")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedAuraOfProtectionLevels, actualAuraOfProtectionLevels);
        Assert.Equal(expectedAuraOfCourageLevels, actualAuraOfCourageLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesSacredOathChoicePoint()
    {
        ClassDefinition paladin = GetClass(LoadClasses(), "dnd5e2014.class.paladin");

        Assert.Contains(
            paladin.LevelFeatures,
            feature =>
                feature.Level == 3 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.sacred-oath");
    }

    [Fact]
    public void CanonicalFile_KeepsPaladinFightingStyleSeparateFromTheSharedEntry()
    {
        IReadOnlyList<ClassDefinition> classes = LoadClasses();
        ClassDefinition paladin = GetClass(classes, "dnd5e2014.class.paladin");

        // Paladin's gateway sentence ("you adopt a style of fighting as
        // your specialty") drops the word "particular" present in
        // Fighter's and Ranger's identical wording ("a particular style
        // of fighting"). Kept separate rather than assumed to be OCR
        // noise, consistent with treating small wording gaps as real
        // until verified otherwise (the same caution applied to College
        // of Valor's Extra Attack).
        Assert.Contains(
            paladin.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.paladin-fighting-style");
        Assert.DoesNotContain(
            paladin.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.fighting-style");
    }

    [Fact]
    public void CanonicalFile_PreservesSorcererMechanics()
    {
        ClassDefinition sorcerer = GetClass(LoadClasses(), "dnd5e2014.class.sorcerer");

        Assert.Equal("Sorcerer", sorcerer.Name);
        Assert.Equal(1, sorcerer.HitDie.Count);
        Assert.Equal(6, sorcerer.HitDie.Sides);
        Assert.Equal(
            ["dnd5e2014.ability.charisma"],
            sorcerer.PrimaryAbilityIds.Select(id => id.Value).ToArray());
        Assert.True(sorcerer.RequiresAllPrimaryAbilities);
        Assert.Equal(
            [
                "dnd5e2014.ability.constitution",
                "dnd5e2014.ability.charisma"
            ],
            sorcerer.SavingThrowProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Empty(sorcerer.ArmorProficiencyCategories);
        Assert.False(sorcerer.ProficientWithShields);
        Assert.Empty(sorcerer.WeaponProficiencyCategories);
        Assert.Equal(
            [
                "dnd5e2014.weapon.dagger",
                "dnd5e2014.weapon.dart",
                "dnd5e2014.weapon.sling",
                "dnd5e2014.weapon.quarterstaff",
                "dnd5e2014.weapon.light-crossbow"
            ],
            sorcerer.WeaponProficiencyIds.Select(id => id.Value).ToArray());
        Assert.Equal(2, sorcerer.SkillChoiceCount);
        Assert.Equal(6, sorcerer.SkillChoiceOptionIds.Count);

        var source = Assert.Single(sorcerer.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(100, source.Page);
        Assert.Equal("Chapter 3: Classes", source.Section);

        Assert.Null(sorcerer.KiProgression);

        SorceryPointsProgressionDetail sorceryPointsProgression =
            sorcerer.SorceryPointsProgression
            ?? throw new InvalidOperationException(
                "Expected Sorcerer to have a Sorcery Points progression.");
        Assert.Equal(19, sorceryPointsProgression.PointsByLevel.Count);
        Assert.All(
            sorceryPointsProgression.PointsByLevel,
            grant => Assert.Equal(grant.CharacterLevel, grant.Points));
        Assert.Equal(
            2,
            sorceryPointsProgression.PointsByLevel[0].CharacterLevel);
        Assert.Equal(
            20,
            sorceryPointsProgression.PointsByLevel[^1].CharacterLevel);
        Assert.False(sorceryPointsProgression.RecoversOnShortRest);

        FontOfMagicConversionDetail fontOfMagicConversion =
            sorcerer.FontOfMagicConversion
            ?? throw new InvalidOperationException(
                "Expected Sorcerer to have a Font of Magic conversion.");
        Assert.Equal(
            [(1, 2), (2, 3), (3, 5), (4, 6), (5, 7)],
            fontOfMagicConversion.SlotCostByLevel
                .OrderBy(grant => grant.SpellSlotLevel)
                .Select(
                    grant =>
                        (grant.SpellSlotLevel, grant.SorceryPointCost)));
    }

    [Fact]
    public void CanonicalFile_PreservesMetamagicAtEachGrantLevel()
    {
        ClassDefinition sorcerer = GetClass(LoadClasses(), "dnd5e2014.class.sorcerer");

        int[] expectedLevels = [3, 10, 17];

        int[] actualLevels = sorcerer.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.metamagic")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesSorcerousOriginChoicePoint()
    {
        ClassDefinition sorcerer = GetClass(LoadClasses(), "dnd5e2014.class.sorcerer");

        Assert.Contains(
            sorcerer.LevelFeatures,
            feature =>
                feature.Level == 1 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.sorcerous-origin");
    }

    [Fact]
    public void CanonicalFile_DoesNotDuplicateWizardsWeaponProficiencyIdList()
    {
        // Sorcerer's named weapon exceptions (dagger, dart, sling,
        // quarterstaff, light crossbow) are identical to Wizard's own
        // list - both classes reference the same underlying WeaponIds
        // rather than each minting redundant data, but they remain two
        // separate ClassDefinition entries since the class itself isn't
        // a shareable unit the way a RuleId citation is.
        IReadOnlyList<ClassDefinition> classes = LoadClasses();
        ClassDefinition sorcerer = GetClass(classes, "dnd5e2014.class.sorcerer");
        ClassDefinition wizard = GetClass(classes, "dnd5e2014.class.wizard");

        Assert.Equal(
            wizard.WeaponProficiencyIds.Select(id => id.Value),
            sorcerer.WeaponProficiencyIds.Select(id => id.Value));
    }

    [Theory]
    [InlineData(
        "dnd5e2014.class.bard",
        "dnd5e2014.spell-slot-progression.full-caster",
        "dnd5e2014.ability.charisma")]
    [InlineData(
        "dnd5e2014.class.cleric",
        "dnd5e2014.spell-slot-progression.full-caster",
        "dnd5e2014.ability.wisdom")]
    [InlineData(
        "dnd5e2014.class.druid",
        "dnd5e2014.spell-slot-progression.full-caster",
        "dnd5e2014.ability.wisdom")]
    [InlineData(
        "dnd5e2014.class.sorcerer",
        "dnd5e2014.spell-slot-progression.full-caster",
        "dnd5e2014.ability.charisma")]
    [InlineData(
        "dnd5e2014.class.wizard",
        "dnd5e2014.spell-slot-progression.full-caster",
        "dnd5e2014.ability.intelligence")]
    [InlineData(
        "dnd5e2014.class.paladin",
        "dnd5e2014.spell-slot-progression.half-caster",
        "dnd5e2014.ability.charisma")]
    [InlineData(
        "dnd5e2014.class.ranger",
        "dnd5e2014.spell-slot-progression.half-caster",
        "dnd5e2014.ability.wisdom")]
    [InlineData(
        "dnd5e2014.class.warlock",
        "dnd5e2014.spell-slot-progression.pact-magic",
        "dnd5e2014.ability.charisma")]
    public void CanonicalFile_CastingClassDeclaresExpectedSpellcasting(
        string classId,
        string expectedProgressionId,
        string expectedAbilityId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Equal(
            expectedProgressionId,
            @class.SpellSlotProgressionId?.Value);
        Assert.Equal(
            expectedAbilityId,
            @class.SpellcastingAbilityId?.Value);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.rogue")]
    public void CanonicalFile_NonCasterClassDeclaresNoSpellcasting(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.SpellSlotProgressionId);
        Assert.Null(@class.SpellcastingAbilityId);
    }

    [Theory]
    [InlineData(
        "dnd5e2014.class.barbarian",
        "dnd5e2014.extra-attack-progression.standard")]
    [InlineData(
        "dnd5e2014.class.fighter",
        "dnd5e2014.extra-attack-progression.fighter")]
    [InlineData(
        "dnd5e2014.class.monk",
        "dnd5e2014.extra-attack-progression.standard")]
    [InlineData(
        "dnd5e2014.class.paladin",
        "dnd5e2014.extra-attack-progression.standard")]
    [InlineData(
        "dnd5e2014.class.ranger",
        "dnd5e2014.extra-attack-progression.standard")]
    public void CanonicalFile_ClassDeclaresExpectedExtraAttackProgression(
        string classId,
        string expectedProgressionId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Equal(
            expectedProgressionId,
            @class.ExtraAttackProgressionId?.Value);
    }

    [Theory]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonExtraAttackClassDeclaresNoProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.ExtraAttackProgressionId);
    }

    [Fact]
    public void CanonicalFile_BarbarianRageMatchesThePhbTable()
    {
        ClassDefinition barbarian =
            GetClass(LoadClasses(), "dnd5e2014.class.barbarian");

        RageProgressionDetail rage = barbarian.RageProgression
            ?? throw new InvalidOperationException(
                "Barbarian is expected to declare a Rage progression.");

        Assert.Equal(
            [(1, 2), (3, 3), (6, 4), (12, 5), (17, 6), (20, (int?)null)],
            rage.UsesByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant =>
                        (grant.CharacterLevel, grant.UsesPerLongRest)));

        Assert.Equal(
            [(1, 2), (9, 3), (16, 4)],
            rage.DamageBonusByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.Bonus)));

        Assert.Equal(1, rage.DurationMinutes);
        Assert.True(rage.RequiresNotWearingHeavyArmor);

        Assert.Equal(
            [
                "dnd5e2014.damage-type.bludgeoning",
                "dnd5e2014.damage-type.piercing",
                "dnd5e2014.damage-type.slashing"
            ],
            rage.ResistedDamageTypeIds
                .Select(id => id.Value)
                .OrderBy(value => value, StringComparer.Ordinal));
    }

    [Theory]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonBarbarianClassDeclaresNoRageProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.RageProgression);
    }

    [Fact]
    public void CanonicalFile_RogueSneakAttackMatchesThePhbTable()
    {
        ClassDefinition rogue =
            GetClass(LoadClasses(), "dnd5e2014.class.rogue");

        SneakAttackProgressionDetail sneakAttack =
            rogue.SneakAttackProgression
            ?? throw new InvalidOperationException(
                "Rogue is expected to declare a Sneak Attack " +
                "progression.");

        Assert.Equal(
            [
                (1, 1), (3, 2), (5, 3), (7, 4), (9, 5),
                (11, 6), (13, 7), (15, 8), (17, 9), (19, 10)
            ],
            sneakAttack.DiceByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant =>
                        (grant.CharacterLevel, grant.Damage.Count)));

        Assert.All(
            sneakAttack.DiceByLevel,
            grant => Assert.Equal(6, grant.Damage.Sides));

        Assert.True(sneakAttack.OncePerTurn);
        Assert.True(sneakAttack.RequiresFinesseOrRangedWeapon);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonRogueClassDeclaresNoSneakAttackProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.SneakAttackProgression);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonMonkClassDeclaresNoKiProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.KiProgression);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonSorcererClassDeclaresNoSorceryPointsProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.SorceryPointsProgression);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonDruidClassDeclaresNoWildShapeProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.WildShapeProgression);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonPaladinClassDeclaresNoAuras(string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.AuraOfProtection);
        Assert.Null(@class.AuraOfCourage);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonBardClassDeclaresNoBardicInspirationProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.BardicInspirationProgression);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonClericClassDeclaresNoChannelDivinityProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.ChannelDivinityProgression);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonWarlockClassDeclaresNoMysticArcanumProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.MysticArcanumProgression);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonSorcererClassDeclaresNoFontOfMagicConversion(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.FontOfMagicConversion);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonBardClassDeclaresNoSongOfRestProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.SongOfRestProgression);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonMonkClassDeclaresNoMartialArtsProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.MartialArtsProgression);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonMonkClassDeclaresNoUnarmoredMovementProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.UnarmoredMovementProgression);
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedMonkTableProgressions()
    {
        ClassDefinition monk =
            Dnd5e2014Ruleset.Instance.Classes.Get(
                new ClassId("dnd5e2014.class.monk"));

        MartialArtsProgressionDetail martialArtsProgression =
            monk.MartialArtsProgression
            ?? throw new InvalidOperationException(
                "Expected Monk to have a Martial Arts progression.");
        Assert.Equal(
            [(1, 4), (5, 6), (11, 8), (17, 10)],
            martialArtsProgression.DieByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.Die.Sides)));

        UnarmoredMovementProgressionDetail unarmoredMovementProgression =
            monk.UnarmoredMovementProgression
            ?? throw new InvalidOperationException(
                "Expected Monk to have an Unarmored Movement progression.");
        Assert.Equal(
            [(2, 10), (6, 15), (10, 20), (14, 25), (18, 30)],
            unarmoredMovementProgression.SpeedBonusByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant => (grant.CharacterLevel, grant.SpeedBonusFeet)));
    }

    private static ClassDefinition GetClass(
        IReadOnlyList<ClassDefinition> classes,
        string id)
    {
        return classes.Single(@class => @class.Id.Value == id);
    }

    private static IReadOnlyList<ClassDefinition> LoadClasses()
    {
        return ClassDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "classes.json"));
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(directory.FullName, "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }
}
