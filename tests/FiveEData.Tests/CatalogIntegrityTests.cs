using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.Ammunition.Serialization;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Armor.Serialization;
using FiveEData.Rules.Equipment.Shields;
using FiveEData.Rules.Equipment.Shields.Serialization;
using FiveEData.Rules.Equipment.Weapons;
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

        IReadOnlyList<RuleDefinition> rules =
            RuleDefinitionLoader.LoadFromJson(
                File.ReadAllText(
                    Path.Combine(root, "Data", "dnd5e2014", "rules.json")));

        IReadOnlyList<ArmorDefinition> armor =
            ArmorDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "armor.json"));

        IReadOnlyList<ShieldDefinition> shields =
            ShieldDefinitionLoader.LoadFromFile(
                Path.Combine(root, "Data", "dnd5e2014", "shields.json"));

        Assert.Empty(
            CatalogIntegrityValidator.Validate(
                weapons,
                sources,
                ammunition,
                rules,
                armor,
                shields));
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
                [weapon],
                sources,
                [],
                [],
                [],
                []);

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
                [weapon],
                [],
                [],
                [],
                [],
                []);

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
                [weapon],
                sources,
                [],
                [],
                [],
                []);

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
                [],
                [],
                [],
                [],
                [armor],
                []);

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
                [],
                [],
                [],
                [],
                [],
                [shield]);

        Assert.Contains(
            errors,
            error => error.Contains("missing source document", StringComparison.Ordinal));
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
