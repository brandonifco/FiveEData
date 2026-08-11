using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Multiclassing;
using FiveEData.Rules.Classes.Serialization;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Tests;

public sealed class MulticlassingProficiencyTests
{
    [Fact]
    public void CanonicalFile_SorcererAndWizardGrantNothing()
    {
        // The table prints an em-dash for both. That is a real fact, but
        // it is carried as a null grant rather than an empty one — the
        // constructor rejects a grant that grants nothing.
        Assert.Equal(
            ["dnd5e2014.class.sorcerer", "dnd5e2014.class.wizard"],
            LoadClasses()
                .Where(@class => @class.MulticlassingProficiencyGrant is null)
                .Select(@class => @class.Id.Value)
                .Order());
    }

    [Fact]
    public void Grant_RejectsGrantingNothing()
    {
        Assert.Throws<ArgumentException>(() =>
            new MulticlassingProficiencyGrant());
    }

    [Fact]
    public void Grant_RejectsRestrictingASkillChoiceItDoesNotOffer()
    {
        Assert.Throws<ArgumentException>(() =>
            new MulticlassingProficiencyGrant(
                armorProficiencyCategories: [ArmorCategory.Light],
                skillChoiceCount: 0,
                skillChoiceFromClassSkillList: true));
    }

    [Fact]
    public void CanonicalFile_BardsSkillChoiceIsUnrestrictedUnlikeRangersAndRogues()
    {
        // "one skill of your choice" (Bard) and "one skill from the
        // class's skill list" (Ranger, Rogue) are two different facts.
        IReadOnlyList<ClassDefinition> classes = LoadClasses();

        Assert.Equal(
            ["dnd5e2014.class.bard", "dnd5e2014.class.ranger", "dnd5e2014.class.rogue"],
            classes
                .Where(@class =>
                    @class.MulticlassingProficiencyGrant?.SkillChoiceCount > 0)
                .Select(@class => @class.Id.Value)
                .Order());

        Assert.False(Grant(classes, "bard").SkillChoiceFromClassSkillList);
        Assert.True(Grant(classes, "ranger").SkillChoiceFromClassSkillList);
        Assert.True(Grant(classes, "rogue").SkillChoiceFromClassSkillList);
    }

    [Fact]
    public void CanonicalFile_BarbarianGrantsShieldsWithoutAnyArmorCategory()
    {
        // "Shields, simple weapons, martial weapons" — the only row that
        // grants shields with no armor category at all, which is why the
        // shield flag is independent of the category list.
        MulticlassingProficiencyGrant barbarian =
            Grant(LoadClasses(), "barbarian");

        Assert.True(barbarian.ProficientWithShields);
        Assert.Empty(barbarian.ArmorProficiencyCategories);
        Assert.Equal(
            [WeaponProficiencyCategory.Simple, WeaponProficiencyCategory.Martial],
            barbarian.WeaponProficiencyCategories);
    }

    [Fact]
    public void CanonicalFile_MonkGrantsANamedWeaponBesideACategory()
    {
        // "Simple weapons, shortswords" — the same category-plus-named
        // -exception shape the class's own starting proficiencies use.
        MulticlassingProficiencyGrant monk = Grant(LoadClasses(), "monk");

        Assert.Equal(
            [WeaponProficiencyCategory.Simple],
            monk.WeaponProficiencyCategories);
        Assert.Equal(
            ["dnd5e2014.weapon.shortsword"],
            monk.WeaponProficiencyIds.Select(id => id.Value));
        Assert.Empty(monk.ArmorProficiencyCategories);
        Assert.False(monk.ProficientWithShields);
    }

    [Fact]
    public void CanonicalFile_GrantsAreAStrictSubsetOfStartingProficiencies()
    {
        // The table's whole purpose: multiclassing never grants more than
        // starting in the class would.
        foreach (ClassDefinition @class in LoadClasses())
        {
            if (@class.MulticlassingProficiencyGrant is not { } grant)
            {
                continue;
            }

            Assert.Subset(
                @class.ArmorProficiencyCategories.ToHashSet(),
                grant.ArmorProficiencyCategories.ToHashSet());
            Assert.Subset(
                @class.WeaponProficiencyCategories.ToHashSet(),
                grant.WeaponProficiencyCategories.ToHashSet());
            Assert.Subset(
                @class.WeaponProficiencyIds.ToHashSet(),
                grant.WeaponProficiencyIds.ToHashSet());
            Assert.Subset(
                @class.ToolProficiencyIds.ToHashSet(),
                grant.ToolProficiencyIds.ToHashSet());
            Assert.True(@class.ProficientWithShields || !grant.ProficientWithShields);
            Assert.True(grant.SkillChoiceCount <= @class.SkillChoiceCount);
        }
    }

    private static MulticlassingProficiencyGrant Grant(
        IReadOnlyList<ClassDefinition> classes,
        string slug)
    {
        ClassDefinition @class =
            classes.Single(candidate =>
                candidate.Id.Value == $"dnd5e2014.class.{slug}");

        return Assert.IsType<MulticlassingProficiencyGrant>(
            @class.MulticlassingProficiencyGrant);
    }

    private static IReadOnlyList<ClassDefinition> LoadClasses() =>
        ClassDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(), "Data", "dnd5e2014", "classes.json"));

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }
}
