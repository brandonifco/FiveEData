using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.TransmutersStoneOptions;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Tests;

public sealed class TransmutersStoneOptionFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new TransmutersStoneOptionId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.transmuters-stone-option.test";

        var id = new TransmutersStoneOptionId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsSourcesAndDamageTypes()
    {
        var sources = new List<SourceReference> { CreateSource() };
        var damageTypeIds = new List<DamageTypeId>
        {
            new("dnd5e2014.damage-type.acid"),
            new("dnd5e2014.damage-type.cold")
        };

        TransmutersStoneOptionDefinition definition = Create(
            "dnd5e2014.transmuters-stone-option.test",
            "Test",
            sources,
            choosableResistedDamageTypeIds: damageTypeIds);

        sources.Clear();
        damageTypeIds.Clear();

        Assert.Single(definition.Sources);
        Assert.Equal(2, definition.ChoosableResistedDamageTypeIds.Count);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        TransmutersStoneOptionDefinition definition = Create(
            null,
            "Test",
            [CreateSource()]);

        Assert.Contains(
            TransmutersStoneOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains("ID", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositiveSpeedBonus()
    {
        TransmutersStoneOptionDefinition definition = Create(
            "dnd5e2014.transmuters-stone-option.test",
            "Test",
            [CreateSource()],
            speedBonusFeet: 0);

        Assert.Contains(
            TransmutersStoneOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "speed bonus",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsUnencumberedGateWithoutASpeedBonus()
    {
        TransmutersStoneOptionDefinition definition = Create(
            "dnd5e2014.transmuters-stone-option.test",
            "Test",
            [CreateSource()],
            requiresUnencumbered: true);

        Assert.Contains(
            TransmutersStoneOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "unencumbered",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsAChoiceOfExactlyOneDamageType()
    {
        TransmutersStoneOptionDefinition definition = Create(
            "dnd5e2014.transmuters-stone-option.test",
            "Test",
            [CreateSource()],
            choosableResistedDamageTypeIds:
                [new DamageTypeId("dnd5e2014.damage-type.acid")]);

        Assert.Contains(
            TransmutersStoneOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "at least two",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDuplicateChoosableDamageTypes()
    {
        TransmutersStoneOptionDefinition definition = Create(
            "dnd5e2014.transmuters-stone-option.test",
            "Test",
            [CreateSource()],
            choosableResistedDamageTypeIds:
            [
                new DamageTypeId("dnd5e2014.damage-type.acid"),
                new DamageTypeId("dnd5e2014.damage-type.acid")
            ]);

        Assert.Contains(
            TransmutersStoneOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "duplicate",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        TransmutersStoneOptionDefinition definition = Create(
            "dnd5e2014.transmuters-stone-option.test",
            "Test",
            []);

        Assert.Contains(
            TransmutersStoneOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains("source", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new TransmutersStoneOptionCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new TransmutersStoneOptionCatalog(
            [
                Create(
                    "dnd5e2014.transmuters-stone-option.z",
                    "Z",
                    [CreateSource()]),
                Create(
                    "dnd5e2014.transmuters-stone-option.a",
                    "A",
                    [CreateSource()])
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.transmuters-stone-option.a",
                "dnd5e2014.transmuters-stone-option.z"
            ],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId = new TransmutersStoneOptionId(
            "dnd5e2014.transmuters-stone-option.a");

        TransmutersStoneOptionDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(
                aId,
                out TransmutersStoneOptionDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId = new TransmutersStoneOptionId(
            "dnd5e2014.transmuters-stone-option.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out TransmutersStoneOptionDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new TransmutersStoneOptionCatalog(
                [
                    Create(
                        "dnd5e2014.transmuters-stone-option.duplicate",
                        "One",
                        [CreateSource()]),
                    Create(
                        "dnd5e2014.transmuters-stone-option.duplicate",
                        "Two",
                        [CreateSource()])
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        TransmutersStoneOptionDefinition definition = Create(
            "dnd5e2014.transmuters-stone-option.test",
            "Test",
            []);

        Assert.Throws<InvalidOperationException>(
            () => new TransmutersStoneOptionCatalog([definition]));
    }

    private static TransmutersStoneOptionDefinition Create(
        string? id,
        string name,
        IEnumerable<SourceReference> sources,
        int? darkvisionRangeFeet = null,
        int? speedBonusFeet = null,
        bool requiresUnencumbered = false,
        AbilityId? savingThrowProficiencyAbilityId = null,
        IEnumerable<DamageTypeId>? choosableResistedDamageTypeIds = null)
    {
        return new TransmutersStoneOptionDefinition(
            id: id is null ? default : new TransmutersStoneOptionId(id),
            name: name,
            darkvisionRangeFeet: darkvisionRangeFeet,
            speedBonusFeet: speedBonusFeet,
            requiresUnencumbered: requiresUnencumbered,
            savingThrowProficiencyAbilityId: savingThrowProficiencyAbilityId,
            choosableResistedDamageTypeIds:
                choosableResistedDamageTypeIds ?? [],
            sources: sources);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 119);
    }
}
