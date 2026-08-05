using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Alignments;
using FiveEData.Rules.Creatures.Alignments.Serialization;

namespace FiveEData.Tests;

public sealed class AlignmentDataFileTests
{
    private const string ExpectedSection =
        "Chapter 4: Personality and Background — Alignment";

    private static readonly ExpectedAlignment[] Expected =
    [
        Alignment(
            "lawful-good",
            "Lawful Good",
            AlignmentEthic.Lawful,
            AlignmentMorality.Good),
        Alignment(
            "neutral-good",
            "Neutral Good",
            AlignmentEthic.Neutral,
            AlignmentMorality.Good),
        Alignment(
            "chaotic-good",
            "Chaotic Good",
            AlignmentEthic.Chaotic,
            AlignmentMorality.Good),
        Alignment(
            "lawful-neutral",
            "Lawful Neutral",
            AlignmentEthic.Lawful,
            AlignmentMorality.Neutral),
        Alignment(
            "neutral",
            "Neutral",
            AlignmentEthic.Neutral,
            AlignmentMorality.Neutral),
        Alignment(
            "chaotic-neutral",
            "Chaotic Neutral",
            AlignmentEthic.Chaotic,
            AlignmentMorality.Neutral),
        Alignment(
            "lawful-evil",
            "Lawful Evil",
            AlignmentEthic.Lawful,
            AlignmentMorality.Evil),
        Alignment(
            "neutral-evil",
            "Neutral Evil",
            AlignmentEthic.Neutral,
            AlignmentMorality.Evil),
        Alignment(
            "chaotic-evil",
            "Chaotic Evil",
            AlignmentEthic.Chaotic,
            AlignmentMorality.Evil)
    ];

    [Fact]
    public void CanonicalFile_ContainsExactAlignmentClosure()
    {
        IReadOnlyList<AlignmentDefinition> definitions =
            LoadCanonical();

        Assert.Equal(9, definitions.Count);
        Assert.Equal(
            9,
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
    public void CanonicalFile_MatchesFirstPrintingAlignments()
    {
        IReadOnlyDictionary<
            AlignmentId,
            AlignmentDefinition> actual =
                LoadCanonical()
                    .ToDictionary(
                        definition => definition.Id);

        foreach (
            ExpectedAlignment expected
            in Expected)
        {
            AlignmentDefinition definition =
                actual[new AlignmentId(expected.Id)];

            Assert.Equal(
                expected.Name,
                definition.Name);
            Assert.Equal(expected.Ethic, definition.Ethic);
            Assert.Equal(
                expected.Morality,
                definition.Morality);

            SourceReference source =
                Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(122, source.Page);
            Assert.Equal(
                ExpectedSection,
                source.Section);
        }
    }

    private static IReadOnlyList<AlignmentDefinition>
        LoadCanonical()
    {
        return AlignmentDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "alignments.json"));
    }

    private static ExpectedAlignment Alignment(
        string suffix,
        string name,
        AlignmentEthic ethic,
        AlignmentMorality morality)
    {
        return new ExpectedAlignment(
            "dnd5e2014.alignment." + suffix,
            name,
            ethic,
            morality);
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

    private sealed record ExpectedAlignment(
        string Id,
        string Name,
        AlignmentEthic Ethic,
        AlignmentMorality Morality);
}
