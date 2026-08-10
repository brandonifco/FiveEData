using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.ThirdEyeOptions;
using FiveEData.Rules.Classes.ThirdEyeOptions.Serialization;

namespace FiveEData.Tests;

public sealed class ThirdEyeOptionDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactOptionClosure()
    {
        Assert.Equal(
            [
                "dnd5e2014.third-eye-option.darkvision",
                "dnd5e2014.third-eye-option.ethereal-sight",
                "dnd5e2014.third-eye-option.greater-comprehension",
                "dnd5e2014.third-eye-option.see-invisibility"
            ],
            LoadCanonical()
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void Darkvision_ReachesSixtyFeet()
    {
        Assert.Equal(
            60,
            Get("dnd5e2014.third-eye-option.darkvision")
                .DarkvisionRangeFeet);
    }

    [Fact]
    public void EtherealSight_ReachesSixtyFeet()
    {
        Assert.Equal(
            60,
            Get("dnd5e2014.third-eye-option.ethereal-sight")
                .EtherealSightRangeFeet);
    }

    [Fact]
    public void SeeInvisibility_ReachesOnlyTenFeet()
    {
        Assert.Equal(
            10,
            Get("dnd5e2014.third-eye-option.see-invisibility")
                .SeeInvisibilityRangeFeet);
    }

    [Fact]
    public void GreaterComprehension_IsTheOnlyOptionWithoutARange()
    {
        ThirdEyeOptionDefinition definition =
            Get("dnd5e2014.third-eye-option.greater-comprehension");

        Assert.True(definition.CanReadAllLanguages);
        Assert.Null(definition.DarkvisionRangeFeet);
        Assert.Null(definition.EtherealSightRangeFeet);
        Assert.Null(definition.SeeInvisibilityRangeFeet);

        Assert.All(
            LoadCanonical().Where(other => other.Id != definition.Id),
            other => Assert.False(other.CanReadAllLanguages));
    }

    [Fact]
    public void AllOptions_CitePhbFirstPrintingPageOneHundredSixteen()
    {
        foreach (ThirdEyeOptionDefinition definition in LoadCanonical())
        {
            var source = Assert.Single(definition.Sources);
            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(116, source.Page);
            Assert.Equal("Chapter 3: Classes", source.Section);
        }
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedResourceMatchingTheDataFile()
    {
        ThirdEyeOptionCatalog catalog =
            Dnd5e2014Ruleset.Instance.ThirdEyeOptions;

        Assert.Equal(
            LoadCanonical()
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal),
            catalog.All.Select(definition => definition.Id.Value));
    }

    private static ThirdEyeOptionDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<ThirdEyeOptionDefinition> LoadCanonical()
    {
        return ThirdEyeOptionDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "third-eye-options.json"));
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
