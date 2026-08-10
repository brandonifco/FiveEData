using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.TransmutersStoneOptions;
using FiveEData.Rules.Classes.TransmutersStoneOptions.Serialization;

namespace FiveEData.Tests;

public sealed class TransmutersStoneOptionDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactOptionClosure()
    {
        Assert.Equal(
            [
                "dnd5e2014.transmuters-stone-option." +
                    "constitution-saving-throw-proficiency",
                "dnd5e2014.transmuters-stone-option.damage-resistance",
                "dnd5e2014.transmuters-stone-option.darkvision",
                "dnd5e2014.transmuters-stone-option.speed-increase"
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
            Get("dnd5e2014.transmuters-stone-option.darkvision")
                .DarkvisionRangeFeet);
    }

    [Fact]
    public void SpeedIncrease_IsGatedOnBeingUnencumbered()
    {
        TransmutersStoneOptionDefinition definition =
            Get("dnd5e2014.transmuters-stone-option.speed-increase");

        Assert.Equal(10, definition.SpeedBonusFeet);
        Assert.True(definition.RequiresUnencumbered);
    }

    [Fact]
    public void SavingThrowProficiency_ReferencesConstitution()
    {
        Assert.Equal(
            "dnd5e2014.ability.constitution",
            Get(
                "dnd5e2014.transmuters-stone-option." +
                "constitution-saving-throw-proficiency")
                .SavingThrowProficiencyAbilityId?.Value);
    }

    [Fact]
    public void DamageResistance_OffersFiveChoosableTypes()
    {
        TransmutersStoneOptionDefinition definition =
            Get("dnd5e2014.transmuters-stone-option.damage-resistance");

        Assert.Equal(
            [
                "dnd5e2014.damage-type.acid",
                "dnd5e2014.damage-type.cold",
                "dnd5e2014.damage-type.fire",
                "dnd5e2014.damage-type.lightning",
                "dnd5e2014.damage-type.thunder"
            ],
            definition.ChoosableResistedDamageTypeIds
                .Select(damageTypeId => damageTypeId.Value));
    }

    [Fact]
    public void OnlyDamageResistance_OffersAChoice()
    {
        Assert.All(
            LoadCanonical()
                .Where(definition =>
                    definition.Id.Value !=
                    "dnd5e2014.transmuters-stone-option.damage-resistance"),
            definition =>
                Assert.Empty(definition.ChoosableResistedDamageTypeIds));
    }

    [Fact]
    public void AllOptions_CitePhbFirstPrintingPageOneHundredNineteen()
    {
        foreach (
            TransmutersStoneOptionDefinition definition in LoadCanonical())
        {
            var source = Assert.Single(definition.Sources);
            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(119, source.Page);
            Assert.Equal("Chapter 3: Classes", source.Section);
        }
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedResourceMatchingTheDataFile()
    {
        TransmutersStoneOptionCatalog catalog =
            Dnd5e2014Ruleset.Instance.TransmutersStoneOptions;

        Assert.Equal(
            LoadCanonical()
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal),
            catalog.All.Select(definition => definition.Id.Value));
    }

    private static TransmutersStoneOptionDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<TransmutersStoneOptionDefinition>
        LoadCanonical()
    {
        return TransmutersStoneOptionDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "transmuters-stone-options.json"));
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
