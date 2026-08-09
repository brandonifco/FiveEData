using FiveEData;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Combat.CombatActions;
using FiveEData.Rules.Combat.CombatActions.Serialization;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Tests;

public sealed class CombatActionDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactCombatActionClosure()
    {
        IReadOnlyList<CombatActionDefinition> definitions = LoadCanonical();

        Assert.Equal(
            [
                "dnd5e2014.combat-action.attack",
                "dnd5e2014.combat-action.cast-a-spell",
                "dnd5e2014.combat-action.dash",
                "dnd5e2014.combat-action.disengage",
                "dnd5e2014.combat-action.dodge",
                "dnd5e2014.combat-action.help",
                "dnd5e2014.combat-action.hide",
                "dnd5e2014.combat-action.ready",
                "dnd5e2014.combat-action.search",
                "dnd5e2014.combat-action.use-an-object"
            ],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_ContainsExactlyTenActions()
    {
        Assert.Equal(10, LoadCanonical().Count);
    }

    [Theory]
    [InlineData("dnd5e2014.combat-action.attack", "Attack", 192)]
    [InlineData("dnd5e2014.combat-action.cast-a-spell", "Cast a Spell", 192)]
    [InlineData("dnd5e2014.combat-action.dash", "Dash", 192)]
    [InlineData("dnd5e2014.combat-action.disengage", "Disengage", 192)]
    [InlineData("dnd5e2014.combat-action.dodge", "Dodge", 192)]
    [InlineData("dnd5e2014.combat-action.help", "Help", 192)]
    [InlineData("dnd5e2014.combat-action.hide", "Hide", 192)]
    [InlineData("dnd5e2014.combat-action.ready", "Ready", 193)]
    [InlineData("dnd5e2014.combat-action.search", "Search", 193)]
    [InlineData(
        "dnd5e2014.combat-action.use-an-object", "Use an Object", 193)]
    public void Action_HasExpectedNameAndCitation(
        string id,
        string expectedName,
        int expectedPage)
    {
        CombatActionDefinition definition = Get(id);

        Assert.Equal(expectedName, definition.Name);

        SourceReference source = Assert.Single(definition.Sources);
        Assert.Equal(
            "dnd5e2014.source.phb-first-printing",
            source.DocumentId.Value);
        Assert.Equal(expectedPage, source.Page);
        Assert.Equal(
            "Chapter 9: Combat — Actions in Combat — " + expectedName,
            source.Section);
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedResourceMatchingTheDataFile()
    {
        CombatActionCatalog catalog = Dnd5e2014Ruleset.Instance.CombatActions;

        Assert.Equal(
            LoadCanonical()
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal),
            catalog.All.Select(definition => definition.Id.Value));
    }

    private static CombatActionDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<CombatActionDefinition> LoadCanonical()
    {
        return CombatActionDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "combat-actions.json"));
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
