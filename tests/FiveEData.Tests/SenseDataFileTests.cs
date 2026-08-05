using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Senses;
using FiveEData.Rules.Creatures.Senses.Serialization;

namespace FiveEData.Tests;

public sealed class SenseDataFileTests
{
    private const string ExpectedSection =
        "Chapter 8: Adventuring — The Environment — " +
        "Vision and Light";

    private static readonly ExpectedSense[] Expected =
    [
        Sense("darkvision", "Darkvision")
    ];

    [Fact]
    public void CanonicalFile_ContainsExactSenseClosure()
    {
        IReadOnlyList<SenseDefinition> definitions =
            LoadCanonical();

        Assert.Single(definitions);
        Assert.Single(
            definitions
                .Select(definition => definition.Id)
                .Distinct());

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
    public void CanonicalFile_MatchesFirstPrintingSenses()
    {
        IReadOnlyDictionary<
            SenseId,
            SenseDefinition> actual =
                LoadCanonical()
                    .ToDictionary(
                        definition => definition.Id);

        foreach (
            ExpectedSense expected
            in Expected)
        {
            SenseDefinition definition =
                actual[new SenseId(expected.Id)];

            Assert.Equal(
                expected.Name,
                definition.Name);

            SourceReference source =
                Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(183, source.Page);
            Assert.Equal(
                ExpectedSection,
                source.Section);
        }
    }

    private static IReadOnlyList<SenseDefinition>
        LoadCanonical()
    {
        return SenseDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "senses.json"));
    }

    private static ExpectedSense Sense(
        string suffix,
        string name)
    {
        return new ExpectedSense(
            "dnd5e2014.sense." + suffix,
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

    private sealed record ExpectedSense(
        string Id,
        string Name);
}
