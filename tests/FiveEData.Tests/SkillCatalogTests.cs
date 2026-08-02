using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Skills;

namespace FiveEData.Tests;

public sealed class SkillCatalogTests
{
    [Fact]
    public void Catalog_OrdersByStableIdAndProvidesLookup()
    {
        var catalog = new SkillCatalog(
            [
                Create("dnd5e2014.skill.z", "Z"),
                Create("dnd5e2014.skill.a", "A")
            ]);

        Assert.Equal(
            [
                "dnd5e2014.skill.a",
                "dnd5e2014.skill.z"
            ],
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        Assert.Equal(
            "A",
            catalog
                .Get(new SkillId("dnd5e2014.skill.a"))
                .Name);

        Assert.True(
            catalog.TryGet(
                new SkillId("dnd5e2014.skill.z"),
                out SkillDefinition? found));

        Assert.NotNull(found);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<SkillDefinition>
        {
            Create("dnd5e2014.skill.one", "One")
        };

        var catalog = new SkillCatalog(source);

        source.Add(
            Create("dnd5e2014.skill.two", "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new SkillCatalog(
                [
                    Create(
                        "dnd5e2014.skill.duplicate",
                        "One"),
                    Create(
                        "dnd5e2014.skill.duplicate",
                        "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        var definition = new SkillDefinition(
            default,
            "Invalid",
            new AbilityId(
                "dnd5e2014.ability.dexterity"),
            [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => new SkillCatalog([definition]));
    }

    [Fact]
    public void Catalog_RejectsDefaultAbilityIdAtTrustBoundary()
    {
        var definition = new SkillDefinition(
            new SkillId("dnd5e2014.skill.invalid"),
            "Invalid",
            default,
            [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => new SkillCatalog([definition]));
    }

    private static SkillDefinition Create(
        string id,
        string name)
    {
        return new SkillDefinition(
            new SkillId(id),
            name,
            new AbilityId(
                "dnd5e2014.ability.dexterity"),
            [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            page: 174);
    }
}
