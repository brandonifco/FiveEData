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
using FiveEData.Rules.Classes.CantripsKnown;
using FiveEData.Rules.Classes.ChannelDivinity;
using FiveEData.Rules.Classes.DestroyUndead;
using FiveEData.Rules.Classes.EldritchInvocationsKnown;
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
using FiveEData.Rules.Classes.Serialization;
using FiveEData.Rules.Classes.SneakAttack;
using FiveEData.Rules.Classes.SongOfRest;
using FiveEData.Rules.Classes.SorceryPoints;
using FiveEData.Rules.Classes.SpellsKnown;
using FiveEData.Rules.Classes.UnarmoredMovement;
using FiveEData.Rules.Classes.WildShape;
using FiveEData.Rules.Classes.WizardSpellbook;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
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

        ActionSurgeProgressionDetail actionSurgeProgression =
            fighter.ActionSurgeProgression
            ?? throw new InvalidOperationException(
                "Expected Fighter to have an Action Surge progression.");
        Assert.Equal(
            [(2, 1), (17, 2)],
            actionSurgeProgression.UsesByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.UsesPerRest)));
        Assert.True(actionSurgeProgression.RecoversOnShortRest);
        Assert.True(actionSurgeProgression.OncePerTurn);

        IndomitableProgressionDetail indomitableProgression =
            fighter.IndomitableProgression
            ?? throw new InvalidOperationException(
                "Expected Fighter to have an Indomitable progression.");
        Assert.Equal(
            [(9, 1), (13, 2), (17, 3)],
            indomitableProgression.UsesByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.UsesPerRest)));
        Assert.False(indomitableProgression.RecoversOnShortRest);
    }

    // Two features on the same class table, at the same 17th-level row, that
    // recover on different rests: Action Surge on a short rest, Indomitable
    // only on a long one. The rest is read per feature, never inferred from
    // the class.
    [Fact]
    public void CanonicalFile_FighterActionSurgeAndIndomitableRecoverOnDifferentRests()
    {
        ClassDefinition fighter =
            GetClass(LoadClasses(), "dnd5e2014.class.fighter");

        ActionSurgeProgressionDetail actionSurgeProgression =
            fighter.ActionSurgeProgression
            ?? throw new InvalidOperationException(
                "Expected Fighter to have an Action Surge progression.");

        IndomitableProgressionDetail indomitableProgression =
            fighter.IndomitableProgression
            ?? throw new InvalidOperationException(
                "Expected Fighter to have an Indomitable progression.");

        Assert.True(actionSurgeProgression.RecoversOnShortRest);
        Assert.False(indomitableProgression.RecoversOnShortRest);
    }

    // These pages were corrected from an off-by-one that placed the whole
    // block a page late. Every one was verified against the rendered page
    // image: p.72 carries Action Surge through Remarkable Athlete, and p.73
    // starts at Additional Fighting Style.
    [Theory]
    [InlineData("dnd5e2014.class-rule.action-surge", 72)]
    [InlineData("dnd5e2014.class-rule.martial-archetype", 72)]
    [InlineData("dnd5e2014.class-rule.fighter-ability-score-improvement", 72)]
    [InlineData("dnd5e2014.class-rule.fighter-extra-attack", 72)]
    [InlineData("dnd5e2014.class-rule.indomitable", 72)]
    [InlineData("dnd5e2014.class-rule.improved-critical", 72)]
    [InlineData("dnd5e2014.class-rule.remarkable-athlete", 72)]
    [InlineData("dnd5e2014.class-rule.additional-fighting-style", 73)]
    [InlineData("dnd5e2014.class-rule.superior-critical", 73)]
    [InlineData("dnd5e2014.class-rule.survivor", 73)]
    public void CanonicalFile_CitesFighterFeaturePagesWhereTheirBodyTextStarts(
        string ruleId,
        int expectedPage)
    {
        RuleDefinition rule =
            Dnd5e2014Ruleset.Instance.Rules.Get(new RuleId(ruleId));

        SourceReference source = Assert.Single(rule.Sources);
        Assert.Equal(expectedPage, source.Page);
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

        PrimalChampionDetail primalChampion =
            barbarian.PrimalChampion
            ?? throw new InvalidOperationException(
                "Expected Barbarian to have Primal Champion.");
        Assert.Equal(
            ["dnd5e2014.ability.strength", "dnd5e2014.ability.constitution"],
            primalChampion.AbilityIds.Select(id => id.Value).ToArray());
        Assert.Equal(4, primalChampion.AbilityScoreIncrease);
        Assert.Equal(24, primalChampion.MaximumAbilityScore);

        BrutalCriticalProgressionDetail brutalCriticalProgression =
            barbarian.BrutalCriticalProgression
            ?? throw new InvalidOperationException(
                "Expected Barbarian to have a Brutal Critical progression.");
        Assert.Equal(
            [(9, 1), (13, 2), (17, 3)],
            brutalCriticalProgression.AdditionalDiceByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant => (grant.CharacterLevel, grant.AdditionalDice)));
        Assert.True(brutalCriticalProgression.RequiresMeleeAttack);

        FastMovementDetail fastMovement =
            barbarian.FastMovement
            ?? throw new InvalidOperationException(
                "Expected Barbarian to have Fast Movement.");
        Assert.Equal(10, fastMovement.SpeedBonusFeet);
        Assert.True(fastMovement.RequiresNotWearingHeavyArmor);
    }

    // Fast Movement and Rage both gate on armor, and on different thresholds:
    // Fast Movement is blocked only by heavy armor, while Monk's Unarmored
    // Movement is blocked by any armor. The threshold is read per feature.
    [Fact]
    public void CanonicalFile_BarbarianFastMovementGatesOnHeavyArmorOnly()
    {
        IReadOnlyList<ClassDefinition> classes = LoadClasses();

        FastMovementDetail fastMovement =
            GetClass(classes, "dnd5e2014.class.barbarian").FastMovement
            ?? throw new InvalidOperationException(
                "Expected Barbarian to have Fast Movement.");

        UnarmoredMovementProgressionDetail unarmoredMovement =
            GetClass(classes, "dnd5e2014.class.monk")
                .UnarmoredMovementProgression
            ?? throw new InvalidOperationException(
                "Expected Monk to have an Unarmored Movement progression.");

        Assert.True(fastMovement.RequiresNotWearingHeavyArmor);
        Assert.True(unarmoredMovement.RequiresNotWearingArmor);
        Assert.True(unarmoredMovement.RequiresNotWieldingShield);
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

        MagicalSecretsProgressionDetail magicalSecretsProgression =
            bard.MagicalSecretsProgression
            ?? throw new InvalidOperationException(
                "Expected Bard to have a Magical Secrets progression.");
        Assert.Equal(
            [(10, 2), (14, 4), (18, 6)],
            magicalSecretsProgression.SpellsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.SpellsKnown)));
        Assert.True(magicalSecretsProgression.CountsAgainstSpellsKnown);

        CantripsKnownProgressionDetail cantripsKnownProgression =
            bard.CantripsKnownProgression
            ?? throw new InvalidOperationException(
                "Expected Bard to have a Cantrips Known progression.");
        Assert.Equal(
            [(1, 2), (4, 3), (10, 4)],
            cantripsKnownProgression.CantripsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.CantripsKnown)));

        SpellsKnownProgressionDetail spellsKnownProgression =
            bard.SpellsKnownProgression
            ?? throw new InvalidOperationException(
                "Expected Bard to have a Spells Known progression.");
        Assert.Equal(
            [
                (1, 4), (2, 5), (3, 6), (4, 7), (5, 8), (6, 9), (7, 10),
                (8, 11), (9, 12), (10, 14), (11, 15), (13, 16), (14, 18),
                (15, 19), (17, 20), (18, 22)
            ],
            spellsKnownProgression.SpellsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.SpellsKnown)));
    }

    // The Bard's own Magical Secrets spells "are included in the number in
    // the Spells Known column" — the table's Spells Known jumps by two at
    // 10th, 14th, and 18th to absorb them. College of Lore's Additional
    // Magical Secrets explicitly does the opposite, which is why this is a
    // field rather than an assumed constant.
    [Fact]
    public void CanonicalFile_BardMagicalSecretsCountAgainstSpellsKnown()
    {
        ClassDefinition bard = GetClass(LoadClasses(), "dnd5e2014.class.bard");

        MagicalSecretsProgressionDetail magicalSecretsProgression =
            bard.MagicalSecretsProgression
            ?? throw new InvalidOperationException(
                "Expected Bard to have a Magical Secrets progression.");

        Assert.True(magicalSecretsProgression.CountsAgainstSpellsKnown);
    }

    [Fact]
    public void CanonicalFile_BardGrantsMagicalSecretsAtEveryProgressionLevel()
    {
        ClassDefinition bard = GetClass(LoadClasses(), "dnd5e2014.class.bard");

        int[] featureLevels = bard.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.magical-secrets")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        MagicalSecretsProgressionDetail magicalSecretsProgression =
            bard.MagicalSecretsProgression
            ?? throw new InvalidOperationException(
                "Expected Bard to have a Magical Secrets progression.");

        int[] progressionLevels = magicalSecretsProgression.SpellsKnownByLevel
            .Select(grant => grant.CharacterLevel)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal([10, 14, 18], featureLevels);
        Assert.Equal(featureLevels, progressionLevels);
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
    public void CanonicalFile_NonBardClassDeclaresNoMagicalSecretsProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.MagicalSecretsProgression);
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedBardMagicalSecretsProgression()
    {
        ClassDefinition bard =
            Dnd5e2014Ruleset.Instance.Classes.Get(
                new ClassId("dnd5e2014.class.bard"));

        MagicalSecretsProgressionDetail magicalSecretsProgression =
            bard.MagicalSecretsProgression
            ?? throw new InvalidOperationException(
                "Expected Bard to have a Magical Secrets progression.");
        Assert.Equal(
            [(10, 2), (14, 4), (18, 6)],
            magicalSecretsProgression.SpellsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.SpellsKnown)));
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

        CantripsKnownProgressionDetail cantripsKnownProgression =
            wizard.CantripsKnownProgression
            ?? throw new InvalidOperationException(
                "Expected Wizard to have a Cantrips Known progression.");
        Assert.Equal(
            [(1, 3), (4, 4), (10, 5)],
            cantripsKnownProgression.CantripsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.CantripsKnown)));

        Assert.Null(wizard.SpellsKnownProgression);

        WizardSpellbookDetail wizardSpellbook =
            wizard.WizardSpellbook
            ?? throw new InvalidOperationException(
                "Expected Wizard to have a spellbook detail.");
        Assert.Equal(6, wizardSpellbook.StartingSpellCount);
        Assert.Equal(2, wizardSpellbook.SpellsAddedPerLevelAfterFirst);
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
    [InlineData("dnd5e2014.class.warlock")]
    public void CanonicalFile_NonWizardClassDeclaresNoWizardSpellbook(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.WizardSpellbook);
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

        DestroyUndeadProgressionDetail destroyUndeadProgression =
            cleric.DestroyUndeadProgression
            ?? throw new InvalidOperationException(
                "Expected Cleric to have a Destroy Undead progression.");
        Assert.Equal(
            [(5, 0.5), (8, 1.0), (11, 2.0), (14, 3.0), (17, 4.0)],
            destroyUndeadProgression.ThresholdsByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant =>
                        (grant.CharacterLevel, grant.MaxChallengeRating)));

        CantripsKnownProgressionDetail cantripsKnownProgression =
            cleric.CantripsKnownProgression
            ?? throw new InvalidOperationException(
                "Expected Cleric to have a Cantrips Known progression.");
        Assert.Equal(
            [(1, 3), (4, 4), (10, 5)],
            cantripsKnownProgression.CantripsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.CantripsKnown)));

        Assert.Null(cleric.SpellsKnownProgression);
    }

    // The Destroy Undead table's first row is CR 1/2, which is why the
    // threshold is a double rather than an int. Wild Shape's
    // MaxChallengeRating is the same type for the same reason.
    [Fact]
    public void CanonicalFile_ClericDestroyUndeadStartsAtFractionalChallengeRating()
    {
        ClassDefinition cleric =
            GetClass(LoadClasses(), "dnd5e2014.class.cleric");

        DestroyUndeadProgressionDetail destroyUndeadProgression =
            cleric.DestroyUndeadProgression
            ?? throw new InvalidOperationException(
                "Expected Cleric to have a Destroy Undead progression.");

        DestroyUndeadThresholdGrant first = destroyUndeadProgression
            .ThresholdsByLevel
            .OrderBy(grant => grant.CharacterLevel)
            .First();

        Assert.Equal(5, first.CharacterLevel);
        Assert.Equal(0.5, first.MaxChallengeRating);
    }

    // Unlike Monk's Martial Arts, the Cleric table's Features column names
    // Destroy Undead at every level its threshold rises — "Destroy Undead
    // (CR 1/2)" at 5th through "(CR 4)" at 17th — so the two lists agree here
    // and LevelFeatures carries all five. The table decides, every time.
    [Fact]
    public void CanonicalFile_ClericGrantsDestroyUndeadAtEveryThresholdLevel()
    {
        ClassDefinition cleric =
            GetClass(LoadClasses(), "dnd5e2014.class.cleric");

        int[] featureLevels = cleric.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.destroy-undead")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        DestroyUndeadProgressionDetail destroyUndeadProgression =
            cleric.DestroyUndeadProgression
            ?? throw new InvalidOperationException(
                "Expected Cleric to have a Destroy Undead progression.");

        int[] thresholdLevels = destroyUndeadProgression.ThresholdsByLevel
            .Select(grant => grant.CharacterLevel)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal([5, 8, 11, 14, 17], featureLevels);
        Assert.Equal(featureLevels, thresholdLevels);
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
    public void CanonicalFile_NonClericClassDeclaresNoDestroyUndeadProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.DestroyUndeadProgression);
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedClericDestroyUndeadProgression()
    {
        ClassDefinition cleric =
            Dnd5e2014Ruleset.Instance.Classes.Get(
                new ClassId("dnd5e2014.class.cleric"));

        DestroyUndeadProgressionDetail destroyUndeadProgression =
            cleric.DestroyUndeadProgression
            ?? throw new InvalidOperationException(
                "Expected Cleric to have a Destroy Undead progression.");
        Assert.Equal(
            [(5, 0.5), (8, 1.0), (11, 2.0), (14, 3.0), (17, 4.0)],
            destroyUndeadProgression.ThresholdsByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant =>
                        (grant.CharacterLevel, grant.MaxChallengeRating)));
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

        CantripsKnownProgressionDetail cantripsKnownProgression =
            warlock.CantripsKnownProgression
            ?? throw new InvalidOperationException(
                "Expected Warlock to have a Cantrips Known progression.");
        Assert.Equal(
            [(1, 2), (4, 3), (10, 4)],
            cantripsKnownProgression.CantripsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.CantripsKnown)));

        SpellsKnownProgressionDetail spellsKnownProgression =
            warlock.SpellsKnownProgression
            ?? throw new InvalidOperationException(
                "Expected Warlock to have a Spells Known progression.");
        Assert.Equal(
            [
                (1, 2), (2, 3), (3, 4), (4, 5), (5, 6), (6, 7), (7, 8),
                (8, 9), (9, 10), (11, 11), (13, 12), (15, 13), (17, 14),
                (19, 15)
            ],
            spellsKnownProgression.SpellsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.SpellsKnown)));
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

    [Theory]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_CasterClassDeclaresACantripsKnownProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.NotNull(@class.CantripsKnownProgression);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    public void CanonicalFile_NonCantripCasterClassDeclaresNoCantripsKnownProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.CantripsKnownProgression);
    }

    [Theory]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    public void CanonicalFile_KnownCasterClassDeclaresASpellsKnownProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.NotNull(@class.SpellsKnownProgression);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonKnownCasterClassDeclaresNoSpellsKnownProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.SpellsKnownProgression);
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

        CantripsKnownProgressionDetail cantripsKnownProgression =
            druid.CantripsKnownProgression
            ?? throw new InvalidOperationException(
                "Expected Druid to have a Cantrips Known progression.");
        Assert.Equal(
            [(1, 2), (4, 3), (10, 4)],
            cantripsKnownProgression.CantripsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.CantripsKnown)));

        Assert.Null(druid.SpellsKnownProgression);
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

        FavoredEnemyProgressionDetail favoredEnemyProgression =
            ranger.FavoredEnemyProgression
            ?? throw new InvalidOperationException(
                "Expected Ranger to have a Favored Enemy progression.");
        Assert.Equal(
            [(1, 1), (6, 2), (14, 3)],
            favoredEnemyProgression.EnemyTypesKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant => (grant.CharacterLevel, grant.EnemyTypesKnown)));
        Assert.True(
            favoredEnemyProgression.GrantsAssociatedLanguagePerChoice);

        NaturalExplorerProgressionDetail naturalExplorerProgression =
            ranger.NaturalExplorerProgression
            ?? throw new InvalidOperationException(
                "Expected Ranger to have a Natural Explorer progression.");
        Assert.Equal(
            [(1, 1), (6, 2), (10, 3)],
            naturalExplorerProgression.FavoredTerrainsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant =>
                        (grant.CharacterLevel, grant.FavoredTerrainsKnown)));

        Assert.Null(ranger.CantripsKnownProgression);

        SpellsKnownProgressionDetail spellsKnownProgression =
            ranger.SpellsKnownProgression
            ?? throw new InvalidOperationException(
                "Expected Ranger to have a Spells Known progression.");
        Assert.Equal(
            [
                (2, 2), (3, 3), (5, 4), (7, 5), (9, 6), (11, 7), (13, 8),
                (15, 9), (17, 10), (19, 11)
            ],
            spellsKnownProgression.SpellsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.SpellsKnown)));
    }

    // Both counts are cumulative totals, not per-level increments: a 14th
    // level ranger knows three favored enemies, not one plus one plus one.
    // Both features also improve together at 6th and then diverge — Favored
    // Enemy at 14th, Natural Explorer at 10th — so neither level list can be
    // derived from the other.
    [Fact]
    public void CanonicalFile_RangerFavoredEnemyAndNaturalExplorerDivergeAfterSixthLevel()
    {
        ClassDefinition ranger =
            GetClass(LoadClasses(), "dnd5e2014.class.ranger");

        FavoredEnemyProgressionDetail favoredEnemyProgression =
            ranger.FavoredEnemyProgression
            ?? throw new InvalidOperationException(
                "Expected Ranger to have a Favored Enemy progression.");

        NaturalExplorerProgressionDetail naturalExplorerProgression =
            ranger.NaturalExplorerProgression
            ?? throw new InvalidOperationException(
                "Expected Ranger to have a Natural Explorer progression.");

        Assert.Equal(
            [1, 6, 14],
            favoredEnemyProgression.EnemyTypesKnownByLevel
                .Select(grant => grant.CharacterLevel)
                .OrderBy(level => level));
        Assert.Equal(
            [1, 6, 10],
            naturalExplorerProgression.FavoredTerrainsKnownByLevel
                .Select(grant => grant.CharacterLevel)
                .OrderBy(level => level));
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.fighter")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonRangerClassDeclaresNoFavoredEnemyProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.FavoredEnemyProgression);
        Assert.Null(@class.NaturalExplorerProgression);
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedRangerQuantizedFeatures()
    {
        ClassDefinition ranger =
            Dnd5e2014Ruleset.Instance.Classes.Get(
                new ClassId("dnd5e2014.class.ranger"));

        FavoredEnemyProgressionDetail favoredEnemyProgression =
            ranger.FavoredEnemyProgression
            ?? throw new InvalidOperationException(
                "Expected Ranger to have a Favored Enemy progression.");
        Assert.Equal(
            [(1, 1), (6, 2), (14, 3)],
            favoredEnemyProgression.EnemyTypesKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant => (grant.CharacterLevel, grant.EnemyTypesKnown)));

        NaturalExplorerProgressionDetail naturalExplorerProgression =
            ranger.NaturalExplorerProgression
            ?? throw new InvalidOperationException(
                "Expected Ranger to have a Natural Explorer progression.");
        Assert.Equal(
            [(1, 1), (6, 2), (10, 3)],
            naturalExplorerProgression.FavoredTerrainsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant =>
                        (grant.CharacterLevel, grant.FavoredTerrainsKnown)));
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

        CantripsKnownProgressionDetail cantripsKnownProgression =
            sorcerer.CantripsKnownProgression
            ?? throw new InvalidOperationException(
                "Expected Sorcerer to have a Cantrips Known progression.");
        Assert.Equal(
            [(1, 4), (4, 5), (10, 6)],
            cantripsKnownProgression.CantripsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.CantripsKnown)));

        SpellsKnownProgressionDetail spellsKnownProgression =
            sorcerer.SpellsKnownProgression
            ?? throw new InvalidOperationException(
                "Expected Sorcerer to have a Spells Known progression.");
        Assert.Equal(
            [
                (1, 2), (2, 3), (3, 4), (4, 5), (5, 6), (6, 7), (7, 8),
                (8, 9), (9, 10), (10, 11), (11, 12), (13, 13), (15, 14),
                (17, 15)
            ],
            spellsKnownProgression.SpellsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.SpellsKnown)));
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
    public void CanonicalFile_NonBarbarianClassDeclaresNoBrutalCriticalProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.BrutalCriticalProgression);
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
    public void CanonicalFile_NonBarbarianClassDeclaresNoFastMovement(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.FastMovement);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonFighterClassDeclaresNoActionSurgeProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.ActionSurgeProgression);
    }

    [Theory]
    [InlineData("dnd5e2014.class.barbarian")]
    [InlineData("dnd5e2014.class.bard")]
    [InlineData("dnd5e2014.class.cleric")]
    [InlineData("dnd5e2014.class.druid")]
    [InlineData("dnd5e2014.class.monk")]
    [InlineData("dnd5e2014.class.paladin")]
    [InlineData("dnd5e2014.class.ranger")]
    [InlineData("dnd5e2014.class.rogue")]
    [InlineData("dnd5e2014.class.sorcerer")]
    [InlineData("dnd5e2014.class.warlock")]
    [InlineData("dnd5e2014.class.wizard")]
    public void CanonicalFile_NonFighterClassDeclaresNoIndomitableProgression(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.IndomitableProgression);
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedFighterQuantizedFeatures()
    {
        ClassDefinition fighter =
            Dnd5e2014Ruleset.Instance.Classes.Get(
                new ClassId("dnd5e2014.class.fighter"));

        ActionSurgeProgressionDetail actionSurgeProgression =
            fighter.ActionSurgeProgression
            ?? throw new InvalidOperationException(
                "Expected Fighter to have an Action Surge progression.");
        Assert.Equal(
            [(2, 1), (17, 2)],
            actionSurgeProgression.UsesByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.UsesPerRest)));

        IndomitableProgressionDetail indomitableProgression =
            fighter.IndomitableProgression
            ?? throw new InvalidOperationException(
                "Expected Fighter to have an Indomitable progression.");
        Assert.Equal(
            [(9, 1), (13, 2), (17, 3)],
            indomitableProgression.UsesByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.UsesPerRest)));
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedBarbarianQuantizedFeatures()
    {
        ClassDefinition barbarian =
            Dnd5e2014Ruleset.Instance.Classes.Get(
                new ClassId("dnd5e2014.class.barbarian"));

        BrutalCriticalProgressionDetail brutalCriticalProgression =
            barbarian.BrutalCriticalProgression
            ?? throw new InvalidOperationException(
                "Expected Barbarian to have a Brutal Critical progression.");
        Assert.Equal(
            [(9, 1), (13, 2), (17, 3)],
            brutalCriticalProgression.AdditionalDiceByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant => (grant.CharacterLevel, grant.AdditionalDice)));

        FastMovementDetail fastMovement =
            barbarian.FastMovement
            ?? throw new InvalidOperationException(
                "Expected Barbarian to have Fast Movement.");
        Assert.Equal(10, fastMovement.SpeedBonusFeet);
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

    [Fact]
    public void CanonicalFile_PreservesRogueTierBScalars()
    {
        ClassDefinition rogue = GetClass(LoadClasses(), "dnd5e2014.class.rogue");

        BlindsenseDetail blindsense =
            rogue.Blindsense
            ?? throw new InvalidOperationException(
                "Expected Rogue to have Blindsense.");
        Assert.Equal(10, blindsense.RangeFeet);
        Assert.True(blindsense.RequiresHearing);

        Assert.Equal(10, rogue.ReliableTalentMinimumD20Roll);
    }

    [Fact]
    public void CanonicalFile_PreservesRangerFeralSenses()
    {
        ClassDefinition ranger =
            GetClass(LoadClasses(), "dnd5e2014.class.ranger");

        FeralSensesDetail feralSenses =
            ranger.FeralSenses
            ?? throw new InvalidOperationException(
                "Expected Ranger to have Feral Senses.");
        Assert.Equal(30, feralSenses.RangeFeet);
        Assert.True(feralSenses.NegatesUnseenAttackDisadvantage);
    }

    [Fact]
    public void CanonicalFile_PreservesPaladinTierBScalars()
    {
        ClassDefinition paladin =
            GetClass(LoadClasses(), "dnd5e2014.class.paladin");

        DivineSenseDetail divineSense =
            paladin.DivineSense
            ?? throw new InvalidOperationException(
                "Expected Paladin to have Divine Sense.");
        Assert.Equal(60, divineSense.RangeFeet);
        Assert.True(divineSense.RecoversOnLongRest);

        ImprovedDivineSmiteDetail improvedDivineSmite =
            paladin.ImprovedDivineSmite
            ?? throw new InvalidOperationException(
                "Expected Paladin to have Improved Divine Smite.");
        Assert.Equal(1, improvedDivineSmite.Damage.Count);
        Assert.Equal(8, improvedDivineSmite.Damage.Sides);
        Assert.Equal(
            "dnd5e2014.damage-type.radiant",
            improvedDivineSmite.DamageTypeId.Value);
        Assert.True(improvedDivineSmite.RequiresMeleeWeapon);
    }

    // Blindsense, Feral Senses, and Divine Sense are all "you are aware of
    // creatures within N feet", and all three carry a different range and a
    // different qualifying condition. The range is read per feature.
    [Fact]
    public void CanonicalFile_TierBAwarenessRangesDifferPerFeature()
    {
        IReadOnlyList<ClassDefinition> classes = LoadClasses();

        int blindsenseRange =
            (GetClass(classes, "dnd5e2014.class.rogue").Blindsense
                ?? throw new InvalidOperationException(
                    "Expected Rogue to have Blindsense.")).RangeFeet;
        int feralSensesRange =
            (GetClass(classes, "dnd5e2014.class.ranger").FeralSenses
                ?? throw new InvalidOperationException(
                    "Expected Ranger to have Feral Senses.")).RangeFeet;
        int divineSenseRange =
            (GetClass(classes, "dnd5e2014.class.paladin").DivineSense
                ?? throw new InvalidOperationException(
                    "Expected Paladin to have Divine Sense.")).RangeFeet;

        Assert.Equal(10, blindsenseRange);
        Assert.Equal(30, feralSensesRange);
        Assert.Equal(60, divineSenseRange);
    }

    // Indomitable Might sits at Barbarian 18th, one row above Primal
    // Champion, and was listed as a Tier B candidate. Reading it shows it
    // carries no number at all — "if your total for a Strength check is less
    // than your Strength score, you can use that score in place of the
    // total". It stays citation-only, the same call Pact Boon earned.
    [Fact]
    public void CanonicalFile_BarbarianIndomitableMightStaysCitationOnly()
    {
        ClassDefinition barbarian =
            GetClass(LoadClasses(), "dnd5e2014.class.barbarian");

        Assert.Contains(
            barbarian.LevelFeatures,
            feature => feature.Level == 18 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.indomitable-might");
        Assert.Contains(
            barbarian.LevelFeatures,
            feature => feature.Level == 20 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.primal-champion");
        Assert.NotNull(barbarian.PrimalChampion);
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
    public void CanonicalFile_NonRogueClassDeclaresNoBlindsenseOrReliableTalent(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.Blindsense);
        Assert.Null(@class.ReliableTalentMinimumD20Roll);
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
    public void CanonicalFile_NonPaladinClassDeclaresNoDivineSenseOrImprovedDivineSmite(
        string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.DivineSense);
        Assert.Null(@class.ImprovedDivineSmite);
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedTierBClassScalars()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.Equal(
            10,
            ruleset.Classes.Get(new ClassId("dnd5e2014.class.rogue"))
                .ReliableTalentMinimumD20Roll);
        Assert.Equal(
            30,
            (ruleset.Classes.Get(new ClassId("dnd5e2014.class.ranger"))
                .FeralSenses
                ?? throw new InvalidOperationException(
                    "Expected Ranger to have Feral Senses.")).RangeFeet);
        Assert.Equal(
            24,
            (ruleset.Classes.Get(new ClassId("dnd5e2014.class.barbarian"))
                .PrimalChampion
                ?? throw new InvalidOperationException(
                    "Expected Barbarian to have Primal Champion."))
                .MaximumAbilityScore);
    }

    [Fact]
    public void CanonicalFile_PreservesMonkFixedKiCosts()
    {
        ClassDefinition monk = GetClass(LoadClasses(), "dnd5e2014.class.monk");

        Assert.Equal(1, monk.StunningStrikeKiCost);
        Assert.Equal(1, monk.DiamondSoulRerollKiCost);
        Assert.Equal(4, monk.PerfectSelfKiPointsRegained);

        EmptyBodyDetail emptyBody =
            monk.EmptyBody
            ?? throw new InvalidOperationException(
                "Expected Monk to have Empty Body.");
        Assert.Equal(4, emptyBody.InvisibilityKiCost);
        Assert.Equal(1, emptyBody.InvisibilityDurationMinutes);
        Assert.Equal(8, emptyBody.AstralProjectionKiCost);
    }

    // Empty Body buys two different things at two different prices from one
    // feature - 4 ki to turn invisible, 8 ki to cast astral projection - so
    // it cannot be a single ki-cost scalar the way Stunning Strike is.
    [Fact]
    public void CanonicalFile_MonkEmptyBodyCarriesTwoDistinctKiCosts()
    {
        ClassDefinition monk = GetClass(LoadClasses(), "dnd5e2014.class.monk");

        EmptyBodyDetail emptyBody =
            monk.EmptyBody
            ?? throw new InvalidOperationException(
                "Expected Monk to have Empty Body.");

        Assert.NotEqual(
            emptyBody.InvisibilityKiCost,
            emptyBody.AstralProjectionKiCost);
    }

    // Perfect Self and Sorcerous Restoration regain points rather than
    // spending them, which is why they are named "…Regained" and not "…Cost".
    [Fact]
    public void CanonicalFile_RegainedResourcesAreDistinctFromCosts()
    {
        IReadOnlyList<ClassDefinition> classes = LoadClasses();

        Assert.Equal(
            4,
            GetClass(classes, "dnd5e2014.class.monk")
                .PerfectSelfKiPointsRegained);
        Assert.Equal(
            4,
            GetClass(classes, "dnd5e2014.class.sorcerer")
                .SorcerousRestorationSorceryPointsRegained);
        Assert.Null(
            GetClass(classes, "dnd5e2014.class.sorcerer")
                .StunningStrikeKiCost);
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
    public void CanonicalFile_NonMonkClassDeclaresNoKiCosts(string classId)
    {
        ClassDefinition @class = GetClass(LoadClasses(), classId);

        Assert.Null(@class.StunningStrikeKiCost);
        Assert.Null(@class.DiamondSoulRerollKiCost);
        Assert.Null(@class.EmptyBody);
        Assert.Null(@class.PerfectSelfKiPointsRegained);
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedFixedResourceCosts()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.Equal(
            8,
            (ruleset.Classes.Get(new ClassId("dnd5e2014.class.monk"))
                .EmptyBody
                ?? throw new InvalidOperationException(
                    "Expected Monk to have Empty Body."))
                .AstralProjectionKiCost);
        Assert.Equal(
            4,
            ruleset.Classes.Get(new ClassId("dnd5e2014.class.sorcerer"))
                .SorcerousRestorationSorceryPointsRegained);
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
