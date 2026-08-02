using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Creatures.Skills.Serialization;

namespace FiveEData.Tests;

public sealed class SkillDataFileTests
{
    private const string ExpectedSection =
        "Chapter 7: Using Ability Scores — " +
        "Ability Checks — Skills";

    private static readonly ExpectedSkill[] Expected =
    [
        new(
            "dnd5e2014.skill.acrobatics",
            "Acrobatics",
            "dnd5e2014.ability.dexterity"),
        new(
            "dnd5e2014.skill.animal-handling",
            "Animal Handling",
            "dnd5e2014.ability.wisdom"),
        new(
            "dnd5e2014.skill.arcana",
            "Arcana",
            "dnd5e2014.ability.intelligence"),
        new(
            "dnd5e2014.skill.athletics",
            "Athletics",
            "dnd5e2014.ability.strength"),
        new(
            "dnd5e2014.skill.deception",
            "Deception",
            "dnd5e2014.ability.charisma"),
        new(
            "dnd5e2014.skill.history",
            "History",
            "dnd5e2014.ability.intelligence"),
        new(
            "dnd5e2014.skill.insight",
            "Insight",
            "dnd5e2014.ability.wisdom"),
        new(
            "dnd5e2014.skill.intimidation",
            "Intimidation",
            "dnd5e2014.ability.charisma"),
        new(
            "dnd5e2014.skill.investigation",
            "Investigation",
            "dnd5e2014.ability.intelligence"),
        new(
            "dnd5e2014.skill.medicine",
            "Medicine",
            "dnd5e2014.ability.wisdom"),
        new(
            "dnd5e2014.skill.nature",
            "Nature",
            "dnd5e2014.ability.intelligence"),
        new(
            "dnd5e2014.skill.perception",
            "Perception",
            "dnd5e2014.ability.wisdom"),
        new(
            "dnd5e2014.skill.performance",
            "Performance",
            "dnd5e2014.ability.charisma"),
        new(
            "dnd5e2014.skill.persuasion",
            "Persuasion",
            "dnd5e2014.ability.charisma"),
        new(
            "dnd5e2014.skill.religion",
            "Religion",
            "dnd5e2014.ability.intelligence"),
        new(
            "dnd5e2014.skill.sleight-of-hand",
            "Sleight of Hand",
            "dnd5e2014.ability.dexterity"),
        new(
            "dnd5e2014.skill.stealth",
            "Stealth",
            "dnd5e2014.ability.dexterity"),
        new(
            "dnd5e2014.skill.survival",
            "Survival",
            "dnd5e2014.ability.wisdom")
    ];

    [Fact]
    public void CanonicalFile_ContainsExactlyEighteenSkills()
    {
        IReadOnlyList<SkillDefinition> definitions =
            LoadCanonical();

        Assert.Equal(18, definitions.Count);
        Assert.Equal(
            18,
            definitions
                .Select(definition => definition.Id)
                .Distinct()
                .Count());
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingSkills()
    {
        IReadOnlyDictionary<
            SkillId,
            SkillDefinition> actual =
                LoadCanonical()
                    .ToDictionary(
                        definition => definition.Id);

        foreach (ExpectedSkill expected in Expected)
        {
            SkillDefinition definition =
                actual[new SkillId(expected.Id)];

            Assert.Equal(
                expected.Name,
                definition.Name);
            Assert.Equal(
                expected.AbilityId,
                definition
                    .NormallyAssociatedAbilityId
                    .Value);

            SourceReference source =
                Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(174, source.Page);
            Assert.Equal(
                ExpectedSection,
                source.Section);
        }
    }

    private static IReadOnlyList<SkillDefinition>
        LoadCanonical()
    {
        return SkillDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "skills.json"));
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

    private sealed record ExpectedSkill(
        string Id,
        string Name,
        string AbilityId);
}
