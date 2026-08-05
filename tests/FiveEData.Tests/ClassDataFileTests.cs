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
        "dnd5e2014.class.barbarian"
    ];

    [Fact]
    public void CanonicalFile_ContainsExactClassClosure()
    {
        IReadOnlyList<ClassDefinition> classes = LoadClasses();

        Assert.Equal(2, classes.Count);
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
                    "dnd5e2014.class-rule.barbarian-ability-score-improvement")
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
