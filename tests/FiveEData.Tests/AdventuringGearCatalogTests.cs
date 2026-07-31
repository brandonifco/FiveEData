using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.AdventuringGear;

namespace FiveEData.Tests;

public sealed class AdventuringGearCatalogTests
{
    [Fact]
    public void Ruleset_ExposesCompleteAdventuringGearCatalog()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.Equal(95, ruleset.AdventuringGear.Count);

        AdventuringGearDefinition backpack =
            ruleset.AdventuringGear.Get(
                new AdventuringGearId(
                    "dnd5e2014.adventuring-gear.backpack"));

        Assert.Equal("Backpack", backpack.Name);
        Assert.Equal(200, backpack.Cost.CopperPieces);
        Assert.Equal(5m, backpack.ListedWeight?.Weight.Pounds);
    }

    [Fact]
    public void GetAndTryGet_HaveExplicitMissingSemantics()
    {
        AdventuringGearCatalog catalog =
            Dnd5e2014Ruleset.Instance.AdventuringGear;
        var existing = new AdventuringGearId(
            "dnd5e2014.adventuring-gear.torch");

        Assert.True(
            catalog.TryGet(
                existing,
                out AdventuringGearDefinition? torch));
        Assert.NotNull(torch);

        var missing = new AdventuringGearId(
            "dnd5e2014.adventuring-gear.does-not-exist");

        Assert.False(catalog.TryGet(missing, out _));
        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missing));
    }

    [Fact]
    public void EnumerationIsDeterministicByStableId()
    {
        string[] actual = Dnd5e2014Ruleset.Instance.AdventuringGear.All
            .Select(definition => definition.Id.Value)
            .ToArray();

        string[] expected = actual
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<AdventuringGearDefinition>
        {
            CreateDefinition("dnd5e2014.adventuring-gear.one", "One")
        };

        var catalog = new AdventuringGearCatalog(source);

        source.Add(
            CreateDefinition("dnd5e2014.adventuring-gear.two", "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        AdventuringGearDefinition first =
            CreateDefinition("dnd5e2014.adventuring-gear.duplicate", "First");
        AdventuringGearDefinition second =
            CreateDefinition("dnd5e2014.adventuring-gear.duplicate", "Second");

        Assert.Throws<ArgumentException>(
            () => new AdventuringGearCatalog([first, second]));
    }

    [Fact]
    public void Catalog_RejectsDefinitionWithDefaultId()
    {
        AdventuringGearDefinition invalid = new(
            default,
            "Invalid",
            new Money(100),
            listedWeight: null,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 150)
            ]);

        Assert.Throws<InvalidOperationException>(
            () => new AdventuringGearCatalog([invalid]));
    }

    private static AdventuringGearDefinition CreateDefinition(
        string id,
        string name)
    {
        return new AdventuringGearDefinition(
            new AdventuringGearId(id),
            name,
            new Money(100),
            new ListedWeight(new Weight(1m)),
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 150)
            ]);
    }
}
