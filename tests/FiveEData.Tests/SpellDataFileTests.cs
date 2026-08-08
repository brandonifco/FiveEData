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
        Assert.Equal(161, LoadCanonical().Count);
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
            13,
            LoadCanonical().Count(spell => spell.Level == 3));
    }

    [Fact]
    public void CanonicalFile_ContainsOnlyCantripsThroughThirdLevelForNow()
    {
        // Levels 4-9 are not built yet. Levels 0-2 are complete; level 3 is
        // being added in alphabetical batches (see CLAUDE.md) and currently
        // holds only the A-C batch, 13 of the PHB's 50 third-level spells.
        Assert.All(
            LoadCanonical(),
            spell => Assert.InRange(spell.Level, 0, 3));
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
    // the first that is neither instantaneous nor a span; Magic Mouth joins
    // them. All three are costed *and* consumed - so far the two facts have
    // never come apart on an "until dispelled" spell - and Continual Flame
    // carries a third PHB cost phrasing, "ruby dust worth 50 gp", with no
    // "at least".
    [Fact]
    public void UntilDispelledSpellsAreCostedAndConsumed()
    {
        SpellDefinition[] dispelled = LoadCanonical()
            .Where(spell => spell.Duration.IsUntilDispelled)
            .OrderBy(spell => spell.Id.Value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "dnd5e2014.spell.arcane-lock",
                "dnd5e2014.spell.continual-flame",
                "dnd5e2014.spell.magic-mouth"
            ],
            dispelled.Select(spell => spell.Id.Value));

        Assert.All(
            dispelled,
            spell =>
            {
                Assert.False(spell.Duration.IsInstantaneous);
                Assert.Null(spell.Duration.Amount);
                Assert.Null(spell.Duration.Unit);
                Assert.True(spell.Components.MaterialIsConsumed);
                Assert.NotNull(spell.Components.MaterialCostGoldPieces);
            });

        Assert.Equal(
            25,
            Get("dnd5e2014.spell.arcane-lock")
                .Components.MaterialCostGoldPieces);
        Assert.Equal(
            50,
            Get("dnd5e2014.spell.continual-flame")
                .Components.MaterialCostGoldPieces);
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
    // 50 spells across the 8 classes. This pins the A-C batch only; later
    // batches will extend the list until all 50 are built.
    [Fact]
    public void ThirdLevelSpellIdsBuiltSoFarAreTheACBatch()
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
                "dnd5e2014.spell.crusaders-mantle"
            ],
            LoadCanonical()
                .Where(spell => spell.Level == 3)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Theory]
    [InlineData("dnd5e2014.spell.animate-dead", "Animate Dead", "necromancy", 212)]
    [InlineData("dnd5e2014.spell.aura-of-vitality", "Aura of Vitality", "evocation", 216)]
    [InlineData("dnd5e2014.spell.clairvoyance", "Clairvoyance", "divination", 222)]
    [InlineData("dnd5e2014.spell.conjure-barrage", "Conjure Barrage", "conjuration", 225)]
    [InlineData("dnd5e2014.spell.counterspell", "Counterspell", "abjuration", 228)]
    [InlineData("dnd5e2014.spell.crusaders-mantle", "Crusader's Mantle", "evocation", 230)]
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
                "dnd5e2014.spell.comprehend-languages",
                "dnd5e2014.spell.detect-magic",
                "dnd5e2014.spell.detect-poison-and-disease",
                "dnd5e2014.spell.find-familiar",
                "dnd5e2014.spell.gentle-repose",
                "dnd5e2014.spell.identify",
                "dnd5e2014.spell.illusory-script",
                "dnd5e2014.spell.locate-animals-or-plants",
                "dnd5e2014.spell.magic-mouth",
                "dnd5e2014.spell.purify-food-and-drink",
                "dnd5e2014.spell.silence",
                "dnd5e2014.spell.speak-with-animals",
                "dnd5e2014.spell.tensers-floating-disk",
                "dnd5e2014.spell.unseen-servant"
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
                "dnd5e2014.spell.arms-of-hadar",
                "dnd5e2014.spell.aura-of-vitality",
                "dnd5e2014.spell.burning-hands",
                "dnd5e2014.spell.color-spray",
                "dnd5e2014.spell.conjure-barrage",
                "dnd5e2014.spell.gust-of-wind",
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
                "dnd5e2014.spell.chromatic-orb",
                "dnd5e2014.spell.clairvoyance",
                "dnd5e2014.spell.continual-flame",
                "dnd5e2014.spell.find-familiar",
                "dnd5e2014.spell.identify",
                "dnd5e2014.spell.illusory-script",
                "dnd5e2014.spell.magic-mouth",
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

    [Fact]
    public void DayLongDurationsAreIllusoryScriptAndGentleRepose()
    {
        SpellDefinition[] days = LoadCanonical()
            .Where(spell => spell.Duration.Unit == SpellDurationUnit.Day)
            .OrderBy(spell => spell.Id.Value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "dnd5e2014.spell.gentle-repose",
                "dnd5e2014.spell.illusory-script"
            ],
            days.Select(spell => spell.Id.Value));

        Assert.All(
            days,
            spell =>
            {
                Assert.Equal(10, spell.Duration.Amount);
                Assert.False(spell.Duration.RequiresConcentration);
            });
    }

    // Gust of Wind drove the Line area shape, which CLAUDE.md had expected
    // Lightning Bolt to bring at third level. It is the only line so far.
    [Fact]
    public void GustOfWindIsTheOnlyLineAreaSoFar()
    {
        SpellDefinition only = Assert.Single(
            LoadCanonical()
                .Where(spell =>
                    spell.Range.AreaShape == SpellAreaShape.Line));

        Assert.Equal("dnd5e2014.spell.gust-of-wind", only.Id.Value);
        Assert.Equal(SpellRangeKind.Self, only.Range.Kind);
        Assert.Equal(60, only.Range.AreaSizeFeet);
    }

    // Find Steed's "10 minutes" was the first casting time whose amount is
    // not 1 - the field always allowed it, but no built spell exercised it.
    // Prayer of Healing and Clairvoyance are the second and third, and all
    // three are 10 minutes.
    [Fact]
    public void CastingTimesWhoseAmountIsNotOneAreAllTenMinutes()
    {
        SpellDefinition[] slow = LoadCanonical()
            .Where(spell => spell.CastingTime.Amount != 1)
            .OrderBy(spell => spell.Id.Value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "dnd5e2014.spell.clairvoyance",
                "dnd5e2014.spell.find-steed",
                "dnd5e2014.spell.prayer-of-healing"
            ],
            slow.Select(spell => spell.Id.Value));

        Assert.All(
            slow,
            spell =>
            {
                Assert.Equal(10, spell.CastingTime.Amount);
                Assert.Equal(
                    SpellCastingTimeUnit.Minute,
                    spell.CastingTime.Unit);
            });
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
    // six V/S/M combinations the built set uses - four spells across three
    // levels. Pinned because a reader scanning components is prone to
    // assume material implies somatic.
    [Fact]
    public void VerbalAndMaterialWithoutSomaticIsTheRarestCombination()
    {
        Assert.Equal(
            [
                "dnd5e2014.spell.darkness",
                "dnd5e2014.spell.feather-fall",
                "dnd5e2014.spell.light",
                "dnd5e2014.spell.suggestion"
            ],
            LoadCanonical()
                .Where(spell => spell.Components.Verbal
                    && !spell.Components.Somatic
                    && spell.Components.Material)
                .Select(spell => spell.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void FindFamiliarIsTheOnlyHourLongCastingSoFar()
    {
        SpellDefinition only = Assert.Single(
            LoadCanonical()
                .Where(spell => spell.CastingTime.Unit
                    == SpellCastingTimeUnit.Hour));

        Assert.Equal("dnd5e2014.spell.find-familiar", only.Id.Value);
        Assert.True(only.IsRitual);
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
