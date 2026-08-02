using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Tests;

public sealed class AbilityCatalogTests
{
    [Fact]
    public void Catalog_OrdersByStableIdAndProvidesLookup()
    {
        var catalog = new AbilityCatalog(
            [
                Create("dnd5e2014.ability.z", "Z"),
                Create("dnd5e2014.ability.a", "A")
            ]);

        Assert.Equal(
            [
                "dnd5e2014.ability.a",
                "dnd5e2014.ability.z"
            ],
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        Assert.Equal(
            "A",
            catalog
                .Get(new AbilityId("dnd5e2014.ability.a"))
                .Name);

        Assert.True(
            catalog.TryGet(
                new AbilityId("dnd5e2014.ability.z"),
                out AbilityDefinition? found));

        Assert.NotNull(found);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<AbilityDefinition>
        {
            Create("dnd5e2014.ability.one", "One")
        };

        var catalog = new AbilityCatalog(source);

        source.Add(
            Create("dnd5e2014.ability.two", "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new AbilityCatalog(
                [
                    Create(
                        "dnd5e2014.ability.duplicate",
                        "One"),
                    Create(
                        "dnd5e2014.ability.duplicate",
                        "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        var definition = new AbilityDefinition(
            default,
            "Invalid",
            [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => new AbilityCatalog([definition]));
    }

    private static AbilityDefinition Create(
        string id,
        string name)
    {
        return new AbilityDefinition(
            new AbilityId(id),
            name,
            [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            page: 173);
    }
}
