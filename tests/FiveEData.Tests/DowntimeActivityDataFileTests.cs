using FiveEData;
using FiveEData.Rules.Adventuring.DowntimeActivities;
using FiveEData.Rules.Adventuring.DowntimeActivities.Serialization;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class DowntimeActivityDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactDowntimeActivityClosure()
    {
        IReadOnlyList<DowntimeActivityDefinition> definitions =
            LoadCanonical();

        Assert.Equal(
            [
                "dnd5e2014.downtime-activity.crafting",
                "dnd5e2014.downtime-activity.practicing-a-profession",
                "dnd5e2014.downtime-activity.recuperating",
                "dnd5e2014.downtime-activity.researching",
                "dnd5e2014.downtime-activity.training"
            ],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_ContainsExactlyFiveActivities()
    {
        Assert.Equal(5, LoadCanonical().Count);
    }

    [Fact]
    public void Crafting_ProgressesFiveGoldPiecesOfMarketValuePerDay()
    {
        DowntimeActivityDefinition definition =
            Get("dnd5e2014.downtime-activity.crafting");

        Assert.Equal("Crafting", definition.Name);
        Assert.Null(definition.RequiredDays);
        Assert.Null(definition.CostPerDayGoldPieces);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Null(definition.SavingThrowDC);
        Assert.Equal(5, definition.MarketValueProgressPerDayGoldPieces);
    }

    // Practicing a Profession grants a Lifestyle tier (modest for free,
    // comfortable or wealthy under specific conditions) rather than a
    // clean number — every fact stays declined, the same "still gets a
    // catalog entry" shape Pact Boon already established.
    [Fact]
    public void PracticingAProfession_DeclinesEveryFact()
    {
        DowntimeActivityDefinition definition =
            Get("dnd5e2014.downtime-activity.practicing-a-profession");

        Assert.Equal("Practicing a Profession", definition.Name);
        Assert.Null(definition.RequiredDays);
        Assert.Null(definition.CostPerDayGoldPieces);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Null(definition.SavingThrowDC);
        Assert.Null(definition.MarketValueProgressPerDayGoldPieces);
    }

    [Fact]
    public void Recuperating_RequiresThreeDaysAndADCFifteenConstitutionSave()
    {
        DowntimeActivityDefinition definition =
            Get("dnd5e2014.downtime-activity.recuperating");

        Assert.Equal("Recuperating", definition.Name);
        Assert.Equal(3, definition.RequiredDays);
        Assert.Null(definition.CostPerDayGoldPieces);
        Assert.Equal(
            "dnd5e2014.ability.constitution",
            definition.SavingThrowAbilityId!.Value.Value);
        Assert.Equal(15, definition.SavingThrowDC);
        Assert.Null(definition.MarketValueProgressPerDayGoldPieces);
    }

    [Fact]
    public void Researching_CostsOneGoldPiecePerDayWithNoFixedDuration()
    {
        DowntimeActivityDefinition definition =
            Get("dnd5e2014.downtime-activity.researching");

        Assert.Equal("Researching", definition.Name);
        Assert.Null(definition.RequiredDays);
        Assert.Equal(1, definition.CostPerDayGoldPieces);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Null(definition.SavingThrowDC);
        Assert.Null(definition.MarketValueProgressPerDayGoldPieces);
    }

    [Fact]
    public void Training_RequiresTwoHundredFiftyDaysAtOneGoldPiecePerDay()
    {
        DowntimeActivityDefinition definition =
            Get("dnd5e2014.downtime-activity.training");

        Assert.Equal("Training", definition.Name);
        Assert.Equal(250, definition.RequiredDays);
        Assert.Equal(1, definition.CostPerDayGoldPieces);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Null(definition.SavingThrowDC);
        Assert.Null(definition.MarketValueProgressPerDayGoldPieces);
    }

    [Fact]
    public void EveryActivity_CitesPage187()
    {
        foreach (DowntimeActivityDefinition definition in LoadCanonical())
        {
            SourceReference source = Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(187, source.Page);
            Assert.Equal(
                "Chapter 8: Adventuring — Between Adventures — Downtime " +
                    "Activities — " + definition.Name,
                source.Section);
        }
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedResourceMatchingTheDataFile()
    {
        DowntimeActivityCatalog catalog =
            Dnd5e2014Ruleset.Instance.DowntimeActivities;

        Assert.Equal(
            LoadCanonical()
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal),
            catalog.All.Select(definition => definition.Id.Value));
    }

    private static DowntimeActivityDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<DowntimeActivityDefinition> LoadCanonical()
    {
        return DowntimeActivityDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "downtime-activities.json"));
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
