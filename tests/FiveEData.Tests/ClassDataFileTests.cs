using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Serialization;
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
        "dnd5e2014.class.warlock"
    ];

    [Fact]
    public void CanonicalFile_ContainsExactClassClosure()
    {
        IReadOnlyList<ClassDefinition> classes = LoadClasses();

        Assert.Equal(8, classes.Count);
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
            "dnd5e2014.class.warlock"
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
