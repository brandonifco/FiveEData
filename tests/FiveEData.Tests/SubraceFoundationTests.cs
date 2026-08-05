using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Races;

namespace FiveEData.Tests;

public sealed class SubraceFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(() => new SubraceId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.subrace.test";

        var id = new SubraceId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsCollections()
    {
        var increases = new List<RaceAbilityScoreIncrease>
        {
            new(new AbilityId("dnd5e2014.ability.wisdom"), 1)
        };
        var traitRuleIds = new List<RuleId>
        {
            new("dnd5e2014.race-rule.dwarven-toughness")
        };
        var sources = new List<SourceReference> { CreateSource() };

        SubraceDefinition subrace = Create(
            "dnd5e2014.subrace.test",
            abilityScoreIncreases: increases,
            traitRuleIds: traitRuleIds,
            sources: sources);

        increases.Clear();
        traitRuleIds.Clear();
        sources.Clear();

        Assert.Single(subrace.AbilityScoreIncreases);
        Assert.Single(subrace.TraitRuleIds);
        Assert.Single(subrace.Sources);
    }

    [Fact]
    public void Definition_AllowsNullSpeedOverride()
    {
        SubraceDefinition subrace = Create("dnd5e2014.subrace.test");

        Assert.Null(subrace.Speed);
    }

    [Fact]
    public void Definition_AllowsSpeedOverride()
    {
        SubraceDefinition subrace = Create(
            "dnd5e2014.subrace.test",
            speed: new Distance(35));

        Assert.Equal(35, subrace.Speed?.Feet);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        var subrace = new SubraceDefinition(
            default,
            "Test",
            new RaceId("dnd5e2014.race.elf"),
            [],
            null,
            0,
            [],
            [CreateSource()]);

        Assert.Contains(
            SubraceDefinitionValidator.Validate(subrace),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsDefaultRaceId()
    {
        var subrace = new SubraceDefinition(
            new SubraceId("dnd5e2014.subrace.test"),
            "Test",
            default,
            [],
            null,
            0,
            [],
            [CreateSource()]);

        Assert.Contains(
            SubraceDefinitionValidator.Validate(subrace),
            error => error.Contains("race ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        SubraceDefinition subrace = Create(
            "dnd5e2014.subrace.test",
            sources: []);

        Assert.Contains(
            SubraceDefinitionValidator.Validate(subrace),
            error => error.Contains(
                "source",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositiveSpeedOverride()
    {
        SubraceDefinition subrace = Create(
            "dnd5e2014.subrace.test",
            speed: new Distance(0));

        Assert.Contains(
            SubraceDefinitionValidator.Validate(subrace),
            error => error.Contains("speed", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDuplicateAbilityScoreIncrease()
    {
        SubraceDefinition subrace = Create(
            "dnd5e2014.subrace.test",
            abilityScoreIncreases:
            [
                new RaceAbilityScoreIncrease(
                    new AbilityId("dnd5e2014.ability.wisdom"),
                    1),
                new RaceAbilityScoreIncrease(
                    new AbilityId("dnd5e2014.ability.wisdom"),
                    1)
            ]);

        Assert.Contains(
            SubraceDefinitionValidator.Validate(subrace),
            error => error.Contains("duplicated", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsDuplicateTraitRule()
    {
        SubraceDefinition subrace = Create(
            "dnd5e2014.subrace.test",
            traitRuleIds:
            [
                new RuleId("dnd5e2014.race-rule.elf-weapon-training"),
                new RuleId("dnd5e2014.race-rule.elf-weapon-training")
            ]);

        Assert.Contains(
            SubraceDefinitionValidator.Validate(subrace),
            error => error.Contains("duplicated", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData(-1)]
    public void Validator_RejectsNegativeAdditionalLanguageChoiceCount(
        int count)
    {
        SubraceDefinition subrace = Create(
            "dnd5e2014.subrace.test",
            additionalLanguageChoiceCount: count);

        Assert.Contains(
            SubraceDefinitionValidator.Validate(subrace),
            error => error.Contains(
                "additional language",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new SubraceCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new SubraceCatalog(
            [
                Create("dnd5e2014.subrace.z", name: "Z"),
                Create("dnd5e2014.subrace.a", name: "A")
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            ["dnd5e2014.subrace.a", "dnd5e2014.subrace.z"],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId = new SubraceId("dnd5e2014.subrace.a");

        SubraceDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(catalog.TryGet(aId, out SubraceDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId = new SubraceId("dnd5e2014.subrace.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(missingId, out SubraceDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<SubraceDefinition>
        {
            Create("dnd5e2014.subrace.one", name: "One")
        };

        var catalog = new SubraceCatalog(source);

        source.Add(Create("dnd5e2014.subrace.two", name: "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new SubraceCatalog(
                [
                    Create("dnd5e2014.subrace.duplicate", name: "One"),
                    Create("dnd5e2014.subrace.duplicate", name: "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        SubraceDefinition subrace = Create(
            "dnd5e2014.subrace.test",
            sources: []);

        Assert.Throws<InvalidOperationException>(
            () => new SubraceCatalog([subrace]));
    }

    private static SubraceDefinition Create(
        string id,
        string name = "Test",
        string raceId = "dnd5e2014.race.elf",
        IEnumerable<RaceAbilityScoreIncrease>? abilityScoreIncreases = null,
        Distance? speed = null,
        int additionalLanguageChoiceCount = 0,
        IEnumerable<RuleId>? traitRuleIds = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new SubraceDefinition(
            new SubraceId(id),
            name,
            new RaceId(raceId),
            abilityScoreIncreases ?? [],
            speed,
            additionalLanguageChoiceCount,
            traitRuleIds ?? [],
            sources ?? [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 20);
    }
}
