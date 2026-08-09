using FiveEData;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Combat.Cover;
using FiveEData.Rules.Combat.Cover.Serialization;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class CoverDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactCoverClosure()
    {
        IReadOnlyList<CoverDefinition> definitions = LoadCanonical();

        Assert.Equal(
            [
                "dnd5e2014.cover.half",
                "dnd5e2014.cover.three-quarters",
                "dnd5e2014.cover.total"
            ],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_ContainsExactlyThreeDegreesOfCover()
    {
        Assert.Equal(3, LoadCanonical().Count);
    }

    [Fact]
    public void HalfCover_GrantsPlusTwoAndDoesNotPreventTargeting()
    {
        CoverDefinition definition = Get("dnd5e2014.cover.half");

        Assert.Equal("Half Cover", definition.Name);
        Assert.Equal(2, definition.ArmorClassBonus);
        Assert.Equal(2, definition.DexteritySavingThrowBonus);
        Assert.False(definition.PreventsBeingTargeted);
    }

    [Fact]
    public void ThreeQuartersCover_GrantsPlusFiveAndDoesNotPreventTargeting()
    {
        CoverDefinition definition = Get("dnd5e2014.cover.three-quarters");

        Assert.Equal("Three-Quarters Cover", definition.Name);
        Assert.Equal(5, definition.ArmorClassBonus);
        Assert.Equal(5, definition.DexteritySavingThrowBonus);
        Assert.False(definition.PreventsBeingTargeted);
    }

    [Fact]
    public void TotalCover_PreventsTargetingWithNoBonuses()
    {
        CoverDefinition definition = Get("dnd5e2014.cover.total");

        Assert.Equal("Total Cover", definition.Name);
        Assert.Null(definition.ArmorClassBonus);
        Assert.Null(definition.DexteritySavingThrowBonus);
        Assert.True(definition.PreventsBeingTargeted);
    }

    [Fact]
    public void EveryDegreeOfCover_CitesPage196()
    {
        foreach (CoverDefinition definition in LoadCanonical())
        {
            SourceReference source = Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(196, source.Page);
            Assert.Equal(
                "Chapter 9: Combat — Cover — " + definition.Name,
                source.Section);
        }
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedResourceMatchingTheDataFile()
    {
        CoverCatalog catalog = Dnd5e2014Ruleset.Instance.Cover;

        Assert.Equal(
            LoadCanonical()
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal),
            catalog.All.Select(definition => definition.Id.Value));
    }

    private static CoverDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<CoverDefinition> LoadCanonical()
    {
        return CoverDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "cover.json"));
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
