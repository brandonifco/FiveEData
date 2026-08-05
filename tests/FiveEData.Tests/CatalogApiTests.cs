using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.AdventuringGear;
using FiveEData.Rules.Equipment.Mounts;
using FiveEData.Rules.Equipment.MountSupport;
using FiveEData.Rules.Equipment.MountsAndVehicles;
using FiveEData.Rules.Equipment.TradeGoods;
using FiveEData.Rules.Equipment.Vehicles;
using FiveEData.Rules.Equipment.Weapons;
using FiveEData.Rules.Equipment.Tools;

namespace FiveEData.Tests;

public sealed class CatalogApiTests
{
    [Fact]
    public void Dnd5e2014Ruleset_ExposesCompleteEmbeddedCatalogs()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.Equal(38, ruleset.Weapons.Count);
        Assert.Equal(4, ruleset.Ammunition.Count);
        Assert.Equal(95, ruleset.AdventuringGear.Count);
        Assert.Equal(13, ruleset.ContainerCapacities.Count);
        Assert.Equal(3, ruleset.ToolFamilies.Count);
        Assert.Equal(37, ruleset.Tools.Count);
        Assert.Equal(8, ruleset.Mounts.Count);
        Assert.Equal(11, ruleset.Vehicles.Count);
        Assert.Equal(8, ruleset.MountSupport.Count);
        Assert.Equal(23, ruleset.TradeGoods.Count);
        Assert.Equal(7, ruleset.Expenses.Lifestyles.Count);
        Assert.Equal(8, ruleset.Expenses.FoodAndDrink.Count);
        Assert.Equal(6, ruleset.Expenses.HospitalityCosts.Count);
        Assert.Equal(7, ruleset.Expenses.MundaneServices.Count);
        Assert.Equal(
            6,
            ruleset.CreatureVocabulary.Abilities.Count);
        Assert.Equal(
            18,
            ruleset.CreatureVocabulary.Skills.Count);
        Assert.Equal(
            16,
            ruleset.CreatureVocabulary.Languages.Count);
        Assert.Equal(
            6,
            ruleset.CreatureVocabulary.Sizes.Count);
        Assert.Equal(9, ruleset.Races.Count);
        Assert.Equal(9, ruleset.Subraces.Count);
        Assert.Equal(5, ruleset.Classes.Count);
        Assert.Equal(13, ruleset.Subclasses.Count);
        Assert.Equal(1, ruleset.Sources.Count);
        Assert.Equal(244, ruleset.Rules.Count);

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

        ContainerCapacityDefinition backpackCapacity =
            ruleset.ContainerCapacities.Get(
                new AdventuringGearId(
                    "dnd5e2014.adventuring-gear.backpack"));

        Assert.Equal(30m, backpackCapacity.GearWeightCapacity?.Pounds);
        Assert.True(backpackCapacity.AllowsExteriorItemAttachment);

        ToolDefinition thievesTools = ruleset.Tools.Get(
            new ToolId("dnd5e2014.tool.thieves-tools"));
        Assert.Equal("Thieves' tools", thievesTools.Name);

        ToolFamilyDefinition artisanTools = ruleset.ToolFamilies.Get(
            new ToolFamilyId("dnd5e2014.tool-family.artisans-tools"));
        Assert.Equal("Artisan's tools", artisanTools.Name);

        MountDefinition warhorse = ruleset.Mounts.Get(
            new MountId("dnd5e2014.mount.warhorse"));
        Assert.Equal("Warhorse", warhorse.Name);
        Assert.Equal(60, warhorse.Speed.Feet);
        Assert.Equal(540m, warhorse.BaseCarryingCapacity.Pounds);

        VehicleDefinition rowboat = ruleset.Vehicles.Get(
            new VehicleId("dnd5e2014.vehicle.rowboat"));
        Assert.Equal("Rowboat", rowboat.Name);
        Assert.Equal(VehicleKind.Water, rowboat.Kind);
        Assert.Equal(1.5m, rowboat.ListedSpeed?.MilesPerHour);
        Assert.Null(rowboat.ListedWeight);

        MountSupportDefinition militarySaddle = ruleset.MountSupport.Get(
            new MountSupportId(
                "dnd5e2014.mount-support.saddle-military"));
        Assert.Equal("Saddle, military", militarySaddle.Name);
        Assert.Equal(2000, militarySaddle.Cost.CopperPieces);
        Assert.Equal(30m, militarySaddle.ListedWeight?.Pounds);

        TradeGoodDefinition gold = ruleset.TradeGoods.Get(
            new TradeGoodId("dnd5e2014.trade-good.gold"));
        Assert.Equal("Gold", gold.Name);
        Assert.Equal(5000, gold.MarketValue.CopperPieces);
        Assert.Equal(
            new TradeGoodPricingBasis(1, TradeGoodUnit.Pound),
            gold.PricingBasis);

        MountVehicleRules mountVehicleRules = ruleset.MountVehicleRules;
        Assert.Equal(
            5,
            mountVehicleRules.DrawnVehicleCarryingCapacityMultiplier);
        Assert.Equal(4, mountVehicleRules.BardingCostMultiplier);
        Assert.Equal(2, mountVehicleRules.BardingWeightMultiplier);
        Assert.Equal(
            100m,
            mountVehicleRules.RowboatOverlandWeight.Pounds);

        SourceDocument phb =
            ruleset.Sources.Get(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"));

        AbilityDefinition strength =
            ruleset.CreatureVocabulary.Abilities.Get(
                new AbilityId(
                    "dnd5e2014.ability.strength"));

        Assert.Equal("Strength", strength.Name);

        SkillDefinition athletics =
            ruleset.CreatureVocabulary.Skills.Get(
                new SkillId(
                    "dnd5e2014.skill.athletics"));

        Assert.Equal("Athletics", athletics.Name);
        Assert.Equal(
            strength.Id,
            athletics.NormallyAssociatedAbilityId);

        LanguageDefinition common =
            ruleset.CreatureVocabulary.Languages.Get(
                new LanguageId(
                    "dnd5e2014.language.common"));

        Assert.Equal("Common", common.Name);
        Assert.Equal(
            LanguageCategory.Standard,
            common.Category);

        CreatureSizeDefinition medium =
            ruleset.CreatureVocabulary.Sizes.Get(
                new CreatureSizeId(
                    "dnd5e2014.creature-size.medium"));

        Assert.Equal("Medium", medium.Name);

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
                new DamageTypeId("dnd5e2014.damage-type.bludgeoning")),
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
