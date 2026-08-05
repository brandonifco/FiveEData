using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
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
            sources ?? [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 71);
    }
}
