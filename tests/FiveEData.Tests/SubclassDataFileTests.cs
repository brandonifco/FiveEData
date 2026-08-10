using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Assassinate;
using FiveEData.Rules.Classes.Auras;
using FiveEData.Rules.Classes.AwakenedMind;
using FiveEData.Rules.Classes.BeguilingDefenses;
using FiveEData.Rules.Classes.CreateThrall;
using FiveEData.Rules.Classes.DarkDelirium;
using FiveEData.Rules.Classes.DeathStrike;
using FiveEData.Rules.Classes.EntropicWard;
using FiveEData.Rules.Classes.FeyPresence;
using FiveEData.Rules.Classes.Frenzy;
using FiveEData.Rules.Classes.InfiltrationExpertise;
using FiveEData.Rules.Classes.IntimidatingPresence;
using FiveEData.Rules.Classes.MistyEscape;
using FiveEData.Rules.Classes.SecondStoryWork;
using FiveEData.Rules.Classes.ThoughtShield;
using FiveEData.Rules.Classes.BendLuck;
using FiveEData.Rules.Classes.WrathOfTheStorm;
using FiveEData.Rules.Classes.ThunderboltStrike;
using FiveEData.Rules.Classes.ShadowStep;
using FiveEData.Rules.Classes.ImprovedCritical;
using FiveEData.Rules.Classes.HurlThroughHell;
using FiveEData.Rules.Classes.CircleForms;
using FiveEData.Rules.Classes.CombatSuperiority;
using FiveEData.Rules.Classes.DiscipleOfTheElements;
using FiveEData.Rules.Classes.DivineStrike;
using FiveEData.Rules.Classes.DraconicResilience;
using FiveEData.Rules.Classes.MagicalSecrets;
using FiveEData.Rules.Classes.Portent;
using FiveEData.Rules.Classes.Serialization;
using FiveEData.Rules.Classes.WardingFlare;
using FiveEData.Rules.Common;
using FiveEData.Rules.Equipment.Armor;

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

    private static readonly string[] ExpectedWarlockSubclassIds =
    [
        "dnd5e2014.subclass.the-archfey",
        "dnd5e2014.subclass.the-fiend",
        "dnd5e2014.subclass.the-great-old-one"
    ];

    private static readonly string[] ExpectedDruidSubclassIds =
    [
        "dnd5e2014.subclass.circle-of-the-land",
        "dnd5e2014.subclass.circle-of-the-moon"
    ];

    private static readonly string[] ExpectedRangerSubclassIds =
    [
        "dnd5e2014.subclass.hunter",
        "dnd5e2014.subclass.beast-master"
    ];

    private static readonly string[] ExpectedPaladinSubclassIds =
    [
        "dnd5e2014.subclass.oath-of-devotion",
        "dnd5e2014.subclass.oath-of-the-ancients",
        "dnd5e2014.subclass.oath-of-vengeance"
    ];

    private static readonly string[] ExpectedSorcererSubclassIds =
    [
        "dnd5e2014.subclass.draconic-bloodline",
        "dnd5e2014.subclass.wild-magic"
    ];

    private static readonly string[] ExpectedSubclassIds =
    [
        .. ExpectedFighterSubclassIds,
        .. ExpectedBarbarianSubclassIds,
        .. ExpectedMonkSubclassIds,
        .. ExpectedRogueSubclassIds,
        .. ExpectedBardSubclassIds,
        .. ExpectedWizardSubclassIds,
        .. ExpectedClericSubclassIds,
        .. ExpectedWarlockSubclassIds,
        .. ExpectedDruidSubclassIds,
        .. ExpectedRangerSubclassIds,
        .. ExpectedPaladinSubclassIds,
        .. ExpectedSorcererSubclassIds
    ];

    [Fact]
    public void CanonicalFile_ContainsExactSubclassClosure()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.Equal(40, subclasses.Count);
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
            ["dnd5e2014.class.warlock"] = 1,
            ["dnd5e2014.class.druid"] = 2,
            ["dnd5e2014.class.ranger"] = 3,
            ["dnd5e2014.class.paladin"] = 3,
            ["dnd5e2014.class.sorcerer"] = 1,
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
    public void CanonicalFile_EveryWarlockSubclassReferencesTheWarlockClass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses.Where(
                subclass => ExpectedWarlockSubclassIds.Contains(subclass.Id.Value)),
            subclass => Assert.Equal(
                "dnd5e2014.class.warlock",
                subclass.ClassId.Value));
    }

    [Fact]
    public void CanonicalFile_EveryDruidSubclassReferencesTheDruidClass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses.Where(
                subclass => ExpectedDruidSubclassIds.Contains(subclass.Id.Value)),
            subclass => Assert.Equal(
                "dnd5e2014.class.druid",
                subclass.ClassId.Value));
    }

    [Fact]
    public void CanonicalFile_EveryRangerSubclassReferencesTheRangerClass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses.Where(
                subclass => ExpectedRangerSubclassIds.Contains(subclass.Id.Value)),
            subclass => Assert.Equal(
                "dnd5e2014.class.ranger",
                subclass.ClassId.Value));
    }

    [Fact]
    public void CanonicalFile_EveryPaladinSubclassReferencesThePaladinClass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses.Where(
                subclass => ExpectedPaladinSubclassIds.Contains(subclass.Id.Value)),
            subclass => Assert.Equal(
                "dnd5e2014.class.paladin",
                subclass.ClassId.Value));
    }

    [Fact]
    public void CanonicalFile_EverySorcererSubclassReferencesTheSorcererClass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses.Where(
                subclass => ExpectedSorcererSubclassIds.Contains(subclass.Id.Value)),
            subclass => Assert.Equal(
                "dnd5e2014.class.sorcerer",
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

        MagicalSecretsProgressionDetail magicalSecretsProgression =
            collegeOfLore.MagicalSecretsProgression
            ?? throw new InvalidOperationException(
                "Expected College of Lore to have a Magical Secrets " +
                "progression.");
        Assert.Equal(
            [(6, 2)],
            magicalSecretsProgression.SpellsKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.SpellsKnown)));
        Assert.False(magicalSecretsProgression.CountsAgainstSpellsKnown);
    }

    // Additional Magical Secrets reuses the Bard's own MagicalSecrets shape,
    // and CountsAgainstSpellsKnown is exactly what separates them: the
    // subclass spells "don't count against the number of bard spells you
    // know", while the class feature's are included in the Spells Known
    // column. Same type, opposite value.
    [Fact]
    public void CanonicalFile_CollegeOfLoreMagicalSecretsDoNotCountAgainstSpellsKnown()
    {
        SubclassDefinition collegeOfLore = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.college-of-lore");

        MagicalSecretsProgressionDetail subclassProgression =
            collegeOfLore.MagicalSecretsProgression
            ?? throw new InvalidOperationException(
                "Expected College of Lore to have a Magical Secrets " +
                "progression.");

        ClassDefinition bard = ClassDefinitionLoader
            .LoadFromFile(
                Path.Combine(
                    FindRepositoryRoot(),
                    "Data",
                    "dnd5e2014",
                    "classes.json"))
            .Single(@class => @class.Id.Value == "dnd5e2014.class.bard");

        MagicalSecretsProgressionDetail classProgression =
            bard.MagicalSecretsProgression
            ?? throw new InvalidOperationException(
                "Expected Bard to have a Magical Secrets progression.");

        Assert.False(subclassProgression.CountsAgainstSpellsKnown);
        Assert.True(classProgression.CountsAgainstSpellsKnown);
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

        PortentProgressionDetail portentProgression =
            divination.PortentProgression
            ?? throw new InvalidOperationException(
                "Expected School of Divination to have a Portent " +
                "progression.");
        Assert.Equal(
            [(2, 2), (14, 3)],
            portentProgression.ForetellingRollsByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant => (grant.CharacterLevel, grant.ForetellingRolls)));
        Assert.True(portentProgression.OncePerTurn);
        Assert.True(portentProgression.RecoversOnLongRest);
    }

    // Portent and Greater Portent are two separately cited features that
    // drive one resource: Greater Portent's whole mechanical content is "you
    // roll three d20s for your Portent feature, rather than two". The 14th
    // level row therefore comes from Greater Portent, and both RuleIds stay
    // in LevelFeatures.
    [Fact]
    public void CanonicalFile_GreaterPortentSuppliesTheFourteenthLevelPortentRow()
    {
        SubclassDefinition divination = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.school-of-divination");

        PortentProgressionDetail portentProgression =
            divination.PortentProgression
            ?? throw new InvalidOperationException(
                "Expected School of Divination to have a Portent " +
                "progression.");

        int portentLevel = divination.LevelFeatures
            .Single(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.portent")
            .Level;
        int greaterPortentLevel = divination.LevelFeatures
            .Single(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.greater-portent")
            .Level;

        Assert.Equal(
            [portentLevel, greaterPortentLevel],
            portentProgression.ForetellingRollsByLevel
                .Select(grant => grant.CharacterLevel)
                .OrderBy(level => level));
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

        DivineStrikeProgressionDetail lifeDivineStrike =
            life.DivineStrikeProgression
            ?? throw new InvalidOperationException(
                "Expected a Divine Strike progression.");
        Assert.Equal(
            "dnd5e2014.damage-type.radiant",
            lifeDivineStrike.FixedDamageTypeId?.Value);
        Assert.Null(lifeDivineStrike.ChoosableDamageTypeIds);
        Assert.False(lifeDivineStrike.MatchesWeaponDamageType);
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
        Assert.Null(light.DivineStrikeProgression);
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

        DivineStrikeProgressionDetail natureDivineStrike =
            nature.DivineStrikeProgression
            ?? throw new InvalidOperationException(
                "Expected a Divine Strike progression.");
        Assert.Null(natureDivineStrike.FixedDamageTypeId);
        Assert.Equal(
            [
                "dnd5e2014.damage-type.cold",
                "dnd5e2014.damage-type.fire",
                "dnd5e2014.damage-type.lightning"
            ],
            natureDivineStrike.ChoosableDamageTypeIds
                ?.Select(id => id.Value)
                .ToArray());
        Assert.False(natureDivineStrike.MatchesWeaponDamageType);
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

        DivineStrikeProgressionDetail tempestDivineStrike =
            tempest.DivineStrikeProgression
            ?? throw new InvalidOperationException(
                "Expected a Divine Strike progression.");
        Assert.Equal(
            "dnd5e2014.damage-type.thunder",
            tempestDivineStrike.FixedDamageTypeId?.Value);
        Assert.Null(tempestDivineStrike.ChoosableDamageTypeIds);
        Assert.False(tempestDivineStrike.MatchesWeaponDamageType);
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

        DivineStrikeProgressionDetail trickeryDivineStrike =
            trickery.DivineStrikeProgression
            ?? throw new InvalidOperationException(
                "Expected a Divine Strike progression.");
        Assert.Equal(
            "dnd5e2014.damage-type.poison",
            trickeryDivineStrike.FixedDamageTypeId?.Value);
        Assert.Null(trickeryDivineStrike.ChoosableDamageTypeIds);
        Assert.False(trickeryDivineStrike.MatchesWeaponDamageType);
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

        DivineStrikeProgressionDetail warDivineStrike =
            war.DivineStrikeProgression
            ?? throw new InvalidOperationException(
                "Expected a Divine Strike progression.");
        Assert.Null(warDivineStrike.FixedDamageTypeId);
        Assert.Null(warDivineStrike.ChoosableDamageTypeIds);
        Assert.True(warDivineStrike.MatchesWeaponDamageType);
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

    [Fact]
    public void CanonicalFile_SharesDivineStrikeDamageByLevelShapeAcrossAllFiveDomains()
    {
        string[] divineStrikeDomainIds =
        [
            "dnd5e2014.subclass.life-domain",
            "dnd5e2014.subclass.nature-domain",
            "dnd5e2014.subclass.tempest-domain",
            "dnd5e2014.subclass.trickery-domain",
            "dnd5e2014.subclass.war-domain"
        ];

        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        foreach (string domainId in divineStrikeDomainIds)
        {
            SubclassDefinition domain = GetSubclass(subclasses, domainId);

            DivineStrikeProgressionDetail divineStrike =
                domain.DivineStrikeProgression
                ?? throw new InvalidOperationException(
                    $"Expected '{domainId}' to have a Divine Strike " +
                    "progression.");

            Assert.Equal(
                [(8, 1, 8), (14, 2, 8)],
                divineStrike.DamageByLevel
                    .Select(grant => (
                        grant.CharacterLevel,
                        grant.Damage.Count,
                        grant.Damage.Sides))
                    .ToArray());
        }
    }

    [Fact]
    public void CanonicalFile_OnlyTheFiveDivineStrikeDomainsHaveADivineStrikeProgression()
    {
        string[] divineStrikeDomainIds =
        [
            "dnd5e2014.subclass.life-domain",
            "dnd5e2014.subclass.nature-domain",
            "dnd5e2014.subclass.tempest-domain",
            "dnd5e2014.subclass.trickery-domain",
            "dnd5e2014.subclass.war-domain"
        ];

        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        IEnumerable<string> otherSubclassIds = subclasses
            .Select(subclass => subclass.Id.Value)
            .Where(id => !divineStrikeDomainIds.Contains(id));

        foreach (string id in otherSubclassIds)
        {
            SubclassDefinition subclass = GetSubclass(subclasses, id);

            Assert.Null(subclass.DivineStrikeProgression);
        }
    }

    [Fact]
    public void CanonicalFile_OnlyCircleOfTheMoonHasACircleFormsProgression()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        IEnumerable<string> otherSubclassIds = subclasses
            .Select(subclass => subclass.Id.Value)
            .Where(
                id => id != "dnd5e2014.subclass.circle-of-the-moon");

        foreach (string id in otherSubclassIds)
        {
            SubclassDefinition subclass = GetSubclass(subclasses, id);

            Assert.Null(subclass.CircleFormsProgression);
        }
    }

    [Fact]
    public void CanonicalFile_OnlyOathOfDevotionAndOathOfTheAncientsHaveAuras()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        IEnumerable<string> otherSubclassIds = subclasses
            .Select(subclass => subclass.Id.Value)
            .Where(
                id => id != "dnd5e2014.subclass.oath-of-devotion" &&
                    id != "dnd5e2014.subclass.oath-of-the-ancients");

        foreach (string id in otherSubclassIds)
        {
            SubclassDefinition subclass = GetSubclass(subclasses, id);

            Assert.Null(subclass.AuraOfDevotion);
            Assert.Null(subclass.AuraOfWarding);
        }
    }

    [Fact]
    public void CanonicalFile_PreservesTheArchfeyMechanics()
    {
        SubclassDefinition archfey = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.the-archfey");

        Assert.Equal("The Archfey", archfey.Name);
        Assert.Equal(1, archfey.ChosenAtLevel);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.archfey-expanded-spell-list",
                "dnd5e2014.class-rule.fey-presence",
                "dnd5e2014.class-rule.misty-escape",
                "dnd5e2014.class-rule.beguiling-defenses",
                "dnd5e2014.class-rule.dark-delirium"
            ],
            archfey.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(archfey.Sources);
        Assert.Equal(108, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesTheFiendMechanics()
    {
        SubclassDefinition fiend = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.the-fiend");

        Assert.Equal("The Fiend", fiend.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.fiend-expanded-spell-list",
                "dnd5e2014.class-rule.dark-ones-blessing",
                "dnd5e2014.class-rule.dark-ones-own-luck",
                "dnd5e2014.class-rule.fiendish-resilience",
                "dnd5e2014.class-rule.hurl-through-hell"
            ],
            fiend.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(fiend.Sources);
        Assert.Equal(109, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesTheGreatOldOneMechanics()
    {
        SubclassDefinition greatOldOne = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.the-great-old-one");

        Assert.Equal("The Great Old One", greatOldOne.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.great-old-one-expanded-spell-list",
                "dnd5e2014.class-rule.awakened-mind",
                "dnd5e2014.class-rule.entropic-ward",
                "dnd5e2014.class-rule.thought-shield",
                "dnd5e2014.class-rule.create-thrall"
            ],
            greatOldOne.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(greatOldOne.Sources);
        Assert.Equal(109, source.Page);
    }

    [Fact]
    public void CanonicalFile_KeepsExpandedSpellListDistinctPerPatronDespiteSharedName()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        IEnumerable<string> expandedSpellListRuleIds = subclasses
            .Where(subclass => ExpectedWarlockSubclassIds.Contains(subclass.Id.Value))
            .SelectMany(subclass => subclass.LevelFeatures)
            .Select(feature => feature.FeatureRuleId.Value)
            .Where(id => id.EndsWith("expanded-spell-list", StringComparison.Ordinal));

        Assert.Equal(3, expandedSpellListRuleIds.Distinct().Count());
    }

    [Fact]
    public void CanonicalFile_PreservesCircleOfTheLandMechanics()
    {
        SubclassDefinition land = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.circle-of-the-land");

        Assert.Equal("Circle of the Land", land.Name);
        Assert.Equal(2, land.ChosenAtLevel);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.land-bonus-cantrip",
                "dnd5e2014.class-rule.natural-recovery",
                "dnd5e2014.class-rule.circle-spells",
                "dnd5e2014.class-rule.circle-spells",
                "dnd5e2014.class-rule.lands-stride",
                "dnd5e2014.class-rule.circle-spells",
                "dnd5e2014.class-rule.circle-spells",
                "dnd5e2014.class-rule.natures-ward",
                "dnd5e2014.class-rule.natures-sanctuary"
            ],
            land.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(land.Sources);
        Assert.Equal(68, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesCircleSpellsAtEachGrantLevel()
    {
        SubclassDefinition land = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.circle-of-the-land");

        int[] expectedLevels = [3, 5, 7, 9];

        int[] actualLevels = land.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.circle-spells")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesCircleOfTheMoonMechanics()
    {
        SubclassDefinition moon = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.circle-of-the-moon");

        Assert.Equal("Circle of the Moon", moon.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.combat-wild-shape",
                "dnd5e2014.class-rule.circle-forms",
                "dnd5e2014.class-rule.primal-strike",
                "dnd5e2014.class-rule.circle-forms",
                "dnd5e2014.class-rule.elemental-wild-shape",
                "dnd5e2014.class-rule.thousand-forms"
            ],
            moon.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(moon.Sources);
        Assert.Equal(69, source.Page);

        CircleFormsProgressionDetail circleFormsProgression =
            moon.CircleFormsProgression
            ?? throw new InvalidOperationException(
                "Expected Circle of the Moon to have a Circle Forms " +
                "progression.");

        Assert.Equal(
            [(2, 1.0), (6, 2.0), (9, 3.0), (12, 4.0), (15, 5.0), (18, 6.0)],
            circleFormsProgression.MaxChallengeRatingByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant =>
                        (grant.CharacterLevel, grant.MaxChallengeRating)));
    }

    [Fact]
    public void CanonicalFile_PreservesCircleFormsAtInitialAndImprovementLevels()
    {
        SubclassDefinition moon = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.circle-of-the-moon");

        int[] expectedLevels = [2, 6];

        int[] actualLevels = moon.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.circle-forms")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesHunterMechanics()
    {
        SubclassDefinition hunter = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.hunter");

        Assert.Equal("Hunter", hunter.Name);
        Assert.Equal(3, hunter.ChosenAtLevel);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.hunters-prey",
                "dnd5e2014.class-rule.defensive-tactics",
                "dnd5e2014.class-rule.multiattack",
                "dnd5e2014.class-rule.superior-hunters-defense"
            ],
            hunter.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(hunter.Sources);
        Assert.Equal(93, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesBeastMasterMechanics()
    {
        SubclassDefinition beastMaster = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.beast-master");

        Assert.Equal("Beast Master", beastMaster.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.rangers-companion",
                "dnd5e2014.class-rule.exceptional-training",
                "dnd5e2014.class-rule.bestial-fury",
                "dnd5e2014.class-rule.share-spells"
            ],
            beastMaster.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(beastMaster.Sources);
        Assert.Equal(93, source.Page);
    }

    [Fact]
    public void CanonicalFile_PreservesOathOfDevotionMechanics()
    {
        SubclassDefinition devotion = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.oath-of-devotion");

        Assert.Equal("Oath of Devotion", devotion.Name);
        Assert.Equal(3, devotion.ChosenAtLevel);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.oath-of-devotion-spells",
                "dnd5e2014.class-rule.channel-divinity-sacred-weapon",
                "dnd5e2014.class-rule.channel-divinity-turn-the-unholy",
                "dnd5e2014.class-rule.oath-of-devotion-spells",
                "dnd5e2014.class-rule.aura-of-devotion",
                "dnd5e2014.class-rule.oath-of-devotion-spells",
                "dnd5e2014.class-rule.oath-of-devotion-spells",
                "dnd5e2014.class-rule.purity-of-spirit",
                "dnd5e2014.class-rule.oath-of-devotion-spells",
                "dnd5e2014.class-rule.holy-nimbus"
            ],
            devotion.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(devotion.Sources);
        Assert.Equal(85, source.Page);

        AuraOfDevotionDetail auraOfDevotion =
            devotion.AuraOfDevotion
            ?? throw new InvalidOperationException(
                "Expected Oath of Devotion to have an Aura of Devotion.");
        Assert.Equal(10, auraOfDevotion.Range.BaseRangeFeet);
        Assert.Equal(30, auraOfDevotion.Range.ExpandedRangeFeet);
        Assert.Equal(18, auraOfDevotion.Range.ExpandedAtLevel);
        Assert.True(auraOfDevotion.RequiresConsciousness);
        Assert.Null(devotion.AuraOfWarding);
    }

    [Fact]
    public void CanonicalFile_PreservesOathSpellsAtEachGrantLevel()
    {
        SubclassDefinition devotion = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.oath-of-devotion");

        int[] expectedLevels = [3, 5, 9, 13, 17];

        int[] actualLevels = devotion.LevelFeatures
            .Where(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.oath-of-devotion-spells")
            .Select(feature => feature.Level)
            .OrderBy(level => level)
            .ToArray();

        Assert.Equal(expectedLevels, actualLevels);
    }

    [Fact]
    public void CanonicalFile_PreservesOathOfTheAncientsMechanics()
    {
        SubclassDefinition ancients = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.oath-of-the-ancients");

        Assert.Equal("Oath of the Ancients", ancients.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.oath-of-the-ancients-spells",
                "dnd5e2014.class-rule.channel-divinity-natures-wrath",
                "dnd5e2014.class-rule.channel-divinity-turn-the-faithless",
                "dnd5e2014.class-rule.oath-of-the-ancients-spells",
                "dnd5e2014.class-rule.aura-of-warding",
                "dnd5e2014.class-rule.oath-of-the-ancients-spells",
                "dnd5e2014.class-rule.oath-of-the-ancients-spells",
                "dnd5e2014.class-rule.undying-sentinel",
                "dnd5e2014.class-rule.oath-of-the-ancients-spells",
                "dnd5e2014.class-rule.elder-champion"
            ],
            ancients.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(ancients.Sources);
        Assert.Equal(86, source.Page);

        AuraOfWardingDetail auraOfWarding =
            ancients.AuraOfWarding
            ?? throw new InvalidOperationException(
                "Expected Oath of the Ancients to have an Aura of " +
                "Warding.");
        Assert.Equal(10, auraOfWarding.Range.BaseRangeFeet);
        Assert.Equal(30, auraOfWarding.Range.ExpandedRangeFeet);
        Assert.Equal(18, auraOfWarding.Range.ExpandedAtLevel);
        Assert.False(auraOfWarding.RequiresConsciousness);
        Assert.Null(ancients.AuraOfDevotion);
    }

    [Fact]
    public void CanonicalFile_PreservesOathOfVengeanceMechanics()
    {
        SubclassDefinition vengeance = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.oath-of-vengeance");

        Assert.Equal("Oath of Vengeance", vengeance.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.oath-of-vengeance-spells",
                "dnd5e2014.class-rule.channel-divinity-abjure-enemy",
                "dnd5e2014.class-rule.channel-divinity-vow-of-enmity",
                "dnd5e2014.class-rule.oath-of-vengeance-spells",
                "dnd5e2014.class-rule.relentless-avenger",
                "dnd5e2014.class-rule.oath-of-vengeance-spells",
                "dnd5e2014.class-rule.oath-of-vengeance-spells",
                "dnd5e2014.class-rule.soul-of-vengeance",
                "dnd5e2014.class-rule.oath-of-vengeance-spells",
                "dnd5e2014.class-rule.avenging-angel"
            ],
            vengeance.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(vengeance.Sources);
        Assert.Equal(87, source.Page);
    }

    [Fact]
    public void CanonicalFile_KeepsEachOathsSpellsAsDistinctRuleIdsDespiteSharedName()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        IEnumerable<string> oathSpellsRuleIds = subclasses
            .Where(subclass => ExpectedPaladinSubclassIds.Contains(subclass.Id.Value))
            .SelectMany(subclass => subclass.LevelFeatures)
            .Select(feature => feature.FeatureRuleId.Value)
            .Where(id => id.StartsWith("dnd5e2014.class-rule.oath-of-", StringComparison.Ordinal) &&
                id.EndsWith("-spells", StringComparison.Ordinal));

        Assert.Equal(3, oathSpellsRuleIds.Distinct().Count());
    }

    [Fact]
    public void CanonicalFile_PreservesDraconicBloodlineMechanics()
    {
        SubclassDefinition draconicBloodline = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.draconic-bloodline");

        Assert.Equal("Draconic Bloodline", draconicBloodline.Name);
        Assert.Equal(1, draconicBloodline.ChosenAtLevel);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.dragon-ancestor",
                "dnd5e2014.class-rule.draconic-resilience",
                "dnd5e2014.class-rule.elemental-affinity",
                "dnd5e2014.class-rule.dragon-wings",
                "dnd5e2014.class-rule.draconic-presence"
            ],
            draconicBloodline.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(draconicBloodline.Sources);
        Assert.Equal(102, source.Page);

        DraconicResilienceDetail draconicResilience =
            draconicBloodline.DraconicResilience
            ?? throw new InvalidOperationException(
                "Expected Draconic Bloodline to have Draconic Resilience.");
        Assert.Equal(1, draconicResilience.HitPointBonusPerLevel);
        Assert.Equal(
            13,
            draconicResilience.UnarmoredArmorClass.BaseArmorClass);
        Assert.True(
            draconicResilience.UnarmoredArmorClass
                .IncludesDexterityModifier);
        Assert.Null(
            draconicResilience.UnarmoredArmorClass
                .MaximumDexterityModifier);
    }

    // Draconic Resilience's unarmored AC reuses ArmorClassFormula rather than
    // minting a base/dex pair, the same type the Armor catalog already uses.
    // Its hit point bonus is the Hill Dwarf HitPointBonusPerLevel shape.
    [Fact]
    public void CanonicalFile_DraconicResilienceReusesTheArmorClassFormulaShape()
    {
        SubclassDefinition draconicBloodline = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.draconic-bloodline");

        DraconicResilienceDetail draconicResilience =
            draconicBloodline.DraconicResilience
            ?? throw new InvalidOperationException(
                "Expected Draconic Bloodline to have Draconic Resilience.");

        Assert.Equal(
            new ArmorClassFormula(13, includesDexterityModifier: true),
            draconicResilience.UnarmoredArmorClass);
    }

    [Fact]
    public void CanonicalFile_PreservesDraconicBloodlineDualLevelOneFeatures()
    {
        SubclassDefinition draconicBloodline = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.draconic-bloodline");

        Assert.Equal(
            2,
            draconicBloodline.LevelFeatures.Count(feature => feature.Level == 1));
    }

    [Fact]
    public void CanonicalFile_PreservesWildMagicMechanics()
    {
        SubclassDefinition wildMagic = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.wild-magic");

        Assert.Equal("Wild Magic", wildMagic.Name);
        Assert.Equal(
            [
                "dnd5e2014.class-rule.wild-magic-surge",
                "dnd5e2014.class-rule.tides-of-chaos",
                "dnd5e2014.class-rule.bend-luck",
                "dnd5e2014.class-rule.controlled-chaos",
                "dnd5e2014.class-rule.spell-bombardment"
            ],
            wildMagic.LevelFeatures
                .Select(feature => feature.FeatureRuleId.Value)
                .ToArray());

        var source = Assert.Single(wildMagic.Sources);
        Assert.Equal(103, source.Page);
    }

    [Theory]
    [InlineData("dnd5e2014.subclass.eldritch-knight")]
    [InlineData("dnd5e2014.subclass.arcane-trickster")]
    public void CanonicalFile_ThirdCasterSubclassDeclaresExpectedSpellcasting(
        string subclassId)
    {
        SubclassDefinition subclass =
            GetSubclass(LoadSubclasses(), subclassId);

        Assert.Equal(
            "dnd5e2014.spell-slot-progression.third-caster",
            subclass.SpellSlotProgressionId?.Value);
        Assert.Equal(
            "dnd5e2014.ability.intelligence",
            subclass.SpellcastingAbilityId?.Value);
    }

    [Fact]
    public void CanonicalFile_NonCastingSubclassesDeclareNoSpellcasting()
    {
        string[] castingSubclassIds =
        [
            "dnd5e2014.subclass.eldritch-knight",
            "dnd5e2014.subclass.arcane-trickster"
        ];

        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        foreach (
            SubclassDefinition subclass
            in subclasses.Where(
                subclass => !castingSubclassIds.Contains(subclass.Id.Value)))
        {
            Assert.True(
                subclass.SpellSlotProgressionId is null,
                $"{subclass.Id} unexpectedly declares a spell slot " +
                "progression.");
            Assert.True(
                subclass.SpellcastingAbilityId is null,
                $"{subclass.Id} unexpectedly declares a spellcasting " +
                "ability.");
        }
    }

    [Fact]
    public void CanonicalFile_OnlyBattleMasterHasACombatSuperiorityProgression()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        IEnumerable<string> otherSubclassIds = subclasses
            .Select(subclass => subclass.Id.Value)
            .Where(id => id != "dnd5e2014.subclass.battle-master");

        foreach (string id in otherSubclassIds)
        {
            SubclassDefinition subclass = GetSubclass(subclasses, id);

            Assert.Null(subclass.CombatSuperiorityProgression);
        }
    }

    [Fact]
    public void CanonicalFile_PreservesBattleMastersCombatSuperiorityProgression()
    {
        SubclassDefinition battleMaster = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.battle-master");

        var source = Assert.Single(battleMaster.Sources);
        Assert.Equal(73, source.Page);

        CombatSuperiorityProgressionDetail combatSuperiority =
            battleMaster.CombatSuperiorityProgression
            ?? throw new InvalidOperationException(
                "Expected Battle Master to have a Combat Superiority " +
                "progression.");

        Assert.Equal(
            [(3, 3), (7, 5), (10, 7), (15, 9)],
            combatSuperiority.ManeuversKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.ManeuversKnown)));

        Assert.Equal(
            [(3, 4), (7, 5), (15, 6)],
            combatSuperiority.DiceCountByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.DiceCount)));

        Assert.Equal(
            [(3, 8), (10, 10), (18, 12)],
            combatSuperiority.DieSizeByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.Die.Sides)));

        Assert.All(
            combatSuperiority.DieSizeByLevel,
            grant => Assert.Equal(1, grant.Die.Count));
    }

    [Fact]
    public void CanonicalFile_OnlyWayOfTheFourElementsHasADiscipleOfTheElementsProgression()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        IEnumerable<string> otherSubclassIds = subclasses
            .Select(subclass => subclass.Id.Value)
            .Where(
                id => id != "dnd5e2014.subclass.way-of-the-four-elements");

        foreach (string id in otherSubclassIds)
        {
            SubclassDefinition subclass = GetSubclass(subclasses, id);

            Assert.Null(subclass.DiscipleOfTheElementsProgression);
        }
    }

    [Fact]
    public void CanonicalFile_PreservesWayOfTheFourElementsDiscipleOfTheElementsProgression()
    {
        SubclassDefinition wayOfFourElements = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.way-of-the-four-elements");

        var source = Assert.Single(wayOfFourElements.Sources);
        Assert.Equal(80, source.Page);

        DiscipleOfTheElementsProgressionDetail discipleOfTheElements =
            wayOfFourElements.DiscipleOfTheElementsProgression
            ?? throw new InvalidOperationException(
                "Expected Way of the Four Elements to have a Disciple of " +
                "the Elements progression.");

        Assert.Equal(
            [(3, 2), (6, 3), (11, 4), (17, 5)],
            discipleOfTheElements.DisciplinesKnownByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant =>
                        (grant.CharacterLevel, grant.DisciplinesKnown)));

        Assert.Equal(
            [(5, 3), (9, 4), (13, 5), (17, 6)],
            discipleOfTheElements.MaxKiPointsPerSpellByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.MaxKiPoints)));
    }

    [Theory]
    [InlineData("dnd5e2014.subclass.college-of-lore")]
    [InlineData("dnd5e2014.subclass.school-of-divination")]
    [InlineData("dnd5e2014.subclass.draconic-bloodline")]
    public void CanonicalFile_QuantizedSubclassFeaturesAreExclusiveToTheirSubclass(
        string subclassId)
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        SubclassDefinition owner = GetSubclass(subclasses, subclassId);

        bool ownerDeclaresExactlyOne =
            new bool[]
            {
                owner.MagicalSecretsProgression is not null,
                owner.PortentProgression is not null,
                owner.DraconicResilience is not null
            }.Count(declared => declared) == 1;

        Assert.True(ownerDeclaresExactlyOne);

        foreach (SubclassDefinition other in subclasses
            .Where(subclass => subclass.Id.Value != subclassId))
        {
            if (other.Id.Value is "dnd5e2014.subclass.college-of-lore"
                or "dnd5e2014.subclass.school-of-divination"
                or "dnd5e2014.subclass.draconic-bloodline")
            {
                continue;
            }

            Assert.Null(other.MagicalSecretsProgression);
            Assert.Null(other.PortentProgression);
            Assert.Null(other.DraconicResilience);
        }
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedQuantizedSubclassFeatures()
    {
        SubclassCatalog catalog = Dnd5e2014Ruleset.Instance.Subclasses;

        PortentProgressionDetail portentProgression =
            catalog.Get(new SubclassId("dnd5e2014.subclass.school-of-divination"))
                .PortentProgression
            ?? throw new InvalidOperationException(
                "Expected School of Divination to have a Portent " +
                "progression.");
        Assert.Equal(
            [(2, 2), (14, 3)],
            portentProgression.ForetellingRollsByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(
                    grant => (grant.CharacterLevel, grant.ForetellingRolls)));

        DraconicResilienceDetail draconicResilience =
            catalog.Get(new SubclassId("dnd5e2014.subclass.draconic-bloodline"))
                .DraconicResilience
            ?? throw new InvalidOperationException(
                "Expected Draconic Bloodline to have Draconic Resilience.");
        Assert.Equal(1, draconicResilience.HitPointBonusPerLevel);

        MagicalSecretsProgressionDetail magicalSecretsProgression =
            catalog.Get(new SubclassId("dnd5e2014.subclass.college-of-lore"))
                .MagicalSecretsProgression
            ?? throw new InvalidOperationException(
                "Expected College of Lore to have a Magical Secrets " +
                "progression.");
        Assert.False(magicalSecretsProgression.CountsAgainstSpellsKnown);
    }

    [Fact]
    public void CanonicalFile_PreservesChampionCriticalHitThresholds()
    {
        SubclassDefinition champion = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.champion");

        ImprovedCriticalProgressionDetail improvedCriticalProgression =
            champion.ImprovedCriticalProgression
            ?? throw new InvalidOperationException(
                "Expected Champion to have an Improved Critical " +
                "progression.");
        Assert.Equal(
            [(3, 19), (15, 18)],
            improvedCriticalProgression.MinimumRollByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Select(grant => (grant.CharacterLevel, grant.MinimumRoll)));
    }

    // Improved Critical and Superior Critical are two cited features driving
    // one threshold, the Portent shape. Uniquely, the value falls as level
    // rises — 19 at 3rd, 18 at 15th — because a lower threshold is the
    // improvement. Every other progression in the codebase ascends.
    [Fact]
    public void CanonicalFile_ChampionCriticalHitThresholdFallsAsLevelRises()
    {
        SubclassDefinition champion = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.champion");

        ImprovedCriticalProgressionDetail improvedCriticalProgression =
            champion.ImprovedCriticalProgression
            ?? throw new InvalidOperationException(
                "Expected Champion to have an Improved Critical " +
                "progression.");

        CriticalHitThresholdGrant[] ordered = improvedCriticalProgression
            .MinimumRollByLevel
            .OrderBy(grant => grant.CharacterLevel)
            .ToArray();

        Assert.True(ordered[^1].MinimumRoll < ordered[0].MinimumRoll);

        int improvedCriticalLevel = champion.LevelFeatures
            .Single(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.improved-critical")
            .Level;
        int superiorCriticalLevel = champion.LevelFeatures
            .Single(
                feature => feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.superior-critical")
            .Level;

        Assert.Equal(
            [improvedCriticalLevel, superiorCriticalLevel],
            ordered.Select(grant => grant.CharacterLevel));
    }

    [Fact]
    public void CanonicalFile_PreservesWayOfShadowShadowStep()
    {
        SubclassDefinition wayOfShadow = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.way-of-shadow");

        ShadowStepDetail shadowStep =
            wayOfShadow.ShadowStep
            ?? throw new InvalidOperationException(
                "Expected Way of Shadow to have Shadow Step.");
        Assert.Equal(60, shadowStep.TeleportRangeFeet);
        Assert.True(shadowStep.GrantsAdvantageOnNextMeleeAttack);
    }

    [Fact]
    public void CanonicalFile_PreservesFiendHurlThroughHell()
    {
        SubclassDefinition fiend = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.the-fiend");

        HurlThroughHellDetail hurlThroughHell =
            fiend.HurlThroughHell
            ?? throw new InvalidOperationException(
                "Expected the Fiend to have Hurl Through Hell.");
        Assert.Equal(10, hurlThroughHell.Damage.Count);
        Assert.Equal(10, hurlThroughHell.Damage.Sides);
        Assert.Equal(
            "dnd5e2014.damage-type.psychic",
            hurlThroughHell.DamageTypeId.Value);
        Assert.True(hurlThroughHell.ExemptsFiends);
        Assert.True(hurlThroughHell.RecoversOnLongRest);
    }

    [Fact]
    public void CanonicalFile_PreservesTempestDomainTierBScalars()
    {
        SubclassDefinition tempest = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.tempest-domain");

        WrathOfTheStormDetail wrathOfTheStorm =
            tempest.WrathOfTheStorm
            ?? throw new InvalidOperationException(
                "Expected Tempest Domain to have Wrath of the Storm.");
        Assert.Equal(5, wrathOfTheStorm.TriggerRangeFeet);
        Assert.Equal(2, wrathOfTheStorm.Damage.Count);
        Assert.Equal(8, wrathOfTheStorm.Damage.Sides);
        Assert.Equal(
            [
                "dnd5e2014.damage-type.lightning",
                "dnd5e2014.damage-type.thunder"
            ],
            wrathOfTheStorm.ChoosableDamageTypeIds
                .Select(id => id.Value)
                .ToArray());
        Assert.Equal(
            "dnd5e2014.ability.dexterity",
            wrathOfTheStorm.SavingThrowAbilityId.Value);
        Assert.True(wrathOfTheStorm.HalfDamageOnSuccessfulSave);
        Assert.Equal(
            "dnd5e2014.ability.wisdom",
            wrathOfTheStorm.UsesPerRest.AbilityId.Value);
        Assert.True(wrathOfTheStorm.UsesPerRest.RecoversOnLongRest);

        ThunderboltStrikeDetail thunderboltStrike =
            tempest.ThunderboltStrike
            ?? throw new InvalidOperationException(
                "Expected Tempest Domain to have Thunderbolt Strike.");
        Assert.Equal(10, thunderboltStrike.PushDistanceFeet);
        Assert.Equal(
            "dnd5e2014.creature-size.large",
            thunderboltStrike.MaximumTargetSizeId.Value);
    }

    // Light Domain's Bonus Cantrip names one specific cantrip, so it is a
    // real grant. Circle of the Land's identically-named feature is "one
    // additional druid cantrip of your choice" — an open choice with no
    // spell to point at — and stays declined.
    [Fact]
    public void CanonicalFile_PreservesLightDomainBonusCantrip()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        SubclassDefinition light =
            GetSubclass(subclasses, "dnd5e2014.subclass.light-domain");

        SpellGrant grant = Assert.Single(light.InnateSpellGrants);
        Assert.Equal("dnd5e2014.spell.light", grant.GrantedSpellId.Value);
        Assert.Equal(1, grant.MinimumCharacterLevel);
        Assert.Equal(SpellGrantFrequency.AtWill, grant.Frequency);

        Assert.Empty(
            GetSubclass(subclasses, "dnd5e2014.subclass.circle-of-the-land")
                .InnateSpellGrants);
    }

    // Thousand Forms is a Circle of the Moon feature, not Circle of the
    // Land's — the two circles' 6th/10th/14th features sit in adjacent
    // columns on the same page, which is easy to misread.
    [Fact]
    public void CanonicalFile_PreservesCircleOfTheMoonThousandForms()
    {
        SubclassDefinition circleOfTheMoon = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.circle-of-the-moon");

        SpellGrant grant =
            Assert.Single(circleOfTheMoon.InnateSpellGrants);
        Assert.Equal(
            "dnd5e2014.spell.alter-self",
            grant.GrantedSpellId.Value);
        Assert.Equal(14, grant.MinimumCharacterLevel);
        Assert.Equal(SpellGrantFrequency.AtWill, grant.Frequency);
        Assert.Null(grant.CastAtSpellLevel);
    }

    [Fact]
    public void CanonicalFile_InnateSpellGrantsAreExclusiveToTheirSubclass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        string[] owners =
        [
            "dnd5e2014.subclass.light-domain",
            "dnd5e2014.subclass.circle-of-the-moon"
        ];

        Assert.All(
            subclasses.Where(
                subclass => !owners.Contains(subclass.Id.Value)),
            subclass => Assert.Empty(subclass.InnateSpellGrants));
    }

    [Fact]
    public void CanonicalFile_PreservesLightDomainWardingFlare()
    {
        SubclassDefinition light = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.light-domain");

        WardingFlareDetail wardingFlare =
            light.WardingFlare
            ?? throw new InvalidOperationException(
                "Expected Light Domain to have Warding Flare.");
        Assert.Equal(30, wardingFlare.TriggerRangeFeet);
        Assert.Equal(
            "dnd5e2014.ability.wisdom",
            wardingFlare.UsesPerRest.AbilityId.Value);
        Assert.True(wardingFlare.UsesPerRest.RecoversOnLongRest);
    }

    [Fact]
    public void CanonicalFile_PreservesWarDomainWarPriest()
    {
        SubclassDefinition war = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.war-domain");

        AbilityModifierUsesGrant warPriest =
            war.WarPriestUsesPerRest
            ?? throw new InvalidOperationException(
                "Expected War Domain to have War Priest.");
        Assert.Equal("dnd5e2014.ability.wisdom", warPriest.AbilityId.Value);
        Assert.True(warPriest.RecoversOnLongRest);
    }

    [Theory]
    [InlineData("dnd5e2014.subclass.champion")]
    [InlineData("dnd5e2014.subclass.way-of-shadow")]
    [InlineData("dnd5e2014.subclass.the-fiend")]
    [InlineData("dnd5e2014.subclass.tempest-domain")]
    [InlineData("dnd5e2014.subclass.light-domain")]
    [InlineData("dnd5e2014.subclass.war-domain")]
    public void CanonicalFile_TierBSubclassScalarsAreExclusiveToTheirSubclass(
        string subclassId)
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        string[] owners =
        [
            "dnd5e2014.subclass.champion",
            "dnd5e2014.subclass.way-of-shadow",
            "dnd5e2014.subclass.the-fiend",
            "dnd5e2014.subclass.tempest-domain",
            "dnd5e2014.subclass.light-domain",
            "dnd5e2014.subclass.war-domain"
        ];

        Assert.Contains(subclassId, owners);

        foreach (SubclassDefinition other in subclasses
            .Where(subclass => !owners.Contains(subclass.Id.Value)))
        {
            Assert.Null(other.ImprovedCriticalProgression);
            Assert.Null(other.ShadowStep);
            Assert.Null(other.HurlThroughHell);
            Assert.Null(other.WrathOfTheStorm);
            Assert.Null(other.ThunderboltStrike);
            Assert.Null(other.WardingFlare);
            Assert.Null(other.WarPriestUsesPerRest);
        }
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedTierBSubclassScalars()
    {
        SubclassCatalog catalog = Dnd5e2014Ruleset.Instance.Subclasses;

        Assert.Equal(
            60,
            (catalog.Get(new SubclassId("dnd5e2014.subclass.way-of-shadow"))
                .ShadowStep
                ?? throw new InvalidOperationException(
                    "Expected Way of Shadow to have Shadow Step."))
                .TeleportRangeFeet);
        Assert.Equal(
            10,
            (catalog.Get(new SubclassId("dnd5e2014.subclass.tempest-domain"))
                .ThunderboltStrike
                ?? throw new InvalidOperationException(
                    "Expected Tempest Domain to have Thunderbolt Strike."))
                .PushDistanceFeet);
        Assert.Equal(
            18,
            (catalog.Get(new SubclassId("dnd5e2014.subclass.champion"))
                .ImprovedCriticalProgression
                ?? throw new InvalidOperationException(
                    "Expected Champion to have an Improved Critical " +
                    "progression."))
                .MinimumRollByLevel
                .OrderBy(grant => grant.CharacterLevel)
                .Last()
                .MinimumRoll);
    }

    [Fact]
    public void CanonicalFile_PreservesSubclassFixedResourceCosts()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.Equal(
            2,
            GetSubclass(subclasses, "dnd5e2014.subclass.way-of-shadow")
                .ShadowArtsKiCost);
        Assert.Equal(
            3,
            GetSubclass(
                subclasses,
                "dnd5e2014.subclass.way-of-the-open-hand")
                .QuiveringPalmKiCost);
        Assert.Equal(
            5,
            GetSubclass(subclasses, "dnd5e2014.subclass.draconic-bloodline")
                .DraconicPresenceSorceryPointCost);

        BendLuckDetail bendLuck =
            GetSubclass(subclasses, "dnd5e2014.subclass.wild-magic").BendLuck
            ?? throw new InvalidOperationException(
                "Expected Wild Magic to have Bend Luck.");
        Assert.Equal(2, bendLuck.SorceryPointCost);
        Assert.Equal(1, bendLuck.Die.Count);
        Assert.Equal(4, bendLuck.Die.Sides);
    }

    // Quivering Palm is a Way of the Open Hand feature, not a Monk class
    // feature: the Monk table's 17th-level row reads "Monastic Tradition
    // feature", and the class LevelFeatures list has no entry for it. Its ki
    // cost therefore lives on the subclass.
    [Fact]
    public void CanonicalFile_QuiveringPalmBelongsToWayOfTheOpenHand()
    {
        SubclassDefinition openHand = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.way-of-the-open-hand");

        Assert.Contains(
            openHand.LevelFeatures,
            feature => feature.Level == 17 &&
                feature.FeatureRuleId.Value ==
                    "dnd5e2014.class-rule.quivering-palm");
        Assert.Equal(3, openHand.QuiveringPalmKiCost);
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedSubclassResourceCosts()
    {
        SubclassCatalog catalog = Dnd5e2014Ruleset.Instance.Subclasses;

        Assert.Equal(
            2,
            catalog.Get(new SubclassId("dnd5e2014.subclass.way-of-shadow"))
                .ShadowArtsKiCost);
        Assert.Equal(
            4,
            (catalog.Get(new SubclassId("dnd5e2014.subclass.wild-magic"))
                .BendLuck
                ?? throw new InvalidOperationException(
                    "Expected Wild Magic to have Bend Luck.")).Die.Sides);
    }

    private static SubclassDefinition GetSubclass(
        IReadOnlyList<SubclassDefinition> subclasses,
        string id)
    {
        return subclasses.Single(subclass => subclass.Id.Value == id);
    }

    [Fact]
    public void CanonicalFile_PreservesBerserkerFeatures()
    {
        SubclassDefinition berserker = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.path-of-the-berserker");

        FrenzyDetail frenzy =
            berserker.Frenzy
            ?? throw new InvalidOperationException(
                "Expected Path of the Berserker to have Frenzy.");
        Assert.True(frenzy.GrantsBonusActionMeleeAttack);
        Assert.Equal(1, frenzy.ExhaustionLevelsWhenRageEnds);

        Assert.Equal(
            [
                "dnd5e2014.condition.charmed",
                "dnd5e2014.condition.frightened"
            ],
            berserker.MindlessRageImmuneConditionIds
                .Select(id => id.Value)
                .ToArray());

        IntimidatingPresenceDetail intimidatingPresence =
            berserker.IntimidatingPresence
            ?? throw new InvalidOperationException(
                "Expected Path of the Berserker to have Intimidating " +
                "Presence.");
        Assert.Equal(30, intimidatingPresence.RangeFeet);
        Assert.Equal(
            "dnd5e2014.ability.wisdom",
            intimidatingPresence.SavingThrowAbilityId.Value);
        Assert.Equal(
            "dnd5e2014.condition.frightened",
            intimidatingPresence.ImposedConditionId.Value);
        Assert.Equal(
            NextTurnDurationTrigger.EndOfYourNextTurn,
            intimidatingPresence.ConditionDurationTrigger);
    }

    [Fact]
    public void CanonicalFile_PreservesThiefSecondStoryWork()
    {
        SubclassDefinition thief =
            GetSubclass(LoadSubclasses(), "dnd5e2014.subclass.thief");

        SecondStoryWorkDetail secondStoryWork =
            thief.SecondStoryWork
            ?? throw new InvalidOperationException(
                "Expected Thief to have Second-Story Work.");
        Assert.True(secondStoryWork.ClimbingCostsNoExtraMovement);
        Assert.True(
            secondStoryWork.AddsDexterityModifierToRunningJumpDistance);
    }

    [Fact]
    public void CanonicalFile_PreservesAssassinFeatures()
    {
        SubclassDefinition assassin =
            GetSubclass(LoadSubclasses(), "dnd5e2014.subclass.assassin");

        AssassinateDetail assassinate =
            assassin.Assassinate
            ?? throw new InvalidOperationException(
                "Expected Assassin to have Assassinate.");
        Assert.True(
            assassinate.GrantsAdvantageAgainstCreaturesThatHaveNotActed);
        Assert.True(assassinate.HitsAgainstSurprisedCreaturesAreCritical);

        InfiltrationExpertiseDetail infiltrationExpertise =
            assassin.InfiltrationExpertise
            ?? throw new InvalidOperationException(
                "Expected Assassin to have Infiltration Expertise.");
        Assert.Equal(7, infiltrationExpertise.RequiredDays);
        Assert.Equal(25, infiltrationExpertise.CostGoldPieces);

        Assert.Equal(3, assassin.ImpostorRequiredStudyHours);

        DeathStrikeDetail deathStrike =
            assassin.DeathStrike
            ?? throw new InvalidOperationException(
                "Expected Assassin to have Death Strike.");
        Assert.Equal(
            "dnd5e2014.ability.constitution",
            deathStrike.SavingThrowAbilityId.Value);
        Assert.Equal(2, deathStrike.DamageMultiplierOnFailedSave);
        Assert.True(deathStrike.RequiresSurprisedTarget);
    }

    [Fact]
    public void CanonicalFile_RichFeatureDetailsAreExclusiveToTheirSubclass()
    {
        IReadOnlyList<SubclassDefinition> subclasses = LoadSubclasses();

        Assert.All(
            subclasses.Where(
                subclass => subclass.Id.Value !=
                    "dnd5e2014.subclass.path-of-the-berserker"),
            subclass =>
            {
                Assert.Null(subclass.Frenzy);
                Assert.Empty(subclass.MindlessRageImmuneConditionIds);
                Assert.Null(subclass.IntimidatingPresence);
            });

        Assert.All(
            subclasses.Where(
                subclass => subclass.Id.Value != "dnd5e2014.subclass.thief"),
            subclass => Assert.Null(subclass.SecondStoryWork));

        Assert.All(
            subclasses.Where(
                subclass =>
                    subclass.Id.Value != "dnd5e2014.subclass.assassin"),
            subclass =>
            {
                Assert.Null(subclass.Assassinate);
                Assert.Null(subclass.InfiltrationExpertise);
                Assert.Null(subclass.ImpostorRequiredStudyHours);
                Assert.Null(subclass.DeathStrike);
            });

        Assert.All(
            subclasses.Where(
                subclass =>
                    subclass.Id.Value != "dnd5e2014.subclass.the-archfey"),
            subclass =>
            {
                Assert.Null(subclass.FeyPresence);
                Assert.Null(subclass.MistyEscape);
                Assert.Null(subclass.BeguilingDefenses);
                Assert.Null(subclass.DarkDelirium);
            });

        Assert.All(
            subclasses.Where(
                subclass =>
                    subclass.Id.Value !=
                    "dnd5e2014.subclass.the-great-old-one"),
            subclass =>
            {
                Assert.Null(subclass.AwakenedMind);
                Assert.Null(subclass.EntropicWard);
                Assert.Null(subclass.ThoughtShield);
                Assert.Null(subclass.CreateThrall);
            });
    }

    [Fact]
    public void CanonicalFile_PreservesArchfeyFeatures()
    {
        SubclassDefinition archfey = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.the-archfey");

        FeyPresenceDetail feyPresence =
            archfey.FeyPresence
            ?? throw new InvalidOperationException(
                "Expected The Archfey to have Fey Presence.");
        Assert.Equal(10, feyPresence.AreaSizeFeet);
        Assert.Equal(
            "dnd5e2014.ability.wisdom",
            feyPresence.SavingThrowAbilityId.Value);
        Assert.Equal(
            [
                "dnd5e2014.condition.charmed",
                "dnd5e2014.condition.frightened"
            ],
            feyPresence.ChoosableConditionIds
                .Select(id => id.Value)
                .ToArray());
        Assert.Equal(
            NextTurnDurationTrigger.EndOfYourNextTurn,
            feyPresence.ConditionDurationTrigger);
        Assert.True(feyPresence.RecoversOnShortRest);

        MistyEscapeDetail mistyEscape =
            archfey.MistyEscape
            ?? throw new InvalidOperationException(
                "Expected The Archfey to have Misty Escape.");
        Assert.Equal(60, mistyEscape.TeleportRangeFeet);
        Assert.True(mistyEscape.GrantsInvisibility);
        Assert.True(mistyEscape.RecoversOnShortRest);

        BeguilingDefensesDetail beguilingDefenses =
            archfey.BeguilingDefenses
            ?? throw new InvalidOperationException(
                "Expected The Archfey to have Beguiling Defenses.");
        Assert.Equal(
            "dnd5e2014.condition.charmed",
            beguilingDefenses.ImmuneConditionId.Value);
        Assert.Equal(
            "dnd5e2014.ability.wisdom",
            beguilingDefenses.ReflectionSavingThrowAbilityId.Value);
        Assert.Equal(1, beguilingDefenses.ReflectionDurationMinutes);

        DarkDeliriumDetail darkDelirium =
            archfey.DarkDelirium
            ?? throw new InvalidOperationException(
                "Expected The Archfey to have Dark Delirium.");
        Assert.Equal(60, darkDelirium.RangeFeet);
        Assert.Equal(
            "dnd5e2014.ability.wisdom",
            darkDelirium.SavingThrowAbilityId.Value);
        Assert.Equal(
            [
                "dnd5e2014.condition.charmed",
                "dnd5e2014.condition.frightened"
            ],
            darkDelirium.ChoosableConditionIds
                .Select(id => id.Value)
                .ToArray());
        Assert.Equal(1, darkDelirium.DurationMinutes);
        Assert.True(darkDelirium.RequiresConcentration);
        Assert.True(darkDelirium.RecoversOnShortRest);
    }

    [Fact]
    public void CanonicalFile_PreservesGreatOldOneFeatures()
    {
        SubclassDefinition greatOldOne = GetSubclass(
            LoadSubclasses(),
            "dnd5e2014.subclass.the-great-old-one");

        AwakenedMindDetail awakenedMind =
            greatOldOne.AwakenedMind
            ?? throw new InvalidOperationException(
                "Expected The Great Old One to have Awakened Mind.");
        Assert.Equal(30, awakenedMind.TelepathyRangeFeet);

        EntropicWardDetail entropicWard =
            greatOldOne.EntropicWard
            ?? throw new InvalidOperationException(
                "Expected The Great Old One to have Entropic Ward.");
        Assert.True(entropicWard.ImposesDisadvantageOnTriggeringAttackRoll);
        Assert.True(entropicWard.GrantsAdvantageOnNextAttackRollIfMissed);
        Assert.Equal(
            NextTurnDurationTrigger.EndOfYourNextTurn,
            entropicWard.AdvantageDurationTrigger);
        Assert.True(entropicWard.RecoversOnShortRest);

        ThoughtShieldDetail thoughtShield =
            greatOldOne.ThoughtShield
            ?? throw new InvalidOperationException(
                "Expected The Great Old One to have Thought Shield.");
        Assert.True(thoughtShield.BlocksTelepathicReading);
        Assert.Equal(
            "dnd5e2014.damage-type.psychic",
            thoughtShield.ResistedDamageTypeId.Value);
        Assert.True(thoughtShield.ReflectsDamageToAttacker);

        CreateThrallDetail createThrall =
            greatOldOne.CreateThrall
            ?? throw new InvalidOperationException(
                "Expected The Great Old One to have Create Thrall.");
        Assert.True(createThrall.RequiresIncapacitatedTarget);
        Assert.Equal(
            "dnd5e2014.condition.charmed",
            createThrall.ImposedConditionId.Value);
        Assert.True(createThrall.GrantsTelepathyWhileOnSamePlane);
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
