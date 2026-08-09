using FiveEData;
using FiveEData.Rules.Adventuring.TravelPace;
using FiveEData.Rules.Adventuring.TravelPace.Serialization;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class TravelPaceDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactTravelPaceClosure()
    {
        IReadOnlyList<TravelPaceDefinition> definitions = LoadCanonical();

        Assert.Equal(
            [
                "dnd5e2014.travel-pace.fast",
                "dnd5e2014.travel-pace.normal",
                "dnd5e2014.travel-pace.slow"
            ],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_ContainsExactlyThreePaces()
    {
        Assert.Equal(3, LoadCanonical().Count);
    }

    [Fact]
    public void Fast_CoversMoreGroundWithAPerceptionPenalty()
    {
        TravelPaceDefinition definition = Get("dnd5e2014.travel-pace.fast");

        Assert.Equal("Fast", definition.Name);
        Assert.Equal(400, definition.FeetPerMinute);
        Assert.Equal(4, definition.MilesPerHour);
        Assert.Equal(30, definition.MilesPerDay);
        Assert.Equal(5, definition.PassiveWisdomPerceptionPenalty);
        Assert.False(definition.AllowsStealth);
    }

    [Fact]
    public void Normal_HasNoEffect()
    {
        TravelPaceDefinition definition =
            Get("dnd5e2014.travel-pace.normal");

        Assert.Equal("Normal", definition.Name);
        Assert.Equal(300, definition.FeetPerMinute);
        Assert.Equal(3, definition.MilesPerHour);
        Assert.Equal(24, definition.MilesPerDay);
        Assert.Null(definition.PassiveWisdomPerceptionPenalty);
        Assert.False(definition.AllowsStealth);
    }

    [Fact]
    public void Slow_CoversLessGroundButAllowsStealth()
    {
        TravelPaceDefinition definition = Get("dnd5e2014.travel-pace.slow");

        Assert.Equal("Slow", definition.Name);
        Assert.Equal(200, definition.FeetPerMinute);
        Assert.Equal(2, definition.MilesPerHour);
        Assert.Equal(18, definition.MilesPerDay);
        Assert.Null(definition.PassiveWisdomPerceptionPenalty);
        Assert.True(definition.AllowsStealth);
    }

    [Fact]
    public void EveryPace_CitesPage182()
    {
        foreach (TravelPaceDefinition definition in LoadCanonical())
        {
            SourceReference source = Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(182, source.Page);
            Assert.Equal(
                "Chapter 8: Adventuring — Movement — Travel Pace",
                source.Section);
        }
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedResourceMatchingTheDataFile()
    {
        TravelPaceCatalog catalog = Dnd5e2014Ruleset.Instance.TravelPaces;

        Assert.Equal(
            LoadCanonical()
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal),
            catalog.All.Select(definition => definition.Id.Value));
    }

    private static TravelPaceDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<TravelPaceDefinition> LoadCanonical()
    {
        return TravelPaceDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "travel-pace.json"));
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
