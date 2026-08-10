using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Serialization;
using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.Races;
using FiveEData.Rules.Creatures.Races.Serialization;
using FiveEData.Rules.Equipment.Tools;

namespace FiveEData.Tests;

/// <summary>
/// Tool proficiency grants span four owning domains, so the shared
/// <see cref="ToolProficiencyChoice"/> invariants and the canonical grants
/// that exercise them live together rather than being split across each
/// owner's data-file tests.
/// </summary>
public sealed class ToolProficiencyGrantTests
{
    private const string ArtisansTools =
        "dnd5e2014.tool-family.artisans-tools";

    private const string MusicalInstrument =
        "dnd5e2014.tool-family.musical-instrument";

    [Fact]
    public void Choice_RejectsANonPositiveCount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ToolProficiencyChoice(
                0,
                toolFamilyIds: [new ToolFamilyId(ArtisansTools)]));
    }

    [Fact]
    public void Choice_RejectsNeitherFamiliesNorOptions()
    {
        Assert.Throws<ArgumentException>(() => new ToolProficiencyChoice(1));
    }

    [Fact]
    public void Choice_RejectsBothFamiliesAndOptions()
    {
        Assert.Throws<ArgumentException>(() =>
            new ToolProficiencyChoice(
                1,
                toolFamilyIds: [new ToolFamilyId(ArtisansTools)],
                toolOptionIds:
                [
                    new ToolId("dnd5e2014.tool.smiths-tools"),
                    new ToolId("dnd5e2014.tool.masons-tools")
                ]));
    }

    [Fact]
    public void Choice_RejectsASingleExplicitOption()
    {
        // A "choice" of one is not a choice — the same rule
        // ChoosableSavingThrowAbilityIds already enforces.
        Assert.Throws<ArgumentException>(() =>
            new ToolProficiencyChoice(
                1,
                toolOptionIds: [new ToolId("dnd5e2014.tool.smiths-tools")]));
    }

    [Fact]
    public void Choice_RejectsGrantingEveryOptionItOffers()
    {
        Assert.Throws<ArgumentException>(() =>
            new ToolProficiencyChoice(
                2,
                toolOptionIds:
                [
                    new ToolId("dnd5e2014.tool.smiths-tools"),
                    new ToolId("dnd5e2014.tool.masons-tools")
                ]));
    }

    [Fact]
    public void Choice_RejectsARepeatedFamily()
    {
        Assert.Throws<ArgumentException>(() =>
            new ToolProficiencyChoice(
                1,
                toolFamilyIds:
                [
                    new ToolFamilyId(ArtisansTools),
                    new ToolFamilyId(ArtisansTools)
                ]));
    }

    [Fact]
    public void Choice_AllowsASingleFamilyGrantingSeveralProficiencies()
    {
        // The Bard's "three musical instruments of your choice" — a count
        // above one is only bounded when explicit options are listed.
        var choice = new ToolProficiencyChoice(
            3,
            toolFamilyIds: [new ToolFamilyId(MusicalInstrument)]);

        Assert.Equal(3, choice.Count);
        Assert.Single(choice.ToolFamilyIds);
        Assert.Empty(choice.ToolOptionIds);
    }

    [Fact]
    public void CanonicalFile_GrantsToolProficienciesToExactlyFourClasses()
    {
        IReadOnlyList<ClassDefinition> classes = LoadClasses();

        string[] granting =
        [
            .. classes
                .Where(@class =>
                    @class.ToolProficiencyIds.Count > 0 ||
                    @class.ToolProficiencyChoice is not null)
                .Select(@class => @class.Id.Value)
                .Order()
        ];

        Assert.Equal(
            [
                "dnd5e2014.class.bard",
                "dnd5e2014.class.druid",
                "dnd5e2014.class.monk",
                "dnd5e2014.class.rogue"
            ],
            granting);
    }

    [Fact]
    public void CanonicalFile_MonkChoiceSpansTwoToolFamilies()
    {
        // "Choose one type of artisan's tools or one musical instrument"
        // (p.77) — the only cross-family choice in the PHB, and the reason
        // ToolFamilyIds is a list rather than a single family.
        ClassDefinition monk = LoadClasses()
            .Single(@class => @class.Id.Value == "dnd5e2014.class.monk");

        ToolProficiencyChoice choice =
            Assert.IsType<ToolProficiencyChoice>(monk.ToolProficiencyChoice);

        Assert.Equal(1, choice.Count);
        Assert.Equal(
            [ArtisansTools, MusicalInstrument],
            choice.ToolFamilyIds.Select(id => id.Value).Order());
        Assert.Empty(monk.ToolProficiencyIds);
    }

    [Fact]
    public void CanonicalFile_DwarfChoiceNamesThreeExplicitTools()
    {
        // "the artisan's tools of your choice: smith's tools, brewer's
        // supplies, or mason's tools" (p.20) — a named subset, not the
        // whole family, which is why ToolOptionIds exists beside
        // ToolFamilyIds.
        RaceDefinition dwarf = LoadRaces()
            .Single(race => race.Id.Value == "dnd5e2014.race.dwarf");

        ToolProficiencyChoice choice =
            Assert.IsType<ToolProficiencyChoice>(dwarf.ToolProficiencyChoice);

        Assert.Equal(1, choice.Count);
        Assert.Empty(choice.ToolFamilyIds);
        Assert.Equal(
            [
                "dnd5e2014.tool.brewers-supplies",
                "dnd5e2014.tool.masons-tools",
                "dnd5e2014.tool.smiths-tools"
            ],
            choice.ToolOptionIds.Select(id => id.Value).Order());
    }

    [Fact]
    public void CanonicalFile_DwarfIsTheOnlyRaceGrantingToolProficiency()
    {
        IReadOnlyList<RaceDefinition> races = LoadRaces();

        Assert.Equal(
            ["dnd5e2014.race.dwarf"],
            races
                .Where(race =>
                    race.ToolProficiencyIds.Count > 0 ||
                    race.ToolProficiencyChoice is not null)
                .Select(race => race.Id.Value)
                .Order());
    }

    [Fact]
    public void CanonicalFile_GrantsToolProficienciesToExactlyTwoSubclasses()
    {
        // Battle Master's Student of War (p.73) chooses one artisan's
        // tools; Assassin's Bonus Proficiencies (p.97) fixes two kits.
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.Equal(
            ["dnd5e2014.subclass.assassin", "dnd5e2014.subclass.battle-master"],
            subclasses
                .Where(subclass =>
                    subclass.ToolProficiencyIds.Count > 0 ||
                    subclass.ToolProficiencyChoice is not null)
                .Select(subclass => subclass.Id.Value)
                .Order());

        SubclassDefinition assassin = subclasses
            .Single(subclass =>
                subclass.Id.Value == "dnd5e2014.subclass.assassin");

        Assert.Null(assassin.ToolProficiencyChoice);
        Assert.Equal(
            ["dnd5e2014.tool.disguise-kit", "dnd5e2014.tool.poisoners-kit"],
            assassin.ToolProficiencyIds.Select(id => id.Value).Order());
    }

    [Fact]
    public void CanonicalFile_FixedGrantsAreNotModelledAsChoices()
    {
        // Druid's herbalism kit and Rogue's thieves' tools are single
        // named tools, not one-option choices.
        IReadOnlyList<ClassDefinition> classes = LoadClasses();

        foreach (string id in
            new[] { "dnd5e2014.class.druid", "dnd5e2014.class.rogue" })
        {
            ClassDefinition @class =
                classes.Single(candidate => candidate.Id.Value == id);

            Assert.Null(@class.ToolProficiencyChoice);
            Assert.Single(@class.ToolProficiencyIds);
        }
    }

    private static IReadOnlyList<ClassDefinition> LoadClasses() =>
        ClassDefinitionLoader.LoadFromFile(CanonicalPath("classes.json"));

    private static IReadOnlyList<SubclassDefinition> LoadSubclasses() =>
        SubclassDefinitionLoader.LoadFromFile(CanonicalPath("subclasses.json"));

    private static IReadOnlyList<RaceDefinition> LoadRaces() =>
        RaceDefinitionLoader.LoadFromFile(CanonicalPath("races.json"));

    private static string CanonicalPath(string fileName) =>
        Path.Combine(FindRepositoryRoot(), "Data", "dnd5e2014", fileName);

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }
}
