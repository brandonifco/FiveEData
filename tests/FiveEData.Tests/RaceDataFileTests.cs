using FiveEData.Rules.Creatures.Races;
using FiveEData.Rules.Creatures.Races.Serialization;

namespace FiveEData.Tests;

public sealed class RaceDataFileTests
{
    private static readonly string[] ExpectedRaceIds =
    [
        "dnd5e2014.race.dragonborn",
        "dnd5e2014.race.dwarf",
        "dnd5e2014.race.elf",
        "dnd5e2014.race.gnome",
        "dnd5e2014.race.half-elf",
        "dnd5e2014.race.half-orc",
        "dnd5e2014.race.halfling",
        "dnd5e2014.race.human",
        "dnd5e2014.race.tiefling"
    ];

    [Fact]
    public void CanonicalFile_ContainsExactRaceClosure()
    {
        IReadOnlyList<RaceDefinition> races = LoadRaces();

        Assert.Equal(9, races.Count);
        Assert.Equal(
            ExpectedRaceIds.OrderBy(id => id, StringComparer.Ordinal),
            races
                .Select(race => race.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_PreservesDwarfMechanics()
    {
        RaceDefinition dwarf =
            GetRace(LoadRaces(), "dnd5e2014.race.dwarf");

        Assert.Equal("Dwarf", dwarf.Name);
        Assert.Equal("dnd5e2014.creature-size.medium", dwarf.Size.Value);
        Assert.Equal(25, dwarf.Speed.Feet);

        RaceAbilityScoreIncrease increase =
            Assert.Single(dwarf.AbilityScoreIncreases);
        Assert.Equal(
            "dnd5e2014.ability.constitution",
            increase.AbilityId.Value);
        Assert.Equal(2, increase.Bonus);

        Assert.Equal(0, dwarf.ChoosableAbilityScoreIncreaseCount);
        Assert.Equal(
            [
                "dnd5e2014.language.common",
                "dnd5e2014.language.dwarvish"
            ],
            dwarf.LanguageIds.Select(id => id.Value).ToArray());
        Assert.Equal(0, dwarf.AdditionalLanguageChoiceCount);

        Assert.Equal(
            [
                "dnd5e2014.race-rule.darkvision",
                "dnd5e2014.race-rule.dwarven-resilience",
                "dnd5e2014.race-rule.dwarven-combat-training",
                "dnd5e2014.race-rule.dwarf-tool-proficiency",
                "dnd5e2014.race-rule.stonecunning"
            ],
            dwarf.TraitRuleIds.Select(id => id.Value).ToArray());

        var source = Assert.Single(dwarf.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(20, source.Page);
        Assert.Equal("Chapter 2: Races", source.Section);
    }

    [Fact]
    public void CanonicalFile_PreservesHumanHasNoNamedTraits()
    {
        RaceDefinition human =
            GetRace(LoadRaces(), "dnd5e2014.race.human");

        Assert.Equal(6, human.AbilityScoreIncreases.Count);
        Assert.All(
            human.AbilityScoreIncreases,
            increase => Assert.Equal(1, increase.Bonus));
        Assert.Equal(
            6,
            human.AbilityScoreIncreases
                .Select(increase => increase.AbilityId)
                .Distinct()
                .Count());
        Assert.Equal(1, human.AdditionalLanguageChoiceCount);
        Assert.Empty(human.TraitRuleIds);
    }

    [Fact]
    public void CanonicalFile_PreservesHalfElfChoosableAbilityScoreIncrease()
    {
        RaceDefinition halfElf =
            GetRace(LoadRaces(), "dnd5e2014.race.half-elf");

        RaceAbilityScoreIncrease increase =
            Assert.Single(halfElf.AbilityScoreIncreases);
        Assert.Equal("dnd5e2014.ability.charisma", increase.AbilityId.Value);
        Assert.Equal(2, increase.Bonus);
        Assert.Equal(2, halfElf.ChoosableAbilityScoreIncreaseCount);
        Assert.Equal(1, halfElf.AdditionalLanguageChoiceCount);
    }

    [Fact]
    public void CanonicalFile_PreservesHalflingHasNoDarkvision()
    {
        RaceDefinition halfling =
            GetRace(LoadRaces(), "dnd5e2014.race.halfling");

        Assert.DoesNotContain(
            halfling.TraitRuleIds,
            id => id.Value == "dnd5e2014.race-rule.darkvision");
        Assert.Equal("dnd5e2014.creature-size.small", halfling.Size.Value);
    }

    private static RaceDefinition GetRace(
        IReadOnlyList<RaceDefinition> races,
        string id)
    {
        return races.Single(race => race.Id.Value == id);
    }

    private static IReadOnlyList<RaceDefinition> LoadRaces()
    {
        return RaceDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "races.json"));
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
