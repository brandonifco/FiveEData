using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class SpellSlotProgressionFoundationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Id_InvalidValue_IsRejected(string? value)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new SpellSlotProgressionId(value!));
    }

    [Fact]
    public void Id_ExposesValueAndStringRepresentation()
    {
        const string value = "dnd5e2014.spell-slot-progression.test";

        var id = new SpellSlotProgressionId(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void SpellSlotCount_RejectsOutOfRangeSpellLevel(int spellLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new SpellSlotCount(spellLevel, 1));
    }

    [Fact]
    public void SpellSlotCount_RejectsNonPositiveCount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new SpellSlotCount(1, 0));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void SpellSlotLevelEntry_RejectsOutOfRangeCharacterLevel(
        int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new SpellSlotLevelEntry(characterLevel, []));
    }

    [Fact]
    public void Definition_DefensivelySnapshotsEntriesAndSources()
    {
        var entries = new List<SpellSlotLevelEntry>
        {
            new(1, [new SpellSlotCount(1, 2)])
        };
        var sources = new List<SourceReference> { CreateSource() };

        SpellSlotProgressionDefinition definition = CreateOneEntry(
            "dnd5e2014.spell-slot-progression.test",
            entries,
            sources);

        entries.Clear();
        sources.Clear();

        Assert.Single(definition.LevelEntries);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsDefaultId()
    {
        SpellSlotProgressionDefinition definition = CreateFullTwentyLevels(
            null);

        Assert.Contains(
            SpellSlotProgressionDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsFewerThanTwentyLevels()
    {
        SpellSlotProgressionDefinition definition = CreateOneEntry(
            "dnd5e2014.spell-slot-progression.test",
            [new SpellSlotLevelEntry(1, [new SpellSlotCount(1, 2)])],
            [CreateSource()]);

        Assert.Contains(
            SpellSlotProgressionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "exactly one entry for each character level",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDuplicateCharacterLevel()
    {
        var entries = new List<SpellSlotLevelEntry>();

        for (int level = 1; level <= 20; level++)
        {
            entries.Add(new SpellSlotLevelEntry(level, []));
        }

        entries.Add(new SpellSlotLevelEntry(1, []));

        SpellSlotProgressionDefinition definition = CreateOneEntry(
            "dnd5e2014.spell-slot-progression.test",
            entries,
            [CreateSource()]);

        Assert.Contains(
            SpellSlotProgressionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "duplicated",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsDuplicateSpellLevelWithinEntry()
    {
        var entries = new List<SpellSlotLevelEntry>();

        for (int level = 1; level <= 20; level++)
        {
            entries.Add(
                level == 1
                    ? new SpellSlotLevelEntry(
                        level,
                        [
                            new SpellSlotCount(1, 2),
                            new SpellSlotCount(1, 3)
                        ])
                    : new SpellSlotLevelEntry(level, []));
        }

        SpellSlotProgressionDefinition definition = CreateOneEntry(
            "dnd5e2014.spell-slot-progression.test",
            entries,
            [CreateSource()]);

        Assert.Contains(
            SpellSlotProgressionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "duplicates spell level",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        SpellSlotProgressionDefinition definition = CreateOneEntry(
            "dnd5e2014.spell-slot-progression.test",
            AllTwentyLevelsWithNoSlots(),
            []);

        Assert.Contains(
            SpellSlotProgressionDefinitionValidator.Validate(definition),
            error =>
                error.Contains(
                    "source",
                    StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Catalog_NullInputIsRejected()
    {
        Assert.Throws<ArgumentNullException>(
            () => new SpellSlotProgressionCatalog(null!));
    }

    [Fact]
    public void Catalog_OrdersAndProvidesCompleteLookupSemantics()
    {
        var catalog = new SpellSlotProgressionCatalog(
            [
                CreateFullTwentyLevels(
                    "dnd5e2014.spell-slot-progression.z"),
                CreateFullTwentyLevels(
                    "dnd5e2014.spell-slot-progression.a")
            ]);

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.spell-slot-progression.a",
                "dnd5e2014.spell-slot-progression.z"
            ],
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        var aId =
            new SpellSlotProgressionId(
                "dnd5e2014.spell-slot-progression.a");

        SpellSlotProgressionDefinition found = catalog.Get(aId);

        Assert.True(
            catalog.TryGet(
                aId,
                out SpellSlotProgressionDefinition? tryFound));
        Assert.Same(found, tryFound);

        var missingId =
            new SpellSlotProgressionId(
                "dnd5e2014.spell-slot-progression.missing");

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(missingId));
        Assert.False(
            catalog.TryGet(
                missingId,
                out SpellSlotProgressionDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new SpellSlotProgressionCatalog(
                [
                    CreateFullTwentyLevels(
                        "dnd5e2014.spell-slot-progression.duplicate"),
                    CreateFullTwentyLevels(
                        "dnd5e2014.spell-slot-progression.duplicate")
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinitionAtTrustBoundary()
    {
        SpellSlotProgressionDefinition definition = CreateOneEntry(
            "dnd5e2014.spell-slot-progression.test",
            [],
            [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => new SpellSlotProgressionCatalog([definition]));
    }

    private static IReadOnlyList<SpellSlotLevelEntry>
        AllTwentyLevelsWithNoSlots()
    {
        var entries = new List<SpellSlotLevelEntry>();

        for (int level = 1; level <= 20; level++)
        {
            entries.Add(new SpellSlotLevelEntry(level, []));
        }

        return entries;
    }

    private static SpellSlotProgressionDefinition CreateFullTwentyLevels(
        string? id)
    {
        return CreateOneEntry(
            id,
            AllTwentyLevelsWithNoSlots(),
            [CreateSource()]);
    }

    private static SpellSlotProgressionDefinition CreateOneEntry(
        string? id,
        IEnumerable<SpellSlotLevelEntry> entries,
        IEnumerable<SourceReference> sources)
    {
        return new SpellSlotProgressionDefinition(
            id is null ? default : new SpellSlotProgressionId(id),
            "Test",
            SpellSlotRecoveryRest.LongRest,
            entries,
            sources);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 112);
    }
}
