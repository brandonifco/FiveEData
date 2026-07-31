using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Tests;

public sealed class CatalogApiTests
{
    [Fact]
    public void Dnd5e2014Ruleset_ExposesCompleteEmbeddedCatalogs()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.Equal(38, ruleset.Weapons.Count);
        Assert.Equal(4, ruleset.Ammunition.Count);
        Assert.Equal(1, ruleset.Sources.Count);
        Assert.Equal(2, ruleset.Rules.Count);

        WeaponDefinition longsword =
            ruleset.Weapons.Get(
                new WeaponId("dnd5e2014.weapon.longsword"));

        Assert.Equal("Longsword", longsword.Name);
        Assert.Equal(
            new DiceExpression(1, 8),
            longsword.Damage?.Dice);

        AmmunitionDefinition arrows =
            ruleset.Ammunition.Get(
                new AmmunitionTypeId("dnd5e2014.ammunition.arrow"));

        Assert.Equal("Arrows", arrows.Name);

        SourceDocument phb =
            ruleset.Sources.Get(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"));

        Assert.Equal("Player's Handbook", phb.Title);
    }

    [Fact]
    public void WeaponCatalog_GetAndTryGet_HaveExplicitMissingSemantics()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        var existing =
            new WeaponId("dnd5e2014.weapon.dagger");

        Assert.True(
            ruleset.Weapons.TryGet(
                existing,
                out WeaponDefinition? dagger));
        Assert.NotNull(dagger);
        Assert.Equal("Dagger", dagger.Name);

        var missing =
            new WeaponId("dnd5e2014.weapon.does-not-exist");

        Assert.False(
            ruleset.Weapons.TryGet(
                missing,
                out WeaponDefinition? missingWeapon));
        Assert.Null(missingWeapon);

        Assert.Throws<KeyNotFoundException>(
            () => ruleset.Weapons.Get(missing));
    }

    [Fact]
    public void WeaponCatalog_EnumerationIsDeterministicByStableId()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        string[] actual = ruleset.Weapons.All
            .Select(weapon => weapon.Id.Value)
            .ToArray();

        string[] expected = actual
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void WeaponCatalog_DefensivelySnapshotsInput()
    {
        var source = new List<WeaponDefinition>
        {
            CreateTestWeapon("dnd5e2014.weapon.one", "One")
        };

        var catalog = new WeaponCatalog(source);

        source.Add(
            CreateTestWeapon(
                "dnd5e2014.weapon.two",
                "Two"));

        Assert.Single(catalog.All);
        Assert.Equal(1, catalog.Count);
    }

    [Fact]
    public void WeaponCatalog_RejectsDuplicateIds()
    {
        WeaponDefinition first =
            CreateTestWeapon(
                "dnd5e2014.weapon.duplicate",
                "First");

        WeaponDefinition second =
            CreateTestWeapon(
                "dnd5e2014.weapon.duplicate",
                "Second");

        Assert.Throws<ArgumentException>(
            () => new WeaponCatalog([first, second]));
    }

    private static WeaponDefinition CreateTestWeapon(
        string id,
        string name)
    {
        return new WeaponDefinition(
            new WeaponId(id),
            name,
            WeaponProficiencyCategory.Simple,
            WeaponUsageCategory.Melee,
            cost: null,
            weight: null,
            damage: new WeaponDamage(
                new DiceExpression(1, 4),
                fixedAmount: 0,
                DamageType.Bludgeoning),
            properties: [],
            range: null,
            versatileDamage: null,
            ammunitionTypeId: null,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 149)
            ]);
    }
}
