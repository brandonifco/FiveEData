using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Serialization;

namespace FiveEData.Tests;

public sealed class SubclassDataFileTests
{
    private static readonly string[] ExpectedFighterSubclassIds =
    [
        "dnd5e2014.subclass.champion",
        "dnd5e2014.subclass.battle-master",
        "dnd5e2014.subclass.eldritch-knight"
    ];

    private static readonly string[] ExpectedBarbarianSubclassIds =
    [
        "dnd5e2014.subclass.path-of-the-berserker",
        "dnd5e2014.subclass.path-of-the-totem-warrior"
    ];

    private static readonly string[] ExpectedSubclassIds =
        [.. ExpectedFighterSubclassIds, .. ExpectedBarbarianSubclassIds];

    [Fact]
    public void CanonicalFile_ContainsExactSubclassClosure()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.Equal(5, subclasses.Count);
        Assert.Equal(
            ExpectedSubclassIds.OrderBy(id => id, StringComparer.Ordinal),
            subclasses
                .Select(subclass => subclass.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_EveryFighterSubclassReferencesTheFighterClass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses.Where(
                subclass => ExpectedFighterSubclassIds.Contains(subclass.Id.Value)),
            subclass => Assert.Equal(
                "dnd5e2014.class.fighter",
                subclass.ClassId.Value));
    }

    [Fact]
    public void CanonicalFile_EveryBarbarianSubclassReferencesTheBarbarianClass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses.Where(
                subclass => ExpectedBarbarianSubclassIds.Contains(subclass.Id.Value)),
            subclass => Assert.Equal(
                "dnd5e2014.class.barbarian",
                subclass.ClassId.Value));
    }

    [Fact]
    public void CanonicalFile_EverySubclassIsChosenAtThirdLevel()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses,
            subclass => Assert.Equal(3, subclass.ChosenAtLevel));
    }

    [Fact]
    public void CanonicalFile_PreservesChampionMechanics()
    {
        SubclassDefinition champion =
            GetSubclass(LoadSubclasses(), "dnd5e2014.subclass.champion");

        Assert.Equal("Champion", champion.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.improved-critical",
                "dnd5e2014.class-rule.remarkable-athlete",
                "dnd5e2014.class-rule.additional-fighting-style",
                "dnd5e2014.class-rule.superior-critical",
                "dnd5e2014.class-rule.survivor"
            ],
            champion.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(champion.Sources);
        Assert.Equal(73, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesEldritchKnightSpellcastingFeature()
    {
        SubclassDefinition eldritchKnight = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.eldritch-knight");

        Assert.Contains(
            eldritchKnight.LevelFeatures,
            feature =>
                feature.Level == 3 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.eldritch-knight-spellcasting");
    }

    [Fact]
    public void CanonicalFile_PreservesPathOfTheBerserkerMechanics()
    {
        SubclassDefinition berserker = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.path-of-the-berserker");

        Assert.Equal("Path of the Berserker", berserker.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.frenzy",
                "dnd5e2014.class-rule.mindless-rage",
                "dnd5e2014.class-rule.intimidating-presence",
                "dnd5e2014.class-rule.retaliation"
            ],
            berserker.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(berserker.Sources);
        Assert.Equal(49, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesPathOfTheTotemWarriorMechanics()
    {
        SubclassDefinition totemWarrior = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.path-of-the-totem-warrior");

        Assert.Equal("Path of the Totem Warrior", totemWarrior.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.spirit-seeker",
                "dnd5e2014.class-rule.totem-spirit",
                "dnd5e2014.class-rule.aspect-of-the-beast",
                "dnd5e2014.class-rule.spirit-walker",
                "dnd5e2014.class-rule.totemic-attunement"
            ],
            totemWarrior.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(totemWarrior.Sources);
        Assert.Equal(50, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesTotemWarriorDualLevelThreeFeatures()
    {
        SubclassDefinition totemWarrior = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.path-of-the-totem-warrior");

        Assert.Equal(
            2,
            totemWarrior.LevelFeatures.Count(feature => feature.Level == 3));
    }

    private static SubclassDefinition GetSubclass(
        IReadOnlyList<SubclassDefinition> subclasses,
        string id)
    {
        return subclasses.Single(subclass => subclass.Id.Value == id);
    }

    private static IReadOnlyList<SubclassDefinition> LoadSubclasses()
    {
        return SubclassDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "subclasses.json"));
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
