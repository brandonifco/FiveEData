using FiveEData.Rules.Classes.Metamagic;
using FiveEData.Rules.Classes.Metamagic.Serialization;

namespace FiveEData.Tests;

public sealed class MetamagicOptionDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsFixedCostStrictly()
    {
        MetamagicOptionDefinition definition = Assert.Single(
            MetamagicOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.metamagic-option.test",
                    "name": "Test",
                    "fixedSorceryPointCost": 1,
                    "costEqualsSpellLevelWithCantripMinimum": false,
                    "protectsCreatureCountUpToSpellcastingModifier": false,
                    "doublesRange": false,
                    "touchRangeBecomesFeet": null,
                    "rerollsDiceCountUpToSpellcastingModifier": false,
                    "doublesDurationMaxHours": null,
                    "grantsDisadvantageOnFirstSavingThrow": false,
                    "changesCastingTimeToBonusAction": false,
                    "removesVerbalAndSomaticComponents": false,
                    "targetsSecondCreatureInRange": false,
                    "sources": [
                      {
                        "documentId": "extension.source.test",
                        "page": 1,
                        "section": "Test section"
                      }
                    ]
                  }
                ]
                """));

        Assert.Equal(
            "extension.metamagic-option.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(1, definition.FixedSorceryPointCost);
        Assert.False(definition.CostEqualsSpellLevelWithCantripMinimum);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsSpellLevelCostRepresentation()
    {
        MetamagicOptionDefinition definition = Assert.Single(
            MetamagicOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.metamagic-option.test",
                    "name": "Test",
                    "fixedSorceryPointCost": null,
                    "costEqualsSpellLevelWithCantripMinimum": true,
                    "protectsCreatureCountUpToSpellcastingModifier": false,
                    "doublesRange": false,
                    "touchRangeBecomesFeet": null,
                    "rerollsDiceCountUpToSpellcastingModifier": false,
                    "doublesDurationMaxHours": null,
                    "grantsDisadvantageOnFirstSavingThrow": false,
                    "changesCastingTimeToBonusAction": false,
                    "removesVerbalAndSomaticComponents": false,
                    "targetsSecondCreatureInRange": true,
                    "sources": [
                      {
                        "documentId": "extension.source.test",
                        "page": 1,
                        "section": "Test section"
                      }
                    ]
                  }
                ]
                """));

        Assert.Null(definition.FixedSorceryPointCost);
        Assert.True(definition.CostEqualsSpellLevelWithCantripMinimum);
        Assert.True(definition.TargetsSecondCreatureInRange);
    }

    [Fact]
    public void ValidDefinition_LoadsMechanismFields()
    {
        MetamagicOptionDefinition definition = Assert.Single(
            MetamagicOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.metamagic-option.test",
                    "name": "Test",
                    "fixedSorceryPointCost": 1,
                    "costEqualsSpellLevelWithCantripMinimum": false,
                    "protectsCreatureCountUpToSpellcastingModifier": true,
                    "doublesRange": true,
                    "touchRangeBecomesFeet": 30,
                    "rerollsDiceCountUpToSpellcastingModifier": true,
                    "doublesDurationMaxHours": 24,
                    "grantsDisadvantageOnFirstSavingThrow": true,
                    "changesCastingTimeToBonusAction": true,
                    "removesVerbalAndSomaticComponents": true,
                    "targetsSecondCreatureInRange": false,
                    "sources": [
                      {
                        "documentId": "extension.source.test",
                        "page": 1,
                        "section": "Test section"
                      }
                    ]
                  }
                ]
                """));

        Assert.True(
            definition.ProtectsCreatureCountUpToSpellcastingModifier);
        Assert.True(definition.DoublesRange);
        Assert.Equal(30, definition.TouchRangeBecomesFeet);
        Assert.True(definition.RerollsDiceCountUpToSpellcastingModifier);
        Assert.Equal(24, definition.DoublesDurationMaxHours);
        Assert.True(definition.GrantsDisadvantageOnFirstSavingThrow);
        Assert.True(definition.ChangesCastingTimeToBonusAction);
        Assert.True(definition.RemovesVerbalAndSomaticComponents);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => MetamagicOptionDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => MetamagicOptionDefinitionLoader.LoadFromJson(
                    "[null]"));

        Assert.Contains(
            "index 0",
            exception.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => MetamagicOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.metamagic-option.test",
                    "name": "Test",
                    "fixedSorceryPointCost": 1,
                    "costEqualsSpellLevelWithCantripMinimum": false,
                    "protectsCreatureCountUpToSpellcastingModifier": false,
                    "doublesRange": false,
                    "touchRangeBecomesFeet": null,
                    "rerollsDiceCountUpToSpellcastingModifier": false,
                    "doublesDurationMaxHours": null,
                    "grantsDisadvantageOnFirstSavingThrow": false,
                    "changesCastingTimeToBonusAction": false,
                    "removesVerbalAndSomaticComponents": false,
                    "targetsSecondCreatureInRange": false,
                    "sources": [],
                    "unexpected": true
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateJsonProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => MetamagicOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.metamagic-option.test",
                    "name": "Test",
                    "name": "Other",
                    "fixedSorceryPointCost": 1,
                    "costEqualsSpellLevelWithCantripMinimum": false,
                    "protectsCreatureCountUpToSpellcastingModifier": false,
                    "doublesRange": false,
                    "touchRangeBecomesFeet": null,
                    "rerollsDiceCountUpToSpellcastingModifier": false,
                    "doublesDurationMaxHours": null,
                    "grantsDisadvantageOnFirstSavingThrow": false,
                    "changesCastingTimeToBonusAction": false,
                    "removesVerbalAndSomaticComponents": false,
                    "targetsSecondCreatureInRange": false,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => MetamagicOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.metamagic-option.test",
                    "name": "Test",
                    "fixedSorceryPointCost": 1,
                    "costEqualsSpellLevelWithCantripMinimum": false,
                    "protectsCreatureCountUpToSpellcastingModifier": false,
                    "doublesRange": false,
                    "touchRangeBecomesFeet": null,
                    "rerollsDiceCountUpToSpellcastingModifier": false,
                    "doublesDurationMaxHours": null,
                    "grantsDisadvantageOnFirstSavingThrow": false,
                    "changesCastingTimeToBonusAction": false,
                    "removesVerbalAndSomaticComponents": false,
                    "targetsSecondCreatureInRange": false
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => MetamagicOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "fixedSorceryPointCost": 1,
                    "costEqualsSpellLevelWithCantripMinimum": false,
                    "protectsCreatureCountUpToSpellcastingModifier": false,
                    "doublesRange": false,
                    "touchRangeBecomesFeet": null,
                    "rerollsDiceCountUpToSpellcastingModifier": false,
                    "doublesDurationMaxHours": null,
                    "grantsDisadvantageOnFirstSavingThrow": false,
                    "changesCastingTimeToBonusAction": false,
                    "removesVerbalAndSomaticComponents": false,
                    "targetsSecondCreatureInRange": false,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        const string one =
            """
            {
              "id": "extension.metamagic-option.test",
              "name": "Test",
              "fixedSorceryPointCost": 1,
              "costEqualsSpellLevelWithCantripMinimum": false,
              "protectsCreatureCountUpToSpellcastingModifier": false,
              "doublesRange": false,
              "touchRangeBecomesFeet": null,
              "rerollsDiceCountUpToSpellcastingModifier": false,
              "doublesDurationMaxHours": null,
              "grantsDisadvantageOnFirstSavingThrow": false,
              "changesCastingTimeToBonusAction": false,
              "removesVerbalAndSomaticComponents": false,
              "targetsSecondCreatureInRange": false,
              "sources": [
                {
                  "documentId": "extension.source.test",
                  "page": 1,
                  "section": "Test section"
                }
              ]
            }
            """;

        string json = $"[{one},{one}]";

        Assert.Throws<InvalidDataException>(
            () => MetamagicOptionDefinitionLoader.LoadFromJson(json));
    }
}
