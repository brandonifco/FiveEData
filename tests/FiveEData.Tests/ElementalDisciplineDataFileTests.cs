using FiveEData.Rules.Classes.ElementalDisciplines;
using FiveEData.Rules.Classes.ElementalDisciplines.Serialization;

namespace FiveEData.Tests;

public sealed class ElementalDisciplineDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactDisciplineClosure()
    {
        IReadOnlyList<ElementalDisciplineDefinition> definitions =
            LoadCanonical();

        Assert.Equal(
            [
                "dnd5e2014.elemental-discipline.breath-of-winter",
                "dnd5e2014.elemental-discipline.clench-of-the-north-wind",
                "dnd5e2014.elemental-discipline.elemental-attunement",
                "dnd5e2014.elemental-discipline.eternal-mountain-defense",
                "dnd5e2014.elemental-discipline.fangs-of-the-fire-snake",
                "dnd5e2014.elemental-discipline.fist-of-four-thunders",
                "dnd5e2014.elemental-discipline.fist-of-unbroken-air",
                "dnd5e2014.elemental-discipline.flames-of-the-phoenix",
                "dnd5e2014.elemental-discipline.gong-of-the-summit",
                "dnd5e2014.elemental-discipline.mist-stance",
                "dnd5e2014.elemental-discipline.ride-the-wind",
                "dnd5e2014.elemental-discipline.river-of-hungry-flame",
                "dnd5e2014.elemental-discipline.rush-of-the-gale-spirits",
                "dnd5e2014.elemental-discipline.shape-the-flowing-river",
                "dnd5e2014.elemental-discipline.sweeping-cinder-strike",
                "dnd5e2014.elemental-discipline.water-whip",
                "dnd5e2014.elemental-discipline.wave-of-rolling-earth"
            ],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Theory]
    [InlineData(
        "dnd5e2014.elemental-discipline.breath-of-winter",
        6,
        17)]
    [InlineData(
        "dnd5e2014.elemental-discipline.clench-of-the-north-wind",
        3,
        6)]
    [InlineData(
        "dnd5e2014.elemental-discipline.eternal-mountain-defense",
        5,
        11)]
    [InlineData(
        "dnd5e2014.elemental-discipline.flames-of-the-phoenix",
        4,
        11)]
    [InlineData(
        "dnd5e2014.elemental-discipline.gong-of-the-summit",
        3,
        6)]
    [InlineData(
        "dnd5e2014.elemental-discipline.mist-stance",
        4,
        11)]
    [InlineData(
        "dnd5e2014.elemental-discipline.ride-the-wind",
        4,
        11)]
    [InlineData(
        "dnd5e2014.elemental-discipline.river-of-hungry-flame",
        5,
        17)]
    [InlineData(
        "dnd5e2014.elemental-discipline.wave-of-rolling-earth",
        6,
        17)]
    public void LevelGatedDiscipline_HasExpectedCostAndLevel(
        string id,
        int expectedKiPointCost,
        int expectedLevel)
    {
        ElementalDisciplineDefinition definition = Get(id);

        Assert.Equal(expectedKiPointCost, definition.KiPointCost);
        Assert.Equal(expectedLevel, definition.RequiredMinimumLevel);
    }

    [Theory]
    [InlineData("dnd5e2014.elemental-discipline.fangs-of-the-fire-snake", 1)]
    [InlineData("dnd5e2014.elemental-discipline.fist-of-four-thunders", 2)]
    [InlineData("dnd5e2014.elemental-discipline.fist-of-unbroken-air", 2)]
    [InlineData(
        "dnd5e2014.elemental-discipline.rush-of-the-gale-spirits",
        2)]
    [InlineData(
        "dnd5e2014.elemental-discipline.shape-the-flowing-river",
        1)]
    [InlineData(
        "dnd5e2014.elemental-discipline.sweeping-cinder-strike",
        2)]
    [InlineData("dnd5e2014.elemental-discipline.water-whip", 2)]
    public void UnrestrictedCostDiscipline_HasExpectedCostAndNoLevel(
        string id,
        int expectedKiPointCost)
    {
        ElementalDisciplineDefinition definition = Get(id);

        Assert.Equal(expectedKiPointCost, definition.KiPointCost);
        Assert.Null(definition.RequiredMinimumLevel);
    }

    [Fact]
    public void ElementalAttunement_HasNoKiCostAndNoLevel()
    {
        ElementalDisciplineDefinition definition =
            Get("dnd5e2014.elemental-discipline.elemental-attunement");

        Assert.Null(definition.KiPointCost);
        Assert.Null(definition.RequiredMinimumLevel);
    }

    [Fact]
    public void AllDisciplines_CitePhbFirstPrintingPageEightyOne()
    {
        foreach (
            ElementalDisciplineDefinition definition in LoadCanonical())
        {
            var source = Assert.Single(definition.Sources);
            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(81, source.Page);
        }
    }

    private static ElementalDisciplineDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<ElementalDisciplineDefinition>
        LoadCanonical()
    {
        return ElementalDisciplineDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "elemental-disciplines.json"));
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
