using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Creatures.Sizes.Serialization;

namespace FiveEData.Tests;

public sealed class CreatureSizeDataFileTests
{
    private const string ExpectedSection =
        "Chapter 9: Combat — Movement and Position — " +
        "Creature Size";

    private static readonly ExpectedCreatureSize[] Expected =
    [
        Size("tiny", "Tiny"),
        Size("small", "Small"),
        Size("medium", "Medium"),
        Size("large", "Large"),
        Size("huge", "Huge"),
        Size("gargantuan", "Gargantuan")
    ];

    [Fact]
    public void CanonicalFile_ContainsExactCreatureSizeClosure()
    {
        IReadOnlyList<CreatureSizeDefinition> definitions =
            LoadCanonical();

        Assert.Equal(6, definitions.Count);
        Assert.Equal(
            6,
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
    public void CanonicalFile_MatchesFirstPrintingCreatureSizes()
    {
        IReadOnlyDictionary<
            CreatureSizeId,
            CreatureSizeDefinition> actual =
                LoadCanonical()
                    .ToDictionary(
                        definition => definition.Id);

        foreach (
            ExpectedCreatureSize expected
            in Expected)
        {
            CreatureSizeDefinition definition =
                actual[new CreatureSizeId(expected.Id)];

            Assert.Equal(
                expected.Name,
                definition.Name);

            SourceReference source =
                Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(191, source.Page);
            Assert.Equal(
                ExpectedSection,
                source.Section);
        }
    }

    private static IReadOnlyList<CreatureSizeDefinition>
        LoadCanonical()
    {
        return CreatureSizeDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "creature-sizes.json"));
    }

    private static ExpectedCreatureSize Size(
        string suffix,
        string name)
    {
        return new ExpectedCreatureSize(
            "dnd5e2014.creature-size." + suffix,
            name);
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

    private sealed record ExpectedCreatureSize(
        string Id,
        string Name);
}
