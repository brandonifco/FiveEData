using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Shields;

namespace FiveEData.Tests;

public sealed class ArmorAndShieldCatalogTests
{
    [Fact]
    public void Ruleset_ExposesCompleteArmorAndShieldCatalogs()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.Equal(12, ruleset.Armor.Count);
        Assert.Equal(1, ruleset.Shields.Count);

        ArmorDefinition plate =
            ruleset.Armor.Get(
                new ArmorId("dnd5e2014.armor.plate"));

        Assert.Equal("Plate", plate.Name);
        Assert.Equal(18, plate.ArmorClass.BaseArmorClass);

        ShieldDefinition shield =
            ruleset.Shields.Get(
                new ShieldId("dnd5e2014.armor.shield"));

        Assert.Equal(2, shield.ArmorClassBonus);
    }

    [Fact]
    public void ArmorCatalog_GetAndTryGet_HaveExplicitMissingSemantics()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;
        var existing = new ArmorId("dnd5e2014.armor.leather");

        Assert.True(
            ruleset.Armor.TryGet(
                existing,
                out ArmorDefinition? leather));
        Assert.NotNull(leather);
        Assert.Equal("Leather", leather.Name);

        var missing = new ArmorId("dnd5e2014.armor.does-not-exist");

        Assert.False(
            ruleset.Armor.TryGet(
                missing,
                out ArmorDefinition? missingArmor));
        Assert.Null(missingArmor);
        Assert.Throws<KeyNotFoundException>(
            () => ruleset.Armor.Get(missing));
    }

    [Fact]
    public void ShieldCatalog_GetAndTryGet_HaveExplicitMissingSemantics()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;
        var existing = new ShieldId("dnd5e2014.armor.shield");

        Assert.True(
            ruleset.Shields.TryGet(
                existing,
                out ShieldDefinition? shield));
        Assert.NotNull(shield);

        var missing = new ShieldId("dnd5e2014.armor.missing-shield");

        Assert.False(
            ruleset.Shields.TryGet(
                missing,
                out ShieldDefinition? missingShield));
        Assert.Null(missingShield);
        Assert.Throws<KeyNotFoundException>(
            () => ruleset.Shields.Get(missing));
    }

    [Fact]
    public void ArmorCatalog_EnumerationIsDeterministicByStableId()
    {
        string[] actual = Dnd5e2014Ruleset.Instance.Armor.All
            .Select(armor => armor.Id.Value)
            .ToArray();

        string[] expected = actual
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Catalogs_DefensivelySnapshotInputs()
    {
        var armorSource = new List<ArmorDefinition>
        {
            CreateArmor("dnd5e2014.armor.one", "One")
        };

        var shieldSource = new List<ShieldDefinition>
        {
            CreateShield("dnd5e2014.armor.shield-one", "One")
        };

        var armorCatalog = new ArmorCatalog(armorSource);
        var shieldCatalog = new ShieldCatalog(shieldSource);

        armorSource.Add(CreateArmor("dnd5e2014.armor.two", "Two"));
        shieldSource.Add(CreateShield("dnd5e2014.armor.shield-two", "Two"));

        Assert.Single(armorCatalog.All);
        Assert.Single(shieldCatalog.All);
    }

    [Fact]
    public void ArmorCatalog_RejectsInvalidDefinition()
    {
        ArmorDefinition invalid = new(
            new ArmorId("dnd5e2014.armor.invalid"),
            "Invalid",
            ArmorCategory.Medium,
            new Money(1000),
            new Weight(10m),
            new ArmorClassFormula(14, includesDexterityModifier: true),
            minimumStrengthForFullSpeed: null,
            imposesStealthDisadvantage: false,
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 145)
            ]);

        Assert.Throws<InvalidOperationException>(
            () => new ArmorCatalog([invalid]));
    }

    [Fact]
    public void ShieldCatalog_RejectsInvalidDefinition()
    {
        ShieldDefinition invalid = new(
            new ShieldId("dnd5e2014.armor.invalid-shield"),
            "Invalid",
            new Money(1000),
            new Weight(6m),
            armorClassBonus: 0,
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 145)
            ]);

        Assert.Throws<InvalidOperationException>(
            () => new ShieldCatalog([invalid]));
    }

    [Fact]
    public void Catalogs_RejectDuplicateIds()
    {
        ArmorDefinition firstArmor =
            CreateArmor("dnd5e2014.armor.duplicate", "First");
        ArmorDefinition secondArmor =
            CreateArmor("dnd5e2014.armor.duplicate", "Second");

        ShieldDefinition firstShield =
            CreateShield("dnd5e2014.armor.duplicate-shield", "First");
        ShieldDefinition secondShield =
            CreateShield("dnd5e2014.armor.duplicate-shield", "Second");

        Assert.Throws<ArgumentException>(
            () => new ArmorCatalog([firstArmor, secondArmor]));
        Assert.Throws<ArgumentException>(
            () => new ShieldCatalog([firstShield, secondShield]));
    }

    private static ArmorDefinition CreateArmor(string id, string name)
    {
        return new ArmorDefinition(
            new ArmorId(id),
            name,
            ArmorCategory.Light,
            new Money(1000),
            new Weight(10m),
            new ArmorClassFormula(11, includesDexterityModifier: true),
            minimumStrengthForFullSpeed: null,
            imposesStealthDisadvantage: false,
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 145)
            ]);
    }

    private static ShieldDefinition CreateShield(string id, string name)
    {
        return new ShieldDefinition(
            new ShieldId(id),
            name,
            new Money(1000),
            new Weight(6m),
            armorClassBonus: 2,
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 145)
            ]);
    }
}
