using FiveEData;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Spells.MagicSchools;
using FiveEData.Rules.Spells.MagicSchools.Serialization;

namespace FiveEData.Tests;

public sealed class MagicSchoolDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactMagicSchoolClosure()
    {
        IReadOnlyList<MagicSchoolDefinition> definitions = LoadCanonical();

        Assert.Equal(
            [
                "dnd5e2014.magic-school.abjuration",
                "dnd5e2014.magic-school.conjuration",
                "dnd5e2014.magic-school.divination",
                "dnd5e2014.magic-school.enchantment",
                "dnd5e2014.magic-school.evocation",
                "dnd5e2014.magic-school.illusion",
                "dnd5e2014.magic-school.necromancy",
                "dnd5e2014.magic-school.transmutation"
            ],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void CanonicalFile_ContainsExactlyEightSchools()
    {
        Assert.Equal(8, LoadCanonical().Count);
    }

    [Theory]
    [InlineData("dnd5e2014.magic-school.abjuration", "Abjuration")]
    [InlineData("dnd5e2014.magic-school.conjuration", "Conjuration")]
    [InlineData("dnd5e2014.magic-school.divination", "Divination")]
    [InlineData("dnd5e2014.magic-school.enchantment", "Enchantment")]
    [InlineData("dnd5e2014.magic-school.evocation", "Evocation")]
    [InlineData("dnd5e2014.magic-school.illusion", "Illusion")]
    [InlineData("dnd5e2014.magic-school.necromancy", "Necromancy")]
    [InlineData("dnd5e2014.magic-school.transmutation", "Transmutation")]
    public void School_HasExpectedName(string id, string expectedName)
    {
        Assert.Equal(expectedName, Get(id).Name);
    }

    [Fact]
    public void EverySchool_CitesTheSchoolsOfMagicSidebarOnPage203()
    {
        foreach (MagicSchoolDefinition definition in LoadCanonical())
        {
            SourceReference source = Assert.Single(definition.Sources);

            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(203, source.Page);
            Assert.Equal(
                "Chapter 10: Spellcasting — The Schools of Magic — " +
                definition.Name,
                source.Section);
        }
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedResourceMatchingTheDataFile()
    {
        MagicSchoolCatalog catalog = Dnd5e2014Ruleset.Instance.MagicSchools;

        Assert.Equal(
            LoadCanonical()
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal),
            catalog.All.Select(definition => definition.Id.Value));
    }

    private static MagicSchoolDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<MagicSchoolDefinition> LoadCanonical()
    {
        return MagicSchoolDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "magic-schools.json"));
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
