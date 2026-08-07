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
        Assert.Equal(42, LoadCanonical().Count);
        Assert.Equal(
            27,
            LoadCanonical().Count(spell => spell.Level == 0));
        Assert.Equal(
            15,
            LoadCanonical().Count(spell => spell.Level == 1));
    }

    [Fact]
    public void CanonicalFile_ContainsOnlyCantripsAndFirstLevelForNow()
    {
        // Levels 2-9 are not built yet. First-level coverage stops at
        // Cure Wounds; see CLAUDE.md for the alphabetical batch plan.
        Assert.All(
            LoadCanonical(),
            spell => Assert.InRange(spell.Level, 0, 1));
    }

    [Fact]
    public void FirstLevelBatchContainsExactlyTheAThroughCSpells()
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
                "dnd5e2014.spell.cure-wounds"
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
    public void RitualsAreExactlyTheTaggedFirstLevelSpells()
    {
        Assert.Equal(
            ["dnd5e2014.spell.alarm", "dnd5e2014.spell.comprehend-languages"],
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
                "dnd5e2014.spell.burning-hands",
                "dnd5e2014.spell.color-spray"
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
    public void ChromaticOrbIsTheOnlyCostedMaterialSoFar()
    {
        SpellDefinition[] costed = LoadCanonical()
            .Where(spell =>
                spell.Components.MaterialCostGoldPieces is not null)
            .ToArray();

        SpellDefinition only = Assert.Single(costed);

        Assert.Equal("dnd5e2014.spell.chromatic-orb", only.Id.Value);
        Assert.Equal(50, only.Components.MaterialCostGoldPieces);
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
