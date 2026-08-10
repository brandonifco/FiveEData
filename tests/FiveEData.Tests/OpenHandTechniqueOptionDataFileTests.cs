using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.OpenHandTechniqueOptions;
using FiveEData.Rules.Classes.OpenHandTechniqueOptions.Serialization;
using FiveEData.Rules.Common;

namespace FiveEData.Tests;

public sealed class OpenHandTechniqueOptionDataFileTests
{
    [Fact]
    public void CanonicalFile_ContainsExactOptionClosure()
    {
        Assert.Equal(
            [
                "dnd5e2014.open-hand-technique-option.knock-prone",
                "dnd5e2014.open-hand-technique-option.prevent-reactions",
                "dnd5e2014.open-hand-technique-option.push"
            ],
            LoadCanonical()
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal));
    }

    [Fact]
    public void KnockProne_UsesADexteritySaveAndTheProneCondition()
    {
        OpenHandTechniqueOptionDefinition definition =
            Get("dnd5e2014.open-hand-technique-option.knock-prone");

        Assert.Equal(
            "dnd5e2014.ability.dexterity",
            definition.SavingThrowAbilityId?.Value);
        Assert.Equal(
            "dnd5e2014.condition.prone",
            definition.ImposedConditionId?.Value);
        Assert.Null(definition.PushDistanceFeet);
    }

    [Fact]
    public void Push_UsesAStrengthSaveAndFifteenFeet()
    {
        OpenHandTechniqueOptionDefinition definition =
            Get("dnd5e2014.open-hand-technique-option.push");

        Assert.Equal(
            "dnd5e2014.ability.strength",
            definition.SavingThrowAbilityId?.Value);
        Assert.Equal(15, definition.PushDistanceFeet);
        Assert.Null(definition.ImposedConditionId);
    }

    [Fact]
    public void PreventReactions_IsTheOnlyOptionWithoutASavingThrow()
    {
        OpenHandTechniqueOptionDefinition definition =
            Get("dnd5e2014.open-hand-technique-option.prevent-reactions");

        Assert.True(definition.PreventsReactions);
        Assert.Equal(
            NextTurnDurationTrigger.EndOfYourNextTurn,
            definition.PreventsReactionsUntil);
        Assert.Null(definition.SavingThrowAbilityId);

        Assert.All(
            LoadCanonical().Where(other => other.Id != definition.Id),
            other => Assert.NotNull(other.SavingThrowAbilityId));
    }

    [Fact]
    public void AllOptions_CitePhbFirstPrintingPageSeventyNine()
    {
        foreach (
            OpenHandTechniqueOptionDefinition definition in LoadCanonical())
        {
            var source = Assert.Single(definition.Sources);
            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                source.DocumentId.Value);
            Assert.Equal(79, source.Page);
            Assert.Equal("Chapter 3: Classes", source.Section);
        }
    }

    [Fact]
    public void Ruleset_ExposesTheEmbeddedResourceMatchingTheDataFile()
    {
        OpenHandTechniqueOptionCatalog catalog =
            Dnd5e2014Ruleset.Instance.OpenHandTechniqueOptions;

        Assert.Equal(
            LoadCanonical()
                .Select(definition => definition.Id.Value)
                .OrderBy(id => id, StringComparer.Ordinal),
            catalog.All.Select(definition => definition.Id.Value));
    }

    private static OpenHandTechniqueOptionDefinition Get(string id)
    {
        return LoadCanonical()
            .Single(definition => definition.Id.Value == id);
    }

    private static IReadOnlyList<OpenHandTechniqueOptionDefinition>
        LoadCanonical()
    {
        return OpenHandTechniqueOptionDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "open-hand-technique-options.json"));
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
