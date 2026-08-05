using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Races;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Tests;

public sealed class RaceFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(() => new RaceId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.race.test";

        var id = new RaceId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void AbilityScoreIncrease_RejectsDefaultAbilityId()
    {
        Assert.Throws<ArgumentException>(
            () => new RaceAbilityScoreIncrease(default, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AbilityScoreIncrease_RejectsNonPositiveBonus(int bonus)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new RaceAbilityScoreIncrease(
                new AbilityId("dnd5e2014.ability.strength"),
                bonus));
    }

    [Fact]
    public void Definition_DefensivelySnapshotsCollections()
    {
        var increases = new List<RaceAbilityScoreIncrease>
        {
            new(new AbilityId("dnd5e2014.ability.strength"), 2)
        };
        var languageIds = new List<LanguageId>
        {
            new("dnd5e2014.language.common")
        };
        var traitRuleIds = new List<RuleId>
        {
            new("dnd5e2014.race-rule.darkvision")
        };
        var sources = new List<SourceReference> { CreateSource() };

        var race = Create(
            "dnd5e2014.race.test",
            abilityScoreIncreases: increases,
            languageIds: languageIds,
            traitRuleIds: traitRuleIds,
            sources: sources);

        increases.Clear();
        languageIds.Clear();
        traitRuleIds.Clear();
        sources.Clear();

        Assert.Single(race.AbilityScoreIncreases);
        Assert.Single(race.LanguageIds);
        Assert.Single(race.TraitRuleIds);
        Assert.Single(race.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        var race = new RaceDefinition(
            default,
            "Test",
            new CreatureSizeId("dnd5e2014.creature-size.medium"),
            new Distance(30),
            [],
            0,
            [],
            0,
            [],
            [CreateSource()]);

        Assert.Contains(
            RaceDefinitionValidator.Validate(race),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        RaceDefinition race = Create(
            "dnd5e2014.race.test",
            sources: []);

        Assert.Contains(
            RaceDefinitionValidator.Validate(race),
            error => error.Contains(
                "source",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositiveSpeed()
    {
        RaceDefinition race = Create(
            "dnd5e2014.race.test",
            speed: new Distance(0));

        Assert.Contains(
            RaceDefinitionValidator.Validate(race),
            error => error.Contains("speed", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDuplicateAbilityScoreIncrease()
    {
        RaceDefinition race = Create(
            "dnd5e2014.race.test",
            abilityScoreIncreases:
            [
                new RaceAbilityScoreIncrease(
                    new AbilityId("dnd5e2014.ability.strength"),
                    1),
                new RaceAbilityScoreIncrease(
                    new AbilityId("dnd5e2014.ability.strength"),
                    2)
            ]);

        Assert.Contains(
            RaceDefinitionValidator.Validate(race),
            error => error.Contains("duplicated", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsDuplicateLanguage()
    {
        RaceDefinition race = Create(
            "dnd5e2014.race.test",
            languageIds:
            [
                new LanguageId("dnd5e2014.language.common"),
                new LanguageId("dnd5e2014.language.common")
            ]);

        Assert.Contains(
            RaceDefinitionValidator.Validate(race),
            error => error.Contains("duplicated", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsDuplicateTraitRule()
    {
        RaceDefinition race = Create(
            "dnd5e2014.race.test",
            traitRuleIds:
            [
                new RuleId("dnd5e2014.race-rule.darkvision"),
                new RuleId("dnd5e2014.race-rule.darkvision")
            ]);

        Assert.Contains(
            RaceDefinitionValidator.Validate(race),
            error => error.Contains("duplicated", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData(-1)]
    public void Validator_RejectsNegativeChoosableAbilityScoreIncreaseCount(
        int count)
    {
        RaceDefinition race = Create(
            "dnd5e2014.race.test",
            choosableAbilityScoreIncreaseCount: count);

        Assert.Contains(
            RaceDefinitionValidator.Validate(race),
            error => error.Contains(
                "choosable",
                StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(-1)]
    public void Validator_RejectsNegativeAdditionalLanguageChoiceCount(
        int count)
    {
        RaceDefinition race = Create(
            "dnd5e2014.race.test",
            additionalLanguageChoiceCount: count);

        Assert.Contains(
            RaceDefinitionValidator.Validate(race),
            error => error.Contains(
                "additional language",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new RaceCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new RaceCatalog(
            [
                Create("dnd5e2014.race.z", name: "Z"),
                Create("dnd5e2014.race.a", name: "A")
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            ["dnd5e2014.race.a", "dnd5e2014.race.z"],
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        var aId = new RaceId("dnd5e2014.race.a");

        RaceDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(catalog.TryGet(aId, out RaceDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId = new RaceId("dnd5e2014.race.missing");

        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(missingId, out RaceDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<RaceDefinition>
        {
            Create("dnd5e2014.race.one", name: "One")
        };

        var catalog = new RaceCatalog(source);

        source.Add(Create("dnd5e2014.race.two", name: "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new RaceCatalog(
                [
                    Create("dnd5e2014.race.duplicate", name: "One"),
                    Create("dnd5e2014.race.duplicate", name: "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        RaceDefinition race = Create("dnd5e2014.race.test", sources: []);

        Assert.Throws<InvalidOperationException>(
            () => new RaceCatalog([race]));
    }

    private static RaceDefinition Create(
        string id,
        string name = "Test",
        CreatureSizeId? size = null,
        Distance? speed = null,
        IEnumerable<RaceAbilityScoreIncrease>? abilityScoreIncreases = null,
        int choosableAbilityScoreIncreaseCount = 0,
        IEnumerable<LanguageId>? languageIds = null,
        int additionalLanguageChoiceCount = 0,
        IEnumerable<RuleId>? traitRuleIds = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new RaceDefinition(
            new RaceId(id),
            name,
            size ?? new CreatureSizeId("dnd5e2014.creature-size.medium"),
            speed ?? new Distance(30),
            abilityScoreIncreases ?? [],
            choosableAbilityScoreIncreaseCount,
            languageIds ?? [],
            additionalLanguageChoiceCount,
            traitRuleIds ?? [],
            sources ?? [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 17);
    }
}
