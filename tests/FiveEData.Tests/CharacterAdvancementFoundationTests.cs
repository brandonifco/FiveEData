using FiveEData.Rules.Characters.CharacterAdvancement;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class CharacterAdvancementFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void Level_RejectsOutOfRangeLevel(int level)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new CharacterAdvancementLevel(level, 0, 2));
    }

    [Fact]
    public void Level_RejectsNegativeExperiencePointThreshold()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new CharacterAdvancementLevel(1, -1, 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void Level_RejectsNonPositiveProficiencyBonus(int bonus)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new CharacterAdvancementLevel(1, 0, bonus));
    }

    [Fact]
    public void Rules_OrderLevelsAndSnapshotSources()
    {
        var sources = new List<SourceReference> { CreateSource() };

        var rules = new CharacterAdvancementRules(
            CreateLevels().Reverse(),
            sources);

        sources.Clear();

        Assert.Equal(
            Enumerable.Range(1, 20),
            rules.Levels.Select(level => level.Level));
        Assert.Single(rules.Sources);
    }

    [Fact]
    public void Rules_ProvideCompleteLookupSemantics()
    {
        CharacterAdvancementRules rules = CreateRules();

        Assert.Equal(5, rules.Get(5).Level);
        Assert.True(rules.TryGet(5, out CharacterAdvancementLevel found));
        Assert.Equal(5, found.Level);

        Assert.Throws<KeyNotFoundException>(() => rules.Get(21));
        Assert.False(rules.TryGet(21, out _));
    }

    [Fact]
    public void Validator_RejectsWrongLevelCount()
    {
        var rules = new CharacterAdvancementRules(
            CreateLevels().Take(19),
            [CreateSource()]);

        Assert.Contains(
            CharacterAdvancementRulesValidator.Validate(rules),
            error =>
                error.Contains(
                    "exactly 20 levels",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsAGapInTheLevelSequence()
    {
        CharacterAdvancementLevel[] levels = CreateLevels();
        levels[4] = new CharacterAdvancementLevel(4, 6_500, 3);

        var rules = new CharacterAdvancementRules(levels, [CreateSource()]);

        Assert.Contains(
            CharacterAdvancementRulesValidator.Validate(rules),
            error =>
                error.Contains(
                    "without gaps",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsANonZeroFirstThreshold()
    {
        CharacterAdvancementLevel[] levels = CreateLevels();
        levels[0] = new CharacterAdvancementLevel(1, 50, 2);

        var rules = new CharacterAdvancementRules(levels, [CreateSource()]);

        Assert.Contains(
            CharacterAdvancementRulesValidator.Validate(rules),
            error =>
                error.Contains(
                    "start at 0 experience",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsNonAscendingExperiencePoints()
    {
        CharacterAdvancementLevel[] levels = CreateLevels();
        levels[2] = new CharacterAdvancementLevel(
            3,
            levels[1].ExperiencePointThreshold,
            2);

        var rules = new CharacterAdvancementRules(levels, [CreateSource()]);

        Assert.Contains(
            CharacterAdvancementRulesValidator.Validate(rules),
            error =>
                error.Contains(
                    "strictly ascend",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_AcceptsAPlateauedProficiencyBonusButNotAFall()
    {
        // The real table holds the same bonus for four levels at a time,
        // so a plateau must stay valid while a decrease must not.
        Assert.Empty(
            CharacterAdvancementRulesValidator.Validate(CreateRules()));

        CharacterAdvancementLevel[] levels = CreateLevels();
        levels[5] = new CharacterAdvancementLevel(
            6,
            levels[5].ExperiencePointThreshold,
            2);

        var falling = new CharacterAdvancementRules(levels, [CreateSource()]);

        Assert.Contains(
            CharacterAdvancementRulesValidator.Validate(falling),
            error =>
                error.Contains(
                    "never decrease",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        var rules = new CharacterAdvancementRules(CreateLevels(), []);

        Assert.Contains(
            CharacterAdvancementRulesValidator.Validate(rules),
            error =>
                error.Contains("source", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void EnsureValid_ThrowsForInvalidRules()
    {
        var rules = new CharacterAdvancementRules(
            CreateLevels().Take(3),
            [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => CharacterAdvancementRulesValidator.EnsureValid(rules));
    }

    private static CharacterAdvancementRules CreateRules()
    {
        return new CharacterAdvancementRules(
            CreateLevels(),
            [CreateSource()]);
    }

    private static CharacterAdvancementLevel[] CreateLevels()
    {
        return Enumerable
            .Range(1, 20)
            .Select(
                level => new CharacterAdvancementLevel(
                    level: level,
                    experiencePointThreshold: (level - 1) * 100,
                    proficiencyBonus: 2 + ((level - 1) / 4)))
            .ToArray();
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 15);
    }
}
