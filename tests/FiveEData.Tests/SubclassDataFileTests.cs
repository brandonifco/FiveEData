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

    private static readonly string[] ExpectedMonkSubclassIds =
    [
        "dnd5e2014.subclass.way-of-the-open-hand",
        "dnd5e2014.subclass.way-of-shadow",
        "dnd5e2014.subclass.way-of-the-four-elements"
    ];

    private static readonly string[] ExpectedRogueSubclassIds =
    [
        "dnd5e2014.subclass.thief",
        "dnd5e2014.subclass.assassin",
        "dnd5e2014.subclass.arcane-trickster"
    ];

    private static readonly string[] ExpectedBardSubclassIds =
    [
        "dnd5e2014.subclass.college-of-lore",
        "dnd5e2014.subclass.college-of-valor"
    ];

    private static readonly string[] ExpectedSubclassIds =
    [
        .. ExpectedFighterSubclassIds,
        .. ExpectedBarbarianSubclassIds,
        .. ExpectedMonkSubclassIds,
        .. ExpectedRogueSubclassIds,
        .. ExpectedBardSubclassIds
    ];

    [Fact]
    public void CanonicalFile_ContainsExactSubclassClosure()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.Equal(13, subclasses.Count);
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
    public void CanonicalFile_EveryMonkSubclassReferencesTheMonkClass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses.Where(
                subclass => ExpectedMonkSubclassIds.Contains(subclass.Id.Value)),
            subclass => Assert.Equal(
                "dnd5e2014.class.monk",
                subclass.ClassId.Value));
    }

    [Fact]
    public void CanonicalFile_EveryRogueSubclassReferencesTheRogueClass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses.Where(
                subclass => ExpectedRogueSubclassIds.Contains(subclass.Id.Value)),
            subclass => Assert.Equal(
                "dnd5e2014.class.rogue",
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

    [Fact]
    public void CanonicalFile_PreservesWayOfTheOpenHandMechanics()
    {
        SubclassDefinition openHand = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.way-of-the-open-hand");

        Assert.Equal("Way of the Open Hand", openHand.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.open-hand-technique",
                "dnd5e2014.class-rule.wholeness-of-body",
                "dnd5e2014.class-rule.tranquility",
                "dnd5e2014.class-rule.quivering-palm"
            ],
            openHand.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(openHand.Sources);
        Assert.Equal(79, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesWayOfShadowMechanics()
    {
        SubclassDefinition shadow = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.way-of-shadow");

        Assert.Equal("Way of Shadow", shadow.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.shadow-arts",
                "dnd5e2014.class-rule.shadow-step",
                "dnd5e2014.class-rule.cloak-of-shadows",
                "dnd5e2014.class-rule.opportunist"
            ],
            shadow.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(shadow.Sources);
        Assert.Equal(80, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesWayOfTheFourElementsMechanics()
    {
        SubclassDefinition fourElements = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.way-of-the-four-elements");

        Assert.Equal("Way of the Four Elements", fourElements.Name);

        var source = Assert.Single(fourElements.Sources);
        Assert.Equal(80, source.Page);
    }

    [Fact]
    public void CanonicalFile_WayOfTheFourElementsReusesDisciplineRuleIdAtEveryTraditionLevel()
    {
        SubclassDefinition fourElements = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.way-of-the-four-elements");

        int[] expectedLevels = [3, 6, 11, 17];

        int[] actualLevels = fourElements.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.disciple-of-the-elements")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesThiefMechanics()
    {
        SubclassDefinition thief = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.thief");

        Assert.Equal("Thief", thief.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.fast-hands",
                "dnd5e2014.class-rule.second-story-work",
                "dnd5e2014.class-rule.supreme-sneak",
                "dnd5e2014.class-rule.use-magic-device",
                "dnd5e2014.class-rule.thiefs-reflexes"
            ],
            thief.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(thief.Sources);
        Assert.Equal(97, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesThiefDualLevelThreeFeatures()
    {
        SubclassDefinition thief = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.thief");

        Assert.Equal(
            2,
            thief.LevelFeatures.Count(feature => feature.Level == 3));
    }

    [Fact]
    public void CanonicalFile_PreservesAssassinMechanics()
    {
        SubclassDefinition assassin = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.assassin");

        Assert.Equal("Assassin", assassin.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.assassin-bonus-proficiencies",
                "dnd5e2014.class-rule.assassinate",
                "dnd5e2014.class-rule.infiltration-expertise",
                "dnd5e2014.class-rule.impostor",
                "dnd5e2014.class-rule.death-strike"
            ],
            assassin.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(assassin.Sources);
        Assert.Equal(97, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesArcaneTricksterSpellcastingFeature()
    {
        SubclassDefinition arcaneTrickster = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.arcane-trickster");

        Assert.Equal("Arcane Trickster", arcaneTrickster.Name);
        Assert.Contains(
            arcaneTrickster.LevelFeatures,
            feature =>
                feature.Level == 3 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.arcane-trickster-spellcasting");

        var source = Assert.Single(arcaneTrickster.Sources);
        Assert.Equal(97, source.Page);
    }

    [Fact]
    public void CanonicalFile_EveryBardSubclassReferencesTheBardClass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses.Where(
                subclass => ExpectedBardSubclassIds.Contains(subclass.Id.Value)),
            subclass => Assert.Equal(
                "dnd5e2014.class.bard",
                subclass.ClassId.Value));
    }

    [Fact]
    public void CanonicalFile_PreservesCollegeOfLoreMechanics()
    {
        SubclassDefinition collegeOfLore = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.college-of-lore");

        Assert.Equal("College of Lore", collegeOfLore.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.college-of-lore-bonus-proficiencies",
                "dnd5e2014.class-rule.cutting-words",
                "dnd5e2014.class-rule.additional-magical-secrets",
                "dnd5e2014.class-rule.peerless-skill"
            ],
            collegeOfLore.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(collegeOfLore.Sources);
        Assert.Equal(55, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesCollegeOfValorMechanics()
    {
        SubclassDefinition collegeOfValor = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.college-of-valor");

        Assert.Equal("College of Valor", collegeOfValor.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.college-of-valor-bonus-proficiencies",
                "dnd5e2014.class-rule.combat-inspiration",
                "dnd5e2014.class-rule.college-of-valor-extra-attack",
                "dnd5e2014.class-rule.battle-magic"
            ],
            collegeOfValor.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(collegeOfValor.Sources);
        Assert.Equal(55, source.Page);
    }

    [Fact]
    public void CanonicalFile_KeepsCollegeOfValorExtraAttackDistinctFromSharedExtraAttack()
    {
        SubclassDefinition collegeOfValor = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.college-of-valor");

        Assert.DoesNotContain(
            collegeOfValor.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.extra-attack");
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
