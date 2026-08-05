using FiveEData.Rules.Creatures.Races;
using FiveEData.Rules.Creatures.Races.Serialization;

namespace FiveEData.Tests;

public sealed class SubraceDataFileTests
{
    private static readonly string[] ExpectedSubraceIds =
    [
        "dnd5e2014.subrace.hill-dwarf",
        "dnd5e2014.subrace.mountain-dwarf",
        "dnd5e2014.subrace.high-elf",
        "dnd5e2014.subrace.wood-elf",
        "dnd5e2014.subrace.dark-elf",
        "dnd5e2014.subrace.lightfoot-halfling",
        "dnd5e2014.subrace.stout-halfling",
        "dnd5e2014.subrace.forest-gnome",
        "dnd5e2014.subrace.rock-gnome"
    ];

    [Fact]
    public void CanonicalFile_ContainsExactSubraceClosure()
    {
        IReadOnlyList<SubraceDefinition> subraces = LoadSubraces();

        Assert.Equal(9, subraces.Count);
        Assert.Equal(
            ExpectedSubraceIds.OrderBy(id => id, StringComparer.Ordinal),
            subraces
                .Select(subrace => subrace.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_EverySubraceReferencesARealRace()
    {
        IReadOnlyList<SubraceDefinition> subraces = LoadSubraces();

        HashSet<string> raceIds =
            RaceDefinitionLoader.LoadFromFile(
                    Path.Combine(
                        FindRepositoryRoot(),
                        "Data",
                        "dnd5e2014",
                        "races.json"))
                .Select(race => race.Id.Value)
                .ToHashSet();

        Assert.All(
            subraces,
            subrace => Assert.Contains(subrace.RaceId.Value, raceIds));
    }

    [Fact]
    public void CanonicalFile_OnlyWoodElfOverridesSpeed()
    {
        IReadOnlyList<SubraceDefinition> subraces = LoadSubraces();

        SubraceDefinition woodElf =
            GetSubrace(subraces, "dnd5e2014.subrace.wood-elf");

        Assert.Equal(35, woodElf.Speed?.Feet);

        Assert.All(
            subraces.Where(
                subrace => subrace.Id.Value != "dnd5e2014.subrace.wood-elf"),
            subrace => Assert.Null(subrace.Speed));
    }

    [Fact]
    public void CanonicalFile_PreservesHillDwarfMechanics()
    {
        SubraceDefinition hillDwarf =
            GetSubrace(LoadSubraces(), "dnd5e2014.subrace.hill-dwarf");

        Assert.Equal("Hill Dwarf", hillDwarf.Name);
        Assert.Equal("dnd5e2014.race.dwarf", hillDwarf.RaceId.Value);

        RaceAbilityScoreIncrease increase =
            Assert.Single(hillDwarf.AbilityScoreIncreases);
        Assert.Equal("dnd5e2014.ability.wisdom", increase.AbilityId.Value);
        Assert.Equal(1, increase.Bonus);

        Assert.Equal(
            "dnd5e2014.race-rule.dwarven-toughness",
            Assert.Single(hillDwarf.TraitRuleIds).Value);

        var source = Assert.Single(hillDwarf.Sources);
        Assert.Equal(20, source.Page);
        Assert.Equal("Chapter 2: Races", source.Section);
    }

    [Fact]
    public void CanonicalFile_PreservesHighElfAdditionalLanguageChoice()
    {
        SubraceDefinition highElf =
            GetSubrace(LoadSubraces(), "dnd5e2014.subrace.high-elf");

        Assert.Equal(1, highElf.AdditionalLanguageChoiceCount);
        Assert.Equal(
            [
                "dnd5e2014.race-rule.elf-weapon-training",
                "dnd5e2014.race-rule.high-elf-cantrip"
            ],
            highElf.TraitRuleIds.Select(id => id.Value).ToArray());
    }

    [Fact]
    public void CanonicalFile_PreservesDarkElfSuperiorDarkvision()
    {
        SubraceDefinition darkElf =
            GetSubrace(LoadSubraces(), "dnd5e2014.subrace.dark-elf");

        Assert.DoesNotContain(
            darkElf.TraitRuleIds,
            id => id.Value == "dnd5e2014.race-rule.darkvision");
        Assert.Contains(
            darkElf.TraitRuleIds,
            id => id.Value == "dnd5e2014.race-rule.superior-darkvision");
    }

    private static SubraceDefinition GetSubrace(
        IReadOnlyList<SubraceDefinition> subraces,
        string id)
    {
        return subraces.Single(subrace => subrace.Id.Value == id);
    }

    private static IReadOnlyList<SubraceDefinition> LoadSubraces()
    {
        return SubraceDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "subraces.json"));
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
