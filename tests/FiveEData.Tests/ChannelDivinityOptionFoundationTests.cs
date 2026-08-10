using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.ChannelDivinityOptions;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Spells;

namespace FiveEData.Tests;

public sealed class ChannelDivinityOptionFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new ChannelDivinityOptionId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.channel-divinity-option.test";

        var id = new ChannelDivinityOptionId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference> { CreateSource() };

        ChannelDivinityOptionDefinition definition = Create(
            "dnd5e2014.channel-divinity-option.test",
            "Test",
            rangeFeet: null,
            savingThrowAbilityId: null,
            durationMinutes: null,
            rollBonus: null,
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Definition_AllowsAllFactsNull()
    {
        ChannelDivinityOptionDefinition definition = Create(
            "dnd5e2014.channel-divinity-option.test",
            "Test",
            rangeFeet: null,
            savingThrowAbilityId: null,
            durationMinutes: null,
            rollBonus: null,
            [CreateSource()]);

        Assert.Null(definition.RangeFeet);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Null(definition.DurationMinutes);
        Assert.Null(definition.RollBonus);

        Assert.Empty(
            ChannelDivinityOptionDefinitionValidator.Validate(definition));
    }

    [Fact]
    public void Definition_ExposesAllFactsTogetherWhenPresent()
    {
        ChannelDivinityOptionDefinition definition = Create(
            "dnd5e2014.channel-divinity-option.test",
            "Test",
            rangeFeet: 60,
            savingThrowAbilityId: new AbilityId("dnd5e2014.ability.wisdom"),
            durationMinutes: 1,
            rollBonus: 10,
            [CreateSource()]);

        Assert.Equal(60, definition.RangeFeet);
        Assert.Equal(
            "dnd5e2014.ability.wisdom",
            definition.SavingThrowAbilityId?.Value);
        Assert.Equal(1, definition.DurationMinutes);
        Assert.Equal(10, definition.RollBonus);
    }

    [Fact]
    public void Definition_ExposesConditionAndSpellFactsWhenPresent()
    {
        ChannelDivinityOptionDefinition definition = Create(
            "dnd5e2014.channel-divinity-option.test",
            "Test",
            rangeFeet: null,
            savingThrowAbilityId: null,
            durationMinutes: null,
            rollBonus: null,
            [CreateSource()],
            imposedConditionId:
                new ConditionId("dnd5e2014.condition.invisible"),
            conditionDurationTrigger:
                NextTurnDurationTrigger.EndOfYourNextTurn,
            maximizesDamageRoll: true,
            grantedSpellId: new SpellId("dnd5e2014.spell.suggestion"),
            automaticallyFailsGrantedSpellSave: true);

        Assert.Equal(
            "dnd5e2014.condition.invisible",
            definition.ImposedConditionId?.Value);
        Assert.Equal(
            NextTurnDurationTrigger.EndOfYourNextTurn,
            definition.ConditionDurationTrigger);
        Assert.True(definition.MaximizesDamageRoll);
        Assert.Equal(
            "dnd5e2014.spell.suggestion",
            definition.GrantedSpellId?.Value);
        Assert.True(definition.AutomaticallyFailsGrantedSpellSave);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        ChannelDivinityOptionDefinition definition = Create(
            null,
            "Test",
            rangeFeet: null,
            savingThrowAbilityId: null,
            durationMinutes: null,
            rollBonus: null,
            [CreateSource()]);

        Assert.Contains(
            ChannelDivinityOptionDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsNonPositiveRangeFeet()
    {
        ChannelDivinityOptionDefinition definition = Create(
            "dnd5e2014.channel-divinity-option.test",
            "Test",
            rangeFeet: 0,
            savingThrowAbilityId: null,
            durationMinutes: null,
            rollBonus: null,
            [CreateSource()]);

        Assert.Contains(
            ChannelDivinityOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "greater than zero feet",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositiveDurationMinutes()
    {
        ChannelDivinityOptionDefinition definition = Create(
            "dnd5e2014.channel-divinity-option.test",
            "Test",
            rangeFeet: null,
            savingThrowAbilityId: null,
            durationMinutes: 0,
            rollBonus: null,
            [CreateSource()]);

        Assert.Contains(
            ChannelDivinityOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "greater than zero minutes",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositiveRollBonus()
    {
        ChannelDivinityOptionDefinition definition = Create(
            "dnd5e2014.channel-divinity-option.test",
            "Test",
            rangeFeet: null,
            savingThrowAbilityId: null,
            durationMinutes: null,
            rollBonus: 0,
            [CreateSource()]);

        Assert.Contains(
            ChannelDivinityOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "greater than zero",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void
        Validator_RejectsConditionDurationTriggerWithoutImposedCondition()
    {
        ChannelDivinityOptionDefinition definition = Create(
            "dnd5e2014.channel-divinity-option.test",
            "Test",
            rangeFeet: null,
            savingThrowAbilityId: null,
            durationMinutes: null,
            rollBonus: null,
            [CreateSource()],
            conditionDurationTrigger:
                NextTurnDurationTrigger.EndOfYourNextTurn);

        Assert.Contains(
            ChannelDivinityOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "condition duration trigger",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void
        Validator_RejectsAutomaticSpellSaveFailureWithoutGrantedSpell()
    {
        ChannelDivinityOptionDefinition definition = Create(
            "dnd5e2014.channel-divinity-option.test",
            "Test",
            rangeFeet: null,
            savingThrowAbilityId: null,
            durationMinutes: null,
            rollBonus: null,
            [CreateSource()],
            automaticallyFailsGrantedSpellSave: true);

        Assert.Contains(
            ChannelDivinityOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "granted spell",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        ChannelDivinityOptionDefinition definition = Create(
            "dnd5e2014.channel-divinity-option.test",
            "Test",
            rangeFeet: null,
            savingThrowAbilityId: null,
            durationMinutes: null,
            rollBonus: null,
            []);

        Assert.Contains(
            ChannelDivinityOptionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ChannelDivinityOptionCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new ChannelDivinityOptionCatalog(
            [
                Create(
                    "dnd5e2014.channel-divinity-option.z",
                    "Z",
                    rangeFeet: null,
                    savingThrowAbilityId: null,
                    durationMinutes: null,
                    rollBonus: null,
                    [CreateSource()]),
                Create(
                    "dnd5e2014.channel-divinity-option.a",
                    "A",
                    rangeFeet: null,
                    savingThrowAbilityId: null,
                    durationMinutes: null,
                    rollBonus: null,
                    [CreateSource()])
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.channel-divinity-option.a",
                "dnd5e2014.channel-divinity-option.z"
            ],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId =
            new ChannelDivinityOptionId("dnd5e2014.channel-divinity-option.a");

        ChannelDivinityOptionDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(
                aId,
                out ChannelDivinityOptionDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId = new ChannelDivinityOptionId(
            "dnd5e2014.channel-divinity-option.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out ChannelDivinityOptionDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new ChannelDivinityOptionCatalog(
                [
                    Create(
                        "dnd5e2014.channel-divinity-option.duplicate",
                        "One",
                        rangeFeet: null,
                        savingThrowAbilityId: null,
                        durationMinutes: null,
                        rollBonus: null,
                        [CreateSource()]),
                    Create(
                        "dnd5e2014.channel-divinity-option.duplicate",
                        "Two",
                        rangeFeet: null,
                        savingThrowAbilityId: null,
                        durationMinutes: null,
                        rollBonus: null,
                        [CreateSource()])
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        ChannelDivinityOptionDefinition definition = Create(
            "dnd5e2014.channel-divinity-option.test",
            "Test",
            rangeFeet: null,
            savingThrowAbilityId: null,
            durationMinutes: null,
            rollBonus: null,
            []);

        Assert.Throws<InvalidOperationException>(
            () => new ChannelDivinityOptionCatalog([definition]));
    }

    private static ChannelDivinityOptionDefinition Create(
        string? id,
        string name,
        int? rangeFeet,
        AbilityId? savingThrowAbilityId,
        int? durationMinutes,
        int? rollBonus,
        IEnumerable<SourceReference> sources,
        ConditionId? imposedConditionId = null,
        NextTurnDurationTrigger? conditionDurationTrigger = null,
        bool maximizesDamageRoll = false,
        SpellId? grantedSpellId = null,
        bool automaticallyFailsGrantedSpellSave = false)
    {
        return new ChannelDivinityOptionDefinition(
            id: id is null ? default : new ChannelDivinityOptionId(id),
            name: name,
            rangeFeet: rangeFeet,
            savingThrowAbilityId: savingThrowAbilityId,
            durationMinutes: durationMinutes,
            rollBonus: rollBonus,
            imposedConditionId: imposedConditionId,
            conditionDurationTrigger: conditionDurationTrigger,
            maximizesDamageRoll: maximizesDamageRoll,
            grantedSpellId: grantedSpellId,
            automaticallyFailsGrantedSpellSave:
                automaticallyFailsGrantedSpellSave,
            sources: sources);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 59);
    }
}
