using FiveEData.Rules.Backgrounds;
using FiveEData.Rules.Backgrounds.Serialization;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class BackgroundDataFileTests
{
    private const string ExpectedSection =
        "Chapter 4: Personality and Background";

    private static readonly ExpectedBackground[] Expected =
    [
        new(
            "acolyte", "Acolyte",
            ["insight", "religion"], 2,
            "shelter-of-the-faithful", 125),
        new(
            "charlatan", "Charlatan",
            ["deception", "sleight-of-hand"], 0,
            "false-identity", 126),
        new(
            "criminal", "Criminal",
            ["deception", "stealth"], 0,
            "criminal-contact", 127),
        new(
            "entertainer", "Entertainer",
            ["acrobatics", "performance"], 0,
            "by-popular-demand", 128),
        new(
            "folk-hero", "Folk Hero",
            ["animal-handling", "survival"], 0,
            "rustic-hospitality", 129),
        new(
            "guild-artisan", "Guild Artisan",
            ["insight", "persuasion"], 1,
            "guild-membership", 130),
        new(
            "hermit", "Hermit",
            ["medicine", "religion"], 1,
            "discovery", 132),
        new(
            "noble", "Noble",
            ["history", "persuasion"], 1,
            "position-of-privilege", 133),
        new(
            "outlander", "Outlander",
            ["athletics", "survival"], 1,
            "wanderer", 134),
        new(
            "sage", "Sage",
            ["arcana", "history"], 2,
            "researcher", 135),
        new(
            "sailor", "Sailor",
            ["athletics", "perception"], 0,
            "ships-passage", 137),
        new(
            "soldier", "Soldier",
            ["athletics", "intimidation"], 0,
            "military-rank", 138),
        new(
            "urchin", "Urchin",
            ["sleight-of-hand", "stealth"], 0,
            "city-secrets", 139)
    ];

    [Fact]
    public void CanonicalFile_ContainsExactBackgroundClosure()
    {
        IReadOnlyList<BackgroundDefinition> definitions =
            LoadCanonical();

        Assert.Equal(13, definitions.Count);
        Assert.Equal(
            Expected
                .Select(expected => "dnd5e2014.background." + expected.Suffix)
                .OrderBy(id => id, StringComparer.Ordinal),
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingBackgrounds()
    {
        IReadOnlyDictionary<BackgroundId, BackgroundDefinition> actual =
            LoadCanonical().ToDictionary(definition => definition.Id);

        foreach (ExpectedBackground expected in Expected)
        {
            BackgroundDefinition definition =
                actual[
                    new BackgroundId(
                        "dnd5e2014.background." + expected.Suffix)];

            Assert.Equal(expected.Name, definition.Name);
            Assert.Equal(
                expected.Skills
                    .Select(suffix => "dnd5e2014.skill." + suffix)
                    .ToArray(),
                definition.SkillProficiencyIds
                    .Select(id => id.Value)
                    .ToArray());
            Assert.Equal(
                expected.LanguageChoiceCount,
                definition.LanguageChoiceCount);
            Assert.Equal(
                "dnd5e2014.background-rule." + expected.FeatureSuffix,
                definition.FeatureRuleId.Value);

            SourceReference source = Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(expected.Page, source.Page);
            Assert.Equal(ExpectedSection, source.Section);
        }
    }

    [Fact]
    public void CanonicalFile_EveryBackgroundGrantsExactlyTwoSkills()
    {
        IReadOnlyList<BackgroundDefinition> definitions =
            LoadCanonical();

        Assert.All(
            definitions,
            definition =>
                Assert.Equal(2, definition.SkillProficiencyIds.Count));
    }

    [Fact]
    public void CanonicalFile_EveryFeatureRuleIdIsDistinct()
    {
        IReadOnlyList<BackgroundDefinition> definitions =
            LoadCanonical();

        Assert.Equal(
            definitions.Count,
            definitions
                .Select(definition => definition.FeatureRuleId)
                .Distinct()
                .Count());
    }

    private static IReadOnlyList<BackgroundDefinition> LoadCanonical()
    {
        return BackgroundDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "backgrounds.json"));
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

    private sealed record ExpectedBackground(
        string Suffix,
        string Name,
        string[] Skills,
        int LanguageChoiceCount,
        string FeatureSuffix,
        int Page);
}
