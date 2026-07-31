using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Common;
using FiveEData.Rules.Equipment.Weapons;
using FiveEData.Rules.Equipment.Weapons.Serialization;

namespace FiveEData.Tests;

public sealed class WeaponDataFileTests
{
    private static readonly string[] ExpectedWeaponIds =
    {
        "dnd5e2014.weapon.club",
        "dnd5e2014.weapon.dagger",
        "dnd5e2014.weapon.greatclub",
        "dnd5e2014.weapon.handaxe",
        "dnd5e2014.weapon.javelin",
        "dnd5e2014.weapon.light-hammer",
        "dnd5e2014.weapon.mace",
        "dnd5e2014.weapon.quarterstaff",
        "dnd5e2014.weapon.sickle",
        "dnd5e2014.weapon.spear",
        "dnd5e2014.weapon.unarmed-strike",
        "dnd5e2014.weapon.light-crossbow",
        "dnd5e2014.weapon.dart",
        "dnd5e2014.weapon.shortbow",
        "dnd5e2014.weapon.sling",
        "dnd5e2014.weapon.battleaxe",
        "dnd5e2014.weapon.flail",
        "dnd5e2014.weapon.glaive",
        "dnd5e2014.weapon.greataxe",
        "dnd5e2014.weapon.greatsword",
        "dnd5e2014.weapon.halberd",
        "dnd5e2014.weapon.lance",
        "dnd5e2014.weapon.longsword",
        "dnd5e2014.weapon.maul",
        "dnd5e2014.weapon.morningstar",
        "dnd5e2014.weapon.pike",
        "dnd5e2014.weapon.rapier",
        "dnd5e2014.weapon.scimitar",
        "dnd5e2014.weapon.shortsword",
        "dnd5e2014.weapon.trident",
        "dnd5e2014.weapon.war-pick",
        "dnd5e2014.weapon.warhammer",
        "dnd5e2014.weapon.whip",
        "dnd5e2014.weapon.blowgun",
        "dnd5e2014.weapon.hand-crossbow",
        "dnd5e2014.weapon.heavy-crossbow",
        "dnd5e2014.weapon.longbow",
        "dnd5e2014.weapon.net"
    };

    [Fact]
    public void WeaponsJson_LoadsCompleteFirstPrintingCatalog()
    {
        IReadOnlyList<WeaponDefinition> weapons = LoadWeapons();

        Assert.Equal(38, weapons.Count);

        foreach (string expectedId in ExpectedWeaponIds)
        {
            Assert.Contains(
                weapons,
                weapon => weapon.Id == new WeaponId(expectedId));
        }
    }

    [Fact]
    public void WeaponsJson_HasExpectedCategoryCounts()
    {
        IReadOnlyList<WeaponDefinition> weapons = LoadWeapons();

        Assert.Equal(
            11,
            weapons.Count(
                weapon =>
                    weapon.ProficiencyCategory == WeaponProficiencyCategory.Simple &&
                    weapon.UsageCategory == WeaponUsageCategory.Melee));

        Assert.Equal(
            4,
            weapons.Count(
                weapon =>
                    weapon.ProficiencyCategory == WeaponProficiencyCategory.Simple &&
                    weapon.UsageCategory == WeaponUsageCategory.Ranged));

        Assert.Equal(
            18,
            weapons.Count(
                weapon =>
                    weapon.ProficiencyCategory == WeaponProficiencyCategory.Martial &&
                    weapon.UsageCategory == WeaponUsageCategory.Melee));

        Assert.Equal(
            5,
            weapons.Count(
                weapon =>
                    weapon.ProficiencyCategory == WeaponProficiencyCategory.Martial &&
                    weapon.UsageCategory == WeaponUsageCategory.Ranged));
    }

    [Fact]
    public void WeaponsJson_PreservesImportantEdgeCases()
    {
        IReadOnlyList<WeaponDefinition> weapons = LoadWeapons();

        WeaponDefinition unarmedStrike =
            GetWeapon(weapons, "dnd5e2014.weapon.unarmed-strike");

        Assert.Null(unarmedStrike.Cost);
        Assert.Null(unarmedStrike.Weight);
        Assert.Null(unarmedStrike.Damage?.Dice);
        Assert.Equal(1, unarmedStrike.Damage?.FixedAmount);
        Assert.Equal(DamageType.Bludgeoning, unarmedStrike.Damage?.Type);

        WeaponDefinition blowgun =
            GetWeapon(weapons, "dnd5e2014.weapon.blowgun");

        Assert.Null(blowgun.Damage?.Dice);
        Assert.Equal(1, blowgun.Damage?.FixedAmount);
        Assert.Equal(
            new AmmunitionTypeId("dnd5e2014.ammunition.blowgun-needle"),
            blowgun.AmmunitionTypeId);

        WeaponDefinition net =
            GetWeapon(weapons, "dnd5e2014.weapon.net");

        Assert.Null(net.Damage);
        Assert.Equal(5, net.Range?.Normal.Feet);
        Assert.Equal(15, net.Range?.Long.Feet);
        Assert.Contains(WeaponProperty.Special, net.Properties);
        Assert.Contains(WeaponProperty.Thrown, net.Properties);
        Assert.Contains(
            new RuleId("dnd5e2014.weapon-rule.net"),
            net.SpecialRuleIds);

        WeaponDefinition dagger =
            GetWeapon(weapons, "dnd5e2014.weapon.dagger");

        Assert.Equal(WeaponUsageCategory.Melee, dagger.UsageCategory);
        Assert.Contains(WeaponProperty.Thrown, dagger.Properties);
        Assert.Equal(20, dagger.Range?.Normal.Feet);
        Assert.Equal(60, dagger.Range?.Long.Feet);

        WeaponDefinition longsword =
            GetWeapon(weapons, "dnd5e2014.weapon.longsword");

        Assert.Equal(new DiceExpression(1, 10), longsword.VersatileDamage);

        WeaponDefinition dart =
            GetWeapon(weapons, "dnd5e2014.weapon.dart");

        Assert.Equal(0.25m, dart.Weight?.Pounds);

        WeaponDefinition sling =
            GetWeapon(weapons, "dnd5e2014.weapon.sling");

        Assert.Null(sling.Weight);
        Assert.Equal(
            new AmmunitionTypeId("dnd5e2014.ammunition.sling-bullet"),
            sling.AmmunitionTypeId);
    }

    [Fact]
    public void WeaponsJson_PreservesRepresentativeTableValues()
    {
        IReadOnlyList<WeaponDefinition> weapons = LoadWeapons();

        WeaponDefinition greatsword =
            GetWeapon(weapons, "dnd5e2014.weapon.greatsword");

        Assert.Equal(5000, greatsword.Cost?.CopperPieces);
        Assert.Equal(6m, greatsword.Weight?.Pounds);
        Assert.Equal(new DiceExpression(2, 6), greatsword.Damage?.Dice);
        Assert.Equal(DamageType.Slashing, greatsword.Damage?.Type);
        Assert.Contains(WeaponProperty.Heavy, greatsword.Properties);
        Assert.Contains(WeaponProperty.TwoHanded, greatsword.Properties);

        WeaponDefinition heavyCrossbow =
            GetWeapon(weapons, "dnd5e2014.weapon.heavy-crossbow");

        Assert.Equal(100, heavyCrossbow.Range?.Normal.Feet);
        Assert.Equal(400, heavyCrossbow.Range?.Long.Feet);
        Assert.Equal(18m, heavyCrossbow.Weight?.Pounds);
        Assert.Contains(WeaponProperty.Loading, heavyCrossbow.Properties);
        Assert.Equal(
            new AmmunitionTypeId("dnd5e2014.ammunition.crossbow-bolt"),
            heavyCrossbow.AmmunitionTypeId);

        WeaponDefinition lance =
            GetWeapon(weapons, "dnd5e2014.weapon.lance");

        Assert.Contains(WeaponProperty.Reach, lance.Properties);
        Assert.Contains(WeaponProperty.Special, lance.Properties);
        Assert.Contains(
            new RuleId("dnd5e2014.weapon-rule.lance"),
            lance.SpecialRuleIds);
        Assert.Equal(2, lance.Sources.Count);
    }

    private static IReadOnlyList<WeaponDefinition> LoadWeapons()
    {
        string repositoryRoot = FindRepositoryRoot();
        string path = Path.Combine(
            repositoryRoot,
            "Data",
            "dnd5e2014",
            "weapons.json");

        return WeaponDefinitionLoader.LoadFromFile(path);
    }

    private static WeaponDefinition GetWeapon(
        IReadOnlyList<WeaponDefinition> weapons,
        string id)
    {
        return Assert.Single(
            weapons.Where(
                weapon => weapon.Id == new WeaponId(id)));
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
