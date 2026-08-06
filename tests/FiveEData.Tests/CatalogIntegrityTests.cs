using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Serialization;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Classes.Spellcasting.Serialization;
using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Classes.ExtraAttack.Serialization;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Abilities.Serialization;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.DamageTypes.Serialization;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Languages.Serialization;
using FiveEData.Rules.Creatures.Races;
using FiveEData.Rules.Creatures.Races.Serialization;
using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Creatures.Sizes.Serialization;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Creatures.Skills.Serialization;
using FiveEData.Rules.Equipment;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.Ammunition.Serialization;
using FiveEData.Rules.Equipment.AdventuringGear;
using FiveEData.Rules.Equipment.AdventuringGear.Serialization;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Armor.Serialization;
using FiveEData.Rules.Equipment.Mounts;
using FiveEData.Rules.Equipment.Mounts.Serialization;
using FiveEData.Rules.Equipment.MountSupport;
using FiveEData.Rules.Equipment.MountSupport.Serialization;
using FiveEData.Rules.Equipment.MountsAndVehicles;
using FiveEData.Rules.Equipment.MountsAndVehicles.Serialization;
using FiveEData.Rules.Equipment.TradeGoods;
using FiveEData.Rules.Equipment.TradeGoods.Serialization;
using FiveEData.Rules.Equipment.Vehicles;
using FiveEData.Rules.Equipment.Vehicles.Serialization;
using FiveEData.Rules.Equipment.Shields;
using FiveEData.Rules.Equipment.Shields.Serialization;
using FiveEData.Rules.Equipment.Weapons;
using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Equipment.Weapons.Serialization;

namespace FiveEData.Tests;

public sealed class CatalogIntegrityTests
{
    [Fact]
    public void PublishedCatalog_HasNoDanglingReferences()
    {
        string root = FindRepositoryRoot();

        IReadOnlyList<SourceDocument> sources =
            SourceDocumentLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "sources.json"));

        IReadOnlyList<AmmunitionDefinition> ammunition =
            AmmunitionDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "ammunition.json"));

        IReadOnlyList<WeaponDefinition> weapons =
            WeaponDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "weapons.json"));

        IReadOnlyList<DamageTypeDefinition> damageTypes =
            DamageTypeDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "damage-types.json"));

        IReadOnlyList<AbilityDefinition> abilities =
            AbilityDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "abilities.json"));

        IReadOnlyList<CreatureSizeDefinition> creatureSizes =
            CreatureSizeDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "creature-sizes.json"));

        IReadOnlyList<LanguageDefinition> languages =
            LanguageDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "languages.json"));

        IReadOnlyList<RaceDefinition> races =
            RaceDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "races.json"));

        IReadOnlyList<SubraceDefinition> subraces =
            SubraceDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "subraces.json"));

        IReadOnlyList<SkillDefinition> skills =
            SkillDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "skills.json"));

        IReadOnlyList<ClassDefinition> classes =
            ClassDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "classes.json"));

        IReadOnlyList<SubclassDefinition> subclasses =
            SubclassDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "subclasses.json"));

        IReadOnlyList<SpellSlotProgressionDefinition> spellSlotProgressions =
            SpellSlotProgressionDefinitionLoader.LoadFromFile(
                Path.Combine(
                    root,
                    "Data",
                    "dnd5e2014",
                    "spell-slot-progressions.json"));

        IReadOnlyList<ExtraAttackProgressionDefinition>
            extraAttackProgressions =
                ExtraAttackProgressionDefinitionLoader.LoadFromFile(
                    Path.Combine(
                        root,
                        "Data",
                        "dnd5e2014",
                        "extra-attack-progressions.json"));

        IReadOnlyList<RuleDefinition> rules = LoadAllRules(root);

        IReadOnlyList<ArmorDefinition> armor =
            ArmorDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "armor.json"));

        IReadOnlyList<ShieldDefinition> shields =
            ShieldDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "shields.json"));

        IReadOnlyList<AdventuringGearDefinition> adventuringGear =
            AdventuringGearDefinitionLoader.LoadFromFile(
                Path.Combine(
                    root,
                    "Data",
                    "dnd5e2014",
                    "adventuring-gear.json"));

        IReadOnlyList<ContainerCapacityDefinition> containerCapacities =
            ContainerCapacityDefinitionLoader.LoadFromFile(
                Path.Combine(
                    root,
                    "Data",
                    "dnd5e2014",
                    "container-capacities.json"));

        IReadOnlyList<MountDefinition> mounts =
            MountDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "mounts.json"));

        IReadOnlyList<VehicleDefinition> vehicles =
            VehicleDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "vehicles.json"));

        IReadOnlyList<MountSupportDefinition> mountSupport =
            MountSupportDefinitionLoader.LoadFromFile(
                Path.Combine(
                    root,
                    "Data",
                    "dnd5e2014",
                    "mount-support.json"));

        MountVehicleRules mountVehicleRules =
            MountVehicleRulesLoader.LoadFromFile(
                Path.Combine(
                    root,
                    "Data",
                    "dnd5e2014",
                    "mount-vehicle-rules.json"));

        IReadOnlyList<TradeGoodDefinition> tradeGoods =
            TradeGoodDefinitionLoader.LoadFromFile(
                Path.Combine(
                    root,
                    "Data",
                    "dnd5e2014",
                    "trade-goods.json"));

        ArmorUsageRules armorUsage =
            ArmorUsageRulesLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "armor-usage.json"));

        Assert.Empty(
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    weapons: weapons,
                    sourceDocuments: sources,
                    ammunition: ammunition,
                    rules: rules,
                    armor: armor,
                    shields: shields,
                    adventuringGear: adventuringGear,
                    containerCapacities: containerCapacities,
                    mounts: mounts,
                    vehicles: vehicles,
                    mountSupport: mountSupport,
                    tradeGoods: tradeGoods,
                    mountVehicleRules: mountVehicleRules,
                    armorUsage: armorUsage,
                    damageTypes: damageTypes,
                    abilities: abilities,
                    sizes: creatureSizes,
                    languages: languages,
                    skills: skills,
                    races: races,
                    subraces: subraces,
                    classes: classes,
                    subclasses: subclasses,
                    spellSlotProgressions: spellSlotProgressions,
                    extraAttackProgressions: extraAttackProgressions)));
    }

    [Fact]
    public void MissingAmmunitionReference_IsRejected()
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        WeaponDefinition weapon = CreateWeapon(
            specialRuleIds: [],
            ammunitionTypeId:
                new AmmunitionTypeId("dnd5e2014.ammunition.missing"));

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    weapons: [weapon],
                    sourceDocuments: sources));

        Assert.Contains(
            errors,
            error => error.Contains("missing ammunition type", StringComparison.Ordinal));
    }

    [Fact]
    public void MissingSourceReference_IsRejected()
    {
        WeaponDefinition weapon = new(
            new WeaponId("dnd5e2014.weapon.test"),
            "Test",
            WeaponProficiencyCategory.Simple,
            WeaponUsageCategory.Melee,
            cost: null,
            weight: null,
            damage: null,
            properties: [],
            range: null,
            versatileDamage: null,
            ammunitionTypeId: null,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId("dnd5e2014.source.missing"),
                    page: 149)
            ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(weapons: [weapon]));

        Assert.Contains(
            errors,
            error => error.Contains("missing source document", StringComparison.Ordinal));
    }

    [Fact]
    public void MissingSpecialRuleReference_IsRejected()
    {
        var sourceId =
            new SourceDocumentId("dnd5e2014.source.phb-first-printing");

        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(sourceId, "Player's Handbook")
        ];

        WeaponDefinition weapon = CreateWeapon(
            specialRuleIds:
            [
                new RuleId("dnd5e2014.weapon-rule.missing")
            ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    weapons: [weapon],
                    sourceDocuments: sources));

        Assert.Contains(
            errors,
            error => error.Contains("missing rule", StringComparison.Ordinal));
    }

    [Fact]
    public void ArmorMissingSourceReference_IsRejected()
    {
        ArmorDefinition armor = new(
            new ArmorId("dnd5e2014.armor.test"),
            "Test armor",
            ArmorCategory.Light,
            new Money(1000),
            new Weight(10m),
            new ArmorClassFormula(11, includesDexterityModifier: true),
            minimumStrengthForFullSpeed: null,
            imposesStealthDisadvantage: false,
            sources:
            [
                new SourceReference(
                    new SourceDocumentId("dnd5e2014.source.missing"),
                    page: 145)
            ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(armor: [armor]));

        Assert.Contains(
            errors,
            error => error.Contains("missing source document", StringComparison.Ordinal));
    }

    [Fact]
    public void ShieldMissingSourceReference_IsRejected()
    {
        ShieldDefinition shield = new(
            new ShieldId("dnd5e2014.armor.shield"),
            "Shield",
            new Money(1000),
            new Weight(6m),
            armorClassBonus: 2,
            sources:
            [
                new SourceReference(
                    new SourceDocumentId("dnd5e2014.source.missing"),
                    page: 145)
            ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(shields: [shield]));

        Assert.Contains(
            errors,
            error => error.Contains("missing source document", StringComparison.Ordinal));
    }

    [Fact]
    public void AdventuringGearMissingSourceReference_IsRejected()
    {
        AdventuringGearDefinition definition = new(
            new AdventuringGearId(
                "dnd5e2014.adventuring-gear.test"),
            "Test gear",
            new Money(100),
            listedWeight: null,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId("dnd5e2014.source.missing"),
                    page: 150)
            ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(adventuringGear: [definition]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "missing source document",
                StringComparison.Ordinal));
    }

    [Fact]
    public void AdventuringGearMissingSpecialRuleReference_IsRejected()
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        AdventuringGearDefinition definition = new(
            new AdventuringGearId(
                "dnd5e2014.adventuring-gear.test"),
            "Test gear",
            new Money(100),
            listedWeight: null,
            specialRuleIds:
            [
                new RuleId("dnd5e2014.adventuring-gear-rule.missing")
            ],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 150)
            ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    sourceDocuments: sources,
                    adventuringGear: [definition]));

        Assert.Contains(
            errors,
            error => error.Contains("missing rule", StringComparison.Ordinal));
    }

    [Fact]
    public void ContainerCapacityMissingGearReference_IsRejected()
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        ContainerCapacityDefinition capacity =
            CreateContainerCapacityDefinition(
                "dnd5e2014.adventuring-gear.missing",
                "dnd5e2014.source.phb-first-printing");

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    sourceDocuments: sources,
                    containerCapacities: [capacity]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "missing adventuring gear",
                StringComparison.Ordinal));
    }

    [Fact]
    public void ContainerCapacityMissingSourceReference_IsRejected()
    {
        AdventuringGearDefinition gear = new(
            new AdventuringGearId(
                "dnd5e2014.adventuring-gear.backpack"),
            "Backpack",
            new Money(200),
            listedWeight: null,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 150)
            ]);

        ContainerCapacityDefinition capacity =
            CreateContainerCapacityDefinition(
                gear.Id.Value,
                "dnd5e2014.source.missing");

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    adventuringGear: [gear],
                    containerCapacities: [capacity]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "missing source document",
                StringComparison.Ordinal));
    }


    [Fact]
    public void ToolMissingFamilyReference_IsRejected()
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        ToolDefinition tool = CreateTool(
            familyId: new ToolFamilyId("dnd5e2014.tool-family.missing"));

        IReadOnlyList<string> errors = CatalogIntegrityValidator.Validate(
            CreateDefinitionSet(sourceDocuments: sources, tools: [tool]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "missing tool family",
                StringComparison.Ordinal));
    }

    [Fact]
    public void ToolMissingRuleReference_IsRejected()
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        ToolDefinition tool = new(
            new ToolId("dnd5e2014.tool.test"),
            "Test tool",
            new Money(100),
            weight: null,
            familyId: null,
            specialRuleIds:
            [
                new RuleId("dnd5e2014.tool-rule.missing")
            ],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 154)
            ]);

        IReadOnlyList<string> errors = CatalogIntegrityValidator.Validate(
            CreateDefinitionSet(sourceDocuments: sources, tools: [tool]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "references missing rule",
                StringComparison.Ordinal));
    }

    [Fact]
    public void ToolFamilyMissingRuleReference_IsRejected()
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        ToolFamilyDefinition family = new(
            new ToolFamilyId("dnd5e2014.tool-family.test"),
            "Test family",
            specialRuleIds:
            [
                new RuleId("dnd5e2014.tool-rule.missing")
            ],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 154)
            ]);

        IReadOnlyList<string> errors = CatalogIntegrityValidator.Validate(
            CreateDefinitionSet(
                sourceDocuments: sources,
                toolFamilies: [family]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "references missing rule",
                StringComparison.Ordinal));
    }

    [Fact]
    public void ToolAndToolFamilyMissingSources_AreRejected()
    {
        ToolDefinition tool = CreateTool(
            sourceDocumentId: "dnd5e2014.source.missing");
        ToolFamilyDefinition family = new(
            new ToolFamilyId("dnd5e2014.tool-family.test"),
            "Test family",
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId("dnd5e2014.source.missing"),
                    page: 154)
            ]);

        IReadOnlyList<string> errors = CatalogIntegrityValidator.Validate(
            CreateDefinitionSet(toolFamilies: [family], tools: [tool]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "missing source document",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MountMissingSourceReference_IsRejected()
    {
        MountDefinition mount = CreateMount(
            sourceDocumentId: "dnd5e2014.source.missing");

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(mounts: [mount]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "missing source document",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MountMissingRuleReference_IsRejected()
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        MountDefinition mount = new(
            new MountId("dnd5e2014.mount.test"),
            "Test mount",
            new Money(100),
            new Distance(40),
            new Weight(100),
            specialRuleIds:
            [
                new RuleId("dnd5e2014.mount-rule.missing")
            ],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 155)
            ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    sourceDocuments: sources,
                    mounts: [mount]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "references missing rule",
                StringComparison.Ordinal));
    }

    [Fact]
    public void VehicleMissingSourceReference_IsRejected()
    {
        VehicleDefinition vehicle = CreateVehicle(
            sourceDocumentId: "dnd5e2014.source.missing");

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(vehicles: [vehicle]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "missing source document",
                StringComparison.Ordinal));
    }

    [Fact]
    public void VehicleMissingRuleReference_IsRejected()
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        VehicleDefinition vehicle = new(
            new VehicleId("dnd5e2014.vehicle.test"),
            "Test vehicle",
            VehicleKind.Land,
            new Money(100),
            listedWeight: new Weight(100),
            listedSpeed: null,
            specialRuleIds:
            [
                new RuleId("dnd5e2014.vehicle-rule.missing")
            ],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 157)
            ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    sourceDocuments: sources,
                    vehicles: [vehicle]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "references missing rule",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MountSupportMissingSourceReference_IsRejected()
    {
        MountSupportDefinition definition = CreateMountSupport(
            sourceDocumentId: "dnd5e2014.source.missing");

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(mountSupport: [definition]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "missing source document",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MountSupportMissingRuleReference_IsRejected()
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        MountSupportDefinition definition = new(
            new MountSupportId("dnd5e2014.mount-support.test"),
            "Test mount support",
            new Money(100),
            listedWeight: new Weight(1),
            specialRuleIds:
            [
                new RuleId("dnd5e2014.mount-support-rule.missing")
            ],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 157)
            ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    sourceDocuments: sources,
                    mountSupport: [definition]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "references missing rule",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MountVehicleRulesMissingRuleReference_IsRejected()
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        MountVehicleRules mountVehicleRules =
            CreateMountVehicleRules(
                drawnVehiclePullingRuleId:
                    new RuleId(
                        "dnd5e2014.mount-vehicle-rule.missing"));

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    sourceDocuments: sources,
                    mountVehicleRules: mountVehicleRules));

        Assert.Contains(
            errors,
            error => error.Contains(
                "Mount and vehicle rules reference missing rule",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MountVehicleRulesMissingSourceReference_IsRejected()
    {
        MountVehicleRules mountVehicleRules =
            CreateMountVehicleRules(
                sourceDocumentId: "dnd5e2014.source.missing");

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    mountVehicleRules: mountVehicleRules));

        Assert.Contains(
            errors,
            error => error.Contains(
                "Mount and vehicle rules references missing source document",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MountVehicleRulesMissingRowboatReference_IsRejected()
    {
        MountVehicleRules mountVehicleRules =
            CreateMountVehicleRules(
                rowboatVehicleId:
                    new VehicleId(
                        "dnd5e2014.vehicle.missing"));

        IReadOnlyList<RuleDefinition> rules =
            mountVehicleRules.ReferencedRuleIds
                .Select(id => new RuleDefinition(
                    id,
                    "Test rule",
                    [
                        new SourceReference(
                            new SourceDocumentId(
                                "dnd5e2014.source.phb-first-printing"),
                            page: 155)
                    ]))
                .ToArray();

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    rules: rules,
                    mountVehicleRules: mountVehicleRules));

        Assert.Contains(
            errors,
            error => error.Contains(
                "missing rowboat vehicle",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MountMissingRequiredSemanticAssociation_IsRejected()
    {
        MountVehicleRules rules = CreateMountVehicleRules();
        MountDefinition mount = CreateMount(
            specialRuleIds: [rules.DrawnVehiclePullingRuleId]);

        IReadOnlyList<string> errors =
            ValidateMountVehicleSemanticIntegrity(
                rules,
                mounts: [mount]);

        Assert.Contains(
            errors,
            error => error.Contains(
                $"Mount '{mount.Id}' is missing required rule association '{rules.BardingRuleId}'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void LandVehicleWrongSemanticAssociations_AreRejected()
    {
        MountVehicleRules rules = CreateMountVehicleRules();
        VehicleDefinition carriage = CreateVehicle(
            id: new VehicleId("dnd5e2014.vehicle.carriage"),
            specialRuleIds:
            [
                rules.VehicleProficiencyRuleId,
                rules.RowedVesselsRuleId
            ]);

        IReadOnlyList<string> errors =
            ValidateMountVehicleSemanticIntegrity(
                rules,
                vehicles: [carriage]);

        Assert.Contains(
            errors,
            error => error.Contains(
                $"Vehicle '{carriage.Id}' is missing required rule association '{rules.DrawnVehiclePullingRuleId}'",
                StringComparison.Ordinal));
        Assert.Contains(
            errors,
            error => error.Contains(
                $"Vehicle '{carriage.Id}' has forbidden rule association '{rules.RowedVesselsRuleId}'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void NonRowedWaterVehicleWrongSemanticAssociations_AreRejected()
    {
        MountVehicleRules rules = CreateMountVehicleRules();
        VehicleDefinition galley = CreateVehicle(
            id: new VehicleId("dnd5e2014.vehicle.galley"),
            kind: VehicleKind.Water,
            specialRuleIds: [rules.RowedVesselsRuleId]);

        IReadOnlyList<string> errors =
            ValidateMountVehicleSemanticIntegrity(
                rules,
                vehicles: [galley]);

        Assert.Contains(
            errors,
            error => error.Contains(
                $"Vehicle '{galley.Id}' is missing required rule association '{rules.VehicleProficiencyRuleId}'",
                StringComparison.Ordinal));
        Assert.Contains(
            errors,
            error => error.Contains(
                $"Vehicle '{galley.Id}' has forbidden rule association '{rules.RowedVesselsRuleId}'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void KeelboatAndRowboatMissingRowedAssociation_AreRejected()
    {
        MountVehicleRules rules = CreateMountVehicleRules();
        VehicleDefinition keelboat = CreateVehicle(
            id: new VehicleId("dnd5e2014.vehicle.keelboat"),
            kind: VehicleKind.Water,
            specialRuleIds: [rules.VehicleProficiencyRuleId]);
        VehicleDefinition rowboat = CreateVehicle(
            id: new VehicleId("dnd5e2014.vehicle.rowboat"),
            kind: VehicleKind.Water,
            specialRuleIds: [rules.VehicleProficiencyRuleId]);

        IReadOnlyList<string> errors =
            ValidateMountVehicleSemanticIntegrity(
                rules,
                vehicles: [keelboat, rowboat]);

        Assert.Contains(
            errors,
            error => error.Contains(
                $"Vehicle '{keelboat.Id}' is missing required rule association '{rules.RowedVesselsRuleId}'",
                StringComparison.Ordinal));
        Assert.Contains(
            errors,
            error => error.Contains(
                $"Vehicle '{rowboat.Id}' is missing required rule association '{rules.RowedVesselsRuleId}'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MilitaryAndExoticSaddleAssociations_AreEnforced()
    {
        MountVehicleRules rules = CreateMountVehicleRules();
        MountSupportDefinition militarySaddle = CreateMountSupport(
            id: new MountSupportId(
                "dnd5e2014.mount-support.saddle-military"),
            specialRuleIds: []);
        MountSupportDefinition exoticSaddle = CreateMountSupport(
            id: new MountSupportId(
                "dnd5e2014.mount-support.saddle-exotic"),
            specialRuleIds: [rules.MilitarySaddleRuleId]);

        IReadOnlyList<string> errors =
            ValidateMountVehicleSemanticIntegrity(
                rules,
                mountSupport: [militarySaddle, exoticSaddle]);

        Assert.Contains(
            errors,
            error => error.Contains(
                $"Mount support '{militarySaddle.Id}' is missing " +
                "required rule association " +
                $"'{rules.MilitarySaddleRuleId}'",
                StringComparison.Ordinal));
        Assert.Contains(
            errors,
            error => error.Contains(
                $"Mount support '{exoticSaddle.Id}' is missing required rule association '{rules.ExoticSaddleRuleId}'",
                StringComparison.Ordinal));
        Assert.Contains(
            errors,
            error => error.Contains(
                $"Mount support '{exoticSaddle.Id}' has forbidden rule association '{rules.MilitarySaddleRuleId}'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void ExistingButWrongRowboatIdentity_IsRejected()
    {
        VehicleId galleyId = new("dnd5e2014.vehicle.galley");
        MountVehicleRules rules = CreateMountVehicleRules(
            rowboatVehicleId: galleyId);
        VehicleDefinition galley = CreateVehicle(
            id: galleyId,
            kind: VehicleKind.Water,
            specialRuleIds: [rules.VehicleProficiencyRuleId]);

        IReadOnlyList<string> errors =
            ValidateMountVehicleSemanticIntegrity(
                rules,
                vehicles: [galley]);

        Assert.Contains(
            errors,
            error => error.Contains(
                "rowboat vehicle ID must be 'dnd5e2014.vehicle.rowboat'",
                StringComparison.Ordinal));
        Assert.DoesNotContain(
            errors,
            error => error.Contains(
                "missing rowboat vehicle",
                StringComparison.Ordinal));
    }

    [Fact]
    public void TradeGoodMissingRuleReference_IsRejected()
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        TradeGoodDefinition tradeGood = CreateTradeGood(
            specialRuleIds:
            [
                new RuleId(
                    "dnd5e2014.trade-good-rule.full-value-and-currency")
            ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    sourceDocuments: sources,
                    tradeGoods: [tradeGood]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "references missing rule",
                StringComparison.Ordinal));
    }

    [Fact]
    public void TradeGoodMissingRequiredAssociation_IsRejected()
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        RuleDefinition rule = new(
            new RuleId(
                "dnd5e2014.trade-good-rule.full-value-and-currency"),
            "Trade-good rule",
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 144)
            ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    sourceDocuments: sources,
                    rules: [rule],
                    tradeGoods:
                    [
                        CreateTradeGood(specialRuleIds: [])
                    ]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "missing required rule association",
                StringComparison.Ordinal));
    }

    [Fact]
    public void TradeGoodExistingButWrongAssociation_IsRejected()
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        RuleDefinition requiredRule = new(
            new RuleId(
                "dnd5e2014.trade-good-rule.full-value-and-currency"),
            "Trade-good rule",
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 144)
            ]);

        RuleDefinition wrongRule = new(
            new RuleId("dnd5e2014.rule.existing-but-wrong"),
            "Existing but wrong",
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 157)
            ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    sourceDocuments: sources,
                    rules: [requiredRule, wrongRule],
                    tradeGoods:
                    [
                        CreateTradeGood(
                            specialRuleIds: [wrongRule.Id])
                    ]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "missing required rule association",
                StringComparison.Ordinal));
        Assert.DoesNotContain(
            errors,
            error => error.Contains(
                "references missing rule",
                StringComparison.Ordinal));
    }

    [Fact]
    public void ArmorUsageMissingRuleReference_IsRejected()
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        ArmorUsageRules armorUsage = CreateArmorUsageRules();

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    sourceDocuments: sources,
                    armorUsage: armorUsage));

        Assert.Contains(
            errors,
            error => error.Contains(
                "Armor usage rules reference missing rule",
                StringComparison.Ordinal));
    }

    [Fact]
    public void ArmorUsageMissingSourceReference_IsRejected()
    {
        ArmorUsageRules armorUsage = CreateArmorUsageRules(
            sourceDocumentId: "dnd5e2014.source.missing");

        IReadOnlyList<RuleDefinition> rules =
            armorUsage.ReferencedRuleIds
                .Select(id => new RuleDefinition(
                    id,
                    "Test rule",
                    [
                        new SourceReference(
                            new SourceDocumentId(
                                "dnd5e2014.source.phb-first-printing"),
                            page: 144)
                    ]))
                .ToArray();

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    rules: rules,
                    armorUsage: armorUsage));

        Assert.Contains(
            errors,
            error => error.Contains(
                "Armor usage rules references missing source document",
                StringComparison.Ordinal));
    }

    private static IReadOnlyList<string> ValidateMountVehicleSemanticIntegrity(
        MountVehicleRules rules,
        IReadOnlyList<MountDefinition>? mounts = null,
        IReadOnlyList<VehicleDefinition>? vehicles = null,
        IReadOnlyList<MountSupportDefinition>? mountSupport = null)
    {
        IReadOnlyList<SourceDocument> sources =
        [
            new SourceDocument(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                "Player's Handbook")
        ];

        IReadOnlyList<RuleDefinition> ruleDefinitions =
            rules.ReferencedRuleIds
                .Select(id => new RuleDefinition(
                    id,
                    "Test rule",
                    [
                        new SourceReference(
                            new SourceDocumentId(
                                "dnd5e2014.source.phb-first-printing"),
                            page: 155)
                    ]))
                .ToArray();

        var vehicleDefinitions = new List<VehicleDefinition>(vehicles ?? []);
        var canonicalRowboatId =
            new VehicleId("dnd5e2014.vehicle.rowboat");

        if (!vehicleDefinitions.Any(
                definition => definition.Id == canonicalRowboatId))
        {
            vehicleDefinitions.Add(
                CreateVehicle(
                    id: canonicalRowboatId,
                    kind: VehicleKind.Water,
                    specialRuleIds:
                    [
                        rules.VehicleProficiencyRuleId,
                        rules.RowedVesselsRuleId
                    ]));
        }

        return CatalogIntegrityValidator.Validate(
            CreateDefinitionSet(
                sourceDocuments: sources,
                rules: ruleDefinitions,
                mounts: mounts,
                vehicles: vehicleDefinitions,
                mountSupport: mountSupport,
                mountVehicleRules: rules));
    }

    private static RulesetDefinitionSet CreateDefinitionSet(
        IReadOnlyList<WeaponDefinition>? weapons = null,
        IReadOnlyList<SourceDocument>? sourceDocuments = null,
        IReadOnlyList<AmmunitionDefinition>? ammunition = null,
        IReadOnlyList<RuleDefinition>? rules = null,
        IReadOnlyList<ArmorDefinition>? armor = null,
        IReadOnlyList<ShieldDefinition>? shields = null,
        IReadOnlyList<AdventuringGearDefinition>? adventuringGear = null,
        IReadOnlyList<ContainerCapacityDefinition>? containerCapacities = null,
        IReadOnlyList<ToolFamilyDefinition>? toolFamilies = null,
        IReadOnlyList<ToolDefinition>? tools = null,
        IReadOnlyList<MountDefinition>? mounts = null,
        IReadOnlyList<VehicleDefinition>? vehicles = null,
        IReadOnlyList<MountSupportDefinition>? mountSupport = null,
        IReadOnlyList<TradeGoodDefinition>? tradeGoods = null,
        MountVehicleRules? mountVehicleRules = null,
        ArmorUsageRules? armorUsage = null,
        IReadOnlyList<DamageTypeDefinition>? damageTypes = null,
        IReadOnlyList<AbilityDefinition>? abilities = null,
        IReadOnlyList<CreatureSizeDefinition>? sizes = null,
        IReadOnlyList<LanguageDefinition>? languages = null,
        IReadOnlyList<SkillDefinition>? skills = null,
        IReadOnlyList<RaceDefinition>? races = null,
        IReadOnlyList<SubraceDefinition>? subraces = null,
        IReadOnlyList<ClassDefinition>? classes = null,
        IReadOnlyList<SubclassDefinition>? subclasses = null,
        IReadOnlyList<SpellSlotProgressionDefinition>?
            spellSlotProgressions = null,
        IReadOnlyList<ExtraAttackProgressionDefinition>?
            extraAttackProgressions = null)
    {
        var equipment = new EquipmentDefinitionSet(
            weapons: weapons ?? [],
            ammunition: ammunition ?? [],
            armor: armor ?? [],
            shields: shields ?? [],
            adventuringGear: adventuringGear ?? [],
            containerCapacities: containerCapacities ?? [],
            toolFamilies: toolFamilies ?? [],
            tools: tools ?? [],
            mounts: mounts ?? [],
            vehicles: vehicles ?? [],
            mountSupport: mountSupport ?? [],
            tradeGoods: tradeGoods ?? [],
            mountVehicleRules: mountVehicleRules,
            armorUsage: armorUsage);

        var expenses = new ExpenseDefinitionSet(
            lifestyles: [],
            foodAndDrink: [],
            hospitalityCosts: [],
            mundaneServices: []);

        return new RulesetDefinitionSet(
            sourceDocuments: sourceDocuments ?? [],
            rules: rules ?? [],
            equipment: equipment,
            expenses: expenses,
            creatureVocabulary:
                new CreatureVocabularyDefinitionSet(
                    abilities: abilities ?? [],
                    skills: skills ?? [],
                    languages: languages ?? [],
                    sizes: sizes ?? [],
                    conditions: [],
                    damageTypes: damageTypes ?? [],
                    senses: [],
                    alignments: []),
            races: new RaceDefinitionSet(
                races: races ?? [],
                subraces: subraces ?? []),
            classes: new ClassDefinitionSet(
                classes: classes ?? [],
                subclasses: subclasses ?? []),
            fightingStyles: [],
            metamagicOptions: [],
            battleMasterManeuvers: [],
            eldritchInvocations: [],
            elementalDisciplines: [],
            channelDivinityOptions: [],
            spellSlotProgressions: spellSlotProgressions ?? [],
            extraAttackProgressions: extraAttackProgressions ?? [],
            backgrounds: []);
    }

    private static ToolDefinition CreateTool(
        ToolFamilyId? familyId = null,
        string sourceDocumentId = "dnd5e2014.source.phb-first-printing")
    {
        return new ToolDefinition(
            new ToolId("dnd5e2014.tool.test"),
            "Test tool",
            new Money(100),
            weight: null,
            familyId,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(sourceDocumentId),
                    page: 154)
            ]);
    }

    private static MountDefinition CreateMount(
        string sourceDocumentId =
            "dnd5e2014.source.phb-first-printing",
        MountId? id = null,
        IEnumerable<RuleId>? specialRuleIds = null)
    {
        return new MountDefinition(
            id ?? new MountId("dnd5e2014.mount.test"),
            "Test mount",
            new Money(100),
            new Distance(40),
            new Weight(100),
            specialRuleIds ?? [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(sourceDocumentId),
                    page: 155)
            ]);
    }

    private static VehicleDefinition CreateVehicle(
        string sourceDocumentId =
            "dnd5e2014.source.phb-first-printing",
        VehicleId? id = null,
        VehicleKind kind = VehicleKind.Land,
        IEnumerable<RuleId>? specialRuleIds = null)
    {
        return new VehicleDefinition(
            id ?? new VehicleId("dnd5e2014.vehicle.test"),
            "Test vehicle",
            kind,
            new Money(100),
            listedWeight:
                kind == VehicleKind.Land ? new Weight(100) : null,
            listedSpeed:
                kind == VehicleKind.Water ? new VehicleSpeed(1) : null,
            specialRuleIds ?? [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(sourceDocumentId),
                    page: 157)
            ]);
    }

    private static MountSupportDefinition CreateMountSupport(
        string sourceDocumentId =
            "dnd5e2014.source.phb-first-printing",
        MountSupportId? id = null,
        IEnumerable<RuleId>? specialRuleIds = null)
    {
        return new MountSupportDefinition(
            id ?? new MountSupportId("dnd5e2014.mount-support.test"),
            "Test mount support",
            new Money(100),
            listedWeight: new Weight(1),
            specialRuleIds ?? [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(sourceDocumentId),
                    page: 157)
            ]);
    }

    private static TradeGoodDefinition CreateTradeGood(
        string sourceDocumentId =
            "dnd5e2014.source.phb-first-printing",
        IEnumerable<RuleId>? specialRuleIds = null)
    {
        return new TradeGoodDefinition(
            new TradeGoodId("dnd5e2014.trade-good.test"),
            "Test trade good",
            new Money(100),
            new TradeGoodPricingBasis(1, TradeGoodUnit.Pound),
            specialRuleIds ?? [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(sourceDocumentId),
                    page: 157)
            ]);
    }

    private static MountVehicleRules CreateMountVehicleRules(
        RuleId? drawnVehiclePullingRuleId = null,
        VehicleId? rowboatVehicleId = null,
        string sourceDocumentId =
            "dnd5e2014.source.phb-first-printing")
    {
        return new MountVehicleRules(
            drawnVehiclePullingRuleId ??
                new RuleId(
                    "dnd5e2014.mount-vehicle-rule.drawn-vehicle-pulling-capacity"),
            5,
            drawnVehicleCapacityIncludesVehicleWeight: true,
            multipleAnimalsCombineCarryingCapacity: true,
            new RuleId(
                "dnd5e2014.mount-vehicle-rule.other-mount-availability"),
            otherMountsAreRare: true,
            otherMountsNormallyAvailableForPurchase: false,
            new RuleId(
                "dnd5e2014.mount-vehicle-rule.barding"),
            bardingAvailableForAnyArmorType: true,
            bardingCostMultiplier: 4,
            bardingWeightMultiplier: 2,
            new RuleId(
                "dnd5e2014.mount-vehicle-rule.military-saddle"),
            militarySaddleGrantsAdvantageOnChecksToRemainMounted: true,
            new RuleId(
                "dnd5e2014.mount-vehicle-rule.exotic-saddle"),
            exoticSaddleRequiredForAquaticOrFlyingMounts: true,
            new RuleId(
                "dnd5e2014.mount-vehicle-rule.vehicle-proficiency"),
            [VehicleKind.Land, VehicleKind.Water],
            vehicleProficiencyAddsProficiencyBonusToDifficultControlChecks:
                true,
            new RuleId(
                "dnd5e2014.mount-vehicle-rule.rowed-vessels"),
            new VehicleSpeed(3),
            downstreamCurrentAddsToVehicleSpeed: true,
            rowedVesselsCanBeRowedAgainstSignificantCurrent: false,
            rowedVesselsCanBePulledUpstreamByDraftAnimals: true,
            rowboatVehicleId ??
                new VehicleId("dnd5e2014.vehicle.rowboat"),
            new Weight(100),
            [
                new SourceReference(
                    new SourceDocumentId(sourceDocumentId),
                    page: 155)
            ]);
    }

    private static ContainerCapacityDefinition CreateContainerCapacityDefinition(
        string gearId,
        string sourceDocumentId)
    {
        return new ContainerCapacityDefinition(
            new AdventuringGearId(gearId),
            new ContainerVolume(1m, ContainerVolumeUnit.CubicFoot),
            liquidVolume: null,
            new Weight(30m),
            allowsExteriorItemAttachment: false,
            [
                new SourceReference(
                    new SourceDocumentId(sourceDocumentId),
                    page: 153,
                    section: "Container Capacity")
            ]);
    }

    private static ArmorUsageRules CreateArmorUsageRules(
        string sourceDocumentId = "dnd5e2014.source.phb-first-printing")
    {
        var minute = new EquipmentChangeDuration(
            1,
            EquipmentChangeTimeUnit.Minute);
        var action = new EquipmentChangeDuration(
            1,
            EquipmentChangeTimeUnit.Action);

        return new ArmorUsageRules(
            new RuleId("dnd5e2014.armor-rule.proficiency"),
            new RuleId("dnd5e2014.armor-rule.strength-speed"),
            new RuleId("dnd5e2014.armor-rule.stealth"),
            new RuleId("dnd5e2014.armor-rule.shield"),
            new RuleId("dnd5e2014.armor-rule.don-doff"),
            new ArmorProficiencyConsequences(true, true, true, true),
            new Distance(10),
            shieldHandsRequired: 1,
            maximumBenefitingShields: 1,
            requiresFullDonDurationForArmorClassBenefit: true,
            doffingWithHelpDivisor: 2,
            new EquipmentChangeTiming(minute, minute),
            new EquipmentChangeTiming(minute, minute),
            new EquipmentChangeTiming(minute, minute),
            new EquipmentChangeTiming(action, action),
            [
                new SourceReference(
                    new SourceDocumentId(sourceDocumentId),
                    page: 144)
            ]);
    }

    private static WeaponDefinition CreateWeapon(
        IEnumerable<RuleId> specialRuleIds,
        AmmunitionTypeId? ammunitionTypeId = null)
    {
        return new WeaponDefinition(
            new WeaponId("dnd5e2014.weapon.test"),
            "Test",
            WeaponProficiencyCategory.Simple,
            WeaponUsageCategory.Melee,
            cost: null,
            weight: null,
            damage: null,
            properties: [],
            range: null,
            versatileDamage: null,
            ammunitionTypeId,
            specialRuleIds,
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 149)
            ]);
    }

    private static IReadOnlyList<RuleDefinition> LoadAllRules(string root)
    {
        string rulesDirectory = Path.Combine(root, "Data", "dnd5e2014", "rules");

        return RuleDefinitionLoader.LoadAndMergeFromFiles(
            [
                Path.Combine(rulesDirectory, "weapon-rule.json"),
                Path.Combine(rulesDirectory, "armor-rule.json"),
                Path.Combine(rulesDirectory, "adventuring-gear-rule.json"),
                Path.Combine(rulesDirectory, "tool-rule.json"),
                Path.Combine(rulesDirectory, "mount-vehicle-rule.json"),
                Path.Combine(rulesDirectory, "trade-good-rule.json"),
                Path.Combine(rulesDirectory, "expense-rule.json"),
                Path.Combine(rulesDirectory, "lifestyle-rule.json"),
                Path.Combine(rulesDirectory, "race-rule.json"),
                Path.Combine(rulesDirectory, "class-rule.json")
            ]);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

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
