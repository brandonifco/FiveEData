using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Abilities.Serialization;

namespace FiveEData.Tests;

public sealed class AbilityDataFileTests
{
    private const string ExpectedSection =
        "Chapter 7: Using Ability Scores — " +
        "Ability Scores and Modifiers";

    private static readonly ExpectedAbility[] Expected =
    [
        new(
            "dnd5e2014.ability.strength",
            "Strength"),
        new(
            "dnd5e2014.ability.dexterity",
            "Dexterity"),
        new(
            "dnd5e2014.ability.constitution",
            "Constitution"),
        new(
            "dnd5e2014.ability.intelligence",
            "Intelligence"),
        new(
            "dnd5e2014.ability.wisdom",
            "Wisdom"),
        new(
            "dnd5e2014.ability.charisma",
            "Charisma")
    ];

    [Fact]
    public void CanonicalFile_ContainsExactlySixAbilities()
    {
        IReadOnlyList<AbilityDefinition> definitions =
            LoadCanonical();

        Assert.Equal(6, definitions.Count);
        Assert.Equal(
            6,
            definitions
                .Select(definition => definition.Id)
                .Distinct()
                .Count());
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingAbilities()
    {
        IReadOnlyDictionary<
            AbilityId,
            AbilityDefinition> actual =
                LoadCanonical()
                    .ToDictionary(
                        definition => definition.Id);

        foreach (ExpectedAbility expected in Expected)
        {
            AbilityDefinition definition =
                actual[new AbilityId(expected.Id)];

            Assert.Equal(
                expected.Name,
                definition.Name);

            SourceReference source =
                Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(173, source.Page);
            Assert.Equal(
                ExpectedSection,
                source.Section);
        }
    }

    private static IReadOnlyList<AbilityDefinition>
        LoadCanonical()
    {
        return AbilityDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "abilities.json"));
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

    private sealed record ExpectedAbility(
        string Id,
        string Name);
}
