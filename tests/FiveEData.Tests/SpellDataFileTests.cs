using FiveEData;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Spells;
using FiveEData.Rules.Spells.Serialization;

namespace FiveEData.Tests;

public sealed class SpellDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactBuiltSpellClosure()
    {
        Assert.Equal(361, LoadCanonical().Count);
        Assert.Equal(
            27,
            LoadCanonical().Count(spell => spell.Level == 0));
        Assert.Equal(
            62,
            LoadCanonical().Count(spell => spell.Level == 1));
        Assert.Equal(
            59,
            LoadCanonical().Count(spell => spell.Level == 2));
        Assert.Equal(
            50,
            LoadCanonical().Count(spell => spell.Level == 3));
        Assert.Equal(
            35,
            LoadCanonical().Count(spell => spell.Level == 4));
        Assert.Equal(
            42,
            LoadCanonical().Count(spell => spell.Level == 5));
        Assert.Equal(
            32,
            LoadCanonical().Count(spell => spell.Level == 6));
        Assert.Equal(
            20,
            LoadCanonical().Count(spell => spell.Level == 7));
        Assert.Equal(
            18,
            LoadCanonical().Count(spell => spell.Level == 8));
        Assert.Equal(
            16,
            LoadCanonical().Count(spell => spell.Level == 9));
    }

    // The Spells domain now spans every PHB level, 0 through 9, complete.
    // "Trap the Soul" (named on the Wizard 8th-level class list, p.212)
    // is not a real spell in this printing - it has no description page
    // anywhere in the book (see EighthLevelSpellIdsBuiltSoFar) because it
    // was never actually included in the Spell Descriptions section. The
    // class list entry is a PHB appendix error, not a scanning gap, so
    // 361 is the complete real count, not 362.
    [Fact]
    public void CanonicalFile_SpansEveryPhbSpellLevelCompletely()
    {
        Assert.All(
            LoadCanonical(),
            spell => Assert.InRange(spell.Level, 0, 9));
    }

    // Read off the eight class spell lists on pp.207-210, whose 2nd-level
    // sections hold 22/17/18/8/13/24/12/34 entries; their union is 59.
    [Theory]
    [InlineData("dnd5e2014.class.bard", 22)]
    [InlineData("dnd5e2014.class.cleric", 17)]
    [InlineData("dnd5e2014.class.druid", 18)]
    [InlineData("dnd5e2014.class.paladin", 8)]
    [InlineData("dnd5e2014.class.ranger", 13)]
    [InlineData("dnd5e2014.class.sorcerer", 24)]
    [InlineData("dnd5e2014.class.warlock", 12)]
    [InlineData("dnd5e2014.class.wizard", 34)]
    public void ClassSecondLevelListHasExpectedSize(
        string classId,
        int expectedCount)
    {
        Assert.Equal(
            expectedCount,
            LoadCanonical()
                .Count(spell => spell.Level == 2
                    && spell.AvailableToClassIds
                        .Any(id => id.Value == classId)));
    }

    [Fact]
    public void SecondLevelContainsExactlyThePhbsFiftyNineSpells()
    {
        Assert.Equal(
            [
                "dnd5e2014.spell.aid",
                "dnd5e2014.spell.alter-self",
                "dnd5e2014.spell.animal-messenger",
                "dnd5e2014.spell.arcane-lock",
                "dnd5e2014.spell.augury",
                "dnd5e2014.spell.barkskin",
                "dnd5e2014.spell.beast-sense",
                "dnd5e2014.spell.blindness-deafness",
                "dnd5e2014.spell.blur",
                "dnd5e2014.spell.branding-smite",
                "dnd5e2014.spell.calm-emotions",
                "dnd5e2014.spell.cloud-of-daggers",
                "dnd5e2014.spell.continual-flame",
                "dnd5e2014.spell.cordon-of-arrows",
                "dnd5e2014.spell.crown-of-madness",
                "dnd5e2014.spell.darkness",
                "dnd5e2014.spell.darkvision",
                "dnd5e2014.spell.detect-thoughts",
                "dnd5e2014.spell.enhance-ability",
                "dnd5e2014.spell.enlarge-reduce",
                "dnd5e2014.spell.enthrall",
                "dnd5e2014.spell.find-steed",
                "dnd5e2014.spell.find-traps",
                "dnd5e2014.spell.flame-blade",
                "dnd5e2014.spell.flaming-sphere",
                "dnd5e2014.spell.gentle-repose",
                "dnd5e2014.spell.gust-of-wind",
                "dnd5e2014.spell.heat-metal",
                "dnd5e2014.spell.hold-person",
                "dnd5e2014.spell.invisibility",
                "dnd5e2014.spell.knock",
                "dnd5e2014.spell.lesser-restoration",
                "dnd5e2014.spell.levitate",
                "dnd5e2014.spell.locate-animals-or-plants",
                "dnd5e2014.spell.locate-object",
                "dnd5e2014.spell.magic-mouth",
                "dnd5e2014.spell.magic-weapon",
                "dnd5e2014.spell.melfs-acid-arrow",
                "dnd5e2014.spell.mirror-image",
                "dnd5e2014.spell.misty-step",
                "dnd5e2014.spell.moonbeam",
                "dnd5e2014.spell.nystuls-magic-aura",
                "dnd5e2014.spell.pass-without-trace",
                "dnd5e2014.spell.phantasmal-force",
                "dnd5e2014.spell.prayer-of-healing",
                "dnd5e2014.spell.protection-from-poison",
                "dnd5e2014.spell.ray-of-enfeeblement",
                "dnd5e2014.spell.rope-trick",
                "dnd5e2014.spell.scorching-ray",
                "dnd5e2014.spell.see-invisibility",
                "dnd5e2014.spell.shatter",
                "dnd5e2014.spell.silence",
                "dnd5e2014.spell.spider-climb",
                "dnd5e2014.spell.spike-growth",
                "dnd5e2014.spell.spiritual-weapon",
                "dnd5e2014.spell.suggestion",
                "dnd5e2014.spell.warding-bond",
                "dnd5e2014.spell.web",
                "dnd5e2014.spell.zone-of-truth"
            ],
            LoadCanonical()
                .Where(spell => spell.Level == 2)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Theory]
    [InlineData("dnd5e2014.spell.aid", "Aid", "abjuration", 211)]
    [InlineData("dnd5e2014.spell.augury", "Augury", "divination", 215)]
    [InlineData("dnd5e2014.spell.blur", "Blur", "illusion", 219)]
    [InlineData("dnd5e2014.spell.cloud-of-daggers", "Cloud of Daggers", "conjuration", 222)]
    [InlineData("dnd5e2014.spell.crown-of-madness", "Crown of Madness", "enchantment", 229)]
    [InlineData("dnd5e2014.spell.darkvision", "Darkvision", "transmutation", 230)]
    [InlineData("dnd5e2014.spell.enthrall", "Enthrall", "enchantment", 238)]
    [InlineData("dnd5e2014.spell.flaming-sphere", "Flaming Sphere", "conjuration", 242)]
    [InlineData("dnd5e2014.spell.gentle-repose", "Gentle Repose", "necromancy", 245)]
    [InlineData("dnd5e2014.spell.hold-person", "Hold Person", "enchantment", 251)]
    [InlineData("dnd5e2014.spell.knock", "Knock", "transmutation", 254)]
    [InlineData("dnd5e2014.spell.melfs-acid-arrow", "Melf's Acid Arrow", "evocation", 259)]
    [InlineData("dnd5e2014.spell.misty-step", "Misty Step", "conjuration", 260)]
    [InlineData("dnd5e2014.spell.nystuls-magic-aura", "Nystul's Magic Aura", "illusion", 263)]
    [InlineData("dnd5e2014.spell.pass-without-trace", "Pass without Trace", "abjuration", 264)]
    [InlineData("dnd5e2014.spell.protection-from-poison", "Protection from Poison", "abjuration", 270)]
    [InlineData("dnd5e2014.spell.rope-trick", "Rope Trick", "transmutation", 272)]
    [InlineData("dnd5e2014.spell.silence", "Silence", "illusion", 275)]
    [InlineData("dnd5e2014.spell.spiritual-weapon", "Spiritual Weapon", "evocation", 278)]
    [InlineData("dnd5e2014.spell.web", "Web", "conjuration", 287)]
    [InlineData("dnd5e2014.spell.zone-of-truth", "Zone of Truth", "enchantment", 289)]
    public void SecondLevelSpell_HasExpectedNameSchoolAndPage(
        string id,
        string expectedName,
        string expectedSchool,
        int expectedPage)
    {
        SpellDefinition spell = Get(id);

        Assert.Equal(2, spell.Level);
        Assert.Equal(expectedName, spell.Name);
        Assert.Equal(
            $"dnd5e2014.magic-school.{expectedSchool}",
            spell.SchoolId.Value);
        Assert.Equal(expectedPage, Assert.Single(spell.Sources).Page);
    }

    // Arcane Lock and Continual Flame drove the "until dispelled" duration,
    // the first that is neither instantaneous nor a span; Magic Mouth and
    // Glyph of Warding join them - both print "Until dispelled or
    // triggered", which maps to the same flag since it carries no span
    // either. Hallow is a plain "Until dispelled" with no trigger clause.
    // Programmed Illusion joins them too, still costed and still not
    // stated as consumed. Sequester and Simulacrum (both plain "Until
    // dispelled") and Symbol ("Until dispelled or triggered") join at
    // seventh level, all three both costed and consumed. Continual Flame
    // carries a third PHB cost phrasing, "ruby dust worth 50 gp", with no
    // "at least". Drawmij's Instant Summons and Magic Jar both break the
    // "always consumed" pattern the first five held: neither spell's
    // material description uses the word "consumes" (a fresh sapphire is
    // needed each casting for one; the other's container is never
    // described as used up), so MaterialIsConsumed stays false for both
    // rather than being inferred. Imprisonment (9th level) breaks the
    // "always costed" pattern the first eleven held: its cost is printed
    // as "500 gp per Hit Die of the target", a formula rather than a
    // flat figure, so MaterialCostGoldPieces stays null (declined) per
    // the "store what's printed, not derived" rule - the same shape
    // Clone's two-part bundle uses, just triggered by a formula instead
    // of multiple items.
    [Fact]
    public void UntilDispelledSpellsAreMostlyCostedButNotAllConsumed()
    {
        SpellDefinition[] dispelled = LoadCanonical()
            .Where(spell => spell.Duration.IsUntilDispelled)
            .OrderBy(spell => spell.Id.Value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "dnd5e2014.spell.arcane-lock",
                "dnd5e2014.spell.continual-flame",
                "dnd5e2014.spell.drawmijs-instant-summons",
                "dnd5e2014.spell.glyph-of-warding",
                "dnd5e2014.spell.hallow",
                "dnd5e2014.spell.imprisonment",
                "dnd5e2014.spell.magic-jar",
                "dnd5e2014.spell.magic-mouth",
                "dnd5e2014.spell.programmed-illusion",
                "dnd5e2014.spell.sequester",
                "dnd5e2014.spell.simulacrum",
                "dnd5e2014.spell.symbol"
            ],
            dispelled.Select(spell => spell.Id.Value));

        Assert.All(
            dispelled,
            spell =>
            {
                Assert.False(spell.Duration.IsInstantaneous);
                Assert.Null(spell.Duration.Amount);
                Assert.Null(spell.Duration.Unit);
            });

        Assert.Null(
            Get("dnd5e2014.spell.imprisonment")
                .Components.MaterialCostGoldPieces);

        Assert.Equal(
            25,
            Get("dnd5e2014.spell.arcane-lock")
                .Components.MaterialCostGoldPieces);
        Assert.Equal(
            200,
            Get("dnd5e2014.spell.glyph-of-warding")
                .Components.MaterialCostGoldPieces);
        Assert.Equal(
            1000,
            Get("dnd5e2014.spell.hallow")
                .Components.MaterialCostGoldPieces);
        Assert.Equal(
            50,
            Get("dnd5e2014.spell.continual-flame")
                .Components.MaterialCostGoldPieces);
        Assert.Equal(
            1000,
            Get("dnd5e2014.spell.drawmijs-instant-summons")
                .Components.MaterialCostGoldPieces);
        Assert.False(
            Get("dnd5e2014.spell.drawmijs-instant-summons")
                .Components.MaterialIsConsumed);
        Assert.Equal(
            500,
            Get("dnd5e2014.spell.magic-jar")
                .Components.MaterialCostGoldPieces);
        Assert.False(
            Get("dnd5e2014.spell.magic-jar")
                .Components.MaterialIsConsumed);
        Assert.Equal(
            25,
            Get("dnd5e2014.spell.programmed-illusion")
                .Components.MaterialCostGoldPieces);
        Assert.False(
            Get("dnd5e2014.spell.programmed-illusion")
                .Components.MaterialIsConsumed);
        Assert.Equal(
            5000,
            Get("dnd5e2014.spell.sequester")
                .Components.MaterialCostGoldPieces);
        Assert.True(
            Get("dnd5e2014.spell.sequester").Components.MaterialIsConsumed);
        Assert.Equal(
            1500,
            Get("dnd5e2014.spell.simulacrum")
                .Components.MaterialCostGoldPieces);
        Assert.True(
            Get("dnd5e2014.spell.simulacrum").Components.MaterialIsConsumed);
        Assert.Equal(
            1000,
            Get("dnd5e2014.spell.symbol")
                .Components.MaterialCostGoldPieces);
        Assert.True(
            Get("dnd5e2014.spell.symbol").Components.MaterialIsConsumed);
    }

    // Aid's 8 hours, Animal Messenger's 24 and Cordon of Arrows' 8 are flat
    // spans, not the "up to" kind - the second level's first evidence that
    // a long duration need not be dismissible.
    [Fact]
    public void LongFlatDurationsAreNeitherUpToNorConcentration()
    {
        SpellDefinition[] flat =
        [
            Get("dnd5e2014.spell.aid"),
            Get("dnd5e2014.spell.animal-messenger"),
            Get("dnd5e2014.spell.cordon-of-arrows")
        ];

        Assert.All(
            flat,
            spell =>
            {
                Assert.Equal(SpellDurationUnit.Hour, spell.Duration.Unit);
                Assert.False(spell.Duration.IsUpTo);
                Assert.False(spell.Duration.RequiresConcentration);
            });

        Assert.Equal(24, Get("dnd5e2014.spell.animal-messenger")
            .Duration.Amount);
    }

    // The class spell list appendix (pp.207-210) gives a 3rd-level union of
    // 50 spells across the 8 classes, built across four alphabetical
    // batches (A-C, D-H, L-P, R-W) - the same closure convention as
    // FirstLevelContainsExactlyThePhbsSixtyTwoSpells and
    // SecondLevelContainsExactlyThePhbsFiftyNineSpells.
    [Fact]
    public void ThirdLevelContainsExactlyThePhbsFiftySpells()
    {
        Assert.Equal(
            [
                "dnd5e2014.spell.animate-dead",
                "dnd5e2014.spell.aura-of-vitality",
                "dnd5e2014.spell.beacon-of-hope",
                "dnd5e2014.spell.bestow-curse",
                "dnd5e2014.spell.blinding-smite",
                "dnd5e2014.spell.blink",
                "dnd5e2014.spell.call-lightning",
                "dnd5e2014.spell.clairvoyance",
                "dnd5e2014.spell.conjure-animals",
                "dnd5e2014.spell.conjure-barrage",
                "dnd5e2014.spell.counterspell",
                "dnd5e2014.spell.create-food-and-water",
                "dnd5e2014.spell.crusaders-mantle",
                "dnd5e2014.spell.daylight",
                "dnd5e2014.spell.dispel-magic",
                "dnd5e2014.spell.elemental-weapon",
                "dnd5e2014.spell.fear",
                "dnd5e2014.spell.feign-death",
                "dnd5e2014.spell.fireball",
                "dnd5e2014.spell.fly",
                "dnd5e2014.spell.gaseous-form",
                "dnd5e2014.spell.glyph-of-warding",
                "dnd5e2014.spell.haste",
                "dnd5e2014.spell.hunger-of-hadar",
                "dnd5e2014.spell.hypnotic-pattern",
                "dnd5e2014.spell.leomunds-tiny-hut",
                "dnd5e2014.spell.lightning-arrow",
                "dnd5e2014.spell.lightning-bolt",
                "dnd5e2014.spell.magic-circle",
                "dnd5e2014.spell.major-image",
                "dnd5e2014.spell.mass-healing-word",
                "dnd5e2014.spell.meld-into-stone",
                "dnd5e2014.spell.nondetection",
                "dnd5e2014.spell.phantom-steed",
                "dnd5e2014.spell.plant-growth",
                "dnd5e2014.spell.protection-from-energy",
                "dnd5e2014.spell.remove-curse",
                "dnd5e2014.spell.revivify",
                "dnd5e2014.spell.sending",
                "dnd5e2014.spell.sleet-storm",
                "dnd5e2014.spell.slow",
                "dnd5e2014.spell.speak-with-dead",
                "dnd5e2014.spell.speak-with-plants",
                "dnd5e2014.spell.spirit-guardians",
                "dnd5e2014.spell.stinking-cloud",
                "dnd5e2014.spell.tongues",
                "dnd5e2014.spell.vampiric-touch",
                "dnd5e2014.spell.water-breathing",
                "dnd5e2014.spell.water-walk",
                "dnd5e2014.spell.wind-wall"
            ],
            LoadCanonical()
                .Where(spell => spell.Level == 3)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    // Read off the eight class spell lists on pp.207-210, whose 3rd-level
    // sections hold 16/20/13/10/5/20/12/29 entries; their union is 50.
    [Theory]
    [InlineData("dnd5e2014.class.bard", 16)]
    [InlineData("dnd5e2014.class.cleric", 20)]
    [InlineData("dnd5e2014.class.druid", 13)]
    [InlineData("dnd5e2014.class.paladin", 10)]
    [InlineData("dnd5e2014.class.ranger", 5)]
    [InlineData("dnd5e2014.class.sorcerer", 20)]
    [InlineData("dnd5e2014.class.warlock", 12)]
    [InlineData("dnd5e2014.class.wizard", 29)]
    public void ClassThirdLevelListHasExpectedSize(
        string classId,
        int expectedCount)
    {
        Assert.Equal(
            expectedCount,
            LoadCanonical()
                .Count(spell => spell.Level == 3
                    && spell.AvailableToClassIds
                        .Any(id => id.Value == classId)));
    }

    // The class spell list appendix (pp.207-210) gives a 4th-level union of
    // 35 spells across the 8 classes, built across three alphabetical
    // batches (A-D, D-I, L-W) - down from 3rd level's 50, the trend driven
    // by half-casters (Paladin, Ranger) and Warlock's Pact Magic
    // approaching their 5th-level cap.
    [Fact]
    public void FourthLevelContainsExactlyThePhbsThirtyFiveSpells()
    {
        Assert.Equal(
            [
                "dnd5e2014.spell.arcane-eye",
                "dnd5e2014.spell.aura-of-life",
                "dnd5e2014.spell.aura-of-purity",
                "dnd5e2014.spell.banishment",
                "dnd5e2014.spell.blight",
                "dnd5e2014.spell.compulsion",
                "dnd5e2014.spell.confusion",
                "dnd5e2014.spell.conjure-minor-elementals",
                "dnd5e2014.spell.conjure-woodland-beings",
                "dnd5e2014.spell.control-water",
                "dnd5e2014.spell.death-ward",
                "dnd5e2014.spell.dimension-door",
                "dnd5e2014.spell.divination",
                "dnd5e2014.spell.dominate-beast",
                "dnd5e2014.spell.evards-black-tentacles",
                "dnd5e2014.spell.fabricate",
                "dnd5e2014.spell.fire-shield",
                "dnd5e2014.spell.freedom-of-movement",
                "dnd5e2014.spell.giant-insect",
                "dnd5e2014.spell.grasping-vine",
                "dnd5e2014.spell.greater-invisibility",
                "dnd5e2014.spell.guardian-of-faith",
                "dnd5e2014.spell.hallucinatory-terrain",
                "dnd5e2014.spell.ice-storm",
                "dnd5e2014.spell.leomunds-secret-chest",
                "dnd5e2014.spell.locate-creature",
                "dnd5e2014.spell.mordenkainens-faithful-hound",
                "dnd5e2014.spell.mordenkainens-private-sanctum",
                "dnd5e2014.spell.otilukes-resilient-sphere",
                "dnd5e2014.spell.phantasmal-killer",
                "dnd5e2014.spell.polymorph",
                "dnd5e2014.spell.staggering-smite",
                "dnd5e2014.spell.stone-shape",
                "dnd5e2014.spell.stoneskin",
                "dnd5e2014.spell.wall-of-fire"
            ],
            LoadCanonical()
                .Where(spell => spell.Level == 4)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    // Read off the eight class spell lists on pp.207-210, whose 4th-level
    // sections hold 8/8/16/6/5/10/4/23 entries; their union is 35.
    [Theory]
    [InlineData("dnd5e2014.class.bard", 8)]
    [InlineData("dnd5e2014.class.cleric", 8)]
    [InlineData("dnd5e2014.class.druid", 16)]
    [InlineData("dnd5e2014.class.paladin", 6)]
    [InlineData("dnd5e2014.class.ranger", 5)]
    [InlineData("dnd5e2014.class.sorcerer", 10)]
    [InlineData("dnd5e2014.class.warlock", 4)]
    [InlineData("dnd5e2014.class.wizard", 23)]
    public void ClassFourthLevelListHasExpectedSize(
        string classId,
        int expectedCount)
    {
        Assert.Equal(
            expectedCount,
            LoadCanonical()
                .Count(spell => spell.Level == 4
                    && spell.AvailableToClassIds
                        .Any(id => id.Value == classId)));
    }

    // Leomund's Secret Chest prints two separately-costed material items in
    // one description - the 5,000 gp chest and its 50 gp replica - rather
    // than one figure or a per-item cost like Warding Bond's. No single
    // number represents "the" cost, so MaterialCostGoldPieces is declined
    // (left null) rather than picking one of the two figures, the same
    // partial-decline shape Plant Growth's compound casting time used.
    [Fact]
    public void LeomundsSecretChestDeclinesItsTwoPartMaterialCost()
    {
        SpellComponents components =
            Get("dnd5e2014.spell.leomunds-secret-chest").Components;

        Assert.True(components.Material);
        Assert.Null(components.MaterialCostGoldPieces);
        Assert.False(components.MaterialIsConsumed);
        Assert.Contains("5,000 gp", components.MaterialDescription!);
        Assert.Contains("50 gp", components.MaterialDescription!);
    }

    // Locate Creature reaches six classes, the widest 4th-level membership
    // in the book - matching Hold Person's six at second level.
    [Fact]
    public void LocateCreatureIsAvailableToSixClasses()
    {
        Assert.Equal(
            [
                "dnd5e2014.class.bard",
                "dnd5e2014.class.cleric",
                "dnd5e2014.class.druid",
                "dnd5e2014.class.paladin",
                "dnd5e2014.class.ranger",
                "dnd5e2014.class.wizard"
            ],
            Get("dnd5e2014.spell.locate-creature").AvailableToClassIds
                .Select(id => id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    // The class spell list appendix (pp.207-210) gives a 5th-level union of
    // 42 spells across the 8 classes, built across four alphabetical
    // batches (A-C, C-G, G-P, R-W) - up from 4th level's 35, so the
    // per-level decline (62/59/50/35) isn't monotonic. Every list grew or
    // held from 4th to 5th, since that's each class's last level before
    // Paladin/Ranger/Warlock's lists stop entirely.
    [Fact]
    public void FifthLevelContainsExactlyThePhbsFortyTwoSpells()
    {
        Assert.Equal(
            [
                "dnd5e2014.spell.animate-objects",
                "dnd5e2014.spell.antilife-shell",
                "dnd5e2014.spell.awaken",
                "dnd5e2014.spell.banishing-smite",
                "dnd5e2014.spell.bigbys-hand",
                "dnd5e2014.spell.circle-of-power",
                "dnd5e2014.spell.cloudkill",
                "dnd5e2014.spell.commune",
                "dnd5e2014.spell.commune-with-nature",
                "dnd5e2014.spell.cone-of-cold",
                "dnd5e2014.spell.conjure-elemental",
                "dnd5e2014.spell.conjure-volley",
                "dnd5e2014.spell.contact-other-plane",
                "dnd5e2014.spell.contagion",
                "dnd5e2014.spell.creation",
                "dnd5e2014.spell.destructive-wave",
                "dnd5e2014.spell.dispel-evil-and-good",
                "dnd5e2014.spell.dominate-person",
                "dnd5e2014.spell.dream",
                "dnd5e2014.spell.flame-strike",
                "dnd5e2014.spell.geas",
                "dnd5e2014.spell.greater-restoration",
                "dnd5e2014.spell.hallow",
                "dnd5e2014.spell.hold-monster",
                "dnd5e2014.spell.insect-plague",
                "dnd5e2014.spell.legend-lore",
                "dnd5e2014.spell.mass-cure-wounds",
                "dnd5e2014.spell.mislead",
                "dnd5e2014.spell.modify-memory",
                "dnd5e2014.spell.passwall",
                "dnd5e2014.spell.planar-binding",
                "dnd5e2014.spell.raise-dead",
                "dnd5e2014.spell.rarys-telepathic-bond",
                "dnd5e2014.spell.reincarnate",
                "dnd5e2014.spell.scrying",
                "dnd5e2014.spell.seeming",
                "dnd5e2014.spell.swift-quiver",
                "dnd5e2014.spell.telekinesis",
                "dnd5e2014.spell.teleportation-circle",
                "dnd5e2014.spell.tree-stride",
                "dnd5e2014.spell.wall-of-force",
                "dnd5e2014.spell.wall-of-stone"
            ],
            LoadCanonical()
                .Where(spell => spell.Level == 5)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    // Read off the eight class spell lists on pp.207-210, whose 5th-level
    // sections hold 16/13/14/6/4/11/4/23 entries; their union is 42.
    [Theory]
    [InlineData("dnd5e2014.class.bard", 16)]
    [InlineData("dnd5e2014.class.cleric", 13)]
    [InlineData("dnd5e2014.class.druid", 14)]
    [InlineData("dnd5e2014.class.paladin", 6)]
    [InlineData("dnd5e2014.class.ranger", 4)]
    [InlineData("dnd5e2014.class.sorcerer", 11)]
    [InlineData("dnd5e2014.class.warlock", 4)]
    [InlineData("dnd5e2014.class.wizard", 23)]
    public void ClassFifthLevelListHasExpectedSize(
        string classId,
        int expectedCount)
    {
        Assert.Equal(
            expectedCount,
            LoadCanonical()
                .Count(spell => spell.Level == 5
                    && spell.AvailableToClassIds
                        .Any(id => id.Value == classId)));
    }

    // The class spell list appendix (pp.207-210) gives a 6th-level union of
    // 32 spells across the 8 classes - down from 5th level's 42, as
    // predicted: Paladin and Ranger's lists stop entirely at 5th. Warlock's
    // Pact Magic slots also cap at 5th level, but the class's own spell
    // list keeps going through 9th (for Mystic Arcanum, which grants one
    // higher-level spell known without a matching slot) - a correction to
    // this file's earlier assumption that all three classes would drop out
    // together. Built across three alphabetical batches (A-E, F-M, M-W).
    // Wall of Thorns was missed on the first pass through the M-W batch -
    // it was read directly off the class list appendix (Druid's list) but
    // dropped from the initial page-by-page description pass, caught only
    // when the Druid class-count theory below failed at 8 instead of 9.
    // The PHB's actual count is 32, not the 31 this file previously
    // recorded.
    [Fact]
    public void SixthLevelContainsExactlyThePhbsThirtyTwoSpells()
    {
        Assert.Equal(
            [
                "dnd5e2014.spell.arcane-gate",
                "dnd5e2014.spell.blade-barrier",
                "dnd5e2014.spell.chain-lightning",
                "dnd5e2014.spell.circle-of-death",
                "dnd5e2014.spell.conjure-fey",
                "dnd5e2014.spell.contingency",
                "dnd5e2014.spell.create-undead",
                "dnd5e2014.spell.disintegrate",
                "dnd5e2014.spell.drawmijs-instant-summons",
                "dnd5e2014.spell.eyebite",
                "dnd5e2014.spell.find-the-path",
                "dnd5e2014.spell.flesh-to-stone",
                "dnd5e2014.spell.forbiddance",
                "dnd5e2014.spell.globe-of-invulnerability",
                "dnd5e2014.spell.guards-and-wards",
                "dnd5e2014.spell.harm",
                "dnd5e2014.spell.heal",
                "dnd5e2014.spell.heroes-feast",
                "dnd5e2014.spell.magic-jar",
                "dnd5e2014.spell.mass-suggestion",
                "dnd5e2014.spell.move-earth",
                "dnd5e2014.spell.otilukes-freezing-sphere",
                "dnd5e2014.spell.ottos-irresistible-dance",
                "dnd5e2014.spell.planar-ally",
                "dnd5e2014.spell.programmed-illusion",
                "dnd5e2014.spell.sunbeam",
                "dnd5e2014.spell.transport-via-plants",
                "dnd5e2014.spell.true-seeing",
                "dnd5e2014.spell.wall-of-ice",
                "dnd5e2014.spell.wall-of-thorns",
                "dnd5e2014.spell.wind-walk",
                "dnd5e2014.spell.word-of-recall"
            ],
            LoadCanonical()
                .Where(spell => spell.Level == 6)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    // Read off the eight class spell lists on pp.207-210, whose 6th-level
    // sections hold 7/10/9/0/0/10/8/20 entries, summing to 74 across
    // classes but a union of only 32 spells, since most 6th-level spells
    // appear on more than one class's list. The Wizard figure was first
    // recorded as 17 during the F-M batch and corrected
    // to 20 here - a four-column class list page split Wizard's 6th-level
    // entries across a column boundary (Programmed Illusion at the bottom
    // of one column, Sunbeam/True Seeing/Wall of Ice at the top of the
    // next), the same kind of miss the 3rd-level Wizard count made. A
    // second pre-existing error surfaced alongside it: Conjure Fey (built
    // in the A-E batch) was tagged available to Wizard, but the Wizard
    // list on p.212 doesn't include it - only Druid and Warlock do. Fixed
    // in the same commit that added Wall of Thorns, per the standing rule
    // to fix an error where it's found.
    [Theory]
    [InlineData("dnd5e2014.class.bard", 7)]
    [InlineData("dnd5e2014.class.cleric", 10)]
    [InlineData("dnd5e2014.class.druid", 9)]
    [InlineData("dnd5e2014.class.paladin", 0)]
    [InlineData("dnd5e2014.class.ranger", 0)]
    [InlineData("dnd5e2014.class.sorcerer", 10)]
    [InlineData("dnd5e2014.class.warlock", 8)]
    [InlineData("dnd5e2014.class.wizard", 20)]
    public void ClassSixthLevelListHasExpectedSize(
        string classId,
        int expectedCount)
    {
        Assert.Equal(
            expectedCount,
            LoadCanonical()
                .Count(spell => spell.Level == 6
                    && spell.AvailableToClassIds
                        .Any(id => id.Value == classId)));
    }

    [Theory]
    [InlineData("dnd5e2014.spell.arcane-gate", "Arcane Gate", "conjuration", 214)]
    [InlineData("dnd5e2014.spell.blade-barrier", "Blade Barrier", "evocation", 218)]
    [InlineData("dnd5e2014.spell.chain-lightning", "Chain Lightning", "evocation", 221)]
    [InlineData("dnd5e2014.spell.circle-of-death", "Circle of Death", "necromancy", 221)]
    [InlineData("dnd5e2014.spell.conjure-fey", "Conjure Fey", "conjuration", 226)]
    [InlineData("dnd5e2014.spell.contingency", "Contingency", "evocation", 227)]
    [InlineData("dnd5e2014.spell.create-undead", "Create Undead", "necromancy", 229)]
    [InlineData("dnd5e2014.spell.disintegrate", "Disintegrate", "transmutation", 233)]
    [InlineData("dnd5e2014.spell.drawmijs-instant-summons", "Drawmij's Instant Summons", "conjuration", 235)]
    [InlineData("dnd5e2014.spell.eyebite", "Eyebite", "necromancy", 238)]
    [InlineData("dnd5e2014.spell.find-the-path", "Find the Path", "divination", 240)]
    [InlineData("dnd5e2014.spell.flesh-to-stone", "Flesh to Stone", "transmutation", 243)]
    [InlineData("dnd5e2014.spell.forbiddance", "Forbiddance", "abjuration", 243)]
    [InlineData("dnd5e2014.spell.globe-of-invulnerability", "Globe of Invulnerability", "abjuration", 245)]
    [InlineData("dnd5e2014.spell.guards-and-wards", "Guards and Wards", "abjuration", 248)]
    [InlineData("dnd5e2014.spell.harm", "Harm", "necromancy", 249)]
    [InlineData("dnd5e2014.spell.heal", "Heal", "evocation", 250)]
    [InlineData("dnd5e2014.spell.heroes-feast", "Heroes' Feast", "conjuration", 250)]
    [InlineData("dnd5e2014.spell.magic-jar", "Magic Jar", "necromancy", 257)]
    [InlineData("dnd5e2014.spell.mass-suggestion", "Mass Suggestion", "enchantment", 258)]
    [InlineData("dnd5e2014.spell.move-earth", "Move Earth", "transmutation", 263)]
    [InlineData("dnd5e2014.spell.otilukes-freezing-sphere", "Otiluke's Freezing Sphere", "evocation", 263)]
    [InlineData("dnd5e2014.spell.ottos-irresistible-dance", "Otto's Irresistible Dance", "enchantment", 264)]
    [InlineData("dnd5e2014.spell.planar-ally", "Planar Ally", "conjuration", 265)]
    [InlineData("dnd5e2014.spell.programmed-illusion", "Programmed Illusion", "illusion", 269)]
    [InlineData("dnd5e2014.spell.sunbeam", "Sunbeam", "evocation", 279)]
    [InlineData("dnd5e2014.spell.transport-via-plants", "Transport via Plants", "conjuration", 283)]
    [InlineData("dnd5e2014.spell.true-seeing", "True Seeing", "divination", 284)]
    [InlineData("dnd5e2014.spell.wall-of-ice", "Wall of Ice", "evocation", 285)]
    [InlineData("dnd5e2014.spell.wall-of-thorns", "Wall of Thorns", "conjuration", 287)]
    [InlineData("dnd5e2014.spell.wind-walk", "Wind Walk", "transmutation", 288)]
    [InlineData("dnd5e2014.spell.word-of-recall", "Word of Recall", "conjuration", 289)]
    public void SixthLevelSpell_HasExpectedNameSchoolAndPage(
        string id,
        string expectedName,
        string expectedSchool,
        int expectedPage)
    {
        SpellDefinition spell = Get(id);

        Assert.Equal(6, spell.Level);
        Assert.Equal(expectedName, spell.Name);
        Assert.Equal(
            $"dnd5e2014.magic-school.{expectedSchool}",
            spell.SchoolId.Value);
        Assert.Equal(expectedPage, Assert.Single(spell.Sources).Page);
    }

    // The class spell list appendix (pp.207-210) gives a 7th-level union
    // of 20 spells across the 8 classes - down from 6th level's 32, since
    // Bard/Cleric/Druid/Sorcerer/Warlock/Wizard lists all shrink at once
    // (Paladin and Ranger stayed at zero from 6th). Built across two
    // alphabetical batches (A-M, P-T).
    [Fact]
    public void SeventhLevelContainsExactlyThePhbsTwentySpells()
    {
        Assert.Equal(
            [
                "dnd5e2014.spell.conjure-celestial",
                "dnd5e2014.spell.delayed-blast-fireball",
                "dnd5e2014.spell.divine-word",
                "dnd5e2014.spell.etherealness",
                "dnd5e2014.spell.finger-of-death",
                "dnd5e2014.spell.fire-storm",
                "dnd5e2014.spell.forcecage",
                "dnd5e2014.spell.mirage-arcane",
                "dnd5e2014.spell.mordenkainens-magnificent-mansion",
                "dnd5e2014.spell.mordenkainens-sword",
                "dnd5e2014.spell.plane-shift",
                "dnd5e2014.spell.prismatic-spray",
                "dnd5e2014.spell.project-image",
                "dnd5e2014.spell.regenerate",
                "dnd5e2014.spell.resurrection",
                "dnd5e2014.spell.reverse-gravity",
                "dnd5e2014.spell.sequester",
                "dnd5e2014.spell.simulacrum",
                "dnd5e2014.spell.symbol",
                "dnd5e2014.spell.teleport"
            ],
            LoadCanonical()
                .Where(spell => spell.Level == 7)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    // Read off the eight class spell lists on pp.207-210, whose 7th-level
    // sections hold 10/8/5/0/0/8/4/15 entries; their union is 20. Paladin
    // and Ranger stay at zero (their lists stopped at 5th). Warlock's
    // count (4) is smaller than 6th's (8), consistent with Mystic Arcanum
    // eligibility rather than Pact Magic slots, which never reach this
    // high.
    [Theory]
    [InlineData("dnd5e2014.class.bard", 10)]
    [InlineData("dnd5e2014.class.cleric", 8)]
    [InlineData("dnd5e2014.class.druid", 5)]
    [InlineData("dnd5e2014.class.paladin", 0)]
    [InlineData("dnd5e2014.class.ranger", 0)]
    [InlineData("dnd5e2014.class.sorcerer", 8)]
    [InlineData("dnd5e2014.class.warlock", 4)]
    [InlineData("dnd5e2014.class.wizard", 15)]
    public void ClassSeventhLevelListHasExpectedSize(
        string classId,
        int expectedCount)
    {
        Assert.Equal(
            expectedCount,
            LoadCanonical()
                .Count(spell => spell.Level == 7
                    && spell.AvailableToClassIds
                        .Any(id => id.Value == classId)));
    }

    [Theory]
    [InlineData("dnd5e2014.spell.conjure-celestial", "Conjure Celestial", "conjuration", 225)]
    [InlineData("dnd5e2014.spell.delayed-blast-fireball", "Delayed Blast Fireball", "evocation", 230)]
    [InlineData("dnd5e2014.spell.divine-word", "Divine Word", "evocation", 234)]
    [InlineData("dnd5e2014.spell.etherealness", "Etherealness", "transmutation", 238)]
    [InlineData("dnd5e2014.spell.finger-of-death", "Finger of Death", "necromancy", 241)]
    [InlineData("dnd5e2014.spell.fire-storm", "Fire Storm", "evocation", 242)]
    [InlineData("dnd5e2014.spell.forcecage", "Forcecage", "evocation", 243)]
    [InlineData("dnd5e2014.spell.mirage-arcane", "Mirage Arcane", "illusion", 260)]
    [InlineData("dnd5e2014.spell.mordenkainens-magnificent-mansion", "Mordenkainen's Magnificent Mansion", "conjuration", 261)]
    [InlineData("dnd5e2014.spell.mordenkainens-sword", "Mordenkainen's Sword", "evocation", 262)]
    [InlineData("dnd5e2014.spell.plane-shift", "Plane Shift", "conjuration", 266)]
    [InlineData("dnd5e2014.spell.prismatic-spray", "Prismatic Spray", "evocation", 267)]
    [InlineData("dnd5e2014.spell.project-image", "Project Image", "illusion", 270)]
    [InlineData("dnd5e2014.spell.regenerate", "Regenerate", "transmutation", 271)]
    [InlineData("dnd5e2014.spell.resurrection", "Resurrection", "necromancy", 272)]
    [InlineData("dnd5e2014.spell.reverse-gravity", "Reverse Gravity", "transmutation", 272)]
    [InlineData("dnd5e2014.spell.sequester", "Sequester", "transmutation", 274)]
    [InlineData("dnd5e2014.spell.simulacrum", "Simulacrum", "illusion", 276)]
    [InlineData("dnd5e2014.spell.symbol", "Symbol", "abjuration", 280)]
    [InlineData("dnd5e2014.spell.teleport", "Teleport", "conjuration", 281)]
    public void SeventhLevelSpell_HasExpectedNameSchoolAndPage(
        string id,
        string expectedName,
        string expectedSchool,
        int expectedPage)
    {
        SpellDefinition spell = Get(id);

        Assert.Equal(7, spell.Level);
        Assert.Equal(expectedName, spell.Name);
        Assert.Equal(
            $"dnd5e2014.magic-school.{expectedSchool}",
            spell.SchoolId.Value);
        Assert.Equal(expectedPage, Assert.Single(spell.Sources).Page);
    }

    // The class spell list appendix (pp.207-210) naively reads as an
    // 8th-level union of 19 spells across the 8 classes, but one Wizard
    // entry - Trap the Soul - is a PHB appendix error with no real spell
    // behind it: this printing's Spell Descriptions section has no entry
    // for it anywhere in its correct alphabetical position (verified
    // against high-resolution page images across the entire T range,
    // p.279-285 - the text runs continuously from Transport via Plants
    // into Tree Stride with no gap), and it was never supposed to be
    // there. The real 8th-level count is 18, not 19; this level is
    // fully built and closed, matching the PHB's actual content exactly.
    [Fact]
    public void EighthLevelContainsAllEighteenRealSpells()
    {
        Assert.Equal(
            [
                "dnd5e2014.spell.animal-shapes",
                "dnd5e2014.spell.antimagic-field",
                "dnd5e2014.spell.antipathy-sympathy",
                "dnd5e2014.spell.clone",
                "dnd5e2014.spell.control-weather",
                "dnd5e2014.spell.demiplane",
                "dnd5e2014.spell.dominate-monster",
                "dnd5e2014.spell.earthquake",
                "dnd5e2014.spell.feeblemind",
                "dnd5e2014.spell.glibness",
                "dnd5e2014.spell.holy-aura",
                "dnd5e2014.spell.incendiary-cloud",
                "dnd5e2014.spell.maze",
                "dnd5e2014.spell.mind-blank",
                "dnd5e2014.spell.power-word-stun",
                "dnd5e2014.spell.sunburst",
                "dnd5e2014.spell.telepathy",
                "dnd5e2014.spell.tsunami"
            ],
            LoadCanonical()
                .Where(spell => spell.Level == 8)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    // Read off the eight class spell lists on pp.207-210, whose 8th-level
    // sections nominally hold 5/4/7/0/0/5/5/14 entries. Wizard's real
    // count is 13, not 14 - the 14th name, Trap the Soul, is the
    // appendix error described on EighthLevelContainsAllEighteenRealSpells,
    // not a spell that exists. Do not "fix" this to 14; 13 is correct.
    [Theory]
    [InlineData("dnd5e2014.class.bard", 5)]
    [InlineData("dnd5e2014.class.cleric", 4)]
    [InlineData("dnd5e2014.class.druid", 7)]
    [InlineData("dnd5e2014.class.paladin", 0)]
    [InlineData("dnd5e2014.class.ranger", 0)]
    [InlineData("dnd5e2014.class.sorcerer", 5)]
    [InlineData("dnd5e2014.class.warlock", 5)]
    [InlineData("dnd5e2014.class.wizard", 13)]
    public void ClassEighthLevelListHasExpectedSize(
        string classId,
        int expectedCount)
    {
        Assert.Equal(
            expectedCount,
            LoadCanonical()
                .Count(spell => spell.Level == 8
                    && spell.AvailableToClassIds
                        .Any(id => id.Value == classId)));
    }

    [Theory]
    [InlineData("dnd5e2014.spell.animal-shapes", "Animal Shapes", "transmutation", 212)]
    [InlineData("dnd5e2014.spell.antimagic-field", "Antimagic Field", "abjuration", 213)]
    [InlineData("dnd5e2014.spell.antipathy-sympathy", "Antipathy/Sympathy", "enchantment", 214)]
    [InlineData("dnd5e2014.spell.clone", "Clone", "necromancy", 222)]
    [InlineData("dnd5e2014.spell.control-weather", "Control Weather", "transmutation", 228)]
    [InlineData("dnd5e2014.spell.demiplane", "Demiplane", "conjuration", 231)]
    [InlineData("dnd5e2014.spell.dominate-monster", "Dominate Monster", "enchantment", 235)]
    [InlineData("dnd5e2014.spell.earthquake", "Earthquake", "evocation", 236)]
    [InlineData("dnd5e2014.spell.feeblemind", "Feeblemind", "enchantment", 239)]
    [InlineData("dnd5e2014.spell.glibness", "Glibness", "transmutation", 245)]
    [InlineData("dnd5e2014.spell.holy-aura", "Holy Aura", "abjuration", 251)]
    [InlineData("dnd5e2014.spell.incendiary-cloud", "Incendiary Cloud", "conjuration", 253)]
    [InlineData("dnd5e2014.spell.maze", "Maze", "conjuration", 258)]
    [InlineData("dnd5e2014.spell.mind-blank", "Mind Blank", "abjuration", 259)]
    [InlineData("dnd5e2014.spell.power-word-stun", "Power Word Stun", "enchantment", 267)]
    [InlineData("dnd5e2014.spell.sunburst", "Sunburst", "evocation", 279)]
    [InlineData("dnd5e2014.spell.telepathy", "Telepathy", "evocation", 281)]
    [InlineData("dnd5e2014.spell.tsunami", "Tsunami", "conjuration", 284)]
    public void EighthLevelSpell_HasExpectedNameSchoolAndPage(
        string id,
        string expectedName,
        string expectedSchool,
        int expectedPage)
    {
        SpellDefinition spell = Get(id);

        Assert.Equal(8, spell.Level);
        Assert.Equal(expectedName, spell.Name);
        Assert.Equal(
            $"dnd5e2014.magic-school.{expectedSchool}",
            spell.SchoolId.Value);
        Assert.Equal(expectedPage, Assert.Single(spell.Sources).Page);
    }

    // Clone's material bundle has two separately-costed items - a 1,000
    // gp diamond and a 2,000 gp vessel - with no single figure that
    // represents "the" cost, the same partial-decline shape Legend Lore
    // and Leomund's Secret Chest already used. Unlike Leomund's Secret
    // Chest (where neither item's consumption is stated), Clone's flesh
    // component is explicitly consumed, so MaterialIsConsumed is true
    // even though the field can't represent "only one of the two items."
    [Fact]
    public void CloneDeclinesItsTwoPartMaterialCost()
    {
        SpellComponents components = Get("dnd5e2014.spell.clone").Components;

        Assert.Null(components.MaterialCostGoldPieces);
        Assert.True(components.MaterialIsConsumed);
    }

    // Demiplane is Somatic-only - no verbal, no material - the first
    // non-cantrip spell on that combination (True Strike is the only
    // cantrip on it).
    [Fact]
    public void DemiplaneIsSomaticOnly()
    {
        SpellComponents components =
            Get("dnd5e2014.spell.demiplane").Components;

        Assert.False(components.Verbal);
        Assert.True(components.Somatic);
        Assert.False(components.Material);
    }

    // The class spell list appendix (pp.207-210) gives a 9th-level union
    // of 16 spells across the 8 classes - down from 8th level's real 18
    // (the appendix's own 8th-level union naively reads as 19, but Trap
    // the Soul is an appendix error with no real spell behind it; see
    // EighthLevelContainsAllEighteenRealSpells). Paladin and Ranger stay
    // at zero. Built across two alphabetical batches (A-P, P-W). This
    // closes the Spells domain completely: every PHB spell level (0-9)
    // is now fully built, 361 spells total.
    [Fact]
    public void NinthLevelContainsExactlyThePhbsSixteenSpells()
    {
        Assert.Equal(
            [
                "dnd5e2014.spell.astral-projection",
                "dnd5e2014.spell.foresight",
                "dnd5e2014.spell.gate",
                "dnd5e2014.spell.imprisonment",
                "dnd5e2014.spell.mass-heal",
                "dnd5e2014.spell.meteor-swarm",
                "dnd5e2014.spell.power-word-heal",
                "dnd5e2014.spell.power-word-kill",
                "dnd5e2014.spell.prismatic-wall",
                "dnd5e2014.spell.shapechange",
                "dnd5e2014.spell.storm-of-vengeance",
                "dnd5e2014.spell.time-stop",
                "dnd5e2014.spell.true-polymorph",
                "dnd5e2014.spell.true-resurrection",
                "dnd5e2014.spell.weird",
                "dnd5e2014.spell.wish"
            ],
            LoadCanonical()
                .Where(spell => spell.Level == 9)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    // Read off the eight class spell lists on pp.207-210, whose 9th-level
    // sections hold 4/4/4/5/5/12/0/0 entries; their union is 16.
    [Theory]
    [InlineData("dnd5e2014.class.bard", 4)]
    [InlineData("dnd5e2014.class.cleric", 4)]
    [InlineData("dnd5e2014.class.druid", 4)]
    [InlineData("dnd5e2014.class.paladin", 0)]
    [InlineData("dnd5e2014.class.ranger", 0)]
    [InlineData("dnd5e2014.class.sorcerer", 5)]
    [InlineData("dnd5e2014.class.warlock", 5)]
    [InlineData("dnd5e2014.class.wizard", 12)]
    public void ClassNinthLevelListHasExpectedSize(
        string classId,
        int expectedCount)
    {
        Assert.Equal(
            expectedCount,
            LoadCanonical()
                .Count(spell => spell.Level == 9
                    && spell.AvailableToClassIds
                        .Any(id => id.Value == classId)));
    }

    [Theory]
    [InlineData("dnd5e2014.spell.astral-projection", "Astral Projection", "necromancy", 215)]
    [InlineData("dnd5e2014.spell.foresight", "Foresight", "divination", 244)]
    [InlineData("dnd5e2014.spell.gate", "Gate", "conjuration", 244)]
    [InlineData("dnd5e2014.spell.imprisonment", "Imprisonment", "abjuration", 252)]
    [InlineData("dnd5e2014.spell.mass-heal", "Mass Heal", "conjuration", 258)]
    [InlineData("dnd5e2014.spell.meteor-swarm", "Meteor Swarm", "evocation", 258)]
    [InlineData("dnd5e2014.spell.power-word-heal", "Power Word Heal", "evocation", 266)]
    [InlineData("dnd5e2014.spell.power-word-kill", "Power Word Kill", "enchantment", 266)]
    [InlineData("dnd5e2014.spell.prismatic-wall", "Prismatic Wall", "abjuration", 267)]
    [InlineData("dnd5e2014.spell.shapechange", "Shapechange", "transmutation", 274)]
    [InlineData("dnd5e2014.spell.storm-of-vengeance", "Storm of Vengeance", "conjuration", 279)]
    [InlineData("dnd5e2014.spell.time-stop", "Time Stop", "transmutation", 283)]
    [InlineData("dnd5e2014.spell.true-polymorph", "True Polymorph", "transmutation", 283)]
    [InlineData("dnd5e2014.spell.true-resurrection", "True Resurrection", "necromancy", 284)]
    [InlineData("dnd5e2014.spell.weird", "Weird", "illusion", 288)]
    [InlineData("dnd5e2014.spell.wish", "Wish", "conjuration", 288)]
    public void NinthLevelSpell_HasExpectedNameSchoolAndPage(
        string id,
        string expectedName,
        string expectedSchool,
        int expectedPage)
    {
        SpellDefinition spell = Get(id);

        Assert.Equal(9, spell.Level);
        Assert.Equal(expectedName, spell.Name);
        Assert.Equal(
            $"dnd5e2014.magic-school.{expectedSchool}",
            spell.SchoolId.Value);
        Assert.Equal(expectedPage, Assert.Single(spell.Sources).Page);
    }

    // Shapechange's material is costed (1,500 gp jade circlet) but not
    // stated as consumed - you wear it, you don't use it up.
    [Fact]
    public void ShapechangeIsCostedButNotConsumed()
    {
        SpellComponents components =
            Get("dnd5e2014.spell.shapechange").Components;

        Assert.Equal(1500, components.MaterialCostGoldPieces);
        Assert.False(components.MaterialIsConsumed);
    }

    // Astral Projection's material bundle is costed per creature affected
    // ("for each creature you affect... one jacinth worth at least 1,000
    // gp and one ornately carved bar of silver worth at least 100 gp"),
    // combining Clone's two-item shape with a multiplier the field can't
    // represent either. MaterialCostGoldPieces stays null (declined);
    // unlike Clone, the whole bundle is explicitly consumed.
    [Fact]
    public void AstralProjectionDeclinesItsPerCreatureMaterialCost()
    {
        SpellComponents components =
            Get("dnd5e2014.spell.astral-projection").Components;

        Assert.Null(components.MaterialCostGoldPieces);
        Assert.True(components.MaterialIsConsumed);
    }

    // Meteor Swarm prints "Range: 1 mile" - the third range printed in
    // miles rather than feet (after Clairvoyance and Project Image),
    // canonicalized to 5,280 feet by the same unit-conversion rule.
    [Fact]
    public void MeteorSwarmRangeIsCanonicalizedFromMiles()
    {
        Assert.Equal(
            5280,
            Get("dnd5e2014.spell.meteor-swarm").Range.DistanceFeet);
    }

    // Mirage Arcane prints "Range: Sight" - reach is whatever the caster
    // can see, not a bounded distance and not the same conditional-rule
    // shape as Dream's Special. SpellRangeKind gained a Sight member and
    // SpellRange.Sight() factory, the same "self/touch/distance" enum
    // extended by one case that Unlimited and Special already did.
    // Tsunami joins it at 8th level and Storm of Vengeance at 9th,
    // confirming Sight is a real recurring PHB range category rather
    // than a one-off.
    [Fact]
    public void SightRangesAreMirageArcaneStormOfVengeanceAndTsunami()
    {
        SpellDefinition[] sight = LoadCanonical()
            .Where(spell => spell.Range.Kind == SpellRangeKind.Sight)
            .OrderBy(spell => spell.Id.Value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "dnd5e2014.spell.mirage-arcane",
                "dnd5e2014.spell.storm-of-vengeance",
                "dnd5e2014.spell.tsunami"
            ],
            sight.Select(spell => spell.Id.Value));

        Assert.All(
            sight,
            spell =>
            {
                Assert.Null(spell.Range.DistanceFeet);
                Assert.Null(spell.Range.AreaShape);
            });
    }

    // Etherealness prints a flat "Up to 8 hours" with no concentration -
    // the same unit-doesn't-imply-shape distinction Prestidigitation
    // already pins at the Hour unit (Prestidigitation is "up to" with no
    // concentration too), now shown on a non-cantrip.
    [Fact]
    public void EtherealnessIsUpToWithoutConcentration()
    {
        SpellDuration duration = Get("dnd5e2014.spell.etherealness").Duration;

        Assert.True(duration.IsUpTo);
        Assert.False(duration.RequiresConcentration);
        Assert.Equal(8, duration.Amount);
        Assert.Equal(SpellDurationUnit.Hour, duration.Unit);
    }

    // Mordenkainen's Magnificent Mansion is the third spell (after
    // Warding Bond and Legend Lore) whose material cost is per item
    // rather than total - three items "each item worth at least 5 gp".
    // Unlike Legend Lore's two differently-costed items, all three items
    // here share one figure, so the printed 5 gp is stored directly,
    // same convention as Warding Bond.
    [Fact]
    public void MordenkainensMagnificentMansionStoresThePrintedPerItemCost()
    {
        SpellComponents components =
            Get("dnd5e2014.spell.mordenkainens-magnificent-mansion")
                .Components;

        Assert.Equal(5, components.MaterialCostGoldPieces);
        Assert.False(components.MaterialIsConsumed);
        Assert.Contains(
            "each item",
            components.MaterialDescription!,
            StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("dnd5e2014.spell.animate-objects", "Animate Objects", "transmutation", 213)]
    [InlineData("dnd5e2014.spell.awaken", "Awaken", "transmutation", 216)]
    [InlineData("dnd5e2014.spell.bigbys-hand", "Bigby's Hand", "evocation", 218)]
    [InlineData("dnd5e2014.spell.circle-of-power", "Circle of Power", "abjuration", 221)]
    [InlineData("dnd5e2014.spell.cloudkill", "Cloudkill", "conjuration", 222)]
    [InlineData("dnd5e2014.spell.commune", "Commune", "divination", 223)]
    [InlineData("dnd5e2014.spell.commune-with-nature", "Commune with Nature", "divination", 224)]
    [InlineData("dnd5e2014.spell.conjure-elemental", "Conjure Elemental", "conjuration", 225)]
    [InlineData("dnd5e2014.spell.conjure-volley", "Conjure Volley", "conjuration", 226)]
    [InlineData("dnd5e2014.spell.contact-other-plane", "Contact Other Plane", "divination", 226)]
    [InlineData("dnd5e2014.spell.contagion", "Contagion", "necromancy", 227)]
    [InlineData("dnd5e2014.spell.creation", "Creation", "illusion", 229)]
    [InlineData("dnd5e2014.spell.destructive-wave", "Destructive Wave", "evocation", 231)]
    [InlineData("dnd5e2014.spell.dispel-evil-and-good", "Dispel Evil and Good", "abjuration", 233)]
    [InlineData("dnd5e2014.spell.dominate-person", "Dominate Person", "enchantment", 235)]
    [InlineData("dnd5e2014.spell.dream", "Dream", "illusion", 236)]
    [InlineData("dnd5e2014.spell.flame-strike", "Flame Strike", "evocation", 242)]
    [InlineData("dnd5e2014.spell.geas", "Geas", "enchantment", 244)]
    [InlineData("dnd5e2014.spell.greater-restoration", "Greater Restoration", "abjuration", 246)]
    [InlineData("dnd5e2014.spell.hallow", "Hallow", "evocation", 249)]
    [InlineData("dnd5e2014.spell.hold-monster", "Hold Monster", "enchantment", 251)]
    [InlineData("dnd5e2014.spell.insect-plague", "Insect Plague", "conjuration", 254)]
    [InlineData("dnd5e2014.spell.legend-lore", "Legend Lore", "divination", 254)]
    [InlineData("dnd5e2014.spell.mass-cure-wounds", "Mass Cure Wounds", "conjuration", 258)]
    [InlineData("dnd5e2014.spell.mislead", "Mislead", "illusion", 260)]
    [InlineData("dnd5e2014.spell.modify-memory", "Modify Memory", "enchantment", 261)]
    [InlineData("dnd5e2014.spell.passwall", "Passwall", "transmutation", 264)]
    [InlineData("dnd5e2014.spell.planar-binding", "Planar Binding", "abjuration", 265)]
    [InlineData("dnd5e2014.spell.raise-dead", "Raise Dead", "necromancy", 270)]
    [InlineData("dnd5e2014.spell.rarys-telepathic-bond", "Rary's Telepathic Bond", "divination", 270)]
    [InlineData("dnd5e2014.spell.reincarnate", "Reincarnate", "transmutation", 271)]
    [InlineData("dnd5e2014.spell.scrying", "Scrying", "divination", 273)]
    [InlineData("dnd5e2014.spell.swift-quiver", "Swift Quiver", "transmutation", 279)]
    [InlineData("dnd5e2014.spell.telekinesis", "Telekinesis", "transmutation", 280)]
    [InlineData("dnd5e2014.spell.tree-stride", "Tree Stride", "conjuration", 283)]
    [InlineData("dnd5e2014.spell.wall-of-force", "Wall of Force", "evocation", 285)]
    [InlineData("dnd5e2014.spell.wall-of-stone", "Wall of Stone", "evocation", 287)]
    public void FifthLevelSpell_HasExpectedNameSchoolAndPage(
        string id,
        string expectedName,
        string expectedSchool,
        int expectedPage)
    {
        SpellDefinition spell = Get(id);

        Assert.Equal(5, spell.Level);
        Assert.Equal(expectedName, spell.Name);
        Assert.Equal(
            $"dnd5e2014.magic-school.{expectedSchool}",
            spell.SchoolId.Value);
        Assert.Equal(expectedPage, Assert.Single(spell.Sources).Page);
    }

    // Creation prints "Duration: Special" - the actual duration is a
    // lookup table keyed by the material created (1 day for vegetable
    // matter down to 1 minute for adamantine or mithral), which stays in
    // the citation. SpellDuration.IsSpecial carries no amount/unit of its
    // own, the same shape IsUntilDispelled already established. Astral
    // Projection joins it at 9th level - its "Special" duration is that
    // the spell lasts until dismissed or ended by one of several stated
    // conditions (dispel magic, 0 hit points, a severed silver cord),
    // again a compound rule left in the citation rather than an
    // amount/unit.
    [Fact]
    public void SpecialDurationsAreCreationAndAstralProjection()
    {
        SpellDefinition[] special = LoadCanonical()
            .Where(spell => spell.Duration.IsSpecial)
            .OrderBy(spell => spell.Id.Value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            ["dnd5e2014.spell.astral-projection", "dnd5e2014.spell.creation"],
            special.Select(spell => spell.Id.Value));

        Assert.All(
            special,
            spell =>
            {
                Assert.False(spell.Duration.IsInstantaneous);
                Assert.False(spell.Duration.IsUntilDispelled);
                Assert.Null(spell.Duration.Amount);
                Assert.Null(spell.Duration.Unit);
            });
    }

    // Dream prints "Range: Special" - the target must be on the same
    // plane of existence as the caster, a conditional rule that stays in
    // the citation. SpellRangeKind.Special mirrors Unlimited's shape (no
    // distance, no area).
    [Fact]
    public void DreamIsTheOnlySpecialRangeSoFar()
    {
        SpellDefinition only = Assert.Single(
            LoadCanonical()
                .Where(spell => spell.Range.Kind == SpellRangeKind.Special));

        Assert.Equal("dnd5e2014.spell.dream", only.Id.Value);
        Assert.Null(only.Range.DistanceFeet);
        Assert.Null(only.Range.AreaShape);
    }

    // The Paladin spell list appendix (p.209) prints this spell's name as
    // "Destructive Smite" - plausible company for six other Paladin
    // "___ Smite" spells already built (Banishing, Blinding, Searing,
    // Staggering, Thunderous, Wrathful), but the spell's own description
    // page (p.231) unambiguously headers it "Destructive Wave", matching
    // the spell's real published name. The description page wins, the
    // same "errata and body prose beat printing artifacts" rule that
    // corrected the Dwarf's throwing hammer.
    [Fact]
    public void DestructiveWaveIsNamedFromItsDescriptionNotTheAppendix()
    {
        SpellDefinition spell = Get("dnd5e2014.spell.destructive-wave");

        Assert.Equal("Destructive Wave", spell.Name);
        Assert.Contains(
            "dnd5e2014.class.paladin",
            spell.AvailableToClassIds.Select(id => id.Value));
    }

    // Legend Lore is the second spell (after Leomund's Secret Chest) whose
    // material component bundles two separately-costed items - incense at
    // 250 gp and four ivory strips at 50 gp each - with no single figure
    // that represents "the" cost. MaterialCostGoldPieces stays declined
    // (null) rather than picking one; MaterialIsConsumed is true because
    // the PHB explicitly states the incense (part of the bundle) is
    // consumed, even though the ivory strips aren't.
    [Fact]
    public void LegendLoreDeclinesItsTwoPartMaterialCost()
    {
        SpellComponents components =
            Get("dnd5e2014.spell.legend-lore").Components;

        Assert.True(components.Material);
        Assert.Null(components.MaterialCostGoldPieces);
        Assert.True(components.MaterialIsConsumed);
        Assert.Contains("250 gp", components.MaterialDescription!);
        Assert.Contains("50 gp each", components.MaterialDescription!);
    }

    [Theory]
    [InlineData("dnd5e2014.spell.arcane-eye", "Arcane Eye", "divination", 214)]
    [InlineData("dnd5e2014.spell.aura-of-life", "Aura of Life", "abjuration", 216)]
    [InlineData("dnd5e2014.spell.banishment", "Banishment", "abjuration", 218)]
    [InlineData("dnd5e2014.spell.blight", "Blight", "necromancy", 219)]
    [InlineData("dnd5e2014.spell.confusion", "Confusion", "enchantment", 224)]
    [InlineData("dnd5e2014.spell.conjure-woodland-beings", "Conjure Woodland Beings", "conjuration", 226)]
    [InlineData("dnd5e2014.spell.control-water", "Control Water", "transmutation", 227)]
    [InlineData("dnd5e2014.spell.death-ward", "Death Ward", "abjuration", 230)]
    [InlineData("dnd5e2014.spell.dimension-door", "Dimension Door", "conjuration", 233)]
    [InlineData("dnd5e2014.spell.divination", "Divination", "divination", 234)]
    [InlineData("dnd5e2014.spell.evards-black-tentacles", "Evard's Black Tentacles", "conjuration", 238)]
    [InlineData("dnd5e2014.spell.fabricate", "Fabricate", "transmutation", 239)]
    [InlineData("dnd5e2014.spell.fire-shield", "Fire Shield", "evocation", 242)]
    [InlineData("dnd5e2014.spell.giant-insect", "Giant Insect", "transmutation", 245)]
    [InlineData("dnd5e2014.spell.guardian-of-faith", "Guardian of Faith", "conjuration", 246)]
    [InlineData("dnd5e2014.spell.hallucinatory-terrain", "Hallucinatory Terrain", "illusion", 249)]
    [InlineData("dnd5e2014.spell.ice-storm", "Ice Storm", "evocation", 252)]
    [InlineData("dnd5e2014.spell.leomunds-secret-chest", "Leomund's Secret Chest", "conjuration", 254)]
    [InlineData("dnd5e2014.spell.locate-creature", "Locate Creature", "divination", 256)]
    [InlineData("dnd5e2014.spell.mordenkainens-faithful-hound", "Mordenkainen's Faithful Hound", "conjuration", 261)]
    [InlineData("dnd5e2014.spell.otilukes-resilient-sphere", "Otiluke's Resilient Sphere", "evocation", 265)]
    [InlineData("dnd5e2014.spell.phantasmal-killer", "Phantasmal Killer", "illusion", 266)]
    [InlineData("dnd5e2014.spell.polymorph", "Polymorph", "transmutation", 266)]
    [InlineData("dnd5e2014.spell.staggering-smite", "Staggering Smite", "evocation", 278)]
    [InlineData("dnd5e2014.spell.stoneskin", "Stoneskin", "abjuration", 278)]
    [InlineData("dnd5e2014.spell.wall-of-fire", "Wall of Fire", "evocation", 285)]
    public void FourthLevelSpell_HasExpectedNameSchoolAndPage(
        string id,
        string expectedName,
        string expectedSchool,
        int expectedPage)
    {
        SpellDefinition spell = Get(id);

        Assert.Equal(4, spell.Level);
        Assert.Equal(expectedName, spell.Name);
        Assert.Equal(
            $"dnd5e2014.magic-school.{expectedSchool}",
            spell.SchoolId.Value);
        Assert.Equal(expectedPage, Assert.Single(spell.Sources).Page);
    }

    [Theory]
    [InlineData("dnd5e2014.spell.animate-dead", "Animate Dead", "necromancy", 212)]
    [InlineData("dnd5e2014.spell.aura-of-vitality", "Aura of Vitality", "evocation", 216)]
    [InlineData("dnd5e2014.spell.clairvoyance", "Clairvoyance", "divination", 222)]
    [InlineData("dnd5e2014.spell.conjure-barrage", "Conjure Barrage", "conjuration", 225)]
    [InlineData("dnd5e2014.spell.counterspell", "Counterspell", "abjuration", 228)]
    [InlineData("dnd5e2014.spell.crusaders-mantle", "Crusader's Mantle", "evocation", 230)]
    [InlineData("dnd5e2014.spell.dispel-magic", "Dispel Magic", "abjuration", 234)]
    [InlineData("dnd5e2014.spell.fireball", "Fireball", "evocation", 241)]
    [InlineData("dnd5e2014.spell.glyph-of-warding", "Glyph of Warding", "abjuration", 245)]
    [InlineData("dnd5e2014.spell.hunger-of-hadar", "Hunger of Hadar", "conjuration", 251)]
    [InlineData("dnd5e2014.spell.hypnotic-pattern", "Hypnotic Pattern", "illusion", 252)]
    [InlineData("dnd5e2014.spell.leomunds-tiny-hut", "Leomund's Tiny Hut", "evocation", 255)]
    [InlineData("dnd5e2014.spell.lightning-bolt", "Lightning Bolt", "evocation", 255)]
    [InlineData("dnd5e2014.spell.magic-circle", "Magic Circle", "abjuration", 256)]
    [InlineData("dnd5e2014.spell.meld-into-stone", "Meld into Stone", "transmutation", 259)]
    [InlineData("dnd5e2014.spell.nondetection", "Nondetection", "abjuration", 263)]
    [InlineData("dnd5e2014.spell.phantom-steed", "Phantom Steed", "illusion", 266)]
    [InlineData("dnd5e2014.spell.protection-from-energy", "Protection from Energy", "abjuration", 270)]
    [InlineData("dnd5e2014.spell.revivify", "Revivify", "conjuration", 272)]
    [InlineData("dnd5e2014.spell.sending", "Sending", "evocation", 274)]
    [InlineData("dnd5e2014.spell.spirit-guardians", "Spirit Guardians", "conjuration", 278)]
    [InlineData("dnd5e2014.spell.tongues", "Tongues", "divination", 283)]
    [InlineData("dnd5e2014.spell.vampiric-touch", "Vampiric Touch", "necromancy", 285)]
    [InlineData("dnd5e2014.spell.wind-wall", "Wind Wall", "evocation", 288)]
    public void ThirdLevelSpell_HasExpectedNameSchoolAndPage(
        string id,
        string expectedName,
        string expectedSchool,
        int expectedPage)
    {
        SpellDefinition spell = Get(id);

        Assert.Equal(3, spell.Level);
        Assert.Equal(expectedName, spell.Name);
        Assert.Equal(
            $"dnd5e2014.magic-school.{expectedSchool}",
            spell.SchoolId.Value);
        Assert.Equal(expectedPage, Assert.Single(spell.Sources).Page);
    }

    // Blink is the first Minute-unit duration that is flat rather than "up
    // to" or concentration - the same distinction Shield/True Strike pin at
    // the Round unit, now shown at Minute too.
    [Fact]
    public void BlinksMinuteDurationIsFlatNotUpToOrConcentration()
    {
        SpellDuration blink = Get("dnd5e2014.spell.blink").Duration;

        Assert.Equal(SpellDurationUnit.Minute, blink.Unit);
        Assert.Equal(1, blink.Amount);
        Assert.False(blink.RequiresConcentration);
        Assert.False(blink.IsUpTo);
    }

    // Clairvoyance is the first spell whose PHB range is stated in miles
    // rather than feet. SpellRange has no separate unit - DistanceFeet is
    // always feet-denominated - so "1 mile" is canonicalized to 5,280 feet,
    // the same kind of unit canonicalization already applied elsewhere
    // (gold pieces, hit points), not a derived total like Warding Bond's
    // per-item cost.
    [Fact]
    public void ClairvoyanceRangeIsOneMileCanonicalizedToFeet()
    {
        Assert.Equal(
            5280,
            Get("dnd5e2014.spell.clairvoyance").Range.DistanceFeet);
    }

    // Plant Growth prints "Casting Time: 1 action or 8 hours" - the first
    // header with two alternative casting times, each producing a
    // different effect (an instant local burst vs. a slower, wider
    // blessing). CastingTime stores the 1-action primary value; the 8-hour
    // long-form mode is a compound alternative that changes the spell's
    // effect, so it stays in the citation, the same "content this project
    // doesn't model as its own domain" line Rock Gnome's Tinker sits on -
    // not a schema gap to fill later.
    [Fact]
    public void PlantGrowthStoresOnlyTheOneActionPrimaryCastingTime()
    {
        SpellCastingTime castingTime =
            Get("dnd5e2014.spell.plant-growth").CastingTime;

        Assert.Equal(1, castingTime.Amount);
        Assert.Equal(SpellCastingTimeUnit.Action, castingTime.Unit);
    }

    // Leomund's Tiny Hut prints "Self (10-foot-radius hemisphere)" - a
    // physical dome, geometrically distinct from a full-sphere Radius aura
    // like Aura of Vitality's. It earns its own SpellAreaShape value rather
    // than reusing Radius.
    [Fact]
    public void LeomundsTinyHutIsTheOnlyHemisphereAreaSoFar()
    {
        SpellDefinition only = Assert.Single(
            LoadCanonical()
                .Where(spell =>
                    spell.Range.AreaShape == SpellAreaShape.Hemisphere));

        Assert.Equal("dnd5e2014.spell.leomunds-tiny-hut", only.Id.Value);
        Assert.Equal(SpellRangeKind.Self, only.Range.Kind);
        Assert.Equal(10, only.Range.AreaSizeFeet);
    }

    // Sending prints "Range: Unlimited" - no distance in feet at all,
    // distinct from a large bounded Distance. Telepathy joins it at 8th
    // level, confirming this is a real PHB range category, not a one-off.
    [Fact]
    public void UnlimitedRangesAreSendingAndTelepathy()
    {
        SpellDefinition[] unlimited = LoadCanonical()
            .Where(spell => spell.Range.Kind == SpellRangeKind.Unlimited)
            .OrderBy(spell => spell.Id.Value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            ["dnd5e2014.spell.sending", "dnd5e2014.spell.telepathy"],
            unlimited.Select(spell => spell.Id.Value));

        Assert.All(
            unlimited,
            spell =>
            {
                Assert.Null(spell.Range.DistanceFeet);
                Assert.Null(spell.Range.AreaShape);
                Assert.Null(spell.Range.AreaSizeFeet);
            });
    }

    [Fact]
    public void FirstLevelContainsExactlyThePhbsSixtyTwoSpells()
    {
        Assert.Equal(
            [
                "dnd5e2014.spell.alarm",
                "dnd5e2014.spell.animal-friendship",
                "dnd5e2014.spell.armor-of-agathys",
                "dnd5e2014.spell.arms-of-hadar",
                "dnd5e2014.spell.bane",
                "dnd5e2014.spell.bless",
                "dnd5e2014.spell.burning-hands",
                "dnd5e2014.spell.charm-person",
                "dnd5e2014.spell.chromatic-orb",
                "dnd5e2014.spell.color-spray",
                "dnd5e2014.spell.command",
                "dnd5e2014.spell.compelled-duel",
                "dnd5e2014.spell.comprehend-languages",
                "dnd5e2014.spell.create-or-destroy-water",
                "dnd5e2014.spell.cure-wounds",
                "dnd5e2014.spell.detect-evil-and-good",
                "dnd5e2014.spell.detect-magic",
                "dnd5e2014.spell.detect-poison-and-disease",
                "dnd5e2014.spell.disguise-self",
                "dnd5e2014.spell.dissonant-whispers",
                "dnd5e2014.spell.divine-favor",
                "dnd5e2014.spell.ensnaring-strike",
                "dnd5e2014.spell.entangle",
                "dnd5e2014.spell.expeditious-retreat",
                "dnd5e2014.spell.faerie-fire",
                "dnd5e2014.spell.false-life",
                "dnd5e2014.spell.feather-fall",
                "dnd5e2014.spell.find-familiar",
                "dnd5e2014.spell.fog-cloud",
                "dnd5e2014.spell.goodberry",
                "dnd5e2014.spell.grease",
                "dnd5e2014.spell.guiding-bolt",
                "dnd5e2014.spell.hail-of-thorns",
                "dnd5e2014.spell.healing-word",
                "dnd5e2014.spell.hellish-rebuke",
                "dnd5e2014.spell.heroism",
                "dnd5e2014.spell.hex",
                "dnd5e2014.spell.hunters-mark",
                "dnd5e2014.spell.identify",
                "dnd5e2014.spell.illusory-script",
                "dnd5e2014.spell.inflict-wounds",
                "dnd5e2014.spell.jump",
                "dnd5e2014.spell.longstrider",
                "dnd5e2014.spell.mage-armor",
                "dnd5e2014.spell.magic-missile",
                "dnd5e2014.spell.protection-from-evil-and-good",
                "dnd5e2014.spell.purify-food-and-drink",
                "dnd5e2014.spell.ray-of-sickness",
                "dnd5e2014.spell.sanctuary",
                "dnd5e2014.spell.searing-smite",
                "dnd5e2014.spell.shield",
                "dnd5e2014.spell.shield-of-faith",
                "dnd5e2014.spell.silent-image",
                "dnd5e2014.spell.sleep",
                "dnd5e2014.spell.speak-with-animals",
                "dnd5e2014.spell.tashas-hideous-laughter",
                "dnd5e2014.spell.tensers-floating-disk",
                "dnd5e2014.spell.thunderous-smite",
                "dnd5e2014.spell.thunderwave",
                "dnd5e2014.spell.unseen-servant",
                "dnd5e2014.spell.witch-bolt",
                "dnd5e2014.spell.wrathful-smite"
            ],
            LoadCanonical()
                .Where(spell => spell.Level == 1)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Theory]
    [InlineData("dnd5e2014.spell.alarm", "Alarm", "abjuration", 211)]
    [InlineData("dnd5e2014.spell.arms-of-hadar", "Arms of Hadar", "conjuration", 215)]
    [InlineData("dnd5e2014.spell.color-spray", "Color Spray", "illusion", 222)]
    [InlineData("dnd5e2014.spell.compelled-duel", "Compelled Duel", "enchantment", 224)]
    [InlineData("dnd5e2014.spell.cure-wounds", "Cure Wounds", "evocation", 230)]
    public void FirstLevelSpell_HasExpectedNameSchoolAndPage(
        string id,
        string expectedName,
        string expectedSchool,
        int expectedPage)
    {
        SpellDefinition spell = Get(id);

        Assert.Equal(1, spell.Level);
        Assert.Equal(expectedName, spell.Name);
        Assert.Equal(
            $"dnd5e2014.magic-school.{expectedSchool}",
            spell.SchoolId.Value);
        Assert.Equal(expectedPage, Assert.Single(spell.Sources).Page);
    }

    [Fact]
    public void CompelledDuelIsCastAsABonusActionAtFirstLevel()
    {
        SpellCastingTime castingTime =
            Get("dnd5e2014.spell.compelled-duel").CastingTime;

        Assert.Equal(SpellCastingTimeUnit.BonusAction, castingTime.Unit);
        Assert.Equal(1, castingTime.Amount);
    }

    [Fact]
    public void NoCantripIsARitual()
    {
        Assert.All(
            LoadCanonical().Where(spell => spell.IsCantrip),
            spell => Assert.False(spell.IsRitual));
    }

    [Fact]
    public void RitualsAreExactlyTheTaggedSpells()
    {
        Assert.Equal(
            [
                "dnd5e2014.spell.alarm",
                "dnd5e2014.spell.animal-messenger",
                "dnd5e2014.spell.augury",
                "dnd5e2014.spell.beast-sense",
                "dnd5e2014.spell.commune",
                "dnd5e2014.spell.commune-with-nature",
                "dnd5e2014.spell.comprehend-languages",
                "dnd5e2014.spell.contact-other-plane",
                "dnd5e2014.spell.detect-magic",
                "dnd5e2014.spell.detect-poison-and-disease",
                "dnd5e2014.spell.divination",
                "dnd5e2014.spell.drawmijs-instant-summons",
                "dnd5e2014.spell.feign-death",
                "dnd5e2014.spell.find-familiar",
                "dnd5e2014.spell.forbiddance",
                "dnd5e2014.spell.gentle-repose",
                "dnd5e2014.spell.identify",
                "dnd5e2014.spell.illusory-script",
                "dnd5e2014.spell.leomunds-tiny-hut",
                "dnd5e2014.spell.locate-animals-or-plants",
                "dnd5e2014.spell.magic-mouth",
                "dnd5e2014.spell.meld-into-stone",
                "dnd5e2014.spell.phantom-steed",
                "dnd5e2014.spell.purify-food-and-drink",
                "dnd5e2014.spell.rarys-telepathic-bond",
                "dnd5e2014.spell.silence",
                "dnd5e2014.spell.speak-with-animals",
                "dnd5e2014.spell.tensers-floating-disk",
                "dnd5e2014.spell.unseen-servant",
                "dnd5e2014.spell.water-breathing",
                "dnd5e2014.spell.water-walk"
            ],
            LoadCanonical()
                .Where(spell => spell.IsRitual)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void AreaOfEffectRangesAreSelfRangedWithShapeAndSize()
    {
        SpellDefinition[] areas = LoadCanonical()
            .Where(spell => spell.Range.AreaShape is not null)
            .OrderBy(spell => spell.Id.Value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "dnd5e2014.spell.antilife-shell",
                "dnd5e2014.spell.antimagic-field",
                "dnd5e2014.spell.arms-of-hadar",
                "dnd5e2014.spell.aura-of-life",
                "dnd5e2014.spell.aura-of-purity",
                "dnd5e2014.spell.aura-of-vitality",
                "dnd5e2014.spell.burning-hands",
                "dnd5e2014.spell.circle-of-power",
                "dnd5e2014.spell.color-spray",
                "dnd5e2014.spell.cone-of-cold",
                "dnd5e2014.spell.conjure-barrage",
                "dnd5e2014.spell.control-weather",
                "dnd5e2014.spell.destructive-wave",
                "dnd5e2014.spell.fear",
                "dnd5e2014.spell.globe-of-invulnerability",
                "dnd5e2014.spell.gust-of-wind",
                "dnd5e2014.spell.leomunds-tiny-hut",
                "dnd5e2014.spell.lightning-bolt",
                "dnd5e2014.spell.prismatic-spray",
                "dnd5e2014.spell.speak-with-plants",
                "dnd5e2014.spell.spirit-guardians",
                "dnd5e2014.spell.sunbeam",
                "dnd5e2014.spell.thunderwave"
            ],
            areas.Select(spell => spell.Id.Value));

        Assert.All(
            areas,
            spell =>
            {
                Assert.Equal(SpellRangeKind.Self, spell.Range.Kind);
                Assert.NotNull(spell.Range.AreaSizeFeet);
            });

        Assert.Equal(
            SpellAreaShape.Radius,
            Get("dnd5e2014.spell.arms-of-hadar").Range.AreaShape);
        Assert.Equal(
            SpellAreaShape.Cone,
            Get("dnd5e2014.spell.burning-hands").Range.AreaShape);
        Assert.Equal(
            15,
            Get("dnd5e2014.spell.burning-hands").Range.AreaSizeFeet);
        Assert.Equal(
            SpellAreaShape.Hemisphere,
            Get("dnd5e2014.spell.leomunds-tiny-hut").Range.AreaShape);
        Assert.Equal(
            SpellAreaShape.Line,
            Get("dnd5e2014.spell.lightning-bolt").Range.AreaShape);
    }

    [Fact]
    public void CostedMaterialsAreTrackedSeparatelyFromConsumedOnes()
    {
        // Chromatic Orb's 50 gp diamond is kept; Find Familiar's 10 gp of
        // charcoal and incense is destroyed. Cost and consumption are
        // independent facts, which is why they are separate fields.
        Assert.Equal(
            [
                "dnd5e2014.spell.arcane-lock",
                "dnd5e2014.spell.augury",
                "dnd5e2014.spell.awaken",
                "dnd5e2014.spell.chromatic-orb",
                "dnd5e2014.spell.circle-of-death",
                "dnd5e2014.spell.clairvoyance",
                "dnd5e2014.spell.contingency",
                "dnd5e2014.spell.continual-flame",
                "dnd5e2014.spell.create-undead",
                "dnd5e2014.spell.divination",
                "dnd5e2014.spell.drawmijs-instant-summons",
                "dnd5e2014.spell.find-familiar",
                "dnd5e2014.spell.find-the-path",
                "dnd5e2014.spell.forbiddance",
                "dnd5e2014.spell.forcecage",
                "dnd5e2014.spell.gate",
                "dnd5e2014.spell.glyph-of-warding",
                "dnd5e2014.spell.greater-restoration",
                "dnd5e2014.spell.guards-and-wards",
                "dnd5e2014.spell.hallow",
                "dnd5e2014.spell.heroes-feast",
                "dnd5e2014.spell.holy-aura",
                "dnd5e2014.spell.identify",
                "dnd5e2014.spell.illusory-script",
                "dnd5e2014.spell.magic-circle",
                "dnd5e2014.spell.magic-jar",
                "dnd5e2014.spell.magic-mouth",
                "dnd5e2014.spell.mordenkainens-magnificent-mansion",
                "dnd5e2014.spell.mordenkainens-sword",
                "dnd5e2014.spell.nondetection",
                "dnd5e2014.spell.planar-binding",
                "dnd5e2014.spell.plane-shift",
                "dnd5e2014.spell.programmed-illusion",
                "dnd5e2014.spell.project-image",
                "dnd5e2014.spell.raise-dead",
                "dnd5e2014.spell.reincarnate",
                "dnd5e2014.spell.resurrection",
                "dnd5e2014.spell.revivify",
                "dnd5e2014.spell.scrying",
                "dnd5e2014.spell.sequester",
                "dnd5e2014.spell.shapechange",
                "dnd5e2014.spell.simulacrum",
                "dnd5e2014.spell.stoneskin",
                "dnd5e2014.spell.symbol",
                "dnd5e2014.spell.teleportation-circle",
                "dnd5e2014.spell.true-resurrection",
                "dnd5e2014.spell.true-seeing",
                "dnd5e2014.spell.warding-bond"
            ],
            LoadCanonical()
                .Where(spell =>
                    spell.Components.MaterialCostGoldPieces is not null)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));

        // Protection from Evil and Good is consumed with no stated cost —
        // all four cost/consumed combinations now appear, which is the
        // evidence these are two fields rather than one.
        SpellComponents protection =
            Get("dnd5e2014.spell.protection-from-evil-and-good").Components;
        Assert.Null(protection.MaterialCostGoldPieces);
        Assert.True(protection.MaterialIsConsumed);

        SpellComponents orb = Get("dnd5e2014.spell.chromatic-orb").Components;
        Assert.Equal(50, orb.MaterialCostGoldPieces);
        Assert.False(orb.MaterialIsConsumed);

        SpellComponents familiar =
            Get("dnd5e2014.spell.find-familiar").Components;
        Assert.Equal(10, familiar.MaterialCostGoldPieces);
        Assert.True(familiar.MaterialIsConsumed);
    }

    // Counterspell joins the three 1st/2nd-level reaction spells at 3rd
    // level; it is also the first reaction spell with no verbal component
    // (Somatic only).
    [Fact]
    public void ReactionSpellsAreCounterspellFeatherFallHellishRebukeAndShield()
    {
        Assert.Equal(
            [
                "dnd5e2014.spell.counterspell",
                "dnd5e2014.spell.feather-fall",
                "dnd5e2014.spell.hellish-rebuke",
                "dnd5e2014.spell.shield"
            ],
            LoadCanonical()
                .Where(spell => spell.CastingTime.Unit
                    == SpellCastingTimeUnit.Reaction)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    // Thunderwave drove the Cube area shape; it is the first spell whose
    // area is neither a cone nor a radius. Lightning Bolt's line still
    // awaits a 3rd-level batch.
    [Fact]
    public void ThunderwaveIsTheOnlyCubeAreaSoFar()
    {
        SpellDefinition only = Assert.Single(
            LoadCanonical()
                .Where(spell =>
                    spell.Range.AreaShape == SpellAreaShape.Cube));

        Assert.Equal("dnd5e2014.spell.thunderwave", only.Id.Value);
        Assert.Equal(SpellRangeKind.Self, only.Range.Kind);
        Assert.Equal(15, only.Range.AreaSizeFeet);
    }

    // Shield's round is a flat duration, not an "up to" one — the same unit
    // True Strike uses on the concentration side of that distinction. Both
    // are pinned so the two never get collapsed.
    [Fact]
    public void ShieldsRoundDurationIsFlatWhileTrueStrikesIsUpTo()
    {
        SpellDuration shield = Get("dnd5e2014.spell.shield").Duration;
        SpellDuration trueStrike = Get("dnd5e2014.spell.true-strike").Duration;

        Assert.Equal(SpellDurationUnit.Round, shield.Unit);
        Assert.Equal(1, shield.Amount);
        Assert.False(shield.RequiresConcentration);
        Assert.False(shield.IsUpTo);

        Assert.Equal(SpellDurationUnit.Round, trueStrike.Unit);
        Assert.True(trueStrike.RequiresConcentration);
        Assert.True(trueStrike.IsUpTo);
    }

    // Every Round-unit duration before Tsunami was a flat amount of 1
    // (Shield, True Strike). Tsunami's "Concentration, up to 6 rounds" is
    // the first Round-unit amount greater than 1 - the field already
    // allowed it, the same non-event as Find Steed's 10-minute casting
    // time at first level.
    [Fact]
    public void TsunamiIsTheFirstRoundDurationLongerThanOne()
    {
        SpellDuration tsunami = Get("dnd5e2014.spell.tsunami").Duration;

        Assert.Equal(SpellDurationUnit.Round, tsunami.Unit);
        Assert.Equal(6, tsunami.Amount);
        Assert.True(tsunami.RequiresConcentration);
        Assert.True(tsunami.IsUpTo);
    }

    // The four Paladin smites are the only 1st-level spells available to
    // exactly one class that is not a full caster, and all four share the
    // same header shape: bonus action, Self, verbal only, concentration up
    // to 1 minute.
    [Theory]
    [InlineData("dnd5e2014.spell.searing-smite")]
    [InlineData("dnd5e2014.spell.thunderous-smite")]
    [InlineData("dnd5e2014.spell.wrathful-smite")]
    public void PaladinSmitesShareTheSameHeaderShape(string id)
    {
        SpellDefinition smite = Get(id);

        Assert.Equal(
            "dnd5e2014.class.paladin",
            Assert.Single(smite.AvailableToClassIds).Value);
        Assert.Equal(SpellCastingTimeUnit.BonusAction, smite.CastingTime.Unit);
        Assert.Equal(SpellRangeKind.Self, smite.Range.Kind);
        Assert.True(smite.Components.Verbal);
        Assert.False(smite.Components.Somatic);
        Assert.False(smite.Components.Material);
        Assert.True(smite.Duration.RequiresConcentration);
        Assert.Equal(1, smite.Duration.Amount);
        Assert.Equal(SpellDurationUnit.Minute, smite.Duration.Unit);
    }

    [Theory]
    [InlineData("dnd5e2014.spell.ray-of-sickness", "Ray of Sickness", "necromancy", 271)]
    [InlineData("dnd5e2014.spell.sanctuary", "Sanctuary", "abjuration", 272)]
    [InlineData("dnd5e2014.spell.silent-image", "Silent Image", "illusion", 276)]
    [InlineData("dnd5e2014.spell.speak-with-animals", "Speak with Animals", "divination", 277)]
    [InlineData("dnd5e2014.spell.tensers-floating-disk", "Tenser's Floating Disk", "conjuration", 282)]
    [InlineData("dnd5e2014.spell.witch-bolt", "Witch Bolt", "evocation", 289)]
    public void RThroughWSpell_HasExpectedNameSchoolAndPage(
        string id,
        string expectedName,
        string expectedSchool,
        int expectedPage)
    {
        SpellDefinition spell = Get(id);

        Assert.Equal(1, spell.Level);
        Assert.Equal(expectedName, spell.Name);
        Assert.Equal(
            $"dnd5e2014.magic-school.{expectedSchool}",
            spell.SchoolId.Value);
        Assert.Equal(expectedPage, Assert.Single(spell.Sources).Page);
    }

    // Thunderwave reaches four classes, the widest 1st-level spell list
    // membership in the book; the smites reach one. Both are read off the
    // Chapter 11 class spell lists, not the spell description.
    [Fact]
    public void ThunderwaveIsAvailableToFourClasses()
    {
        Assert.Equal(
            [
                "dnd5e2014.class.bard",
                "dnd5e2014.class.druid",
                "dnd5e2014.class.sorcerer",
                "dnd5e2014.class.wizard"
            ],
            Get("dnd5e2014.spell.thunderwave").AvailableToClassIds
                .Select(id => id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    // Illusory Script and Gentle Repose share the same 10-day span; Geas
    // (30 days) and Contagion (7 days) broke the "always 10" pattern at
    // fifth level. Contingency (10 days) rejoins the 10-day group at
    // sixth, and Mirage Arcane joins it at seventh. Find the Path is the
    // first Day-unit duration that requires concentration
    // ("Concentration, up to 1 day") rather than being a flat span - the
    // same distinction Shield/True Strike already pin at the Round unit,
    // now shown at Day too; Forbiddance stays flat at 1 day, so the two
    // facts (unit vs. concentration) are independent here as well. Project
    // Image is the second concentration Day-unit duration, also "up to 1
    // day".
    [Fact]
    public void DayLongDurationsCoverThreeDistinctSpans()
    {
        SpellDefinition[] days = LoadCanonical()
            .Where(spell => spell.Duration.Unit == SpellDurationUnit.Day)
            .OrderBy(spell => spell.Id.Value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "dnd5e2014.spell.antipathy-sympathy",
                "dnd5e2014.spell.contagion",
                "dnd5e2014.spell.contingency",
                "dnd5e2014.spell.find-the-path",
                "dnd5e2014.spell.forbiddance",
                "dnd5e2014.spell.geas",
                "dnd5e2014.spell.gentle-repose",
                "dnd5e2014.spell.illusory-script",
                "dnd5e2014.spell.mirage-arcane",
                "dnd5e2014.spell.project-image"
            ],
            days.Select(spell => spell.Id.Value));

        Assert.Equal(
            10,
            Get("dnd5e2014.spell.antipathy-sympathy").Duration.Amount);
        Assert.Equal(7, Get("dnd5e2014.spell.contagion").Duration.Amount);
        Assert.Equal(10, Get("dnd5e2014.spell.contingency").Duration.Amount);
        Assert.Equal(30, Get("dnd5e2014.spell.geas").Duration.Amount);
        Assert.Equal(10, Get("dnd5e2014.spell.gentle-repose").Duration.Amount);
        Assert.Equal(10, Get("dnd5e2014.spell.illusory-script").Duration.Amount);
        Assert.Equal(10, Get("dnd5e2014.spell.mirage-arcane").Duration.Amount);

        SpellDuration projectImage = Get("dnd5e2014.spell.project-image")
            .Duration;
        Assert.Equal(1, projectImage.Amount);
        Assert.True(projectImage.RequiresConcentration);
        Assert.True(projectImage.IsUpTo);

        SpellDuration findThePath = Get("dnd5e2014.spell.find-the-path")
            .Duration;
        Assert.Equal(1, findThePath.Amount);
        Assert.True(findThePath.RequiresConcentration);
        Assert.True(findThePath.IsUpTo);

        SpellDuration forbiddance = Get("dnd5e2014.spell.forbiddance")
            .Duration;
        Assert.Equal(1, forbiddance.Amount);
        Assert.False(forbiddance.RequiresConcentration);
        Assert.False(forbiddance.IsUpTo);
    }

    // Gust of Wind drove the Line area shape at second level; Lightning
    // Bolt brought the second, longest example (100 feet vs. Gust of
    // Wind's 60); Sunbeam is the third and, at 6th level, the first
    // outside cantrip/1st/2nd level, matching Gust of Wind's 60 feet.
    [Fact]
    public void LineAreasAreGustOfWindLightningBoltAndSunbeam()
    {
        SpellDefinition[] lines = LoadCanonical()
            .Where(spell => spell.Range.AreaShape == SpellAreaShape.Line)
            .OrderBy(spell => spell.Id.Value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "dnd5e2014.spell.gust-of-wind",
                "dnd5e2014.spell.lightning-bolt",
                "dnd5e2014.spell.sunbeam"
            ],
            lines.Select(spell => spell.Id.Value));

        Assert.All(
            lines,
            spell => Assert.Equal(SpellRangeKind.Self, spell.Range.Kind));

        Assert.Equal(
            60,
            Get("dnd5e2014.spell.gust-of-wind").Range.AreaSizeFeet);
        Assert.Equal(
            100,
            Get("dnd5e2014.spell.lightning-bolt").Range.AreaSizeFeet);
        Assert.Equal(
            60,
            Get("dnd5e2014.spell.sunbeam").Range.AreaSizeFeet);
    }

    // Find Steed's "10 minutes" was the first casting time whose amount is
    // not 1 - the field always allowed it, but no built spell exercised it.
    // Every Minute-unit one built since is 10 minutes too; the Hour-unit
    // amounts (Awaken, Hallow, Planar Binding) are pinned separately by
    // HourLongCastingTimesSpanSevenSpellsAndThreeAmounts.
    [Fact]
    public void NonOneMinuteCastingTimesAreAllTenMinutes()
    {
        SpellDefinition[] slow = LoadCanonical()
            .Where(spell => spell.CastingTime.Amount != 1
                && spell.CastingTime.Unit == SpellCastingTimeUnit.Minute)
            .OrderBy(spell => spell.Id.Value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "dnd5e2014.spell.clairvoyance",
                "dnd5e2014.spell.contingency",
                "dnd5e2014.spell.control-weather",
                "dnd5e2014.spell.fabricate",
                "dnd5e2014.spell.find-steed",
                "dnd5e2014.spell.forbiddance",
                "dnd5e2014.spell.guards-and-wards",
                "dnd5e2014.spell.hallucinatory-terrain",
                "dnd5e2014.spell.heroes-feast",
                "dnd5e2014.spell.legend-lore",
                "dnd5e2014.spell.mirage-arcane",
                "dnd5e2014.spell.mordenkainens-private-sanctum",
                "dnd5e2014.spell.planar-ally",
                "dnd5e2014.spell.prayer-of-healing",
                "dnd5e2014.spell.scrying"
            ],
            slow.Select(spell => spell.Id.Value));

        Assert.All(
            slow,
            spell => Assert.Equal(10, spell.CastingTime.Amount));
    }

    // Hold Person reaches six classes, the widest membership in the book so
    // far - three more than Thunderwave's four at first level.
    [Fact]
    public void HoldPersonIsAvailableToSixClasses()
    {
        Assert.Equal(
            [
                "dnd5e2014.class.bard",
                "dnd5e2014.class.cleric",
                "dnd5e2014.class.druid",
                "dnd5e2014.class.sorcerer",
                "dnd5e2014.class.warlock",
                "dnd5e2014.class.wizard"
            ],
            Get("dnd5e2014.spell.hold-person").AvailableToClassIds
                .Select(id => id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    // Warding Bond is the first costed material whose printed figure is
    // per item rather than total: "a pair of platinum rings worth at least
    // 50 gp each". The field stores the figure the PHB prints, 50, not the
    // derived 100 - the "each" survives in MaterialDescription, so a
    // consumer that wants the total can still get there.
    [Fact]
    public void WardingBondStoresThePrintedPerItemCostNotTheTotal()
    {
        SpellComponents components = Get("dnd5e2014.spell.warding-bond")
            .Components;

        Assert.Equal(50, components.MaterialCostGoldPieces);
        Assert.False(components.MaterialIsConsumed);
        Assert.Contains(
            "each",
            components.MaterialDescription!,
            StringComparison.Ordinal);
    }

    // Verbal and material with no somatic component is the rarest of the
    // six V/S/M combinations the built set uses - seven spells across six
    // levels, Tongues joining at third, Teleportation Circle at fifth, and
    // Mass Suggestion at sixth. Pinned because a reader scanning
    // components is prone to assume material implies somatic.
    [Fact]
    public void VerbalAndMaterialWithoutSomaticIsTheRarestCombination()
    {
        Assert.Equal(
            [
                "dnd5e2014.spell.darkness",
                "dnd5e2014.spell.feather-fall",
                "dnd5e2014.spell.light",
                "dnd5e2014.spell.mass-suggestion",
                "dnd5e2014.spell.suggestion",
                "dnd5e2014.spell.teleportation-circle",
                "dnd5e2014.spell.tongues"
            ],
            LoadCanonical()
                .Where(spell => spell.Components.Verbal
                    && !spell.Components.Somatic
                    && spell.Components.Material)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    // Find Familiar was the only Hour-long casting time until Glyph of
    // Warding joined it at third level; Find Familiar is a ritual and
    // Glyph of Warding isn't, so the two facts stay independent. Awaken's
    // "8 hours" is the first Hour-unit casting time whose amount isn't 1;
    // Hallow (24) and Planar Binding (1) followed, and Raise Dead and
    // Reincarnate (both 1) closed out fifth level's Hour-unit spells.
    // Resurrection (1 hour) and Simulacrum (12 hours, a fourth amount)
    // join at seventh; Antipathy/Sympathy and Clone (both 1 hour) join at
    // eighth; Astral Projection and True Resurrection (both 1 hour) join
    // at ninth.
    [Fact]
    public void HourLongCastingTimesSpanThirteenSpellsAndFourAmounts()
    {
        SpellDefinition[] hourLong = LoadCanonical()
            .Where(spell => spell.CastingTime.Unit
                == SpellCastingTimeUnit.Hour)
            .OrderBy(spell => spell.Id.Value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "dnd5e2014.spell.antipathy-sympathy",
                "dnd5e2014.spell.astral-projection",
                "dnd5e2014.spell.awaken",
                "dnd5e2014.spell.clone",
                "dnd5e2014.spell.find-familiar",
                "dnd5e2014.spell.glyph-of-warding",
                "dnd5e2014.spell.hallow",
                "dnd5e2014.spell.planar-binding",
                "dnd5e2014.spell.raise-dead",
                "dnd5e2014.spell.reincarnate",
                "dnd5e2014.spell.resurrection",
                "dnd5e2014.spell.simulacrum",
                "dnd5e2014.spell.true-resurrection"
            ],
            hourLong.Select(spell => spell.Id.Value));

        Assert.True(Get("dnd5e2014.spell.find-familiar").IsRitual);
        Assert.False(Get("dnd5e2014.spell.glyph-of-warding").IsRitual);
        Assert.False(Get("dnd5e2014.spell.awaken").IsRitual);
        Assert.Equal(8, Get("dnd5e2014.spell.awaken").CastingTime.Amount);
        Assert.Equal(24, Get("dnd5e2014.spell.hallow").CastingTime.Amount);
        Assert.Equal(
            1,
            Get("dnd5e2014.spell.planar-binding").CastingTime.Amount);
        Assert.Equal(
            12,
            Get("dnd5e2014.spell.simulacrum").CastingTime.Amount);
    }

    [Fact]
    public void EverySpell_CitesTheSpellDescriptionsSection()
    {
        foreach (SpellDefinition spell in LoadCanonical())
        {
            SourceReference source = Assert.Single(spell.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.InRange(source.Page!.Value, 211, 289);
            Assert.Equal(
                $"Chapter 11: Spells — Spell Descriptions — {spell.Name}",
                source.Section);
        }
    }

    [Fact]
    public void EverySchoolOfMagic_IsExercisedByACantrip()
    {
        Assert.Equal(
            8,
            LoadCanonical()
                .Where(spell => spell.IsCantrip)
                .Select(spell => spell.SchoolId.Value)
                .Distinct()
                .Count());
    }

    [Theory]
    [InlineData("dnd5e2014.spell.acid-splash", "Acid Splash", "conjuration", 211)]
    [InlineData("dnd5e2014.spell.eldritch-blast", "Eldritch Blast", "evocation", 237)]
    [InlineData("dnd5e2014.spell.mending", "Mending", "transmutation", 259)]
    [InlineData("dnd5e2014.spell.minor-illusion", "Minor Illusion", "illusion", 260)]
    [InlineData("dnd5e2014.spell.true-strike", "True Strike", "divination", 284)]
    [InlineData("dnd5e2014.spell.vicious-mockery", "Vicious Mockery", "enchantment", 285)]
    public void Spell_HasExpectedNameSchoolAndPage(
        string id,
        string expectedName,
        string expectedSchool,
        int expectedPage)
    {
        SpellDefinition spell = Get(id);

        Assert.Equal(expectedName, spell.Name);
        Assert.Equal(
            $"dnd5e2014.magic-school.{expectedSchool}",
            spell.SchoolId.Value);
        Assert.Equal(expectedPage, Assert.Single(spell.Sources).Page);
    }

    [Fact]
    public void MendingIsTheOnlyCantripCastOverAMinute()
    {
        SpellDefinition[] slow = LoadCanonical()
            .Where(spell => spell.IsCantrip
                && spell.CastingTime.Unit == SpellCastingTimeUnit.Minute)
            .ToArray();

        Assert.Equal(
            ["dnd5e2014.spell.mending"],
            slow.Select(spell => spell.Id.Value));
    }

    [Fact]
    public void ShillelaghIsTheOnlyCantripCastAsABonusAction()
    {
        SpellDefinition[] bonus = LoadCanonical()
            .Where(spell => spell.IsCantrip
                && spell.CastingTime.Unit
                    == SpellCastingTimeUnit.BonusAction)
            .ToArray();

        Assert.Equal(
            ["dnd5e2014.spell.shillelagh"],
            bonus.Select(spell => spell.Id.Value));
    }

    [Fact]
    public void ComponentCombinationsAreGenuinelyIndependent()
    {
        // Six distinct V/S/M combinations appear across the 27 cantrips,
        // which is why the three flags are independent booleans rather
        // than one enum. Friends and Minor Illusion have no verbal
        // component; Light has no somatic; Thaumaturgy and Vicious
        // Mockery are verbal only; True Strike is somatic only.
        Assert.Equal(
            6,
            LoadCanonical()
                .Where(spell => spell.IsCantrip)
                .Select(spell =>
                (
                    spell.Components.Verbal,
                    spell.Components.Somatic,
                    spell.Components.Material
                ))
                .Distinct()
                .Count());

        Assert.False(Get("dnd5e2014.spell.friends").Components.Verbal);
        Assert.False(Get("dnd5e2014.spell.light").Components.Somatic);
        Assert.False(Get("dnd5e2014.spell.true-strike").Components.Verbal);
        Assert.False(Get("dnd5e2014.spell.thaumaturgy").Components.Somatic);
    }

    [Fact]
    public void UpToDurationsExistIndependentlyOfConcentration()
    {
        SpellDuration prestidigitation =
            Get("dnd5e2014.spell.prestidigitation").Duration;
        SpellDuration trueStrike =
            Get("dnd5e2014.spell.true-strike").Duration;

        Assert.True(prestidigitation.IsUpTo);
        Assert.False(prestidigitation.RequiresConcentration);
        Assert.Equal(SpellDurationUnit.Hour, prestidigitation.Unit);

        Assert.True(trueStrike.IsUpTo);
        Assert.True(trueStrike.RequiresConcentration);
        Assert.Equal(SpellDurationUnit.Round, trueStrike.Unit);
    }

    [Fact]
    public void NeitherPaladinNorRangerHasACantrip()
    {
        string[] classIds = LoadCanonical()
            .Where(spell => spell.IsCantrip)
            .SelectMany(spell => spell.AvailableToClassIds)
            .Select(classId => classId.Value)
            .Distinct()
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.DoesNotContain("dnd5e2014.class.paladin", classIds);
        Assert.DoesNotContain("dnd5e2014.class.ranger", classIds);
        Assert.Equal(
            [
                "dnd5e2014.class.bard",
                "dnd5e2014.class.cleric",
                "dnd5e2014.class.druid",
                "dnd5e2014.class.sorcerer",
                "dnd5e2014.class.warlock",
                "dnd5e2014.class.wizard"
            ],
            classIds);
    }

    [Theory]
    [InlineData("dnd5e2014.class.bard", 11)]
    [InlineData("dnd5e2014.class.cleric", 7)]
    [InlineData("dnd5e2014.class.druid", 8)]
    [InlineData("dnd5e2014.class.sorcerer", 16)]
    [InlineData("dnd5e2014.class.warlock", 9)]
    [InlineData("dnd5e2014.class.wizard", 16)]
    public void ClassCantripListHasExpectedSize(
        string classId,
        int expectedCount)
    {
        Assert.Equal(
            expectedCount,
            LoadCanonical()
                .Count(spell => spell.IsCantrip
                    && spell.AvailableToClassIds
                        .Any(id => id.Value == classId)));
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedResourceMatchingTheDataFile()
    {
        SpellCatalog catalog = Dnd5e2014Ruleset.Instance.Spells;

        Assert.Equal(
            LoadCanonical()
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal),
            catalog.All.Select(spell => spell.Id.Value));
    }

    // 11 of the 27 cantrips deal damage directly; the other 16 are utility
    // or buff effects (or, like Light and Minor Illusion, gate a
    // conditional save/check that isn't this project's "damage dice, save
    // DC/ability, condition applied" scope) and carry no SpellDamageEffect.
    [Fact]
    public void ElevenCantripsHaveADamageEffect()
    {
        Assert.Equal(
            11,
            LoadCanonical()
                .Count(spell => spell.IsCantrip && spell.DamageEffect is not null));
    }

    [Theory]
    [InlineData("dnd5e2014.spell.blade-ward")]
    [InlineData("dnd5e2014.spell.dancing-lights")]
    [InlineData("dnd5e2014.spell.druidcraft")]
    [InlineData("dnd5e2014.spell.friends")]
    [InlineData("dnd5e2014.spell.guidance")]
    [InlineData("dnd5e2014.spell.light")]
    [InlineData("dnd5e2014.spell.mage-hand")]
    [InlineData("dnd5e2014.spell.mending")]
    [InlineData("dnd5e2014.spell.message")]
    [InlineData("dnd5e2014.spell.minor-illusion")]
    [InlineData("dnd5e2014.spell.prestidigitation")]
    [InlineData("dnd5e2014.spell.resistance")]
    [InlineData("dnd5e2014.spell.shillelagh")]
    [InlineData("dnd5e2014.spell.spare-the-dying")]
    [InlineData("dnd5e2014.spell.thaumaturgy")]
    [InlineData("dnd5e2014.spell.true-strike")]
    public void UtilityAndBuffCantrip_HasNoDamageEffect(string id)
    {
        Assert.Null(Get(id).DamageEffect);
    }

    // Every damage cantrip except Eldritch Blast increases its damage die
    // count by one at 5th, 11th, and 17th character level (p.201,
    // "Cantrips"); Eldritch Blast stays a flat 1d10 and instead gains
    // extra beams at those levels, a targeting-multiplicity fact this pass
    // declines to model (see CLAUDE.md).
    [Theory]
    [InlineData("dnd5e2014.spell.acid-splash", "acid", null, "dexterity", 6)]
    [InlineData("dnd5e2014.spell.chill-touch", "necrotic", "Ranged", null, 8)]
    [InlineData("dnd5e2014.spell.fire-bolt", "fire", "Ranged", null, 10)]
    [InlineData("dnd5e2014.spell.poison-spray", "poison", null, "constitution", 12)]
    [InlineData("dnd5e2014.spell.produce-flame", "fire", "Ranged", null, 8)]
    [InlineData("dnd5e2014.spell.ray-of-frost", "cold", "Ranged", null, 8)]
    [InlineData("dnd5e2014.spell.sacred-flame", "radiant", null, "dexterity", 8)]
    [InlineData("dnd5e2014.spell.shocking-grasp", "lightning", "Melee", null, 8)]
    [InlineData("dnd5e2014.spell.thorn-whip", "piercing", "Melee", null, 6)]
    [InlineData("dnd5e2014.spell.vicious-mockery", "psychic", null, "wisdom", 4)]
    public void ScalingDamageCantrip_HasExpectedFourTierProgression(
        string id,
        string expectedDamageType,
        string? expectedAttackRollType,
        string? expectedSavingThrowAbility,
        int expectedDieSides)
    {
        SpellDamageEffect effect = Get(id).DamageEffect!;

        Assert.Equal(
            $"dnd5e2014.damage-type.{expectedDamageType}",
            effect.DamageTypeId.Value);
        Assert.Equal(
            expectedAttackRollType,
            effect.AttackRollType?.ToString());
        Assert.Equal(
            expectedSavingThrowAbility is null
                ? null
                : $"dnd5e2014.ability.{expectedSavingThrowAbility}",
            effect.SavingThrowAbilityId?.Value);

        Assert.Equal(
            [(1, 1), (5, 2), (11, 3), (17, 4)],
            effect.DamageByCharacterLevel
                .Select(tier => (tier.CharacterLevel, tier.Damage.Count)));
        Assert.All(
            effect.DamageByCharacterLevel,
            tier => Assert.Equal(expectedDieSides, tier.Damage.Sides));
    }

    [Fact]
    public void EldritchBlastDealsAFlatOneD10RegardlessOfCharacterLevel()
    {
        SpellDamageEffect effect =
            Get("dnd5e2014.spell.eldritch-blast").DamageEffect!;

        Assert.Equal("dnd5e2014.damage-type.force", effect.DamageTypeId.Value);
        Assert.Equal(SpellAttackRollType.Ranged, effect.AttackRollType);
        Assert.Null(effect.SavingThrowAbilityId);

        SpellDamageTierGrant tier = Assert.Single(effect.DamageByCharacterLevel);
        Assert.Equal(1, tier.CharacterLevel);
        Assert.Equal(1, tier.Damage.Count);
        Assert.Equal(10, tier.Damage.Sides);
    }

    private static SpellDefinition Get(string id)
    {
        return LoadCanonical().Single(spell => spell.Id.Value == id);
    }

    private static IReadOnlyList<SpellDefinition> LoadCanonical()
    {
        return SpellDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "spells.json"));
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
