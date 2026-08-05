using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Serialization;

namespace FiveEData.Tests;

public sealed class SubclassDataFileTests
{
    private static readonly string[] ExpectedSubclassIds =
    [
        "dnd5e2014.subclass.champion",
        "dnd5e2014.subclass.battle-master",
        "dnd5e2014.subclass.eldritch-knight"
    ];

    [Fact]
    public void CanonicalFile_ContainsExactSubclassClosure()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.Equal(3, subclasses.Count);
        Assert.Equal(
            ExpectedSubclassIds.OrderBy(id => id, StringComparer.Ordinal),
            subclasses
                .Select(subclass => subclass.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_EverySubclassReferencesTheFighterClass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses,
            subclass => Assert.Equal(
                "dnd5e2014.class.fighter",
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
