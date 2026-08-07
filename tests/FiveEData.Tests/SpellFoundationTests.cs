using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Spells;
using FiveEData.Rules.Spells.MagicSchools;

namespace FiveEData.Tests;

public sealed class SpellFoundationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Id_RejectsEmptyValue(string value)
    {
        Assert.Throws<ArgumentException>(() => new SpellId(value));
    }

    [Fact]
    public void Definition_CopiesCollectionsDefensively()
    {
        var classes = new List<ClassId>
        {
            new("dnd5e2014.class.wizard")
        };
        var sources = new List<SourceReference> { TestSource() };

        SpellDefinition definition =
            Create(classes: classes, sources: sources);

        classes.Add(new ClassId("dnd5e2014.class.bard"));
        sources.Add(TestSource());

        Assert.Single(definition.AvailableToClassIds);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Definition_TreatsLevelZeroAsCantrip()
    {
        Assert.True(Create(level: 0).IsCantrip);
        Assert.False(Create(level: 1).IsCantrip);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(10)]
    public void Validator_RejectsLevelOutsideZeroThroughNine(int level)
    {
        Assert.Contains(
            SpellDefinitionValidator.Validate(Create(level: level)),
            error => error.Contains("between 0 and 9", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsEmptyClassList()
    {
        Assert.Contains(
            SpellDefinitionValidator.Validate(Create(classes: [])),
            error => error.Contains(
                "at least one class",
                StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsRepeatedClass()
    {
        SpellDefinition definition = Create(classes:
        [
            new ClassId("dnd5e2014.class.wizard"),
            new ClassId("dnd5e2014.class.wizard")
        ]);

        Assert.Contains(
            SpellDefinitionValidator.Validate(definition),
            error => error.Contains("repeat a class", StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        Assert.Contains(
            SpellDefinitionValidator.Validate(Create(sources: [])),
            error => error.Contains(
                "at least one source reference",
                StringComparison.Ordinal));
    }

    [Fact]
    public void Validator_RejectsRitualCantrip()
    {
        Assert.Contains(
            SpellDefinitionValidator.Validate(
                Create(level: 0, isRitual: true)),
            error => error.Contains(
                "must not be tagged as a ritual",
                StringComparison.Ordinal));
    }

    [Fact]
    public void Range_SelfWithAreaCarriesShapeAndSize()
    {
        SpellRange range =
            SpellRange.SelfWithArea(SpellAreaShape.Cone, 15);

        Assert.Equal(SpellRangeKind.Self, range.Kind);
        Assert.Equal(SpellAreaShape.Cone, range.AreaShape);
        Assert.Equal(15, range.AreaSizeFeet);
        Assert.Null(range.DistanceFeet);
        Assert.Null(SpellRange.Self().AreaShape);
    }

    [Theory]
    [InlineData(SpellAreaShape.Cone)]
    [InlineData(SpellAreaShape.Radius)]
    [InlineData(SpellAreaShape.Cube)]
    public void Range_SelfWithAreaAcceptsEveryDefinedShape(
        SpellAreaShape shape)
    {
        SpellRange range = SpellRange.SelfWithArea(shape, 15);

        Assert.Equal(shape, range.AreaShape);
        Assert.Equal(15, range.AreaSizeFeet);
    }

    [Fact]
    public void Components_RejectCostWithoutMaterial()
    {
        Assert.Throws<ArgumentException>(
            () => new SpellComponents(true, true, false, null, 50));
    }

    [Fact]
    public void Components_RejectConsumedWithoutMaterial()
    {
        Assert.Throws<ArgumentException>(
            () => new SpellComponents(
                true, true, false, null, null, materialIsConsumed: true));
    }

    [Fact]
    public void Components_CarryStatedMaterialCost()
    {
        var components =
            new SpellComponents(true, true, true, "a diamond", 50);

        Assert.Equal(50, components.MaterialCostGoldPieces);
    }

    [Fact]
    public void Components_RejectNoComponentsAtAll()
    {
        Assert.Throws<ArgumentException>(
            () => new SpellComponents(false, false, false, null));
    }

    [Fact]
    public void Components_RequireDescriptionWhenMaterial()
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new SpellComponents(true, false, true, null));
    }

    [Fact]
    public void Components_RejectDescriptionWithoutMaterial()
    {
        Assert.Throws<ArgumentException>(
            () => new SpellComponents(true, false, false, "a bit of fleece"));
    }

    [Fact]
    public void Range_SelfAndTouchCarryNoDistance()
    {
        Assert.Null(SpellRange.Self().DistanceFeet);
        Assert.Null(SpellRange.Touch().DistanceFeet);
        Assert.Equal(60, SpellRange.Distance(60).DistanceFeet);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Range_RejectsNonPositiveDistance(int feet)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => SpellRange.Distance(feet));
    }

    [Fact]
    public void Duration_InstantaneousCarriesNoAmount()
    {
        SpellDuration duration = SpellDuration.Instantaneous();

        Assert.True(duration.IsInstantaneous);
        Assert.False(duration.IsUntilDispelled);
        Assert.False(duration.RequiresConcentration);
        Assert.False(duration.IsUpTo);
        Assert.Null(duration.Amount);
        Assert.Null(duration.Unit);
    }

    // "Until dispelled" is a third duration kind alongside instantaneous
    // and timed — not an unbounded amount, so it carries neither an amount
    // nor a unit and is never a concentration or "up to" duration.
    [Fact]
    public void Duration_UntilDispelledCarriesNoAmountAndIsNotInstantaneous()
    {
        SpellDuration duration = SpellDuration.UntilDispelled();

        Assert.True(duration.IsUntilDispelled);
        Assert.False(duration.IsInstantaneous);
        Assert.False(duration.RequiresConcentration);
        Assert.False(duration.IsUpTo);
        Assert.Null(duration.Amount);
        Assert.Null(duration.Unit);
    }

    [Fact]
    public void Duration_TimedIsNeitherInstantaneousNorUntilDispelled()
    {
        SpellDuration duration =
            SpellDuration.Fixed(8, SpellDurationUnit.Hour);

        Assert.False(duration.IsInstantaneous);
        Assert.False(duration.IsUntilDispelled);
    }

    [Fact]
    public void Duration_ConcentrationIsAlwaysAnUpToDuration()
    {
        SpellDuration duration =
            SpellDuration.Concentration(1, SpellDurationUnit.Minute);

        Assert.True(duration.RequiresConcentration);
        Assert.True(duration.IsUpTo);
    }

    [Fact]
    public void Duration_UpToIsIndependentOfConcentration()
    {
        SpellDuration duration =
            SpellDuration.UpTo(1, SpellDurationUnit.Hour);

        Assert.True(duration.IsUpTo);
        Assert.False(duration.RequiresConcentration);
    }

    [Fact]
    public void Duration_FixedIsNeitherUpToNorConcentration()
    {
        SpellDuration duration =
            SpellDuration.Fixed(1, SpellDurationUnit.Round);

        Assert.False(duration.IsUpTo);
        Assert.False(duration.RequiresConcentration);
    }

    [Fact]
    public void CastingTime_RejectsNonPositiveAmount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new SpellCastingTime(0, SpellCastingTimeUnit.Action));
    }

    [Fact]
    public void Catalog_OrdersByIdAndRejectsDuplicates()
    {
        var catalog = new SpellCatalog(
        [
            Create(id: "dnd5e2014.spell.light"),
            Create(id: "dnd5e2014.spell.acid-splash")
        ]);

        Assert.Equal(
            ["dnd5e2014.spell.acid-splash", "dnd5e2014.spell.light"],
            catalog.All.Select(definition => definition.Id.Value));

        Assert.Throws<ArgumentException>(
            () => new SpellCatalog(
            [
                Create(id: "dnd5e2014.spell.light"),
                Create(id: "dnd5e2014.spell.light")
            ]));
    }

    [Fact]
    public void Catalog_GetAndTryGetBehaveConsistently()
    {
        var catalog = new SpellCatalog(
            [Create(id: "dnd5e2014.spell.light")]);

        Assert.Equal(1, catalog.Count);
        Assert.True(
            catalog.TryGet(
                new SpellId("dnd5e2014.spell.light"),
                out SpellDefinition? found));
        Assert.NotNull(found);
        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(new SpellId("dnd5e2014.spell.missing")));
    }

    private static SpellDefinition Create(
        string id = "dnd5e2014.spell.acid-splash",
        int level = 0,
        bool isRitual = false,
        IEnumerable<ClassId>? classes = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new SpellDefinition(
            new SpellId(id),
            "Acid Splash",
            level,
            new MagicSchoolId("dnd5e2014.magic-school.conjuration"),
            new SpellCastingTime(1, SpellCastingTimeUnit.Action),
            SpellRange.Distance(60),
            new SpellComponents(true, true, false, null),
            SpellDuration.Instantaneous(),
            isRitual,
            classes ?? [new ClassId("dnd5e2014.class.wizard")],
            sources ?? [TestSource()]);
    }

    private static SourceReference TestSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            211,
            "Chapter 11: Spells — Spell Descriptions — Acid Splash");
    }
}
