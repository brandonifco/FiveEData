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

    private static readonly string[] ExpectedWizardSubclassIds =
    [
        "dnd5e2014.subclass.school-of-abjuration",
        "dnd5e2014.subclass.school-of-conjuration",
        "dnd5e2014.subclass.school-of-divination",
        "dnd5e2014.subclass.school-of-enchantment",
        "dnd5e2014.subclass.school-of-evocation",
        "dnd5e2014.subclass.school-of-illusion",
        "dnd5e2014.subclass.school-of-necromancy",
        "dnd5e2014.subclass.school-of-transmutation"
    ];

    private static readonly string[] ExpectedClericSubclassIds =
    [
        "dnd5e2014.subclass.knowledge-domain",
        "dnd5e2014.subclass.life-domain",
        "dnd5e2014.subclass.light-domain",
        "dnd5e2014.subclass.nature-domain",
        "dnd5e2014.subclass.tempest-domain",
        "dnd5e2014.subclass.trickery-domain",
        "dnd5e2014.subclass.war-domain"
    ];

    private static readonly string[] ExpectedSubclassIds =
    [
        .. ExpectedFighterSubclassIds,
        .. ExpectedBarbarianSubclassIds,
        .. ExpectedMonkSubclassIds,
        .. ExpectedRogueSubclassIds,
        .. ExpectedBardSubclassIds,
        .. ExpectedWizardSubclassIds,
        .. ExpectedClericSubclassIds
    ];

    [Fact]
    public void CanonicalFile_ContainsExactSubclassClosure()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.Equal(28, subclasses.Count);
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

    private static readonly IReadOnlyDictionary<string, int> ExpectedChosenAtLevelByClassId =
        new Dictionary<string, int>
        {
            ["dnd5e2014.class.fighter"] = 3,
            ["dnd5e2014.class.barbarian"] = 3,
            ["dnd5e2014.class.monk"] = 3,
            ["dnd5e2014.class.rogue"] = 3,
            ["dnd5e2014.class.bard"] = 3,
            ["dnd5e2014.class.wizard"] = 2,
            ["dnd5e2014.class.cleric"] = 1,
        };

    [Fact]
    public void CanonicalFile_EverySubclassIsChosenAtItsClassesExpectedLevel()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses,
            subclass => Assert.Equal(
                ExpectedChosenAtLevelByClassId[subclass.ClassId.Value],
                subclass.ChosenAtLevel));
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
    public void CanonicalFile_EveryWizardSubclassReferencesTheWizardClass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses.Where(
                subclass => ExpectedWizardSubclassIds.Contains(subclass.Id.Value)),
            subclass => Assert.Equal(
                "dnd5e2014.class.wizard",
                subclass.ClassId.Value));
    }

    [Fact]
    public void CanonicalFile_EveryClericSubclassReferencesTheClericClass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses.Where(
                subclass => ExpectedClericSubclassIds.Contains(subclass.Id.Value)),
            subclass => Assert.Equal(
                "dnd5e2014.class.cleric",
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
        Assert.Equal(54, source.Page);
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

    [Fact]
    public void CanonicalFile_PreservesSchoolOfAbjurationMechanics()
    {
        SubclassDefinition abjuration = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.school-of-abjuration");

        Assert.Equal("School of Abjuration", abjuration.Name);
        Assert.Equal(2, abjuration.ChosenAtLevel);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.abjuration-savant",
                "dnd5e2014.class-rule.arcane-ward",
                "dnd5e2014.class-rule.projected-ward",
                "dnd5e2014.class-rule.improved-abjuration",
                "dnd5e2014.class-rule.spell-resistance"
            ],
            abjuration.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(abjuration.Sources);
        Assert.Equal(115, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesSchoolOfConjurationMechanics()
    {
        SubclassDefinition conjuration = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.school-of-conjuration");

        Assert.Equal("School of Conjuration", conjuration.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.conjuration-savant",
                "dnd5e2014.class-rule.minor-conjuration",
                "dnd5e2014.class-rule.benign-transposition",
                "dnd5e2014.class-rule.focused-conjuration",
                "dnd5e2014.class-rule.durable-summons"
            ],
            conjuration.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(conjuration.Sources);
        Assert.Equal(116, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesSchoolOfDivinationMechanics()
    {
        SubclassDefinition divination = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.school-of-divination");

        Assert.Equal("School of Divination", divination.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.divination-savant",
                "dnd5e2014.class-rule.portent",
                "dnd5e2014.class-rule.expert-divination",
                "dnd5e2014.class-rule.the-third-eye",
                "dnd5e2014.class-rule.greater-portent"
            ],
            divination.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(divination.Sources);
        Assert.Equal(116, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesSchoolOfEnchantmentMechanics()
    {
        SubclassDefinition enchantment = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.school-of-enchantment");

        Assert.Equal("School of Enchantment", enchantment.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.enchantment-savant",
                "dnd5e2014.class-rule.hypnotic-gaze",
                "dnd5e2014.class-rule.instinctive-charm",
                "dnd5e2014.class-rule.split-enchantment",
                "dnd5e2014.class-rule.alter-memories"
            ],
            enchantment.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(enchantment.Sources);
        Assert.Equal(117, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesSchoolOfEvocationMechanics()
    {
        SubclassDefinition evocation = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.school-of-evocation");

        Assert.Equal("School of Evocation", evocation.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.evocation-savant",
                "dnd5e2014.class-rule.sculpt-spells",
                "dnd5e2014.class-rule.potent-cantrip",
                "dnd5e2014.class-rule.empowered-evocation",
                "dnd5e2014.class-rule.overchannel"
            ],
            evocation.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(evocation.Sources);
        Assert.Equal(117, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesSchoolOfIllusionMechanics()
    {
        SubclassDefinition illusion = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.school-of-illusion");

        Assert.Equal("School of Illusion", illusion.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.illusion-savant",
                "dnd5e2014.class-rule.improved-minor-illusion",
                "dnd5e2014.class-rule.malleable-illusions",
                "dnd5e2014.class-rule.illusory-self",
                "dnd5e2014.class-rule.illusory-reality"
            ],
            illusion.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(illusion.Sources);
        Assert.Equal(118, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesSchoolOfNecromancyMechanics()
    {
        SubclassDefinition necromancy = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.school-of-necromancy");

        Assert.Equal("School of Necromancy", necromancy.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.necromancy-savant",
                "dnd5e2014.class-rule.grim-harvest",
                "dnd5e2014.class-rule.undead-thralls",
                "dnd5e2014.class-rule.inured-to-undeath",
                "dnd5e2014.class-rule.command-undead"
            ],
            necromancy.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(necromancy.Sources);
        Assert.Equal(118, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesSchoolOfTransmutationMechanics()
    {
        SubclassDefinition transmutation = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.school-of-transmutation");

        Assert.Equal("School of Transmutation", transmutation.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.transmutation-savant",
                "dnd5e2014.class-rule.minor-alchemy",
                "dnd5e2014.class-rule.transmuters-stone",
                "dnd5e2014.class-rule.shapechanger",
                "dnd5e2014.class-rule.master-transmuter"
            ],
            transmutation.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(transmutation.Sources);
        Assert.Equal(119, source.Page);
    }

    [Fact]
    public void CanonicalFile_EverySchoolSavantFeatureIsItsOwnDistinctRuleId()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        IEnumerable<string> savantRuleIds = subclasses
            .Where(subclass => ExpectedWizardSubclassIds.Contains(subclass.Id.Value))
            .SelectMany(subclass => subclass.LevelFeatures)
            .Select(feature => feature.FeatureRuleId.Value)
            .Where(id => id.EndsWith("-savant", StringComparison.Ordinal));

        Assert.Equal(8, savantRuleIds.Distinct().Count());
    }

    [Fact]
    public void CanonicalFile_PreservesKnowledgeDomainMechanics()
    {
        SubclassDefinition knowledge = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.knowledge-domain");

        Assert.Equal("Knowledge Domain", knowledge.Name);
        Assert.Equal(1, knowledge.ChosenAtLevel);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.blessings-of-knowledge",
                "dnd5e2014.class-rule.channel-divinity-knowledge-of-the-ages",
                "dnd5e2014.class-rule.channel-divinity-read-thoughts",
                "dnd5e2014.class-rule.potent-spellcasting",
                "dnd5e2014.class-rule.visions-of-the-past"
            ],
            knowledge.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(knowledge.Sources);
        Assert.Equal(59, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesLifeDomainMechanics()
    {
        SubclassDefinition life = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.life-domain");

        Assert.Equal("Life Domain", life.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.life-bonus-proficiency",
                "dnd5e2014.class-rule.disciple-of-life",
                "dnd5e2014.class-rule.channel-divinity-preserve-life",
                "dnd5e2014.class-rule.blessed-healer",
                "dnd5e2014.class-rule.life-divine-strike",
                "dnd5e2014.class-rule.supreme-healing"
            ],
            life.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(life.Sources);
        Assert.Equal(60, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesLightDomainMechanics()
    {
        SubclassDefinition light = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.light-domain");

        Assert.Equal("Light Domain", light.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.light-bonus-cantrip",
                "dnd5e2014.class-rule.warding-flare",
                "dnd5e2014.class-rule.channel-divinity-radiance-of-the-dawn",
                "dnd5e2014.class-rule.improved-flare",
                "dnd5e2014.class-rule.potent-spellcasting",
                "dnd5e2014.class-rule.corona-of-light"
            ],
            light.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(light.Sources);
        Assert.Equal(60, source.Page);
    }

    [Fact]
    public void CanonicalFile_SharesPotentSpellcastingRuleIdBetweenKnowledgeAndLight()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        SubclassDefinition knowledge = GetSubclass(
            subclasses, "dnd5e2014.subclass.knowledge-domain");
        SubclassDefinition light = GetSubclass(
            subclasses, "dnd5e2014.subclass.light-domain");

        Assert.Contains(
            knowledge.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.potent-spellcasting");
        Assert.Contains(
            light.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.potent-spellcasting");
    }

    [Fact]
    public void CanonicalFile_PreservesNatureDomainMechanics()
    {
        SubclassDefinition nature = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.nature-domain");

        Assert.Equal("Nature Domain", nature.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.acolyte-of-nature",
                "dnd5e2014.class-rule.nature-bonus-proficiency",
                "dnd5e2014.class-rule.channel-divinity-charm-animals-and-plants",
                "dnd5e2014.class-rule.dampen-elements",
                "dnd5e2014.class-rule.nature-divine-strike",
                "dnd5e2014.class-rule.master-of-nature"
            ],
            nature.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(nature.Sources);
        Assert.Equal(61, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesTempestDomainMechanics()
    {
        SubclassDefinition tempest = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.tempest-domain");

        Assert.Equal("Tempest Domain", tempest.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.martial-and-heavy-armor-proficiency",
                "dnd5e2014.class-rule.wrath-of-the-storm",
                "dnd5e2014.class-rule.channel-divinity-destructive-wrath",
                "dnd5e2014.class-rule.thunderbolt-strike",
                "dnd5e2014.class-rule.tempest-divine-strike",
                "dnd5e2014.class-rule.stormborn"
            ],
            tempest.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(tempest.Sources);
        Assert.Equal(62, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesTrickeryDomainMechanics()
    {
        SubclassDefinition trickery = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.trickery-domain");

        Assert.Equal("Trickery Domain", trickery.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.blessing-of-the-trickster",
                "dnd5e2014.class-rule.channel-divinity-invoke-duplicity",
                "dnd5e2014.class-rule.channel-divinity-cloak-of-shadows",
                "dnd5e2014.class-rule.trickery-divine-strike",
                "dnd5e2014.class-rule.improved-duplicity"
            ],
            trickery.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(trickery.Sources);
        Assert.Equal(62, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesWarDomainMechanics()
    {
        SubclassDefinition war = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.war-domain");

        Assert.Equal("War Domain", war.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.martial-and-heavy-armor-proficiency",
                "dnd5e2014.class-rule.war-priest",
                "dnd5e2014.class-rule.channel-divinity-guided-strike",
                "dnd5e2014.class-rule.channel-divinity-war-gods-blessing",
                "dnd5e2014.class-rule.war-divine-strike",
                "dnd5e2014.class-rule.avatar-of-battle"
            ],
            war.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(war.Sources);
        Assert.Equal(63, source.Page);
    }

    [Fact]
    public void CanonicalFile_SharesMartialAndHeavyArmorProficiencyRuleIdBetweenTempestAndWar()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        SubclassDefinition tempest = GetSubclass(
            subclasses, "dnd5e2014.subclass.tempest-domain");
        SubclassDefinition war = GetSubclass(
            subclasses, "dnd5e2014.subclass.war-domain");

        Assert.Contains(
            tempest.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.martial-and-heavy-armor-proficiency");
        Assert.Contains(
            war.LevelFeatures,
            feature => feature.FeatureRuleId.Value ==
                "dnd5e2014.class-rule.martial-and-heavy-armor-proficiency");
    }

    [Fact]
    public void CanonicalFile_KeepsDivineStrikeDistinctPerDomainDespiteSharedName()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        IEnumerable<string> divineStrikeRuleIds = subclasses
            .Where(subclass => ExpectedClericSubclassIds.Contains(subclass.Id.Value))
            .SelectMany(subclass => subclass.LevelFeatures)
            .Select(feature => feature.FeatureRuleId.Value)
            .Where(id => id.EndsWith("divine-strike", StringComparison.Ordinal));

        Assert.Equal(5, divineStrikeRuleIds.Distinct().Count());
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
