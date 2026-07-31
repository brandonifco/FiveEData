using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Equipment.Tools.Serialization;

namespace FiveEData.Tests;

public sealed class ToolFamilyDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactSourceFamilies()
    {
        IReadOnlyList<ToolFamilyDefinition> definitions = LoadCanonical();

        Assert.Equal(3, definitions.Count);

        Assert.Equal(
            [
                "dnd5e2014.tool-family.artisans-tools",
                "dnd5e2014.tool-family.gaming-set",
                "dnd5e2014.tool-family.musical-instrument"
            ],
            definitions
                .Select(definition => definition.Id.Value)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray());

        Assert.All(definitions, definition => Assert.Empty(definition.SpecialRuleIds));
    }

    [Fact]
    public void CanonicalFile_PreservesNamesAndProvenance()
    {
        IReadOnlyDictionary<string, string> expected =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["dnd5e2014.tool-family.artisans-tools"] = "Artisan's tools",
                ["dnd5e2014.tool-family.gaming-set"] = "Gaming set",
                ["dnd5e2014.tool-family.musical-instrument"] = "Musical instrument"
            };

        foreach (ToolFamilyDefinition definition in LoadCanonical())
        {
            Assert.Equal(expected[definition.Id.Value], definition.Name);

            var source = Assert.Single(definition.Sources);
            Assert.Equal(154, source.Page);
            Assert.Equal("Chapter 5: Equipment — Tools", source.Section);
        }
    }

    private static IReadOnlyList<ToolFamilyDefinition> LoadCanonical()
    {
        return ToolFamilyDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "tool-families.json"));
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }
}
