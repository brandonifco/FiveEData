using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Classes.Spellcasting.Serialization;

namespace FiveEData.Tests;

public sealed class SpellSlotProgressionDataFileTests
{
    private const string FullCaster =
        "dnd5e2014.spell-slot-progression.full-caster";
    private const string HalfCaster =
        "dnd5e2014.spell-slot-progression.half-caster";
    private const string ThirdCaster =
        "dnd5e2014.spell-slot-progression.third-caster";
    private const string PactMagic =
        "dnd5e2014.spell-slot-progression.pact-magic";

    [Fact]
    public void CanonicalFile_ContainsExactProgressionClosure()
    {
        IReadOnlyList<SpellSlotProgressionDefinition> definitions =
            LoadCanonical();

        Assert.Equal(
            [FullCaster, HalfCaster, PactMagic, ThirdCaster],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void EveryProgression_DefinesAllTwentyCharacterLevels()
    {
        foreach (
            SpellSlotProgressionDefinition definition
            in LoadCanonical())
        {
            Assert.Equal(
                Enumerable.Range(1, 20),
                definition.LevelEntries
                    .Select(entry => entry.CharacterLevel)
                    .OrderBy(level => level));
        }
    }

    [Fact]
    public void FullCaster_MatchesWizardsTableAtKnownLevels()
    {
        SpellSlotProgressionDefinition definition = Get(FullCaster);

        Assert.Equal(SpellSlotRecoveryRest.LongRest, definition.RecoveryRest);

        AssertSlots(definition, 1, (1, 2));
        AssertSlots(definition, 3, (1, 4), (2, 2));
        AssertSlots(definition, 5, (1, 4), (2, 3), (3, 2));
        AssertSlots(
            definition,
            11,
            (1, 4), (2, 3), (3, 3), (4, 3), (5, 2), (6, 1));
        AssertSlots(
            definition,
            20,
            (1, 4), (2, 3), (3, 3), (4, 3), (5, 3), (6, 2), (7, 2),
            (8, 1), (9, 1));
    }

    [Fact]
    public void HalfCaster_MatchesRangerAndPaladinTablesAtKnownLevels()
    {
        SpellSlotProgressionDefinition definition = Get(HalfCaster);

        Assert.Equal(SpellSlotRecoveryRest.LongRest, definition.RecoveryRest);

        AssertSlots(definition, 1);
        AssertSlots(definition, 2, (1, 2));
        AssertSlots(definition, 5, (1, 4), (2, 2));
        AssertSlots(
            definition,
            20,
            (1, 4), (2, 3), (3, 3), (4, 3), (5, 2));
    }

    [Fact]
    public void ThirdCaster_MatchesEldritchKnightAndArcaneTricksterAtKnownLevels()
    {
        SpellSlotProgressionDefinition definition = Get(ThirdCaster);

        Assert.Equal(SpellSlotRecoveryRest.LongRest, definition.RecoveryRest);

        AssertSlots(definition, 1);
        AssertSlots(definition, 2);
        AssertSlots(definition, 3, (1, 2));
        AssertSlots(definition, 7, (1, 4), (2, 2));
        AssertSlots(
            definition,
            20,
            (1, 4), (2, 3), (3, 3), (4, 1));
    }

    [Fact]
    public void PactMagic_MatchesWarlocksTableAtKnownLevels()
    {
        SpellSlotProgressionDefinition definition = Get(PactMagic);

        Assert.Equal(
            SpellSlotRecoveryRest.ShortOrLongRest,
            definition.RecoveryRest);

        AssertSlots(definition, 1, (1, 1));
        AssertSlots(definition, 2, (1, 2));
        AssertSlots(definition, 3, (2, 2));
        AssertSlots(definition, 11, (5, 3));
        AssertSlots(definition, 17, (5, 4));
        AssertSlots(definition, 20, (5, 4));
    }

    [Theory]
    [InlineData(FullCaster, 53, 57, 65, 100, 112)]
    [InlineData(HalfCaster, 83, 90)]
    [InlineData(ThirdCaster, 75, 98)]
    [InlineData(PactMagic, 106)]
    public void Sources_CitePhbFirstPrintingAtExpectedPages(
        string id,
        params int[] expectedPages)
    {
        SpellSlotProgressionDefinition definition = Get(id);

        Assert.Equal(
            expectedPages
                .Cast<int?>()
                .OrderBy(page => page),
            definition.Sources
                .Select(source => source.Page)
                .OrderBy(page => page));

        Assert.All(
            definition.Sources,
            source =>
                Assert.Equal(
                    "dnd5e2014.source.phb-first-printing",
                    source.DocumentId.Value));
    }

    private static void AssertSlots(
        SpellSlotProgressionDefinition definition,
        int characterLevel,
        params (int SpellLevel, int Count)[] expectedSlots)
    {
        SpellSlotLevelEntry entry = definition.LevelEntries
            .Single(entry => entry.CharacterLevel == characterLevel);

        Assert.Equal(
            expectedSlots
                .OrderBy(slot => slot.SpellLevel)
                .Select(slot => (slot.SpellLevel, slot.Count)),
            entry.Slots
                .OrderBy(slot => slot.SpellLevel)
                .Select(slot => (slot.SpellLevel, slot.Count)));
    }

    private static SpellSlotProgressionDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<SpellSlotProgressionDefinition>
        LoadCanonical()
    {
        return SpellSlotProgressionDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "spell-slot-progressions.json"));
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
}
