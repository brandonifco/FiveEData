using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Languages.Serialization;

namespace FiveEData.Tests;

public sealed class LanguageDataFileTests
{
    private const string ExpectedSection =
        "Chapter 4: Personality and Background — Languages";

    private static readonly ExpectedLanguage[] Expected =
    [
        Standard("common", "Common"),
        Standard("dwarvish", "Dwarvish"),
        Standard("elvish", "Elvish"),
        Standard("giant", "Giant"),
        Standard("gnomish", "Gnomish"),
        Standard("goblin", "Goblin"),
        Standard("halfling", "Halfling"),
        Standard("orc", "Orc"),
        Exotic("abyssal", "Abyssal"),
        Exotic("celestial", "Celestial"),
        Exotic("draconic", "Draconic"),
        Exotic("deep-speech", "Deep Speech"),
        Exotic("infernal", "Infernal"),
        Exotic("primordial", "Primordial"),
        Exotic("sylvan", "Sylvan"),
        Exotic("undercommon", "Undercommon")
    ];

    [Fact]
    public void CanonicalFile_ContainsExactLanguageClosure()
    {
        IReadOnlyList<LanguageDefinition> definitions =
            LoadCanonical();

        Assert.Equal(16, definitions.Count);
        Assert.Equal(
            16,
            definitions
                .Select(definition => definition.Id)
                .Distinct()
                .Count());

        Assert.Equal(
            Expected
                .Select(expected => expected.Id)
                .OrderBy(
                    id => id,
                    StringComparer.Ordinal),
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(
                    id => id,
                    StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_HasEightStandardAndEightExoticLanguages()
    {
        IReadOnlyList<LanguageDefinition> definitions =
            LoadCanonical();

        Assert.Equal(
            8,
            definitions.Count(
                definition =>
                    definition.Category ==
                    LanguageCategory.Standard));

        Assert.Equal(
            8,
            definitions.Count(
                definition =>
                    definition.Category ==
                    LanguageCategory.Exotic));
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingLanguages()
    {
        IReadOnlyDictionary<
            LanguageId,
            LanguageDefinition> actual =
                LoadCanonical()
                    .ToDictionary(
                        definition => definition.Id);

        foreach (ExpectedLanguage expected in Expected)
        {
            LanguageDefinition definition =
                actual[new LanguageId(expected.Id)];

            Assert.Equal(
                expected.Name,
                definition.Name);
            Assert.Equal(
                expected.Category,
                definition.Category);

            SourceReference source =
                Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(123, source.Page);
            Assert.Equal(
                ExpectedSection,
                source.Section);
        }
    }

    private static IReadOnlyList<LanguageDefinition>
        LoadCanonical()
    {
        return LanguageDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "languages.json"));
    }

    private static ExpectedLanguage Standard(
        string suffix,
        string name)
    {
        return new ExpectedLanguage(
            "dnd5e2014.language." + suffix,
            name,
            LanguageCategory.Standard);
    }

    private static ExpectedLanguage Exotic(
        string suffix,
        string name)
    {
        return new ExpectedLanguage(
            "dnd5e2014.language." + suffix,
            name,
            LanguageCategory.Exotic);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(
                        directory.FullName,
                        "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }

    private sealed record ExpectedLanguage(
        string Id,
        string Name,
        LanguageCategory Category);
}
