using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.FightingStyles;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class FightingStyleFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new FightingStyleId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.fighting-style.test";

        var id = new FightingStyleId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void RollBonus_RejectsNonPositiveAmount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new FightingStyleRollBonus(
                FightingStyleRollTarget.AttackRoll,
                0,
                FightingStyleWeaponRequirement.RangedWeapon));
    }

    [Fact]
    public void DamageDieReroll_RejectsNonPositiveThreshold()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new FightingStyleDamageDieReroll(
                0,
                FightingStyleWeaponRequirement
                    .MeleeWeaponWithTwoHandedOrVersatileProperty));
    }

    [Fact]
    public void Reaction_RejectsNonPositiveRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new FightingStyleReaction(
                new Distance(0),
                requiresShield: true));
    }

    [Fact]
    public void Definition_DefensivelySnapshotsSourcesAndClasses()
    {
        var classIds = new List<ClassId>
        {
            new("dnd5e2014.class.fighter")
        };
        var sources = new List<SourceReference> { CreateSource() };

        FightingStyleDefinition definition = CreateOffHand(
            "dnd5e2014.fighting-style.test",
            "Test",
            classIds,
            sources);

        classIds.Clear();
        sources.Clear();

        Assert.Single(definition.AvailableToClassIds);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        FightingStyleDefinition definition = CreateOffHand(
            null,
            "Test",
            [new ClassId("dnd5e2014.class.fighter")],
            [CreateSource()]);

        Assert.Contains(
            FightingStyleDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsNoAvailableClasses()
    {
        FightingStyleDefinition definition = CreateOffHand(
            "dnd5e2014.fighting-style.test",
            "Test",
            [],
            [CreateSource()]);

        Assert.Contains(
            FightingStyleDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "at least one class",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDuplicateAvailableClass()
    {
        var fighter = new ClassId("dnd5e2014.class.fighter");

        FightingStyleDefinition definition = CreateOffHand(
            "dnd5e2014.fighting-style.test",
            "Test",
            [fighter, fighter],
            [CreateSource()]);

        Assert.Contains(
            FightingStyleDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "duplicated",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsZeroMechanicalEffects()
    {
        var definition = new FightingStyleDefinition(
            new FightingStyleId("dnd5e2014.fighting-style.test"),
            "Test",
            [new ClassId("dnd5e2014.class.fighter")],
            rollBonus: null,
            armorClassBonus: null,
            damageDieReroll: null,
            reaction: null,
            grantsOffHandAbilityModifierDamage: false,
            [CreateSource()]);

        Assert.Contains(
            FightingStyleDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "exactly one mechanical",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMoreThanOneMechanicalEffect()
    {
        var definition = new FightingStyleDefinition(
            new FightingStyleId("dnd5e2014.fighting-style.test"),
            "Test",
            [new ClassId("dnd5e2014.class.fighter")],
            rollBonus: new FightingStyleRollBonus(
                FightingStyleRollTarget.AttackRoll,
                2,
                FightingStyleWeaponRequirement.RangedWeapon),
            armorClassBonus: 1,
            damageDieReroll: null,
            reaction: null,
            grantsOffHandAbilityModifierDamage: false,
            [CreateSource()]);

        Assert.Contains(
            FightingStyleDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "exactly one mechanical",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        FightingStyleDefinition definition = CreateOffHand(
            "dnd5e2014.fighting-style.test",
            "Test",
            [new ClassId("dnd5e2014.class.fighter")],
            []);

        Assert.Contains(
            FightingStyleDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new FightingStyleCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new FightingStyleCatalog(
            [
                CreateOffHand(
                    "dnd5e2014.fighting-style.z",
                    "Z",
                    [new ClassId("dnd5e2014.class.fighter")],
                    [CreateSource()]),
                CreateOffHand(
                    "dnd5e2014.fighting-style.a",
                    "A",
                    [new ClassId("dnd5e2014.class.fighter")],
                    [CreateSource()])
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.fighting-style.a",
                "dnd5e2014.fighting-style.z"
            ],
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        var aId = new FightingStyleId("dnd5e2014.fighting-style.a");

        FightingStyleDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(aId, out FightingStyleDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new FightingStyleId("dnd5e2014.fighting-style.missing");

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out FightingStyleDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new FightingStyleCatalog(
                [
                    CreateOffHand(
                        "dnd5e2014.fighting-style.duplicate",
                        "One",
                        [new ClassId("dnd5e2014.class.fighter")],
                        [CreateSource()]),
                    CreateOffHand(
                        "dnd5e2014.fighting-style.duplicate",
                        "Two",
                        [new ClassId("dnd5e2014.class.fighter")],
                        [CreateSource()])
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        FightingStyleDefinition definition = CreateOffHand(
            "dnd5e2014.fighting-style.test",
            "Test",
            [],
            [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => new FightingStyleCatalog([definition]));
    }

    private static FightingStyleDefinition CreateOffHand(
        string? id,
        string name,
        IEnumerable<ClassId> availableToClassIds,
        IEnumerable<SourceReference> sources)
    {
        return new FightingStyleDefinition(
            id is null ? default : new FightingStyleId(id),
            name,
            availableToClassIds,
            rollBonus: null,
            armorClassBonus: null,
            damageDieReroll: null,
            reaction: null,
            grantsOffHandAbilityModifierDamage: true,
            sources);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 72);
    }
}
