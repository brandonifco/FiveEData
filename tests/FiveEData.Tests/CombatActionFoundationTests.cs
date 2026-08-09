using FiveEData.Rules.Catalog;
using FiveEData.Rules.Combat.CombatActions;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class CombatActionFoundationTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Id_RejectsEmptyValue(string value)
    {
        Assert.Throws<ArgumentException>(() => new CombatActionId(value));
    }

    [Fact]
    public void Id_RejectsNullValue()
    {
        Assert.Throws<ArgumentNullException>(() => new CombatActionId(null!));
    }

    [Fact]
    public void Id_RoundTripsValue()
    {
        var id = new CombatActionId("dnd5e2014.combat-action.attack");

        Assert.Equal("dnd5e2014.combat-action.attack", id.Value);
        Assert.Equal("dnd5e2014.combat-action.attack", id.ToString());
    }

    [Fact]
    public void Definition_CopiesSourcesDefensively()
    {
        var sources = new List<SourceReference> { TestSource() };

        CombatActionDefinition definition = Create(sources: sources);

        sources.Add(TestSource());

        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Validator_RejectsMissingSources()
    {
        CombatActionDefinition definition = Create(sources: []);

        Assert.Contains(
            CombatActionDefinitionValidator.Validate(definition),
            error => error.Contains(
                "at least one source reference",
                StringComparison.Ordinal));
    }

    [Fact]
    public void Catalog_OrdersById()
    {
        var catalog = new CombatActionCatalog(
        [
            Create("dnd5e2014.combat-action.ready", "Ready"),
            Create("dnd5e2014.combat-action.attack", "Attack")
        ]);

        Assert.Equal(
            [
                "dnd5e2014.combat-action.attack",
                "dnd5e2014.combat-action.ready"
            ],
            catalog.All.Select(definition => definition.Id.Value));
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new CombatActionCatalog(
            [
                Create("dnd5e2014.combat-action.dodge", "Dodge"),
                Create("dnd5e2014.combat-action.dodge", "Dodge")
            ]));
    }

    [Fact]
    public void Catalog_GetThrowsForMissingId()
    {
        var catalog = new CombatActionCatalog(
            [Create("dnd5e2014.combat-action.dodge", "Dodge")]);

        Assert.Throws<KeyNotFoundException>(
            () => catalog.Get(
                new CombatActionId("dnd5e2014.combat-action.missing")));
    }

    [Fact]
    public void Catalog_TryGetReportsPresence()
    {
        var catalog = new CombatActionCatalog(
            [Create("dnd5e2014.combat-action.dodge", "Dodge")]);

        Assert.True(
            catalog.TryGet(
                new CombatActionId("dnd5e2014.combat-action.dodge"),
                out CombatActionDefinition? found));
        Assert.Equal("Dodge", found!.Name);

        Assert.False(
            catalog.TryGet(
                new CombatActionId("dnd5e2014.combat-action.missing"),
                out CombatActionDefinition? missing));
        Assert.Null(missing);
    }

    [Fact]
    public void Catalog_ExposesCount()
    {
        var catalog = new CombatActionCatalog(
        [
            Create("dnd5e2014.combat-action.dodge", "Dodge"),
            Create("dnd5e2014.combat-action.dash", "Dash")
        ]);

        Assert.Equal(2, catalog.Count);
    }

    private static CombatActionDefinition Create(
        string id = "dnd5e2014.combat-action.attack",
        string name = "Attack",
        IEnumerable<SourceReference>? sources = null)
    {
        return new CombatActionDefinition(
            new CombatActionId(id),
            name,
            sources ?? [TestSource()]);
    }

    private static SourceReference TestSource()
    {
        return new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            192,
            "Chapter 9: Combat — Actions in Combat");
    }
}
