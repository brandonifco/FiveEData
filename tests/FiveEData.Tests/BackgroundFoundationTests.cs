using FiveEData.Rules.Backgrounds;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Skills;

namespace FiveEData.Tests;

public sealed class BackgroundFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new BackgroundId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.background.test";

        var id = new BackgroundId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Definition_DefensivelySnapshotsSkillsAndSources()
    {
        var skills = new List<SkillId>
        {
            new("dnd5e2014.skill.insight"),
            new("dnd5e2014.skill.religion")
        };
        var sources = new List<SourceReference> { CreateSource() };

        var definition = new BackgroundDefinition(
            new BackgroundId("dnd5e2014.background.test"),
            "Test",
            skills,
            languageChoiceCount: 1,
            new RuleId("dnd5e2014.background-rule.test"),
            null,
            null,
            null,
            null,
            sources);

        skills.Clear();
        sources.Clear();

        Assert.Equal(2, definition.SkillProficiencyIds.Count);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        var definition = new BackgroundDefinition(
            default,
            "Test",
            TwoSkills(),
            languageChoiceCount: 0,
            new RuleId("dnd5e2014.background-rule.test"),
            null,
            null,
            null,
            null,
            [CreateSource()]);

        Assert.Contains(
            BackgroundDefinitionValidator.Validate(definition),
            error =>
                error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        var definition = new BackgroundDefinition(
            new BackgroundId("dnd5e2014.background.test"),
            "Test",
            TwoSkills(),
            languageChoiceCount: 0,
            new RuleId("dnd5e2014.background-rule.test"),
            null,
            null,
            null,
            null,
            []);

        Assert.Contains(
            BackgroundDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsWrongSkillCount()
    {
        var definition = new BackgroundDefinition(
            new BackgroundId("dnd5e2014.background.test"),
            "Test",
            [new SkillId("dnd5e2014.skill.insight")],
            languageChoiceCount: 0,
            new RuleId("dnd5e2014.background-rule.test"),
            null,
            null,
            null,
            null,
            [CreateSource()]);

        Assert.Contains(
            BackgroundDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "exactly two skill",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDuplicateSkills()
    {
        var definition = new BackgroundDefinition(
            new BackgroundId("dnd5e2014.background.test"),
            "Test",
            [
                new SkillId("dnd5e2014.skill.insight"),
                new SkillId("dnd5e2014.skill.insight")
            ],
            languageChoiceCount: 0,
            new RuleId("dnd5e2014.background-rule.test"),
            null,
            null,
            null,
            null,
            [CreateSource()]);

        Assert.Contains(
            BackgroundDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "duplicated",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNegativeLanguageChoiceCount()
    {
        var definition = new BackgroundDefinition(
            new BackgroundId("dnd5e2014.background.test"),
            "Test",
            TwoSkills(),
            languageChoiceCount: -1,
            new RuleId("dnd5e2014.background-rule.test"),
            null,
            null,
            null,
            null,
            [CreateSource()]);

        Assert.Contains(
            BackgroundDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "negative",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositiveAdditionalPeopleFedPerDay()
    {
        var definition = new BackgroundDefinition(
            new BackgroundId("dnd5e2014.background.test"),
            "Test",
            TwoSkills(),
            languageChoiceCount: 0,
            new RuleId("dnd5e2014.background-rule.test"),
            null,
            additionalPeopleFedPerDay: 0,
            null,
            null,
            [CreateSource()]);

        Assert.Contains(
            BackgroundDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "people fed",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonPositiveGuildDuesGoldPerMonth()
    {
        var definition = new BackgroundDefinition(
            new BackgroundId("dnd5e2014.background.test"),
            "Test",
            TwoSkills(),
            languageChoiceCount: 0,
            new RuleId("dnd5e2014.background-rule.test"),
            null,
            null,
            guildDuesGoldPerMonth: 0,
            null,
            [CreateSource()]);

        Assert.Contains(
            BackgroundDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "guild dues",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsFastTravelSpeedMultiplierOfOne()
    {
        var definition = new BackgroundDefinition(
            new BackgroundId("dnd5e2014.background.test"),
            "Test",
            TwoSkills(),
            languageChoiceCount: 0,
            new RuleId("dnd5e2014.background-rule.test"),
            null,
            null,
            null,
            fastTravelSpeedMultiplier: 1,
            [CreateSource()]);

        Assert.Contains(
            BackgroundDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "fast travel speed multiplier",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new BackgroundCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new BackgroundCatalog(
            [
                Create("dnd5e2014.background.z", "Z"),
                Create("dnd5e2014.background.a", "A")
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.background.a",
                "dnd5e2014.background.z"
            ],
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        var aId = new BackgroundId("dnd5e2014.background.a");

        BackgroundDefinition found = catalog.Get(aId);

        Assert.Equal("A", found.Name);
        Assert.True(
            catalog.TryGet(aId, out BackgroundDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new BackgroundId("dnd5e2014.background.missing");

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out BackgroundDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<BackgroundDefinition>
        {
            Create("dnd5e2014.background.one", "One")
        };

        var catalog = new BackgroundCatalog(source);

        source.Add(Create("dnd5e2014.background.two", "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new BackgroundCatalog(
                [
                    Create("dnd5e2014.background.duplicate", "One"),
                    Create("dnd5e2014.background.duplicate", "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        var definition = new BackgroundDefinition(
            default,
            "Invalid",
            TwoSkills(),
            languageChoiceCount: 0,
            new RuleId("dnd5e2014.background-rule.test"),
            null,
            null,
            null,
            null,
            [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => new BackgroundCatalog([definition]));
    }

    private static BackgroundDefinition Create(string id, string name)
    {
        return new BackgroundDefinition(
            new BackgroundId(id),
            name,
            TwoSkills(),
            languageChoiceCount: 0,
            new RuleId("dnd5e2014.background-rule.test"),
            null,
            null,
            null,
            null,
            [CreateSource()]);
    }

    private static SkillId[] TwoSkills()
    {
        return
        [
            new SkillId("dnd5e2014.skill.insight"),
            new SkillId("dnd5e2014.skill.religion")
        ];
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            page: 125);
    }
}
