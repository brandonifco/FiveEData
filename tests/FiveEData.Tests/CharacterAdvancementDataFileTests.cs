using FiveEData.Rules.Characters.CharacterAdvancement;
using FiveEData.Rules.Characters.CharacterAdvancement.Serialization;

namespace FiveEData.Tests;

public sealed class CharacterAdvancementDataFileTests
{
    [Fact]
    public void CanonicalFile_CoversEveryLevelFromOneThroughTwenty()
    {
        CharacterAdvancementRules rules = LoadCanonical();

        Assert.Equal(20, rules.Levels.Count);
        Assert.Equal(
            Enumerable.Range(1, 20),
            rules.Levels.Select(level => level.Level));
    }

    [Theory]
    [InlineData(1, 0, 2)]
    [InlineData(2, 300, 2)]
    [InlineData(3, 900, 2)]
    [InlineData(4, 2_700, 2)]
    [InlineData(5, 6_500, 3)]
    [InlineData(6, 14_000, 3)]
    [InlineData(7, 23_000, 3)]
    [InlineData(8, 34_000, 3)]
    [InlineData(9, 48_000, 4)]
    [InlineData(10, 64_000, 4)]
    [InlineData(11, 85_000, 4)]
    [InlineData(12, 100_000, 4)]
    [InlineData(13, 120_000, 5)]
    [InlineData(14, 140_000, 5)]
    [InlineData(15, 165_000, 5)]
    [InlineData(16, 195_000, 5)]
    [InlineData(17, 225_000, 6)]
    [InlineData(18, 265_000, 6)]
    [InlineData(19, 305_000, 6)]
    [InlineData(20, 355_000, 6)]
    public void CanonicalFile_MatchesThePrintedTable(
        int level,
        int expectedExperiencePoints,
        int expectedProficiencyBonus)
    {
        CharacterAdvancementLevel entry = LoadCanonical().Get(level);

        Assert.Equal(expectedExperiencePoints, entry.ExperiencePointThreshold);
        Assert.Equal(expectedProficiencyBonus, entry.ProficiencyBonus);
    }

    [Fact]
    public void ProficiencyBonusRisesEveryFourLevelsAndNeverFalls()
    {
        IReadOnlyList<CharacterAdvancementLevel> levels =
            LoadCanonical().Levels;

        Assert.All(
            levels,
            level => Assert.Equal(
                2 + ((level.Level - 1) / 4),
                level.ProficiencyBonus));

        for (int index = 1; index < levels.Count; index++)
        {
            Assert.True(
                levels[index].ProficiencyBonus >=
                levels[index - 1].ProficiencyBonus);
        }
    }

    [Fact]
    public void ExperiencePointThresholdsStrictlyAscendFromZero()
    {
        IReadOnlyList<CharacterAdvancementLevel> levels =
            LoadCanonical().Levels;

        Assert.Equal(0, levels[0].ExperiencePointThreshold);

        for (int index = 1; index < levels.Count; index++)
        {
            Assert.True(
                levels[index].ExperiencePointThreshold >
                levels[index - 1].ExperiencePointThreshold);
        }
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(299, 1)]
    [InlineData(300, 2)]
    [InlineData(2_699, 3)]
    [InlineData(6_500, 5)]
    [InlineData(354_999, 19)]
    [InlineData(355_000, 20)]
    [InlineData(10_000_000, 20)]
    public void LevelForExperiencePoints_UsesTheThresholdReached(
        int experiencePoints,
        int expectedLevel)
    {
        Assert.Equal(
            expectedLevel,
            LoadCanonical().LevelForExperiencePoints(experiencePoints));
    }

    [Fact]
    public void LevelForExperiencePoints_RejectsNegativeInput()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => LoadCanonical().LevelForExperiencePoints(-1));
    }

    [Fact]
    public void CanonicalFile_CitesPhbFirstPrintingPageFifteen()
    {
        var source = Assert.Single(LoadCanonical().Sources);

        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(15, source.Page);
        Assert.Equal(
            "Chapter 1: Step-by-Step Characters — Beyond 1st Level — " +
            "Character Advancement",
            source.Section);
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedResourceMatchingTheDataFile()
    {
        CharacterAdvancementRules embedded =
            Dnd5e2014Ruleset.Instance.CharacterAdvancement;

        Assert.Equal(
            LoadCanonical()
                .Levels
                .Select(level =>
                    (level.Level,
                     level.ExperiencePointThreshold,
                     level.ProficiencyBonus)),
            embedded
                .Levels
                .Select(level =>
                    (level.Level,
                     level.ExperiencePointThreshold,
                     level.ProficiencyBonus)));
    }

    private static CharacterAdvancementRules LoadCanonical()
    {
        return CharacterAdvancementRulesLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "character-advancement.json"));
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(directory.FullName, "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }
}
