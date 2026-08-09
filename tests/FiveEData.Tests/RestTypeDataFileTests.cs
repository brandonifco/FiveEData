using FiveEData;
using FiveEData.Rules.Adventuring.Resting;
using FiveEData.Rules.Adventuring.Resting.Serialization;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class RestTypeDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactRestTypeClosure()
    {
        IReadOnlyList<RestTypeDefinition> definitions = LoadCanonical();

        Assert.Equal(
            [
                "dnd5e2014.rest-type.long-rest",
                "dnd5e2014.rest-type.short-rest"
            ],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_ContainsExactlyTwoRestTypes()
    {
        Assert.Equal(2, LoadCanonical().Count);
    }

    [Fact]
    public void ShortRest_RequiresOneHourWithNoCooldownOrHitPointFloor()
    {
        RestTypeDefinition definition =
            Get("dnd5e2014.rest-type.short-rest");

        Assert.Equal("Short Rest", definition.Name);
        Assert.Equal(1, definition.MinimumDurationHours);
        Assert.Null(definition.CooldownHours);
        Assert.Null(definition.MinimumHitPointsToBenefit);
    }

    [Fact]
    public void LongRest_RequiresEightHoursWithADailyCooldownAndHitPointFloor()
    {
        RestTypeDefinition definition = Get("dnd5e2014.rest-type.long-rest");

        Assert.Equal("Long Rest", definition.Name);
        Assert.Equal(8, definition.MinimumDurationHours);
        Assert.Equal(24, definition.CooldownHours);
        Assert.Equal(1, definition.MinimumHitPointsToBenefit);
    }

    [Fact]
    public void EveryRestType_CitesPage186()
    {
        foreach (RestTypeDefinition definition in LoadCanonical())
        {
            SourceReference source = Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(186, source.Page);
            Assert.Equal(
                "Chapter 8: Adventuring — Resting — " + definition.Name,
                source.Section);
        }
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedResourceMatchingTheDataFile()
    {
        RestTypeCatalog catalog = Dnd5e2014Ruleset.Instance.RestTypes;

        Assert.Equal(
            LoadCanonical()
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal),
            catalog.All.Select(definition => definition.Id.Value));
    }

    private static RestTypeDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<RestTypeDefinition> LoadCanonical()
    {
        return RestTypeDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "rest-types.json"));
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
